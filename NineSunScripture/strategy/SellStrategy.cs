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
    /// 卖策略
    /// </summary>
    public class SellStrategy
    {
        /// <summary>
        /// 三档止盈比例为20%、30%、40%
        /// </summary>
        private const float FirstClassStopWin = 0.2f;
        private const float SecondClassStopWin = 0.3f;
        private const float ThirdClassStopWin = 0.4f;
        /// <summary>
        /// 三档止盈仓位为30%、50%、50%
        /// </summary>
        private const float FirstStopWinPosition = 0.3f;
        private const float SecondStopWinPosition = 0.5f;
        private const float ThirdStopWinPosition = 0.5f;
        /// <summary>
        /// 止损跌幅
        /// </summary>
        private const float StopLossRatio = 0.92f;
        /// <summary>
        /// 太弱跌幅
        /// </summary>
        private const float TooWeakRatio = 0.95f;
        /// <summary>
        /// 2点不够强涨幅
        /// </summary>
        private const float NotGoodRatio = 1.01f;
        /// <summary>
        /// 2:30不够强卖出涨幅
        /// </summary>
        private const float GoodByeRatio = 1.05f;

        private Dictionary<string, float> lastTickPrice = new Dictionary<string, float>();
        public void Sell(Quotes quotes, List<Account> accounts, ITrade callback)
        {
            float highLimit = quotes.HighLimit;
            float lowLimit = quotes.LowLimit;
            float curPrice = quotes.Sell1;
            float open = quotes.Open;
            float preClose = quotes.PreClose;
            string code = quotes.Code;
            Position position = AccountHelper.GetPositionOf(accounts, quotes.Code);
            if (curPrice == highLimit || curPrice == lowLimit || null == position)
            {
                return;
            }
            DateTime now = DateTime.Now;
            float avgCost = position.AvgCost;
            if (open != highLimit)
            {
                StopWin(quotes, accounts, callback);
                if (open < quotes.PreClose * StopLossRatio)
                {
                    if (curPrice > open * 1.04)
                    {
                        Logger.log("超低开拉升4%卖" + quotes.Name);
                        Sell(quotes, accounts, callback, 1);
                    }
                    if (now.Hour >= 10 && now.Minute >= 10 && curPrice <= avgCost * TooWeakRatio)
                    {
                        Logger.log("超低开10:10还小于-5%卖" + quotes.Name);
                        Sell(quotes, accounts, callback, 1);
                    }
                }
                else
                {
                    if (curPrice <= avgCost * StopLossRatio || curPrice <= preClose * StopLossRatio)
                    {
                        Logger.log("小于-8%卖" + quotes.Name);
                        Sell(quotes, accounts, callback, 1);
                    }
                    if (now.Hour == 14 && curPrice <= preClose * NotGoodRatio)
                    {
                        Logger.log("2:00小于1%卖" + quotes.Name);
                        Sell(quotes, accounts, callback, 1);
                    }
                    if (now.Hour == 14 && now.Minute >= 30 && curPrice <= preClose * GoodByeRatio)
                    {
                        Logger.log("2:30小于5%卖" + quotes.Name);
                        Sell(quotes, accounts, callback, 1);
                    }
                }
            }
            if (now.Hour == 14 && now.Minute >= 55 && curPrice < highLimit)
            {
                Logger.log("收盘不板卖" + quotes.Name);
                Sell(quotes, accounts, callback, 1);
            }
            if (lastTickPrice[code] == highLimit && curPrice < lastTickPrice[code])
            {
                Logger.log("开板卖" + quotes.Name);
                Sell(quotes, accounts, callback, 1);
            }
            lastTickPrice[code] = curPrice;
        }

        /// <summary>
        /// TODO 【重点】止盈 这里有个卖出之后成本降低导致收益增高的问题，解决方法一个是在本地记录成本
        /// 还有就是开板清掉买回，这个成本如何算
        /// 好像就只有20%这档会触发，除非一直不开板到结尾
        /// </summary>
        /// <param name="quotes">行情对象</param>
        /// <param name="accounts">账户数组</param>
        /// <param name="callback">交易结果回调</param>
        private void StopWin(Quotes quotes, List<Account> accounts, ITrade callback)
        {
            foreach (Account account in accounts)
            {
                List<Order> todayTransactions = TradeAPI.QueryTodayTransaction(account.ClientId);
                bool isSoldToday = false;
                if (todayTransactions.Count > 0)
                {
                    foreach (Order order in todayTransactions)
                    {
                        if (order.Code == quotes.Code)
                        {
                            isSoldToday = true;
                            break;
                        }
                    }
                    if (isSoldToday)
                    {
                        continue;
                    }
                }
                else
                {
                    Position position = AccountHelper.GetPositionOf(account.Positions, quotes.Code);
                    if (null == position)
                    {
                        continue;
                    }
                    if (position.ProfitAndLossPct > ThirdClassStopWin)
                    {
                        Logger.log("40%止盈1/2卖" + quotes.Name);
                        SellByAcct(quotes, account, callback, ThirdStopWinPosition);
                    }
                    else if (position.ProfitAndLossPct > SecondClassStopWin)
                    {
                        Logger.log("30%止盈1/2卖" + quotes.Name);
                        SellByAcct(quotes, account, callback, SecondStopWinPosition);
                    }
                    else if (position.ProfitAndLossPct > FirstClassStopWin)
                    {
                        Logger.log("20%止盈3成卖" + quotes.Name);
                        SellByAcct(quotes, account, callback, FirstStopWinPosition);
                    }
                }
            }
        }

        /// <summary>
        /// 单个账户卖出
        /// </summary>
        /// <param name="quotes">行情对象</param>
        /// <param name="account">账户对象</param>
        /// <param name="sellRatio">卖出比例</param>
        private static void SellByAcct(Quotes quotes, Account account, ITrade callback, float sellRatio)
        {
            //因为是卖出，所以当天登录时候的仓位就可以拿来用，如果是买那就得查询最新的
            Position position = AccountHelper.GetPositionOf(account.Positions, quotes.Code);
            if (null == position || position.AvailableQuantity == 0)
            {
                return;
            }
            Order order = new Order();
            order.ClientId = account.ClientId;
            order.Code = quotes.Code;
            order.Quantity = position.AvailableQuantity;
            if (sellRatio > 0)
            {
                order.Quantity = (int)(order.Quantity * sellRatio);
            }
            int rspCode = TradeAPI.Sell(order);
            string opLog = account.FundAcct + "策略卖出" + quotes.Name + "->"
                + order.Quantity * order.Price + "元";
            Logger.log(opLog);
            callback.OnTradeResult(rspCode, opLog, ApiHelper.ParseErrInfo(order.ErrorInfo));
        }

        /// <summary>
        /// 多账户卖出
        /// </summary>
        /// <param name="quotes">行情对象</param>
        /// <param name="accounts">账户数组</param>
        /// <param name="sellRatio">卖出比例</param>
        private static void Sell(Quotes quotes, List<Account> accounts, ITrade callback, float sellRatio)
        {
            foreach (Account account in accounts)
            {
                SellByAcct(quotes, account, callback, sellRatio);
            }
        }

        /// <summary>
        /// 清仓
        /// </summary>
        /// <param name="accounts">账户列表</param>
        /// <param name="callback">交易接口回调</param>
        public static void SellAll(List<Account> accounts, ITrade callback)
        {
            List<Position> positions;
            Quotes quotes = new Quotes();
            foreach (Account account in accounts)
            {
                positions = TradeAPI.QueryPositions(account.ClientId);
                if (null == positions || positions.Count == 0)
                {
                    continue;
                }
                foreach (Position position in positions)
                {
                    //TODO 模拟盘代码
                    if (MainStrategy.ReserveStocks.Contains(position.Code))
                    {
                        continue;
                    }
                    quotes.Code = position.Code;
                    quotes.Name = position.Name;
                    SellByAcct(quotes, account, callback, 1);
                }
            }
        }
    }
}
