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
        public void Buy(Quotes quotes, List<Account> accounts, ITrade trade)
        {
            if (DateTime.Now.Hour == 14 && DateTime.Now.Minute > 30)
            {
                return;
            }
            float highLimit = quotes.HighLimit;
            float open = quotes.Open;
            string code = quotes.Code;
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
                CancelOrdersCanCancel(accounts, quotes);
                Funds funds = TradeAPI.QueryTotalFunds(accounts);
                //所有账户总可用金额小于每个账号一手的金额或者小于1万，直接退出
                if (funds.AvailableAmt < 10000 || funds.AvailableAmt < highLimit * 100 * accounts.Count)
                {
                    return;
                }
                float positionRatio = 1f;   //买回仓位比例
                Order order = new Order();
                //开板回封买入
                if (getPositionStock(accounts).Contains(code))
                {
                    if (open != highLimit)
                    {
                        positionRatio = 1 / 2;
                    }
                    order.Code = code;
                    order.Price = highLimit;
                    foreach (Account account in accounts)
                    {
                        int quantity = getTodaySoldQuantityOf(account.ClientId, code);
                        order.ClientId = account.ClientId;
                        order.Quantity = (int)(quantity * positionRatio / highLimit);
                        order.Code = code;
                        order.Price = highLimit;
                        int rspCode = TradeAPI.Buy(order);
                        trade.OnTradeResult(rspCode, ApiHelper.ParseErrInfo(order.ErrorInfo));
                        string opLog
                            = account.FundAcct + "买入" + quotes.Name + "->" + order.Quantity + "股";
                        Logger.log(opLog);
                    }
                    return;
                }
                //新开仓个股买入
                order = new Order();
                order.Code = code;
                order.Price = highLimit;
                foreach (Account account in accounts)
                {
                    order.ClientId = account.ClientId;
                    order.Code = code;
                    order.Price = highLimit;
                    order.Quantity = (int)(account.Funds.AvailableAmt / highLimit);
                    int rspCode = TradeAPI.Buy(order);
                    trade.OnTradeResult(rspCode, ApiHelper.ParseErrInfo(order.ErrorInfo));
                    string sellLog
                        = account.FundAcct + "买入" + quotes.Name + "->" + order.Quantity + "股";
                    Logger.log(sellLog);
                }
            }
        }



        /// <summary>
        /// 获取持仓股
        /// </summary>
        /// <param name="accounts">已登录账户</param>
        /// <returns></returns>
        private String getPositionStock(List<Account> accounts)
        {
            string stocks = "";
            List<Position> positions = TradeAPI.QueryPositions(accounts);
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
        /// 撤销可撤的quotes对应股票的买单
        /// </summary>
        /// <param name="accounts">账户对象数组</param>
        /// <param name="quotes">股票对象</param>
        private void CancelOrdersCanCancel(List<Account> accounts, Quotes quotes)
        {
            foreach (Account acccount in accounts)
            {
                List<Order> orders = TradeAPI.QueryOrdersCanCancel(acccount.ClientId);
                foreach (Order order in orders)
                {
                    if (order.Code != quotes.Code && order.Operation == Order.OperationBuy)
                    {
                        order.ClientId = acccount.ClientId;
                        TradeAPI.CancelOrder(order);
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
            foreach (Order order in todaySold)
            {
                if (order.Code == code && order.Operation == Order.OperationSell)
                {
                    quantity += order.Quantity;
                }
            }

            return quantity;
        }
    }
}
