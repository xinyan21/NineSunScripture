﻿using NineSunScripture.model;
using NineSunScripture.trade.api;
using NineSunScripture.trade.helper;
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
    class SellStrategy
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

        private Dictionary<string, float> lastTickPrice = new Dictionary<string, float>();
        public void Sell(Quotes quotes, List<Account> accounts, ITrade trade)
        {
            float highLimit = quotes.HighLimit;
            float lowLimit = quotes.LowLimit;
            float curPrice = quotes.Sell1;
            float open = quotes.Open;
            float preClose = quotes.PreClose;
            string code = quotes.Code;
            if (curPrice == highLimit || curPrice == lowLimit)
            {
                return;
            }
            DateTime now = DateTime.Now;
            float avgCost = AccountHelper.GetPositionOf(accounts, quotes.Code).AvgCost;
            if (open != highLimit)
            {
                StopWin(quotes, accounts, trade);
                if (open < quotes.PreClose * 0.92)
                {
                    if (curPrice > open * 1.04)
                    {
                        //超低开拉升4%卖
                        Sell(quotes, accounts, trade, 1);
                    }
                    if (now.Hour >= 10 && now.Minute >= 10 && curPrice <= avgCost * 0.95)
                    {
                        //超低开10:10还小于-5%卖
                        Sell(quotes, accounts, trade, 1);
                    }
                }
                else
                {
                    if (curPrice <= avgCost * 0.92 || curPrice <= preClose * 0.92)
                    {
                        //小于-8%卖
                        Sell(quotes, accounts, trade, 1);
                    }
                    if (now.Hour == 14 && curPrice <= preClose * 1.01)
                    {
                        //2:00小于1%卖
                        Sell(quotes, accounts, trade, 1);
                    }
                    if (now.Hour == 14 && now.Minute >= 30 && curPrice <= preClose * 1.05)
                    {
                        //2:30小于5%卖
                        Sell(quotes, accounts, trade, 1);
                    }
                }
            }
            if (now.Hour == 14 && now.Minute >= 55 && curPrice < highLimit)
            {
                //收盘不板卖
                Sell(quotes, accounts, trade, 1);
            }
            if (lastTickPrice[code] == highLimit && curPrice < lastTickPrice[code])
            {
                //开板卖
                Sell(quotes, accounts, trade, 1);
            }
            lastTickPrice[code] = curPrice;
        }

        /// <summary>
        /// 止盈
        /// </summary>
        /// <param name="quotes">行情对象</param>
        /// <param name="accounts">账户数组</param>
        /// <param name="trade">交易结果回调</param>
        private void StopWin(Quotes quotes, List<Account> accounts, ITrade trade)
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
                        SellByAcct(quotes, account, trade, ThirdStopWinPosition);
                    }
                    else if (position.ProfitAndLossPct > SecondClassStopWin)
                    {
                        SellByAcct(quotes, account, trade, SecondStopWinPosition);
                    }
                    else if (position.ProfitAndLossPct > FirstClassStopWin)
                    {
                        SellByAcct(quotes, account, trade, FirstStopWinPosition);
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
        private void SellByAcct(Quotes quotes, Account account, ITrade trade, float sellRatio)
        {
            Position position = AccountHelper.GetPositionOf(account.Positions, quotes.Code);
            if (null == position)
            {
                return;
            }
            Order order = new Order();
            order.ClientId = account.ClientId;
            order.Code = quotes.Code;
            order.Quantity = position.StockAvailable;
            if (sellRatio > 0)
            {
                order.Quantity = (int)(order.Quantity * sellRatio);
            }
            int rspCode = TradeAPI.Sell(order);
            trade.OnTradeResult(rspCode, ApiHelper.ParseErrInfo(order.ErrorInfo));
        }

        /// <summary>
        /// 多账户卖出
        /// </summary>
        /// <param name="quotes">行情对象</param>
        /// <param name="accounts">账户数组</param>
        /// <param name="sellRatio">卖出比例</param>
        private void Sell(Quotes quotes, List<Account> accounts, ITrade trade, float sellRatio)
        {
            foreach (Account account in accounts)
            {
                SellByAcct(quotes, account, trade, sellRatio);
            }
        }

    }
}
