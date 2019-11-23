using NineSunScripture.model;
using NineSunScripture.trade.api;
using NineSunScripture.trade.helper;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.strategy
{
    /// <summary>
    /// 买策略
    /// </summary>
    class BuyStrategy
    {
        private Dictionary<string, DateTime> openBoardTime = new Dictionary<string, DateTime>();
        public void Buy(Quotes quotes, List<Account> accounts, ITrade callback)
        {
            if (DateTime.Now.Hour == 14 && DateTime.Now.Minute > 30)
            {
                return;
            }
            float highLimit = quotes.HighLimit;
            float open = quotes.Open;
            string code = quotes.Code;
            float positionRatioCtrl = 1f;   //仓位比例控制
            //记录开板时间
            if (quotes.Sell1 != highLimit && !openBoardTime.ContainsKey(code))
            {
                openBoardTime.Add(code, DateTime.Now);
            }
            //重置开板时间
            if (quotes.Sell1 == highLimit && openBoardTime.ContainsKey(code))
            {
                openBoardTime.Remove(code);
            }
            //成交额小于7000万过滤
            if (quotes.Money < 7000 * 10000)
            {
                return;
            }
            //已经涨停且封单大于1500万，过滤
            if (quotes.Buy1 == highLimit && quotes.Buy2Vol * highLimit > 1500 * 10000)
            {
                return;
            }
            //买一小于7%的直线拉板，过滤
            if (quotes.Buy1 < quotes.PreClose * 1.07)
            {
                return;
            }
            //买一是涨停价、卖一或者卖二是涨停价符合买点
            if (quotes.Buy1 == highLimit || quotes.Sell1 == highLimit || quotes.Sell2 == highLimit)
            {
                if (open == highLimit)
                {
                    int openBoardInterval
                        = (int)(DateTime.Now - openBoardTime[code]).TotalSeconds;
                    //涨停开盘，开板时间小于30秒，过滤
                    if (openBoardInterval < 30)
                    {
                        Logger.log("openBoardInterval->" + openBoardInterval);
                        return;
                    }
                }
                CancelOrdersCanCancel(accounts, quotes, callback);
                Funds funds = AccountHelper.QueryTotalFunds(accounts);
                //所有账户总可用金额小于每个账号一手的金额或者小于1万，直接退出
                if (funds.AvailableAmt < 10000 || funds.AvailableAmt < highLimit * 100 * accounts.Count)
                {
                    return;
                }
                Order order = new Order();
                //开板回封买
                if (getPositionStock(accounts).Contains(code))
                {
                    if (open != highLimit)
                    {
                        positionRatioCtrl = 1 / 2;
                    }
                    order.Code = code;
                    order.Price = highLimit;
                    foreach (Account account in accounts)
                    {
                        int quantity = getTodaySoldQuantityOf(account.ClientId, code);
                        order.ClientId = account.ClientId;
                        order.Quantity = (int)(quantity * positionRatioCtrl / highLimit);
                        order.Code = code;
                        order.Price = highLimit;
                        int rspCode = TradeAPI.Buy(order);
                        string opLog
                            = account.FundAcct + "买入" + quotes.Name + "->" + order.Quantity + "股";
                        Logger.log(opLog);
                        callback.OnTradeResult(rspCode, opLog, ApiHelper.ParseErrInfo(order.ErrorInfo));
                    }
                    return;
                }
                //新开仓个股买
                order = new Order();
                order.Code = code;
                order.Price = highLimit;
                foreach (Account account in accounts)
                {
                    order.ClientId = account.ClientId;
                    //positionRatioCtrl是基于个股仓位风险控制，profitPositionCtrl是基于账户仓位风险控制
                    float profitPositionCtrl = getNewPositionRatio(account);
                    double availableCash = account.Funds.AvailableAmt;
                    if (profitPositionCtrl <= positionRatioCtrl)
                    {
                        positionRatioCtrl = profitPositionCtrl;
                    }
                    if (availableCash > account.Funds.TotalAsset * positionRatioCtrl)
                    {
                        availableCash = account.Funds.TotalAsset * positionRatioCtrl;
                    }
                    order.Quantity = (int)(availableCash / highLimit);
                    int rspCode = TradeAPI.Buy(order);
                    string opLog
                        = account.FundAcct + "买入" + quotes.Name + "->" + order.Quantity + "股";
                    Logger.log(opLog);
                    callback.OnTradeResult(rspCode, opLog, ApiHelper.ParseErrInfo(order.ErrorInfo));
                }
            }
        }

        /// <summary>
        /// 获取新开仓单股仓位>>新账户按盈利计算单股仓位，10%内1/3，10%-20%间1/2，30%以上满仓
        /// </summary>
        /// <param name="fundAcct">资金账号</param>
        /// <returns>仓位比例</returns>
        private float getNewPositionRatio(Account account)
        {
            account.Funds = TradeAPI.QueryFunds(account.ClientId);
            double totalProfitPct = account.Funds.TotalAsset / account.InitTotalAsset;
            if (totalProfitPct < 1.1)
            {
                return 1 / 3;
            }
            else if (totalProfitPct < 1.2)
            {
                return 1 / 2;
            }
            else if (totalProfitPct < 1.3)
            {
                return 0.8f;
            }
            else
            {
                return 1;
            }
        }

        /// <summary>
        /// 获取持仓股，当天已清仓的个股也会在里面，但是可用数量为0
        /// </summary>
        /// <param name="accounts">已登录账户</param>
        /// <returns></returns>
        private String getPositionStock(List<Account> accounts)
        {
            string stocks = "";
            List<Position> positions = AccountHelper.QueryPositions(accounts);
            if (null == positions || positions.Count == 0)
            {
                return stocks;
            }
            foreach (Position item in positions)
            {
                if (!stocks.Contains(item.Code))
                {
                    stocks += item.Code;
                }
            }
            return stocks;
        }

        /// <summary>
        /// 撤销不是quotes的买单以便回笼资金开新仓
        /// </summary>
        /// <param name="accounts">账户对象数组</param>
        /// <param name="quotes">股票对象</param>
        private void CancelOrdersCanCancel(List<Account> accounts, Quotes quotes, ITrade callback)
        {
            foreach (Account account in accounts)
            {
                List<Order> orders = TradeAPI.QueryOrdersCanCancel(account.ClientId);
                if (null == orders || orders.Count == 0)
                {
                    return;
                }
                foreach (Order order in orders)
                {
                    if (order.Code != quotes.Code && order.Operation == Order.OperationBuy)
                    {
                        order.ClientId = account.ClientId;
                        int rspCode = TradeAPI.CancelOrder(order);
                        string opLog = account.FundAcct + "撤销" + quotes.Name + "委托->"
                            + order.Quantity + "股";
                        Logger.log(opLog);
                        callback.OnTradeResult(rspCode, opLog, ApiHelper.ParseErrInfo(order.ErrorInfo));
                    }
                }
            }
        }

        /// <summary>
        /// 获取当天卖出的code股票数量
        /// </summary>
        /// <param name="clientId">登录账号的ID</param>
        /// <param name="code">股票代码</param>
        /// <returns></returns>
        private int getTodaySoldQuantityOf(int clientId, string code)
        {
            int quantity = 0;
            List<Order> todaySold = TradeAPI.QueryTodayTransaction(clientId);
            if (null == todaySold || todaySold.Count == 0)
            {
                return quantity;
            }
            foreach (Order order in todaySold)
            {
                if (order.Code == code && order.Operation == Order.OperationSell)
                {
                    quantity += order.Quantity;
                }
            }

            return quantity;
        }

        /// <summary>
        /// 3:25可用资金自动国债逆回购，只买131810深市一天期，每股100
        /// </summary>
        /// <param name="accounts"></param>
        private void AutoReverseRepurchaseBonds(List<Account> accounts, ITrade callback)
        {
            Account mainAcct = null;
            if (accounts.Count > 0)
            {
                mainAcct = accounts[0];
            }
            Order order = new Order();
            order.Code = "131810";
            Quotes quotes = TradeAPI.QueryQuotes(mainAcct.ClientId, order.Code);
            order.Price = quotes.Buy1;
            foreach (Account account in accounts)
            {
                order.ClientId = account.ClientId;
                double availableCash = account.Funds.AvailableAmt;
                order.Quantity = (int)(availableCash / 1000 * 10);
                int rspCode = TradeAPI.Buy(order);
                string opLog
                    = account.FundAcct + "逆回购" + (int)(availableCash / 1000) * 1000;
                Logger.log(opLog);
                callback.OnTradeResult(rspCode, opLog, ApiHelper.ParseErrInfo(order.ErrorInfo));
            }
        }
    }
}
