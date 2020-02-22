using NineSunScripture.model;
using NineSunScripture.strategy;
using NineSunScripture.trade.persistence;
using NineSunScripture.trade.structApi.api;
using NineSunScripture.util;
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
            account.InitTotalAsset = 20000;
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
            List<Account> localAccounts = JsonDataHelper.Instance.GetAccounts();
            if (null == localAccounts)
            {
                localAccounts = new List<Account>();
            }
            localAccounts.Insert(0, GetTestMainAccount());
            if (!MainStrategy.IsTest)
            {
                localAccounts.Insert(0, GetPrdMainAccount());
            }
            Account mainAcct = localAccounts[0];
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
                        callback.OnTradeResult(1, "行情登录成功", "", false);
                    }
                }
                else
                {
                    string errInfo = ApiHelper.ParseErrInfo(ptrErrorInfo);
                    if (null != callback)
                    {
                        callback.OnTradeResult(0, "行情登录失败", errInfo, true);
                    }
                    return null;
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
            ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();
            short successCnt = 0;
            short failCnt = 0;
            List<Task> tasks = new List<Task>();
            foreach (Account account in localAccounts)
            {
                tasks.Add(Task.Run(() =>
                {
                    IntPtr ptrErrorInfo = Marshal.AllocCoTaskMem(256);
                    try
                    {
                        int tradeSessionId = TradeAPI.Logon(account.BrokerId, account.BrokerServerIP,
                             account.BrokerServerPort, account.VersionOfTHS, account.SalesDepartId,
                             account.AcctType, account.FundAcct, account.FundPassword,
                             account.CommPwd, account.IsRandomMac, ptrErrorInfo);
                        string opLog = "";
                        if (tradeSessionId > 0)
                        {
                            lockSlim.EnterWriteLock();
                            successCnt++;
                            lockSlim.ExitWriteLock();
                            account.TradeSessionId = tradeSessionId;
                            account.Funds = TradeAPI.QueryFunds(tradeSessionId);
                            account.ShareHolderAccts = TradeAPI.QueryShareHolderAccts(tradeSessionId);
                            account.Positions = TradeAPI.QueryPositions(tradeSessionId);
                            if (account.InitTotalAsset == 0)
                            {
                                account.InitTotalAsset = (int)account.Funds.TotalAsset;
                                JsonDataHelper.Instance.InitTotalAsset(account);
                            }
                            if (null != account.ShareHolderAccts && account.ShareHolderAccts.Count > 0)
                            {
                                foreach (ShareHolderAcct shareHolderAcct in account.ShareHolderAccts)
                                {
                                    if (shareHolderAcct.Category == "上海A股")
                                    {
                                        account.ShShareholderAcct = shareHolderAcct.Code;
                                    }
                                    else
                                    {
                                        account.SzShareholderAcct = shareHolderAcct.Code;
                                    }
                                }
                            }
                            //把行情的PriceSessionId给其它账户，就不用每次都拿主账户的PriceSessionId给其它账户
                            if (account != mainAcct)
                            {
                                account.PriceSessionId = mainAcct.PriceSessionId;
                            }
                            loginAccts.Add(account);
                            opLog
                            = "资金账号【" + account.FundAcct + "】登录成功，会话ID为" + tradeSessionId;
                            Logger.Log(opLog);
                        }
                        else
                        {
                            lockSlim.EnterWriteLock();
                            failCnt++;
                            lockSlim.ExitWriteLock();
                            opLog = "资金账号【" + account.FundAcct + "】登录失败，错误信息："
                                + ApiHelper.ParseErrInfo(ptrErrorInfo);
                            Logger.Log(opLog);
                            if (account == mainAcct)
                            {
                                string errInfo = ApiHelper.ParseErrInfo(ptrErrorInfo);
                                callback.OnTradeResult(tradeSessionId, opLog, errInfo, true);
                            }
                        }
                    }
                    finally
                    {
                        Marshal.FreeCoTaskMem(ptrErrorInfo);
                    }
                }));
                Thread.Sleep(1);
            }
            Task.WaitAll(tasks.ToArray());
            if (null != callback)
            {
                bool needReboot = failCnt > 2 ? true : false;
                string info = "交易登录：成功" + successCnt + "个，失败" + failCnt + "个";
                callback.OnTradeResult(needReboot ? 0 : 1, info, "登录失败个数较多", needReboot);
            }
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
            short cnt = 0;
            foreach (Account account in accounts)
            {
                Position temp = GetPositionOf(account.Positions, stock);
                if (null == temp)
                {
                    continue;
                }
                cnt++;
                position.AvailableBalance += temp.AvailableBalance;
                position.AvgCost += temp.AvgCost;
                position.FrozenQuantity += temp.FrozenQuantity;
                position.ProfitAndLoss += temp.ProfitAndLoss;
                position.ProfitAndLossPct += temp.ProfitAndLossPct;
                position.MarketValue += temp.MarketValue;
            }
            position.AvgCost /= cnt;
            position.ProfitAndLossPct /= cnt;

            return position;
        }

        /// <summary>
        /// 查询所有账户总持仓：个股数据全部汇总到一起成为一个账户
        /// </summary>
        /// <param name="accounts">账户数组</param>
        /// <returns></returns>
        public static List<Position> QueryTotalPositions(List<Account> accounts, ITrade callback)
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
                    try
                    {
                        account.Positions = TradeAPI.QueryPositions(account.TradeSessionId);
                        lock (allPositions)
                        {
                            allPositions.AddRange(account.Positions);
                        }
                    }
                    catch (ApiTimeoutException e)
                    {
                        ApiHelper.HandleCriticalException(e, e.Message, callback);
                    }
                    catch (Exception e)
                    {
                        Logger.Exception(e);
                    }
                }));
                Thread.Sleep(1);
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
            Dictionary<string, short> positionCnt = new Dictionary<string, short>();
            //把所有个股持仓数据分别汇总
            foreach (Position item in allPositions)
            {
                foreach (Position item2 in positions)
                {
                    if (item.Code == item2.Code)
                    {
                        if (positionCnt.ContainsKey(item.Code))
                        {
                            positionCnt[item.Code] += 1;
                        }
                        else
                        {
                            positionCnt.Add(item.Code, 1);
                        }
                        item2.AvailableBalance += item.AvailableBalance;
                        item2.AvgCost += item.AvgCost;
                        item2.FrozenQuantity += item.FrozenQuantity;
                        item2.ProfitAndLoss += item.ProfitAndLoss;
                        item2.ProfitAndLossPct += item.ProfitAndLossPct;
                        item2.StockBalance += item.StockBalance;
                        item2.MarketValue += item.MarketValue;
                    }
                }
            }
            foreach (var item in positions)
            {
                item.AvgCost /= positionCnt[item.Code];
                item.ProfitAndLossPct /= positionCnt[item.Code];
            }
            //按市值降序排列
            positions.Sort((x, y) => { return y.MarketValue.CompareTo(x.MarketValue); });

            return positions;
        }

        /// <summary>
        /// 查询总账户持仓股
        /// </summary>
        /// <param name="accounts">账号列表</param>
        /// <returns></returns>
        public static List<Quotes> QueryPositionStocks(List<Account> accounts, ITrade callback)
        {
            List<Quotes> quotes = new List<Quotes>();
            try
            {
                List<Position> positions = QueryTotalPositions(accounts, callback);
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
            }
            catch (ApiTimeoutException e)
            {
                ApiHelper.HandleCriticalException(e, e.Message, callback);
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }

            return quotes;
        }

        /// <summary>
        /// 查询所有账户总资金
        /// </summary>
        /// <param name="accounts">账户列表</param>
        /// <returns></returns>
        public static Funds QueryTotalFunds(List<Account> accounts, ITrade callback)
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
                    try
                    {
                        Funds temp = TradeAPI.QueryFunds(account.TradeSessionId);
                        account.Funds = temp;
                        lock (funds)
                        {
                            funds.AvailableAmt += temp.AvailableAmt;
                            funds.FrozenAmt += temp.FrozenAmt;
                            funds.FundBalance += temp.FundBalance;
                            funds.TotalAsset += temp.TotalAsset;
                        }
                    }
                    catch (ApiTimeoutException e)
                    {
                        ApiHelper.HandleCriticalException(e, e.Message, callback);
                    }
                    catch (Exception e)
                    {
                        Logger.Exception(e);
                    }
                }));
                Thread.Sleep(1);
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
        public static List<Order> QueryTotalCancelOrders(List<Account> accounts, ITrade callback)
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
                    try
                    {
                        List<Order> temp = TradeAPI.QueryOrdersCanCancel(account.TradeSessionId);
                        account.CancelOrders = temp;
                        lock (sourceOrders)
                        {
                            sourceOrders.AddRange(temp);
                        }
                    }
                    catch (ApiTimeoutException e)
                    {
                        ApiHelper.HandleCriticalException(e, e.Message, callback);
                    }
                    catch (Exception e)
                    {
                        Logger.Exception(e);
                    }
                }));
                Thread.Sleep(1);
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
            if (null == order || null == accounts)
            {
                return;
            }
            short successCnt = 0;
            short failCnt = 0;
            string info = "撤单【" + order.Name + "】";
            List<Task> tasks = new List<Task>();
            foreach (Account account in accounts)
            {
                tasks.Add(Task.Run(() =>
                {
                    try
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
                            lock (tasks)
                            {
                                if (rspCode <= 0)
                                {
                                    failCnt++;
                                }
                                if (rspCode != 888)
                                {
                                    successCnt++;
                                }
                            }
                        }
                    }
                    catch (ApiTimeoutException e)
                    {
                        ApiHelper.HandleCriticalException(e, e.Message, callback);
                    }
                    catch (Exception e)
                    {
                        Logger.Exception(e);
                    }
                }));
                Thread.Sleep(1);
            }
            Task.WaitAll(tasks.ToArray());
            if (null != callback)
            {
                info = info + "成功账户个数：" + successCnt + "，失败账户个数：" + failCnt;
                callback.OnTradeResult(MainStrategy.RspCodeOfUpdateAcctInfo, info, "", false);
            }
        }

        /// <summary>
        /// 撤销不是quotes的买单以便回笼资金开新仓
        /// </summary>
        /// <param name="accounts">账户对象数组</param>
        /// <param name="quotes">股票对象</param>
        public static void CancelOrdersCanCancel(
            List<Account> accounts, Quotes quotes, ITrade callback)
        {
            if (null == accounts)
            {
                return;
            }
            List<Task> tasks = new List<Task>();
            foreach (Account account in accounts)
            {
                tasks.Add(Task.Run(() =>
                {
                    CancelOrdersCanCancel(account, quotes, callback);
                }));
                Thread.Sleep(1);
            }
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// 撤销不是quotes的买单以便回笼资金开新仓
        /// </summary>
        /// <param name="account">账户对象</param>
        /// <param name="quotes">股票对象</param>
        public static int CancelOrdersCanCancel(
            Account account, Quotes quotes, ITrade callback)
        {
            if (null == account)
            {
                return 888;
            }
            try
            {
                List<Order> orders = TradeAPI.QueryOrdersCanCancel(account.TradeSessionId);
                if (null == orders || orders.Count == 0)
                {
                    return 888;
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
                        return rspCode;
                    }
                }
            }
            catch (ApiTimeoutException e)
            {
                ApiHelper.HandleCriticalException(e, e.Message, callback);
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
            return 888;
        }

        /// <summary>
        /// 3:25可用资金自动国债逆回购，只买131810深市一天期，每股100
        /// 【要用专门接口，卖出接口不能逆回购】
        /// </summary>
        /// <param name="accounts"></param>
        public static void ReverseRepurchaseBonds(List<Account> accounts, ITrade callback)
        {
            Account mainAcct;
            if (null != accounts && accounts.Count > 0)
            {
                mainAcct = accounts[0];
            }
            else
            {
                return;
            }
            string code = "131810";
            Quotes quotes = PriceAPI.QueryTenthGearPrice(mainAcct.PriceSessionId, code);
            short successCnt = 0;
            List<Task> tasks = new List<Task>();
            List<Account> failAccts = new List<Account>();
            foreach (Account account in accounts)
            {
                tasks.Add(Task.Run(() =>
                {
                    try
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
                        lock (failAccts)
                        {
                            if (rspCode <= 0)
                            {
                                failAccts.Add(account);
                            }
                            else if (rspCode != 888)
                            {
                                successCnt++;
                            }
                        }
                        string opLog
                            = "资金账号【" + account.FundAcct + "】" + "逆回购" + order.Quantity * 100 + "元";
                        Logger.Log(opLog);
                    }
                    catch (ApiTimeoutException e)
                    {
                        ApiHelper.HandleCriticalException(e, e.Message, callback);
                    }
                    catch (Exception e)
                    {
                        Logger.Exception(e);
                    }
                }));
                Thread.Sleep(1);
            }
            Task.WaitAll(tasks.ToArray());
            if (null != callback && (successCnt + failAccts.Count) > 0)
            {
                string tradeResult = "逆回购结果：成功账户"
                    + successCnt + "个，失败账户" + failAccts.Count + "个";
                callback.OnTradeResult(
                    MainStrategy.RspCodeOfUpdateAcctInfo, tradeResult, "", false);
                Utils.LogTradeFailedAccts(tradeResult, failAccts);
            }
        }//END METHOD

        /// <summary>
        ///account账户今天是否卖出过quotes股票
        /// </summary>
        /// <param name="account">账户对象</param>
        /// <param name="quotes">股票对象</param>
        /// <returns></returns>
        public static bool IsSoldToday(Account account, Quotes quotes, ITrade callback)
        {
            if (null == account || null == quotes)
            {
                return false;
            }
            bool isSoldToday = false;
            try
            {
                List<Order> todayTransactions
                        = TradeAPI.QueryTodayTransaction(account.TradeSessionId);
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
            }
            catch (ApiTimeoutException e)
            {
                ApiHelper.HandleCriticalException(e, e.Message, callback);
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
            return isSoldToday;
        }

        /// <summary>
        /// 一键清仓，挂当前价-5%的价格砸，最低价是跌停价砸
        /// </summary>
        /// <param name="accounts">账户列表</param>
        /// <param name="callback">交易接口回调</param>
        public static void SellAll(List<Account> accounts, ITrade callback)
        {
            if (null == accounts)
            {
                return;
            }
            short successCnt = 0;
            List<Position> positions;
            List<Task> tasks = new List<Task>();
            List<Account> failAccts = new List<Account>();
            foreach (Account account in accounts)
            {
                //每个账户开个线程去处理，账户间同时操作，效率提升大大的
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        positions = TradeAPI.QueryPositions(account.TradeSessionId);
                        if (null == positions || positions.Count == 0)
                        {
                            return;
                        }
                        foreach (Position position in positions)
                        {
                            if (0 == position.AvailableBalance)
                            {
                                continue;
                            }
                            Quotes quotes
                            = PriceAPI.QueryTenthGearPrice(account.PriceSessionId, position.Code);
                            quotes.Buy2 = quotes.Buy1 * 0.95f;
                            quotes.Name = position.Name;
                            if (quotes.Buy2 < quotes.LowLimit)
                            {
                                quotes.Buy2 = quotes.LowLimit;
                            }
                            quotes.Buy2 = Utils.FormatTo2Digits(quotes.Buy2);
                            int code = SellWithAcct(quotes, account, callback, 1);
                            lock (failAccts)
                            {
                                if (code <= 0)
                                {
                                    failAccts.Add(account);
                                }
                                else if (code != 888)
                                {
                                    successCnt++;
                                }
                            }
                        }
                    }
                    catch (ApiTimeoutException e)
                    {
                        ApiHelper.HandleCriticalException(e, e.Message, callback);
                    }
                    catch (Exception e)
                    {
                        Logger.Exception(e);
                    }
                }));
                Thread.Sleep(1);
            }
            Task.WaitAll(tasks.ToArray());
            if (null != callback && (successCnt + failAccts.Count) > 0)
            {
                string tradeResult
                    = "一键清仓结果：成功账户" + successCnt + "个，失败账户" + failAccts.Count + "个";
                callback.OnTradeResult(
                    MainStrategy.RspCodeOfUpdateAcctInfo, tradeResult, "", false);
                Utils.LogTradeFailedAccts(tradeResult, failAccts);
            }
        }


        /// <summary>
        /// 单个账户卖出，多线程操作，引用对象修相当于外部变量要加锁或者新建个局部变量替换
        /// </summary>
        /// <param name="quotes">行情对象</param>
        /// <param name="account">账户对象</param>
        /// <param name="sellRatio">卖出比例</param>
        public static int SellWithAcct(
            Quotes stock, Account account, ITrade callback, float sellRatio)
        {
            if (null == stock || null == account)
            {
                return 888;
            }
            Quotes quotes = new Quotes
            {
                Code = stock.Code,
                Name = stock.Name,
                //这里要把输入的价格传进来，否则就查询最新价格直接卖出了
                Buy2 = stock.Buy2
            };
            try
            {
                //这里必须查询最新持仓，连续触发卖点信号会使得卖出失败导致策略重启
                account.Positions = TradeAPI.QueryPositions(account.TradeSessionId);
                Position position = GetPositionOf(account.Positions, quotes.Code);
                if (null == position || position.AvailableBalance == 0)
                {
                    return 888;
                }
                if (quotes.Buy2 == 0)
                {
                    quotes = PriceAPI.QueryTenthGearPrice(account.PriceSessionId, quotes.Code);
                    //这个行情接口不返回name
                    quotes.Name = stock.Name;
                }
                Order order = new Order();
                order.TradeSessionId = account.TradeSessionId;
                order.Code = quotes.Code;
                order.Price = quotes.Buy2;
                order.Quantity = position.AvailableBalance;
                ApiHelper.SetShareholderAcct(account, quotes, order);
                if (sellRatio > 0)
                {
                    order.Quantity = Utils.FixQuantity((int)(order.Quantity * sellRatio));
                }
                if (order.Quantity == 0)
                {
                    return 888;
                }
                int rspCode = TradeAPI.Sell(order);
                string opLog = "资金账号【" + account.FundAcct + "】" + "策略卖出【" + quotes.Name + "】"
                    + (order.Quantity * order.Price / 10000).ToString("0.00####") + "万元";
                string tradeResult = rspCode > 0 ? "#成功" : "#失败：" + order.StrErrorInfo;
                Logger.Log(opLog + tradeResult);
                return rspCode;
            }
            catch (ApiTimeoutException e)
            {
                ApiHelper.HandleCriticalException(e, e.Message, callback);
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
            return 888;
        }

        /// <summary>
        /// 多账户卖出
        /// </summary>
        /// <param name="quotes">行情对象</param>
        /// <param name="accounts">账户数组</param>
        /// <param name="sellRatio">卖出比例</param>
        public static void SellByRatio(
            Quotes quotes, List<Account> accounts, ITrade callback, float sellRatio)
        {
            if (null == accounts || null == quotes)
            {
                return;
            }
            short successCnt = 0;
            List<Task> tasks = new List<Task>();
            List<Account> failAccts = new List<Account>();
            foreach (Account account in accounts)
            {
                //每个账户开个线程去处理，账户间同时操作，效率提升大大的
                tasks.Add(Task.Run(() =>
                {
                    int code = SellWithAcct(quotes, account, callback, sellRatio);
                    lock (failAccts)
                    {
                        if (code <= 0)
                        {
                            failAccts.Add(account);
                        }
                        else if (code != 888)
                        {
                            successCnt++;
                        }
                    }
                }));
                Thread.Sleep(1);
            }
            Task.WaitAll(tasks.ToArray());
            if (null != callback && (successCnt + failAccts.Count) > 0)
            {
                string tradeResult
                   = "【" + quotes.Name + "】卖出" + sellRatio * 100 + "%仓位结果：成功账户"
                   + successCnt + "个，失败账户" + failAccts.Count + "个";
                callback.OnTradeResult(
                    MainStrategy.RspCodeOfUpdateAcctInfo, tradeResult, "", false);
                Utils.LogTradeFailedAccts(tradeResult, failAccts);
            }
        }

        /// <summary>
        /// 获取当天成交的code股票数量
        /// </summary>
        /// <param name="sessionId">登录账号的ID</param>
        /// <param name="code">股票代码</param>
        /// <param name="op">操作方向</param>
        /// <returns></returns>
        public static int GetTodayTransactionQuantityOf(int sessionId, string code, string op)
        {
            int quantity = 0;
            List<Order> todayTransactions = TradeAPI.QueryTodayTransaction(sessionId);
            if (null == todayTransactions || todayTransactions.Count == 0)
            {
                return quantity;
            }
            foreach (Order order in todayTransactions)
            {
                if (order.Code == code && order.Operation.Contains(op))
                {
                    quantity += order.Quantity;
                }
            }

            return quantity;
        }
    }
}