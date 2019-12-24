using NineSunScripture.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using NineSunScripture.trade.api;
using NineSunScripture.strategy;
using NineSunScripture.db;
using NineSunScripture.util.log;
using System.Configuration;

namespace NineSunScripture.trade.helper
{
    public class AccountHelper
    {
        /// <summary>
        /// 默认主账号为账号列表的第一个
        /// </summary>
        /// <returns></returns>
        private static Account GetMainAccount()
        {
            Account account = new Account();
            account.AcctType = 0x30;
            account.BrokerId = 59;
            account.BrokerName = "平安证券";
            account.BrokerServerIP = "43.254.105.92";
            account.BrokerServerPort = 8003;
            account.VersionOfTHS = "E065.20.92";
            account.CommPwd = "";
            account.SalesDepartId = 105;
            account.IsRandomMac = false;
            account.FundAcct = "321019194496";
            account.PriceAcct = "13534068934";
            account.FundPassword = "198921";
            account.PricePassword = "3594035x";
            return account;
        }

        /// <summary>
        /// 登录所有账户，首次在客户端登录会记录初始总资金到数据库
        /// </summary>
        /// <param name="callback">回调接口</param>
        public static List<Account> Login(ITrade callback)
        {
            AcctDbHelper dbHelper = new AcctDbHelper();
            List<Account> dbAccounts = dbHelper.GetAccounts();
            Account mainAcct = GetMainAccount();
            dbAccounts.Insert(0, mainAcct);
            if (null == dbAccounts || dbAccounts.Count == 0)
            {
                return null;
            }
            byte[] byteArrErrorInfo = new byte[256];
            int priceSessionId = PriceAPI.HQ_Logon(
                mainAcct.PriceAcct, mainAcct.PricePassword, byteArrErrorInfo);
            if (priceSessionId > 0)
            {
                mainAcct.PriceSessionId = priceSessionId;
                callback.OnTradeResult(1, "行情登录", "", false);
            }
            else if (null != callback)
            {
                string errInfo = ApiHelper.ParseErrInfo(byteArrErrorInfo);
                callback.OnTradeResult(0, "行情登录", errInfo, true);
            }
            List<Account> loginAccts = new List<Account>();
            foreach (Account account in dbAccounts)
            {
                //TODO 新版本加了营业部ID，后面看要不要加到数据库，后面接口改成字符串了，以后看需求升级
                int tradeSessionId = TradeAPI.Logon(account.BrokerId, account.BrokerServerIP,
                    account.BrokerServerPort, account.VersionOfTHS, "0", account.AcctType,
                    account.FundAcct, account.FundPassword, account.CommPwd,
                    account.IsRandomMac, byteArrErrorInfo);
                string opLog = "";
                if (tradeSessionId > 0)
                {
                    account.TradeSessionId = tradeSessionId;
                    account.Funds = TradeAPI.QueryFunds(tradeSessionId);
                    account.Positions = TradeAPI.QueryPositions(tradeSessionId);
                    account.ShareHolderAccts = TradeAPI.QueryShareHolderAccts(tradeSessionId);
                    if (account.InitTotalAsset == 0)
                    {
                        account.InitTotalAsset = (int)account.Funds.TotalAsset;
                        dbHelper.EditInitTotalAsset(account);
                    }
                    if (account.ShareHolderAccts.Count > 0)
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
                    loginAccts.Add(account);
                    opLog = "资金账号【" + account.FundAcct + "】登录成功，会话ID为" + tradeSessionId;
                    Logger.Log(opLog);
                }
                else
                {
                    opLog = "资金账号【" + account.FundAcct + "】登录失败，信息："
                        + ApiHelper.ParseErrInfo(byteArrErrorInfo);
                    Logger.Log(opLog);
                }
                if (null != callback)
                {
                    string errInfo = ApiHelper.ParseErrInfo(byteArrErrorInfo);
                    bool needReboot = false;
                    if (account == mainAcct || !errInfo.Contains("密码"))
                    {
                        //主账号登录失败或者是其它账户不是因为密码错误都重启策略
                        needReboot = true;
                    }
                    callback.OnTradeResult(tradeSessionId, opLog, errInfo, needReboot);
                }
            }
            return loginAccts;
        }

        /// <summary>
        /// 获取单个账户里stock的持仓信息，一只股票只有一个持仓对象
        /// </summary>
        /// <param name="positions">账户所有持仓</param>
        /// <param name="stock">股票代码</param>
        /// <returns>stock的持仓对象</returns>
        public static Position GetPositionOf(List<Position> positions, string stock)
        {
            foreach (Position position in positions)
            {
                if (position.Code == stock)
                {
                    return position;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取总账户stock的持仓信息
        /// </summary>
        /// <param name="accounts">账户列表</param>
        /// <param name="stock">股票代码</param>
        /// <returns>stock的持仓对象</returns>
        public static Position GetPositionOf(List<Account> accounts, string stock)
        {
            Position position = null;
            foreach (Account account in accounts)
            {
                Position temp = GetPositionOf(account.Positions, stock);
                if (null == temp)
                {
                    continue;
                }
                if (null == position)
                {
                    position = new Position();
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
            List<Position> positions = new List<Position>();
            List<Position> allPositions = new List<Position>(); //所有账户持仓原始数据
            foreach (Account account in accounts)
            {
                allPositions.AddRange(TradeAPI.QueryPositions(account.TradeSessionId));
            }
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
                return funds;
            }
            foreach (Account account in accounts)
            {
                Funds temp = TradeAPI.QueryFunds(account.TradeSessionId);
                funds.AvailableAmt += temp.AvailableAmt;
                funds.FrozenAmt += temp.FrozenAmt;
                funds.FundBalance += temp.FundBalance;
                funds.TotalAsset += temp.TotalAsset;
            }

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
            orders = orders.FindAll(order => order.Code == code);

            return orders;
        }

        /// <summary>
        /// 查询总账户的可撤单，按股票汇总
        /// </summary>
        /// <param name="accounts"></param>
        /// <returns></returns>
        public static List<Order> QueryTotalCancelOrders(List<Account> accounts)
        {
            List<Order> sourceOrders = new List<Order>();
            List<Order> resultOrders = new List<Order>();
            foreach (Account account in accounts)
            {
                sourceOrders.AddRange(TradeAPI.QueryOrdersCanCancel(account.TradeSessionId));
            }
            foreach (Order order in sourceOrders)
            {
                if (resultOrders.Contains(order))
                {
                    Order item = resultOrders.Find(temp => temp.Code == order.Code
                     && temp.Operation == order.Operation);
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
            foreach (Account account in accounts)
            {
                List<Order> orders = TradeAPI.QueryOrdersCanCancel(account.TradeSessionId);
                foreach (Order item in orders)
                {
                    if (!item.Name.Equals(order.Name))
                    {
                        continue;
                    }
                    item.TradeSessionId = account.TradeSessionId;
                    int rspCode = TradeAPI.CancelOrder(item);
                    if (null != callback)
                    {
                        info = "资金账号【" + account.FundAcct + "】" + info;
                        string errInfo = ApiHelper.ParseErrInfo(item.ErrorInfo);
                        callback.OnTradeResult(rspCode, info, errInfo, false);
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
            Account mainAcct = null;
            if (accounts.Count > 0)
            {
                mainAcct = accounts[0];
            }
            else
            {
                return;
            }
            Order order = new Order();
            order.Code = "131810";
            Quotes quotes = TradeAPI.QueryQuotes(mainAcct.TradeSessionId, order.Code);
            order.Price = quotes.Buy3;
            foreach (Account account in accounts)
            {
                order.TradeSessionId = account.TradeSessionId;
                ApiHelper.SetShareholderAcct(account, quotes, order);
                double availableCash = account.Funds.AvailableAmt;
                order.Quantity = (int)(availableCash / 1000) * 10;
                if (order.Quantity == 0)
                {
                    Logger.Log("资金账号【" + account.FundAcct + "】可用金额不够逆回购");
                    continue;
                }
                int rspCode = TradeAPI.Sell(order);
                string opLog
                    = "资金账号【" + account.FundAcct + "】" + "逆回购" + order.Quantity * 100 + "元";
                Logger.Log(opLog);
                if (null != callback)
                {
                    string errInfo = ApiHelper.ParseErrInfo(order.ErrorInfo);
                    callback.OnTradeResult(rspCode, opLog, errInfo, false);
                }
            }//END FOR
        }//END METHOD
    }
}
