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
        /// <summary>
        /// 队列的大小由查询频率控制，保存一分钟的tick数据，主要是供SellIfSealDecrease使用
        /// </summary>
        private Dictionary<string, Queue<Quotes>> historyTicks
            = new Dictionary<string, Queue<Quotes>>();

        public void Sell(Quotes quotes, List<Account> accounts, ITrade callback)
        {
            float highLimit = quotes.HighLimit;
            float lowLimit = quotes.LowLimit;
            float curPrice = quotes.Sell1;
            float open = quotes.Open;
            float preClose = quotes.PreClose;
            string code = quotes.Code;
            if (!historyTicks.ContainsKey(code))
            {
                historyTicks.Add(code, new Queue<Quotes>(60));
            }
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
            Quotes[] ticks = historyTicks[code].ToArray();
            if (ticks.Length > 1 && ticks.Last().Sell1 == highLimit && curPrice < highLimit)
            {
                Logger.log("开板卖" + quotes.Name);
                Sell(quotes, accounts, callback, 1);
            }
            SellIfSealDecrease(accounts, quotes, callback);
            if (historyTicks[code].Count == 60)
            {
                historyTicks[code].Dequeue();
            }
            historyTicks[code].Enqueue(quotes);
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
                List<Order> todayTransactions = TradeAPI.QueryTodayTransaction(account.SessionId);
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
        public static void SellByAcct(Quotes quotes, Account account, ITrade callback, float sellRatio)
        {
            //因为是卖出，所以当天登录时候的仓位就可以拿来用，如果是买那就得查询最新的
            Position position = AccountHelper.GetPositionOf(account.Positions, quotes.Code);
            if (null == position || position.AvailableBalance == 0)
            {
                return;
            }
            if (quotes.LatestPrice == 0)
            {
                quotes = TradeAPI.QueryQuotes(account.SessionId, quotes.Code);
            }
            Order order = new Order();
            order.SessionId = account.SessionId;
            order.Code = quotes.Code;
            order.Price = quotes.Buy2;
            order.Quantity = position.AvailableBalance;
            ApiHelper.SetShareholderAcct(account, quotes, order);
            if (sellRatio > 0)
            {
                order.Quantity = (int)(order.Quantity * sellRatio);
            }
            int rspCode = TradeAPI.Sell(order);
            string opLog = account.FundAcct + "策略卖出【" + quotes.Name + "】"
                + (order.Quantity * order.Price).ToString("0.00####") + "万元";
            Logger.log(opLog);
            if (null != callback)
            {
                callback.OnTradeResult(rspCode, opLog, ApiHelper.ParseErrInfo(account.ErrorInfo));
            }
        }

        /// <summary>
        /// 多账户卖出
        /// </summary>
        /// <param name="quotes">行情对象</param>
        /// <param name="accounts">账户数组</param>
        /// <param name="sellRatio">卖出比例</param>
        public static void Sell(Quotes quotes, List<Account> accounts, ITrade callback, float sellRatio)
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
                positions = TradeAPI.QueryPositions(account.SessionId);
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

        /// <summary>
        /// 封单减少到2000万卖
        /// </summary>
        /// <param name="accounts">账户列表</param>
        /// <param name="quotes">股票对象</param>
        /// <param name="callback">交易接口回调</param>
        private void SellIfSealDecrease(List<Account> accounts, Quotes quotes, ITrade callback)
        {
            Quotes[] ticks = historyTicks[quotes.Code].ToArray();
            if (ticks.Length < 2)
            {
                return;
            }
            if (ticks.First().Buy1Vol * quotes.HighLimit > 3000 * 10000
                && ticks.Last().Buy1Vol * quotes.HighLimit < 2000 * 10000)
            {
                Logger.log("封单减少到2000万卖" + quotes.Name);
                Sell(quotes, accounts, callback);
            }
        }
    }
}
