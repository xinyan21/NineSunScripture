using NineSunScripture.model;
using NineSunScripture.strategy;
using NineSunScripture.trade.api;
using NineSunScripture.trade.helper;
using NineSunScripture.util;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NineSunScripture
{
    /// <summary>
    /// 连板股卖策略
    /// </summary>
    public class ContBoardSellStrategy : Strategy
    {
        /// <summary>
        /// 三档止盈比例为20%、30%、40%
        /// </summary>
        private const float FirstClassStopWin = 20;
        private const float SecondClassStopWin = 30;
        private const float ThirdClassStopWin = 40;
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
        /// 封单减少到1500万就开始卖
        /// </summary>
        private const int MaxSealMoneyToSell = 1500;
        /// <summary>
        /// 封单减少到1000万就清
        /// </summary>
        private const int MinSealMoneyToSell = 1000;
        /// <summary>
        ///封单开始减少前的金额
        /// </summary>
        private const int SealMoneyBeginToDecrease = 3000;

        public void Sell(Quotes quotes, List<Account> accounts, ITrade callback)
        {
            //9:30之前不卖
            if (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 30 && !MainStrategy.IsTest)
            {
                return;
            }
            float highLimit = quotes.HighLimit;
            float lowLimit = quotes.LowLimit;
            float curPrice = quotes.LatestPrice;
            float open = quotes.Open;
            float preClose = quotes.PreClose;
            string code = quotes.Code;
            RWLockSlim.EnterWriteLock();
            if (!historyTicks.ContainsKey(code))
            {
                historyTicks.Add(code, new Queue<Quotes>(DefaultHistoryTickCnt));
            }
            if (historyTicks[code].Count == DefaultHistoryTickCnt)
            {
                historyTicks[code].Dequeue();
            }
            //由于逻辑关系会return，数据必须在进来时就放进去，上一个取倒数第二就行了
            historyTicks[code].Enqueue(quotes);
            RWLockSlim.ExitWriteLock();

            Position position = AccountHelper.GetPositionOf(accounts, quotes.Code);
            if (curPrice == lowLimit || null == position ||
                quotes.LatestPrice == 0 || quotes.Buy1 == 0 || position.AvailableBalance == 0)
            {
                return;
            }
            DateTime now = DateTime.Now;
            float avgCost = position.AvgCost;
            //龙头单独卖
            if (quotes.IsDragonLeader)
            {
                if (now.Minute >= 55 && curPrice < highLimit)
                {
                    Logger.Log("收盘不板卖" + quotes.Name);
                    SellByRatio(quotes, accounts, callback, 1);
                }
                return;
            }
            if (open != highLimit)
            {
                StopWin(quotes, accounts, callback);
                if (open < quotes.PreClose * StopLossRatio)
                {
                    if (curPrice > open * 1.04)
                    {
                        Logger.Log("超低开拉升4%卖" + quotes.Name);
                        SellByRatio(quotes, accounts, callback, 1);
                        return;
                    }
                    if (now.Hour >= 10 && now.Minute >= 10 && curPrice <= avgCost * TooWeakRatio)
                    {
                        Logger.Log("超低开10:10还小于-5%卖" + quotes.Name);
                        SellByRatio(quotes, accounts, callback, 1);
                        return;
                    }
                }
                else
                {
                    if (curPrice <= avgCost * StopLossRatio || curPrice <= preClose * StopLossRatio)
                    {
                        Logger.Log("小于-8%卖" + quotes.Name);
                        SellByRatio(quotes, accounts, callback, 1);
                        return;
                    }
                }
            }
            Quotes[] ticks = historyTicks[code].ToArray();
            if (null != ticks && ticks.Length >= 2 && ticks[ticks.Length - 2].LatestPrice == highLimit
                && quotes.Buy1 < highLimit)
            {
                Logger.Log("开板卖" + quotes.Name);
                SellByRatio(quotes, accounts, callback, 1);
                return;
            }
            if (curPrice == highLimit)
            {
                SellIfSealDecrease(accounts, quotes, callback);
                return;
            }
            if (now.Hour == 14)
            {
                if (curPrice <= preClose * NotGoodRatio)
                {
                    Logger.Log("2:00小于1%卖" + quotes.Name);
                    SellByRatio(quotes, accounts, callback, 1);
                    return;
                }
                if (now.Minute >= 30 && curPrice <= preClose * GoodByeRatio)
                {
                    Logger.Log("2:30小于5%卖" + quotes.Name);
                    SellByRatio(quotes, accounts, callback, 1);
                    return;
                }
                if (now.Minute >= 55 && curPrice < highLimit)
                {
                    Logger.Log("收盘不板卖" + quotes.Name);
                    SellByRatio(quotes, accounts, callback, 1);
                    return;
                }
            }
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
            Task[] tasks = new Task[accounts.Count];
            for (int i = 0; i < accounts.Count; i++)
            {
                Account account = accounts[i];
                //每个账户开个线程去处理，账户间同时操作，效率提升大大的
                tasks[i] = Task.Run(() =>
                {
                    Position position = AccountHelper.GetPositionOf(account.Positions, quotes.Code);
                    if (null == position)
                    {
                        return;
                    }
                    if (position.ProfitAndLossPct < FirstClassStopWin)
                    {
                        return;
                    }
                    float stopWinPosition = 0;  //止盈仓位
                    if (position.ProfitAndLossPct > ThirdClassStopWin)
                    {
                        stopWinPosition = ThirdStopWinPosition;
                        Logger.Log("40%止盈1/2卖" + quotes.Name);
                    }
                    else if (position.ProfitAndLossPct > SecondClassStopWin)
                    {
                        stopWinPosition = SecondStopWinPosition;
                        Logger.Log("30%止盈1/2卖" + quotes.Name);
                    }
                    else if (position.ProfitAndLossPct > FirstClassStopWin)
                    {
                        stopWinPosition = FirstStopWinPosition;
                        Logger.Log("20%止盈3成卖" + quotes.Name);
                    }
                    if (stopWinPosition > 0)
                    {
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
                            if (isSoldToday)
                            {
                                return;
                            }
                        }

                        SellWithAcct(quotes, account, callback, stopWinPosition);
                    }
                });
            }
            Task.WaitAll(tasks);
        }

        /// <summary>
        /// 单个账户卖出，多线程操作，引用对象修相当于外部变量要加锁或者新建个局部变量替换
        /// </summary>
        /// <param name="quotes">行情对象</param>
        /// <param name="account">账户对象</param>
        /// <param name="sellRatio">卖出比例</param>
        public static void SellWithAcct(
            Quotes stock, Account account, ITrade callback, float sellRatio)
        {
            Quotes quotes = new Quotes
            {
                Code = stock.Code,
                Name = stock.Name
            };
            //这里必须查询最新持仓，连续触发卖点信号会使得卖出失败导致策略重启
            account.Positions = TradeAPI.QueryPositions(account.TradeSessionId);
            Position position = AccountHelper.GetPositionOf(account.Positions, quotes.Code);
            if (null == position || position.AvailableBalance == 0)
            {
                return;
            }
            if (quotes.Buy2 == 0)
            {
                string name = quotes.Name;
                quotes = PriceAPI.QueryTenthGearPrice(account.PriceSessionId, quotes.Code);
                //这个行情接口不返回name
                quotes.Name = name;
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
                return;
            }
            int rspCode = TradeAPI.Sell(order);
            string opLog = "资金账号【" + account.FundAcct + "】" + "策略卖出【" + quotes.Name + "】"
                + (order.Quantity * order.Price / 10000).ToString("0.00####") + "万元";
            Logger.Log(opLog+"》"+ ApiHelper.ParseErrInfo(order.ErrorInfo));
            //TODO 这里会导致运行日志添加崩溃，后面再解决
          /* if (null != callback)
            {
                string errInfo = ApiHelper.ParseErrInfo(order.ErrorInfo);
                callback.OnTradeResult(rspCode, opLog, errInfo, false);
            }*/
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
            Task[] tasks = new Task[accounts.Count];
            for (int i = 0; i < accounts.Count; i++)
            {
                Account account = accounts[i];
                //每个账户开个线程去处理，账户间同时操作，效率提升大大的
                tasks[i] = Task.Run(() =>
                {
                    SellWithAcct(quotes, account, callback, sellRatio);
                });
            }
            Task.WaitAll(tasks);
        }

        /// <summary>
        /// 一键清仓，挂当前价-5%的价格砸，最低价是跌停价砸
        /// </summary>
        /// <param name="accounts">账户列表</param>
        /// <param name="callback">交易接口回调</param>
        public static void SellAll(List<Account> accounts, ITrade callback)
        {
            List<Position> positions;
            Quotes quotes = new Quotes();
            Task[] tasks = new Task[accounts.Count];
            for (int i = 0; i < accounts.Count; i++)
            {
                Account account = accounts[i];
                //每个账户开个线程去处理，账户间同时操作，效率提升大大的
                tasks[i] = Task.Run(() =>
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
                        quotes = PriceAPI.QueryTenthGearPrice(account.PriceSessionId, position.Code);
                        quotes.Buy2 = quotes.LatestPrice * 0.95f;
                        if (quotes.Buy2 < quotes.LowLimit)
                        {
                            quotes.Buy2 = quotes.LowLimit;
                        }
                        quotes.Buy2 = Utils.FormatTo2Digits(quotes.Buy2);

                        SellWithAcct(quotes, account, callback, 1);
                    }
                });
            }
            Task.WaitAll(tasks);
        }

        /// <summary>
        /// 封单快速减少到1500万卖1/2
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
            if (ticks.First().Buy1Vol * quotes.HighLimit > SealMoneyBeginToDecrease * 10000
               && ticks.Last().Buy1Vol * quotes.HighLimit < MinSealMoneyToSell * 10000)
            {
                Logger.Log("封单减少到1000万以下清" + quotes.Name);
                SellByRatio(quotes, accounts, callback, 1);
                return;
            }
            if (ticks.First().Buy1Vol * quotes.HighLimit > SealMoneyBeginToDecrease * 10000
                && ticks.Last().Buy1Vol * quotes.HighLimit < MaxSealMoneyToSell * 10000)
            {
                Logger.Log("封单减少到1500万以下卖1/2" + quotes.Name);
                SellByRatio(quotes, accounts, callback, 0.5f);
            }
        }
    }
}
