﻿using NineSunScripture.model;
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
        /// 三档止盈比例为20%、30%、40%
        /// </summary>
        private const float FirstClassStopWin = 20;

        private const float SecondClassStopWin = 30;
        private const float ThirdClassStopWin = 40;

        /// <summary>
        /// 1/2板的止盈比例
        /// </summary>
        private const float Less3BoardsStopWinRatio = 1.07f;

        /// <summary>
        /// 三档止盈仓位为30%、50%、50%
        /// </summary>
        private const float FirstStopWinPosition = 0.3f;

        private const float SecondStopWinPosition = 0.5f;
        private const float ThirdStopWinPosition = 0.5f;

        /// <summary>
        /// 1/2板的止盈仓位
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
                return;
            }
            if (quotes.AvgCost == 0)
            {
                quotes.AvgCost = position.AvgCost;
            }
            //龙头单独卖
            if (quotes.IsDragonLeader)
            {
                Logger.Log("收盘不板卖" + quotes.Name);
                if (now.Minute >= 55 && curPrice < highLimit)
                {
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
        /// <returns></returns>
        private bool StopWinOrLoss(
            List<Account> accounts, ITrade callback, Quotes quotes)
        {
            float avgCost = quotes.AvgCost;
            float highLimit = quotes.HighLimit;
            float curPrice = quotes.LatestPrice;
            float open = quotes.Open;
            float preClose = quotes.PreClose;
            DateTime now = DateTime.Now;
            if (open != highLimit)
            {
                StopWin(quotes, accounts, callback);
                StopWinForLessThan3Boards(accounts, quotes, callback);
                if (open < quotes.PreClose * StopLossRatio)
                {
                    if (curPrice > open * 1.04)
                    {
                        Logger.Log("超低开拉升4%卖" + quotes.Name);
                        AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                        return false;
                    }
                    if (now.Hour >= 10 && now.Minute >= 10 && curPrice <= avgCost * TooWeakRatio)
                    {
                        Logger.Log("超低开10:10还小于-5%卖" + quotes.Name);
                        AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                        return false;
                    }
                }
                else
                {
                    if (curPrice <= avgCost * StopLossRatio || curPrice <= preClose * StopLossRatio)
                    {
                        Logger.Log("小于-8%卖" + quotes.Name);
                        AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                        return false;
                    }
                }
            }
            return true;
        }

        private void OtherSells(List<Account> accounts, ITrade callback, Quotes quotes)
        {
            float highLimit = quotes.HighLimit;
            float curPrice = quotes.LatestPrice;
            float preClose = quotes.PreClose;
            string code = quotes.Code;
            DateTime now = DateTime.Now;

            rwLockSlim.EnterReadLock();
            Quotes[] ticks = historyTicks[code].ToArray();
            rwLockSlim.ExitReadLock();
            if (null != ticks && ticks.Length >= 2 &&
                ticks[ticks.Length - 2].LatestPrice == highLimit && quotes.Buy1 < highLimit)
            {
                Logger.Log("开板卖" + quotes.Name);
                AccountHelper.SellByRatio(quotes, accounts, callback, 1);
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
                    AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                    return;
                }
                if (now.Minute >= 30 && curPrice <= preClose * GoodByeRatio)
                {
                    Logger.Log("2:30小于5%卖" + quotes.Name);
                    AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                    return;
                }
                if (now.Minute >= 55 && curPrice < highLimit)
                {
                    Logger.Log("收盘不板卖" + quotes.Name);
                    AccountHelper.SellByRatio(quotes, accounts, callback, 1);
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
            if (profitPct > ThirdClassStopWin)
            {
                stopWinPosition = ThirdStopWinPosition;
                log = "40%止盈1/2卖" + quotes.Name;
            }
            else if (profitPct > SecondClassStopWin)
            {
                stopWinPosition = SecondStopWinPosition;
                log = "30%止盈1/2卖" + quotes.Name;
            }
            else if (profitPct > FirstClassStopWin)
            {
                stopWinPosition = FirstStopWinPosition;
                log = "20%止盈3成卖" + quotes.Name;
            }

            short successCnt = 0;
            List<Task> tasks = new List<Task>();
            List<Account> failAccts = new List<Account>();
            foreach (Account account in accounts)
            {
                //每个账户开个线程去处理，账户间同时操作，效率提升大大的
                tasks.Add(Task.Run(() =>
                {
                    if (stopWinPosition > 0 && !AccountHelper.IsSoldToday(account, quotes, callback))
                    {
                        int code = AccountHelper.SellWithAcct(quotes, account, callback, stopWinPosition);
                        lock (failAccts)
                        {
                            if (code <= 0)
                            {
                                failAccts.Add(account);
                            }
                            if (code != 888)
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
                string tradeResult = "【" + quotes.Name + "】止盈结果：成功账户"
                    + successCnt + "个，失败账户" + failAccts.Count + "个";
                callback.OnTradeResult(MainStrategy.RspCodeOfUpdateAcctInfo, tradeResult, "", false);
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
            Quotes[] ticks = historyTicks[quotes.Code].ToArray();
            rwLockSlim.ExitReadLock();
            if (ticks.Length < 2)
            {
                return;
            }
            if (ticks.First().Buy1Vol * quotes.HighLimit > SealMoneyBeginToDecrease * 10000
               && ticks.Last().Buy1Vol * quotes.HighLimit < MinSealMoneyToSell * 10000)
            {
                string log = "封单减少到1000万以下清" + quotes.Name;
                Logger.Log(log);
                Utils.ShowRuntimeInfo(callback, log);
                AccountHelper.SellByRatio(quotes, accounts, callback, 1);
                return;
            }
            if (ticks.First().Buy1Vol * quotes.HighLimit > SealMoneyBeginToDecrease * 10000
                && ticks.Last().Buy1Vol * quotes.HighLimit < MaxSealMoneyToSell * 10000)
            {
                string log = "封单减少到1500万以下卖1/2" + quotes.Name;
                Logger.Log(log);
                Utils.ShowRuntimeInfo(callback, log);
                AccountHelper.SellByRatio(quotes, accounts, callback, 0.5f);
            }
        }

        /// <summary>
        /// 3板以下止盈卖点
        /// </summary>
        /// <param name="accounts">账户列表</param>
        /// <param name="quotes">股票对象</param>
        /// <param name="callback">日志回显接口</param>
        private void StopWinForLessThan3Boards(
            List<Account> accounts, Quotes quotes, ITrade callback)
        {
            bool isUpRatioNotEnough = quotes.AvgCost > 0
                && quotes.Sell1 / quotes.AvgCost < Less3BoardsStopWinRatio;
            //下降期不检查连板数，到Less3BoardsStopWinRatio就止盈
            bool isContBoardsNotQualified = Utils.IsUpPeriod() ? quotes.ContBoards >= 3 : false;
            if (null == accounts || null == quotes || isContBoardsNotQualified || isUpRatioNotEnough)
            {
                return;
            }

            short successCnt = 0;
            List<Task> tasks = new List<Task>();
            List<Account> failAccts = new List<Account>();
            foreach (Account account in accounts)
            {
                tasks.Add(Task.Run(() =>
                {
                    if (AccountHelper.IsSoldToday(account, quotes, callback))
                    {
                        return;
                    }
                    int code = AccountHelper.SellWithAcct(
                        quotes, account, callback, Less3BoardsStopWinPosition);
                    lock (failAccts)
                    {
                        if (code <= 0)
                        {
                            failAccts.Add(account);
                        }
                        if (code != 888)
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
                string tradeResult = "3板以下止盈卖结果：成功账户"
                + (accounts.Count - failAccts.Count) + "个，失败账户" + failAccts.Count + "个";
                callback.OnTradeResult(MainStrategy.RspCodeOfUpdateAcctInfo, tradeResult, "", false);
                Utils.LogTradeFailedAccts(tradeResult, failAccts);
            }
        }
    }
}