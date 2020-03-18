using NineSunScripture.model;
using NineSunScripture.trade.structApi.helper;
using NineSunScripture.util;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NineSunScripture.strategy
{
    /// <summary>
    /// 连板股卖策略
    /// </summary>
    public class ContBoardSellStrategy : Strategy
    {
        /// <summary>
        ///普通股三档止盈比例为20%、30%、40%
        /// </summary>
        private const float FirstClassStopWin = 20;
        private const float SecondClassStopWin = 30;
        private const float ThirdClassStopWin = 40;

        /// <summary>
        ///龙头三档止盈比例为30%、50%、70%
        /// </summary>
        private const float DragonLeaderFirstClassStopWin = 30;
        private const float DragonLeaderSecondClassStopWin = 50;
        private const float DragonLeaderThirdClassStopWin = 70;

        /// <summary>
        /// 1/2板的止盈比例
        /// </summary>
        private const float Less3BoardsStopWinRatio = 7;

        /// <summary>
        /// 三档止盈仓位为30%、50%、50%
        /// </summary>
        private const float FirstStopWinPosition = 0.3f;
        private const float SecondStopWinPosition = 0.5f;
        private const float ThirdStopWinPosition = 0.5f;

        /// <summary>
        /// 1、2板的止盈仓位
        /// </summary>
        private const float Less3BoardsStopWinPosition = 0.5f;

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

        private static ReaderWriterLockSlim rwLockSlim = new ReaderWriterLockSlim();

        public void Sell(Quotes quotes, List<Account> accounts, ITrade callback)
        {
            if (null == accounts || null == quotes)
            {
                return;
            }
            //9:30之前不卖
            if (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 30 && !MainStrategy.IsTest)
            {
                return;
            }
            float highLimit = quotes.HighLimit;
            float lowLimit = quotes.LowLimit;
            float curPrice = quotes.Buy1;
            string code = quotes.Code;
            DateTime now = DateTime.Now;

            rwLockSlim.EnterWriteLock();
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
            rwLockSlim.ExitWriteLock();

            Position position = AccountHelper.GetPositionOf(accounts, quotes.Code);
            if (!BasicCheck(quotes, position, curPrice, lowLimit))
            {
                Logger.Log("【" + quotes.Name + "】BasicCheck return");
                return;
            }
            if (quotes.AvgCost == 0)
            {
                quotes.AvgCost = position.AvgCost;
            }
            //龙头单独卖
            if (quotes.IsDragonLeader)
            {
                /* if (quotes.Buy1 <= quotes.PreClose)
                 {
                     Logger.Log("【" + quotes.Name + "】龙头绿盘卖");
                     AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                     return;
                 }*/
                StopWin(quotes, accounts, callback, true);
                if (now.Hour == 14 && now.Minute >= 55 && curPrice < highLimit)
                {
                    Logger.Log("【" + quotes.Name + "】收盘不板卖");
                    AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                }
                return;
            }
            //卖一和2是互斥的，return后就不能执行到后面去
            if (!StopWinOrLoss(accounts, callback, quotes))
            {
                OtherSells(accounts, callback, quotes);
            }
        }

        /// <summary>
        /// 卖点基础筛查，过滤无效的请求，不通过返回false，通过返回true
        /// </summary>
        /// <param name="quotes">股票对象</param>
        /// <param name="position">持仓对象</param>
        /// <param name="curPrice">当前价格</param>
        /// <param name="lowLimit">跌停价</param>
        /// <returns></returns>
        private bool BasicCheck(Quotes quotes, Position position, float curPrice, float lowLimit)
        {
            if (curPrice == lowLimit
                || null == position || quotes.Buy1 == 0 || position.AvailableBalance == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 止盈或者止损
        /// </summary>
        /// <param name="accounts">账户列表</param>
        /// <param name="callback">回调</param>
        /// <param name="quotes">股票对象</param>
        /// <returns>是否已经执行卖点</returns>
        private bool StopWinOrLoss(
            List<Account> accounts, ITrade callback, Quotes quotes)
        {
            float avgCost = quotes.AvgCost;
            float highLimit = quotes.HighLimit;
            float curPrice = quotes.Buy1;
            float open = quotes.Open;
            float preClose = quotes.PreClose;
            DateTime now = DateTime.Now;
            if (open != highLimit)
            {
                Logger.Log("【" + quotes.Name + "】StopWinOrLoss");
                StopWin(quotes, accounts, callback, false);
                if (open < quotes.PreClose * StopLossRatio)
                {
                    if (curPrice > open * 1.04)
                    {
                        Logger.Log("【" + quotes.Name + "】超低开拉升4%卖");
                        AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                        return true;
                    }
                    if (now.Hour >= 10 && now.Minute >= 10 && curPrice <= avgCost * TooWeakRatio)
                    {
                        Logger.Log("【" + quotes.Name + "】超低开10:10还小于-5%卖");
                        AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                        return true;
                    }
                }
                else
                {
                    if (curPrice <= avgCost * StopLossRatio || curPrice <= preClose * StopLossRatio)
                    {
                        Logger.Log("【" + quotes.Name + "】小于-8%卖");
                        AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                        return true;
                    }
                }
            }
            return false;
        }

        private void OtherSells(List<Account> accounts, ITrade callback, Quotes quotes)
        {
            float highLimit = quotes.HighLimit;
            float curPrice = quotes.Buy1;
            float preClose = quotes.PreClose;
            string code = quotes.Code;
            DateTime now = DateTime.Now;

            Logger.Log("【" + quotes.Name + "】OtherSells");

            if (curPrice == highLimit)
            {
                SellIfSealDecrease(accounts, quotes, callback);
                return;
            }

            rwLockSlim.EnterReadLock();
            Quotes[] ticks = historyTicks[code].ToArray();
            rwLockSlim.ExitReadLock();
            if (null != ticks && ticks.Length >= 2 &&
                ticks[ticks.Length - 2].Buy1 == highLimit && quotes.Buy1 < highLimit)
            {
                Logger.Log("【" + quotes.Name + "】开板卖");
                AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                return;
            }
            if (now.Hour == 14)
            {
                if (curPrice <= preClose * NotGoodRatio)
                {
                    Logger.Log("【" + quotes.Name + "】2:00小于1%卖");
                    AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                    return;
                }
                if (now.Minute >= 30 && curPrice <= preClose * GoodByeRatio)
                {
                    Logger.Log("【" + quotes.Name + "】2:30小于5%卖");
                    AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                    return;
                }
                if (now.Minute >= 55 && curPrice < highLimit)
                {
                    Logger.Log("【" + quotes.Name + "】收盘不板卖");
                    AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                    return;
                }
            }
        }

        /// <summary>
        /// 如果不手动设置成本，每次卖出后成本会变
        /// </summary>
        /// <param name="quotes">行情对象</param>
        /// <param name="accounts">账户数组</param>
        /// <param name="callback">交易结果回调</param>
        private void StopWin(
            Quotes quotes, List<Account> accounts, ITrade callback, bool isDragonLeader)
        {
            if (null == accounts || null == quotes)
            {
                return;
            }
            float profitPct = (quotes.Buy1 / quotes.AvgCost - 1) * 100;
            if (profitPct < FirstClassStopWin)
            {
                return;
            }
            string log = "";
            float stopWinPosition = 0;  //止盈仓位
            float firstClass = FirstClassStopWin;
            float secondClass = SecondClassStopWin;
            float thirdClass = ThirdClassStopWin;
            if (isDragonLeader)
            {
                firstClass = DragonLeaderFirstClassStopWin;
                secondClass = DragonLeaderSecondClassStopWin;
                thirdClass = DragonLeaderThirdClassStopWin;
            }
            if (profitPct > thirdClass)
            {
                stopWinPosition = ThirdStopWinPosition;
                log = thirdClass + "%止盈1/2卖【" + quotes.Name + "】";
            }
            else if (profitPct > secondClass)
            {
                stopWinPosition = SecondStopWinPosition;
                log = secondClass + "%止盈1/2卖【" + quotes.Name + "】";
            }
            else if (profitPct > firstClass)
            {
                stopWinPosition = FirstStopWinPosition;
                log = firstClass + "%止盈3成卖【" + quotes.Name + "】";
            }
            else if (profitPct > Less3BoardsStopWinRatio)
            {
                stopWinPosition = Less3BoardsStopWinPosition;
                log = Less3BoardsStopWinRatio + "%止盈5成卖【" + quotes.Name + "】";
            }
            Logger.Log(log);

            short successCnt = 0;
            List<Task> tasks = new List<Task>();
            List<Account> failAccts = new List<Account>();
            foreach (Account account in accounts)
            {
                //每个账户开个线程去处理，账户间同时操作，效率提升大大的
                tasks.Add(Task.Run(() =>
                {
                    if (stopWinPosition > 0
                    && !AccountHelper.IsSoldToday(account, quotes, callback))
                    {
                        int code
                        = AccountHelper.SellWithAcct(quotes, account, callback, stopWinPosition);
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
                }));
                Thread.Sleep(1);
            }
            Task.WaitAll(tasks.ToArray());
            if (null != callback && stopWinPosition > 0 && (successCnt + failAccts.Count) > 0)
            {
                string tradeResult = log + "止盈结果：成功账户"
                    + successCnt + "个，失败账户" + failAccts.Count + "个";
                callback.OnTradeResult(
                    MainStrategy.RspCodeOfUpdateAcctInfo, tradeResult, "", false);
                Utils.LogTradeFailedAccts(tradeResult, failAccts);
            }
        }

        /// <summary>
        /// 封单快速减少到1500万卖1/2
        /// </summary>
        /// <param name="accounts">账户列表</param>
        /// <param name="quotes">股票对象</param>
        /// <param name="callback">交易接口回调</param>
        private void SellIfSealDecrease(List<Account> accounts, Quotes quotes, ITrade callback)
        {
            rwLockSlim.EnterReadLock();
            Queue<Quotes> ticks = historyTicks[quotes.Code];
            if (ticks.Count < 2)
            {
                return;
            }
            rwLockSlim.ExitReadLock();
            Logger.Log("【" + quotes.Name + "】StopWinForLessThan3Boards ");

            if (ticks.First().Buy1Vol * quotes.HighLimit > SealMoneyBeginToDecrease * 10000
               && ticks.Last().Buy1Vol * quotes.HighLimit < MinSealMoneyToSell * 10000)
            {
                string log = "【" + quotes.Name + "】封单减少到1000万以下清 ";
                Logger.Log(log);
                Utils.ShowRuntimeInfo(callback, log);
                AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                return;
            }
            if (ticks.First().Buy1Vol * quotes.HighLimit > SealMoneyBeginToDecrease * 10000
                && ticks.Last().Buy1Vol * quotes.HighLimit < MaxSealMoneyToSell * 10000)
            {
                string log = "【" + quotes.Name + "】封单减少到1500万以下卖1/2";
                Logger.Log(log);
                Utils.ShowRuntimeInfo(callback, log);
                AccountHelper.SellByRatio(quotes, accounts, callback, 0.5f);
            }
        }
    }
}