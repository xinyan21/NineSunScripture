using NineSunScripture.model;
using NineSunScripture.strategy;
using NineSunScripture.trade.persistence;
using NineSunScripture.trade.structApi.api;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NineSunScripture.trade.structApi.helper
{
    public static class AccountHelper
    {
        /// <summary>
        /// 默认主账号为账号列表的第一个
        /// </summary>
        /// <returns></returns>
        private static Account GetPrdMainAccount()
        {
            Account account = new Account();
            account.AcctType = 0x30;
            account.BrokerId = 59;
            account.BrokerName = "平安证券";
            account.BrokerServerIP = "43.254.105.92";
            account.BrokerServerPort = 8003;
            account.VersionOfTHS = "E065.20.92";
            account.CommPwd = "";
            account.SalesDepartId = "105";
            account.IsRandomMac = false;
            account.FundAcct = "321019194496";
            account.PriceAcct = "13534068934";
            account.FundPassword = "198921";
            account.PricePassword = "3594035x";
            //没指定初始资产，差点又被大坑
            account.InitTotalAsset = 60000;
            return account;
        }

        public static Account GetTestMainAccount()
        {
            Account account = new Account();
            account.AcctType = 0x30;
            account.BrokerId = 0;
            account.BrokerName = "同花顺模拟";
            account.BrokerServerIP = "trade.10jqka.com.cn";
            account.BrokerServerPort = 8002;
            account.VersionOfTHS = "E065.18.77";
            account.CommPwd = "";
            account.SalesDepartId = "";
            account.IsRandomMac = false;
            account.FundAcct = "24509958";
            account.PriceAcct = "13534068934";
            account.FundPassword = "3594035x";
            account.PricePassword = "3594035x";
            //没指定初始资产，差点又被大坑
            account.InitTotalAsset = 60000;
            return account;
        }

        /// <summary>
        /// 登录所有账户，首次在客户端登录会记录初始总资金到数据库
        /// </summary>
        /// <param name="callback">回调接口</param>
        public static List<Account> Login(ITrade callback)
        {
            List<Account> localAccounts = JsonDataHelper.GetAccounts();
            Account mainAcct = MainStrategy.IsTest ? GetTestMainAccount() : GetPrdMainAccount();
            //TODO TESET CODE
            //mainAcct = GetPrdMainAccount();
            if (null == localAccounts)
            {
                localAccounts = new List<Account>();
            }
            if (!localAccounts.Contains(mainAcct))
            {
                localAccounts.Insert(0, mainAcct);
            }
            List<Account> loginAccts = new List<Account>();
            IntPtr ptrErrorInfo = Marshal.AllocCoTaskMem(256);
            try
            {
                int priceSessionId = PriceAPI.HQ_Logon(
                  mainAcct.PriceAcct, mainAcct.PricePassword, ptrErrorInfo);
                if (priceSessionId > 0)
                {
                    mainAcct.PriceSessionId = priceSessionId;
                    if (null != callback)
                    {
                        callback.OnTradeResult(1, "行情登录", "", false);
                    }
                }
                else
                {
                    string errInfo = ApiHelper.ParseErrInfo(ptrErrorInfo);
                    if (null != callback)
                    {
                        callback.OnTradeResult(0, "行情登录", errInfo, true);
                    }
                }

                LoginTrade(localAccounts, loginAccts, mainAcct, callback);
            }
            finally
            {
                Marshal.FreeCoTaskMem(ptrErrorInfo);
            }
            return loginAccts;
        }

        private static void LoginTrade(List<Account> localAccounts,
            List<Account> loginAccts, Account mainAcct, ITrade callback)
        {
            Task[] tasks = new Task[localAccounts.Count];
            for (int i = 0; i < localAccounts.Count; i++)
            {
                Account account = localAccounts[i];
                //每个账户开个线程去处理，账户间同时操作，效率提升大大的
                tasks[i] = Task.Run(() =>
                {
                    IntPtr ptrErrorInfo = Marshal.AllocCoTaskMem(256);
                    try
                    {
                        int tradeSessionId = TradeAPI.Logon(account.BrokerId, account.BrokerServerIP,
                             account.BrokerServerPort, account.VersionOfTHS, account.SalesDepartId,
                             account.AcctType, account.FundAcct, account.FundPassword, account.CommPwd,
                             account.IsRandomMac, ptrErrorInfo);
                        string opLog = "";
                        if (tradeSessionId > 0)
                        {
                            account.TradeSessionId = tradeSessionId;
                            account.Funds = TradeAPI.QueryFunds(tradeSessionId);
                            account.ShareHolderAccts = TradeAPI.QueryShareHolderAccts(tradeSessionId);
                            account.Positions = TradeAPI.QueryPositions(tradeSessionId);
                            if (account.InitTotalAsset == 0)
                            {
                                account.InitTotalAsset = (int)account.Funds.TotalAsset;
                                JsonDataHelper.InitTotalAsset(account);
                            }
                            if (null != account.ShareHolderAccts && account.ShareHolderAccts.Count > 0)
                            {
                                foreach (ShareHolderAcct shareHolderAcct in account.ShareHolderAccts)
                                {
                                    if (shareHolderAcct.category == "上海A股")
                                    {
                                        account.ShShareholderAcct = shareHolderAcct.code;
                                    }
                                    else
                                    {
                                        account.SzShareholderAcct = shareHolderAcct.code;
                                    }
                                }
                            }
                            //把行情的PriceSessionId给其它账户，就不用每次都拿主账户的PriceSessionId给其它账户
                            if (account != mainAcct)
                            {
                                account.PriceSessionId = mainAcct.PriceSessionId;
                            }
                            loginAccts.Add(account);
                            opLog = "资金账号【" + account.FundAcct + "】登录成功，会话ID为" + tradeSessionId;
                            Logger.Log(opLog);
                        }
                        else
                        {
                            opLog = "资金账号【" + account.FundAcct + "】登录失败，错误信息："
                                + ApiHelper.ParseErrInfo(ptrErrorInfo);
                            Logger.Log(opLog);
                        }
                        if (null != callback)
                        {
                            string errInfo = ApiHelper.ParseErrInfo(ptrErrorInfo);
                            bool needReboot = false;
                            if (account == mainAcct)
                            {
                                //主账号登录失败或者是其它账户不是因为密码错误都重启策略
                                needReboot = true;
                            }
                            callback.OnTradeResult(tradeSessionId, opLog, errInfo, needReboot);
                        }
                    }
                    finally
                    {
                        Marshal.FreeCoTaskMem(ptrErrorInfo);
                    }
                });
            }
            Task.WaitAll(tasks);
        }

        /// <summary>
        /// 获取单个账户里stock的持仓信息，一只股票只有一个持仓对象
        /// </summary>
        /// <param name="positions">账户所有持仓</param>
        /// <param name="code">股票代码</param>
        /// <returns>stock的持仓对象</returns>
        public static Position GetPositionOf(List<Position> positions, string code)
        {
            if (null == positions)
            {
                return null;
            }
            return positions.Find(t => t.Code == code);
        }

        /// <summary>
        /// 获取总账户stock的持仓信息
        /// </summary>
        /// <param name="accounts">账户列表</param>
        /// <param name="stock">股票代码</param>
        /// <returns>stock的持仓对象</returns>
        public static Position GetPositionOf(List<Account> accounts, string stock)
        {
            if (null == accounts)
            {
                return null;
            }
            Position position = new Position();
            foreach (Account account in accounts)
            {
                Position temp = GetPositionOf(account.Positions, stock);
                if (null == temp)
                {
                    continue;
                }
                position.AvailableBalance += temp.AvailableBalance;
                if (0 == position.AvgCost)
                {
                    position.AvgCost = temp.AvgCost;
                }
                else
                {
                    position.AvgCost = (position.AvgCost + temp.AvgCost) / 2;
                }
                position.FrozenQuantity += temp.FrozenQuantity;
                position.ProfitAndLoss += temp.ProfitAndLoss;
                if (0 == position.ProfitAndLossPct)
                {
                    position.ProfitAndLossPct = temp.ProfitAndLossPct;
                }
                else
                {
                    position.ProfitAndLossPct = (position.ProfitAndLossPct + temp.ProfitAndLossPct) / 2;
                }
                position.MarketValue += temp.MarketValue;
            }
            return position;
        }

        /// <summary>
        /// 查询所有账户总持仓：个股数据全部汇总到一起成为一个账户
        /// </summary>
        /// <param name="accounts">账户数组</param>
        /// <returns></returns>
        public static List<Position> QueryTotalPositions(List<Account> accounts)
        {
            if (null == accounts)
            {
                return null;
            }
            List<Position> positions = new List<Position>();
            List<Position> allPositions = new List<Position>(); //所有账户持仓原始数据
            List<Task> tasks = new List<Task>();
            //所有遍历账户调接口的操作都要改成这种模式，所有账户同时调
            foreach (Account account in accounts)
            {
                tasks.Add(Task.Run(() =>
                {
                    account.Positions = TradeAPI.QueryPositions(account.TradeSessionId);
                    lock (allPositions)
                    {
                        allPositions.AddRange(account.Positions);
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            List<Position> temp = allPositions.Distinct().ToList();
            foreach (Position item in temp)
            {
                Position position = new Position();
                position.Code = item.Code;
                position.Name = item.Name;
                position.Price = item.Price;
                positions.Add(position);
            }
            //把所有个股持仓数据分别汇总
            foreach (Position item in allPositions)
            {
                foreach (Position item2 in positions)
                {
                    if (item.Code == item2.Code)
                    {
                        item2.AvailableBalance += item.AvailableBalance;
                        if (item2.AvgCost == 0)
                        {
                            item2.AvgCost = item.AvgCost;
                        }
                        else
                        {
                            item2.AvgCost = (item2.AvgCost + item.AvgCost) / 2;
                        }
                        item2.FrozenQuantity += item.FrozenQuantity;
                        item2.ProfitAndLoss += item.ProfitAndLoss;
                        if (item2.ProfitAndLossPct == 0)
                        {
                            item2.ProfitAndLossPct = item.ProfitAndLossPct;
                        }
                        else
                        {
                            item2.ProfitAndLossPct = (item2.ProfitAndLossPct + item.ProfitAndLossPct) / 2;
                        }
                        item2.StockBalance += item.StockBalance;
                        item2.MarketValue += item.MarketValue;
                    }
                }
            }

            return positions;
        }

        /// <summary>
        /// 查询总账户持仓股
        /// </summary>
        /// <param name="accounts">账号列表</param>
        /// <returns></returns>
        public static List<Quotes> QueryPositionStocks(List<Account> accounts)
        {
            List<Quotes> quotes = new List<Quotes>();
            List<Position> positions = QueryTotalPositions(accounts);
            Quotes quote;
            foreach (Position position in positions)
            {
                quote = new Quotes();
                quote.Code = position.Code;
                quote.Name = position.Name;
                if (null == quotes.Find(item => item.Code == position.Code))
                {
                    quotes.Add(quote);
                }
            }

            return quotes;
        }

        /// <summary>
        /// 查询所有账户总资金
        /// </summary>
        /// <param name="accounts">账户列表</param>
        /// <returns></returns>
        public static Funds QueryTotalFunds(List<Account> accounts)
        {
            Funds funds = new Funds();
            if (null == accounts)
            {
                return null;
            }
            List<Task> tasks = new List<Task>();
            foreach (Account account in accounts)
            {
                tasks.Add(Task.Run(() =>
                {
                    Funds temp = TradeAPI.QueryFunds(account.TradeSessionId);
                    lock (funds)
                    {
                        funds.AvailableAmt += temp.AvailableAmt;
                        funds.FrozenAmt += temp.FrozenAmt;
                        funds.FundBalance += temp.FundBalance;
                        funds.TotalAsset += temp.TotalAsset;
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());

            return funds;
        }

        /// <summary>
        /// 获取code对应股票的可撤委托
        /// </summary>
        /// <param name="sessionId">会话ID</param>
        /// <param name="code">股票代码</param>
        /// <returns></returns>
        public static List<Order> GetOrdersCanCancelOf(int sessionId, string code)
        {
            List<Order> orders = TradeAPI.QueryOrdersCanCancel(sessionId);
            orders = orders.FindAll(
                order => order.Code == code && order.CanceledQuantity == 0);

            return orders;
        }

        /// <summary>
        /// 查询总账户的可撤单，按股票汇总
        /// </summary>
        /// <param name="accounts"></param>
        /// <returns></returns>
        public static List<Order> QueryTotalCancelOrders(List<Account> accounts)
        {
            if (null == accounts)
            {
                return null;
            }
            List<Order> sourceOrders = new List<Order>();
            List<Order> resultOrders = new List<Order>();
            List<Task> tasks = new List<Task>();
            foreach (Account account in accounts)
            {
                tasks.Add(Task.Run(() =>
                {
                    List<Order> temp = TradeAPI.QueryOrdersCanCancel(account.TradeSessionId);
                    lock (sourceOrders)
                    {
                        sourceOrders.AddRange(temp);
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            foreach (Order order in sourceOrders)
            {
                Order item = resultOrders.Find(temp => temp.Code == order.Code
                     && temp.Operation == order.Operation);
                if (null != item)
                {
                    item.Quantity += order.Quantity;
                    item.Price = (item.Price + order.Price) / 2;
                    item.TransactionPrice = (item.TransactionPrice + order.TransactionPrice) / 2;
                    item.TransactionQuantity += order.TransactionQuantity;
                    item.CanceledQuantity += order.CanceledQuantity;
                }
                else
                {
                    resultOrders.Add(order);
                }
            }
            return resultOrders;
        }

        /// <summary>
        /// 撤order里股票的单
        /// </summary>
        /// <param name="accounts"></param>
        /// <param name="order"></param>
        /// <param name="callback"></param>
        public static void CancelTotalOrders(List<Account> accounts, Order order, ITrade callback)
        {
            string info = "撤单【" + order.Name + "】";
            List<Task> tasks = new List<Task>();
            foreach (Account account in accounts)
            {
                tasks.Add(Task.Run(() =>
                {
                    List<Order> orders = TradeAPI.QueryOrdersCanCancel(account.TradeSessionId);
                    foreach (Order item in orders)
                    {
                        if (!(item.Name.Equals(order.Name) && item.Operation == order.Operation))
                        {
                            continue;
                        }
                        item.TradeSessionId = account.TradeSessionId;
                        int rspCode = TradeAPI.CancelOrder(item);
                        if (null != callback)
                        {
                            info = "资金账号【" + account.FundAcct + "】" + info;
                            callback.OnTradeResult(rspCode, info, item.StrErrorInfo, false);
                        }
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// 撤销不是quotes的买单以便回笼资金开新仓
        /// </summary>
        /// <param name="accounts">账户对象数组</param>
        /// <param name="quotes">股票对象</param>
        public static void CancelOrdersCanCancel(
            List<Account> accounts, Quotes quotes, ITrade callback)
        {
            List<Task> tasks = new List<Task>();
            foreach (Account account in accounts)
            {
                tasks.Add(Task.Run(() =>
                {
                    CancelOrdersCanCancel(account, quotes, callback);
                }));
            }
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// 撤销不是quotes的买单以便回笼资金开新仓
        /// </summary>
        /// <param name="account">账户对象</param>
        /// <param name="quotes">股票对象</param>
        public static void CancelOrdersCanCancel(
            Account account, Quotes quotes, ITrade callback)
        {
            List<Order> orders = TradeAPI.QueryOrdersCanCancel(account.TradeSessionId);
            if (null == orders || orders.Count == 0)
            {
                return;
            }
            foreach (Order order in orders)
            {
                if (order.Code != quotes.Code && order.Operation.Contains(Order.OperationBuy))
                {
                    order.TradeSessionId = account.TradeSessionId;
                    int rspCode = TradeAPI.CancelOrder(order);
                    string opLog = "资金账号【" + account.FundAcct + "】" + "撤销【"
                        + quotes.Name + "】委托->"
                        + (order.Quantity * order.Price).ToString("0.00####") + "万元";
                    Logger.Log(opLog);
                    if (null != callback)
                    {
                        callback.OnTradeResult(rspCode, opLog, order.StrErrorInfo, false);
                    }
                }
            }
        }

        /// <summary>
        /// 3:25可用资金自动国债逆回购，只买131810深市一天期，每股100
        /// 【要用专门接口，卖出接口不能逆回购】
        /// </summary>
        /// <param name="accounts"></param>
        public static void AutoReverseRepurchaseBonds(List<Account> accounts, ITrade callback)
        {
            Account mainAcct;
            if (accounts.Count > 0)
            {
                mainAcct = accounts[0];
            }
            else
            {
                return;
            }
            string code = "131810";
            Quotes quotes = TradeAPI.QueryQuotes(mainAcct.TradeSessionId, code);
            List<Task> tasks = new List<Task>();
            foreach (Account account in accounts)
            {
                tasks.Add(Task.Run(() =>
                {
                    //为了减少并发导致的锁和阻塞问题，每个账户都new一个对象
                    Order order = new Order();
                    order.Code = code;
                    order.Price = quotes.Buy3;
                    //要查最新资金
                    account.Funds = TradeAPI.QueryFunds(account.TradeSessionId);
                    order.TradeSessionId = account.TradeSessionId;
                    ApiHelper.SetShareholderAcct(account, quotes, order);
                    double availableCash = account.Funds.AvailableAmt;
                    order.Quantity = (int)(availableCash / 1000) * 10;
                    if (order.Quantity == 0)
                    {
                        Logger.Log("资金账号【" + account.FundAcct + "】可用金额不够逆回购");
                        return;
                    }
                    int rspCode = TradeAPI.Sell(order);
                    string opLog
                        = "资金账号【" + account.FundAcct + "】" + "逆回购" + order.Quantity * 100 + "元";
                    Logger.Log(opLog);
                    if (null != callback)
                    {
                        callback.OnTradeResult(rspCode, opLog, order.StrErrorInfo, false);
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
        }//END METHOD

        /// <summary>
        ///account账户今天是否卖出过quotes股票
        /// </summary>
        /// <param name="account">账户对象</param>
        /// <param name="quotes">股票对象</param>
        /// <returns></returns>
        public static bool IsSoldToday(Account account, Quotes quotes)
        {
            if (null == account || null == quotes)
            {
                return false;
            }
            List<Order> todayTransactions
                   = TradeAPI.QueryTodayTransaction(account.TradeSessionId);
            bool isSoldToday = false;
            if (todayTransactions.Count > 0)
            {
                foreach (Order order in todayTransactions)
                {
                    if (order.Code == quotes.Code
                        && order.Operation.Contains(Order.OperationSell))
                    {
                        isSoldToday = true;
                        break;
                    }
                }
            }
            return isSoldToday;
        }
    }
}