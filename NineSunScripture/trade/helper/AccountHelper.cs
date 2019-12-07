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

namespace NineSunScripture.trade.helper
{
    public class AccountHelper
    {
        /// <summary>
        /// 登录所有账户，首次在客户端登录会记录初始总资金到数据库
        /// </summary>
        /// <param name="callback">回调接口</param>
        public static List<Account> Login(ITrade callback)
        {
            AcctDbHelper dbHelper = new AcctDbHelper();
            List<Account> dbAccounts = dbHelper.GetAccounts();
            if (null == dbAccounts || dbAccounts.Count == 0)
            {
                return null;
            }
            List<Account> loginAccts = new List<Account>();
            foreach (Account account in dbAccounts)
            {
                //TODO 新版本加了营业部ID，后面看要不要加到数据库
                int sessionId = TradeAPI.Logon(account.BrokerId, account.BrokerServerIP,
                    account.BrokerServerPort, account.VersionOfTHS, 0, account.AcctType,
                    account.FundAcct, account.Password, account.CommPwd,
                    account.IsRandomMac, account.ErrorInfo);
                if (sessionId > 0)
                {
                    account.SessionId = sessionId;
                    account.Funds = TradeAPI.QueryFunds(sessionId);
                    account.Positions = TradeAPI.QueryPositions(sessionId);
                    account.ShareHolderAccts = TradeAPI.QueryShareHolderAccts(sessionId);
                    if (account.InitTotalAsset == 0)
                    {
                        account.InitTotalAsset = (int)account.Funds.TotalAsset;
                        dbHelper.EditInitTotalAsset(account);
                    }
                    Logger.log("资金账号" + account.FundAcct + "登录成功，ID为" + sessionId);
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
                }
                else
                {
                    string opLog = "资金账号" + account.FundAcct + "登录失败，信息："
                        + ApiHelper.ParseErrInfo(account.ErrorInfo);
                    Logger.log(opLog);
                    if (null != callback)
                    {
                        callback.OnTradeResult(0, opLog, ApiHelper.ParseErrInfo(account.ErrorInfo));
                    }
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
                position.AvgCost = (position.AvgCost + temp.AvgCost) / 2;
                position.FrozenQuantity += temp.FrozenQuantity;
                position.ProfitAndLoss += temp.ProfitAndLoss;
                position.ProfitAndLossPct = (position.ProfitAndLossPct + temp.ProfitAndLossPct) / 2;
            }
            return position;
        }

        /// <summary>
        /// 查询所有账户总持仓：个股数据全部汇总到一起成为一个账户
        /// </summary>
        /// <param name="accounts">账户数组</param>
        /// <returns></returns>
        public static List<Position> QueryPositions(List<Account> accounts)
        {
            List<Position> positions = new List<Position>();
            List<Position> allPositions = new List<Position>(); //所有账户持仓原始数据
            foreach (Account account in accounts)
            {
                allPositions.AddRange(TradeAPI.QueryPositions(account.SessionId));
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
                        item2.AvgCost += item.AvgCost;
                        item2.FrozenQuantity += item.FrozenQuantity;
                        item2.ProfitAndLoss += item.ProfitAndLoss;
                        item2.ProfitAndLossPct += item.ProfitAndLossPct;
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
            List<Position> positions = QueryPositions(accounts);
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
                Funds temp = TradeAPI.QueryFunds(account.SessionId);
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
                sourceOrders.AddRange(TradeAPI.QueryOrdersCanCancel(account.SessionId));
            }
            foreach (Order order in sourceOrders)
            {
                if (resultOrders.Contains(order))
                {
                    Order item = resultOrders.Find(temp => temp.Code == order.Code
                     && temp.Operation == order.Operation);
                    item.Quantity += order.Quantity;
                    item.Price = (item.Price + order.Price) / 2;
                    item.TransactionPrice += order.TransactionPrice;
                    item.TransactionQuantity += order.TransactionQuantity;
                }
                else
                {
                    resultOrders.Add(order);
                }
            }
            return resultOrders;
        }

        public static void CancelTotalOrders(List<Account> accounts, Order order, ITrade callback)
        {
            string info = "撤单【" + order.Name + "】";
            foreach (Account account in accounts)
            {
                order.SessionId = account.SessionId;
                int rspCode = TradeAPI.CancelOrder(order);
                if (null != callback)
                {
                    info = "资金账号【" + account.FundAcct + "】" + info;
                    callback.OnTradeResult(rspCode, info, ApiHelper.ParseErrInfo(order.ErrorInfo));
                }
            }
        }
    }
}
