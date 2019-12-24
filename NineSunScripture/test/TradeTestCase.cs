﻿using NineSunScripture.model;
using NineSunScripture.strategy;
using NineSunScripture.trade.api;
using NineSunScripture.trade.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NineSunScripture.util.test
{
    class TradeTestCase
    {
        public void TestSell(List<Account> accounts)
        {
            Order order = new Order();
            order.TradeSessionId = accounts[0].TradeSessionId;
            order.Code = "131810";
            order.Price = 2.3f;
            order.Quantity = 10;
            order.ShareholderAcct = accounts[0].SzShareholderAcct;
            int rspId=TradeAPI.Sell(order);
            if (rspId>0)
            {
                MessageBox.Show("sell success");
            }
            else
            {
                MessageBox.Show("sell failed, " +ApiHelper.ParseErrInfo(order.ErrorInfo));
            }
        }

        public void TestBuy(List<Account> accounts)
        {
            Order order = new Order();
            order.TradeSessionId = accounts[0].TradeSessionId;
            order.Code = "300341";
            order.Price = 13.95f;
            order.Quantity = 200;
            order.ShareholderAcct = accounts[0].SzShareholderAcct;
            TradeAPI.Buy(order);
        }

        public void TestBuyStrategy(List<Account> accounts)
        {
            int cnt = 0;
            HitBoardStrategy buyStrategy = new HitBoardStrategy();
            Quotes quotes = new Quotes();
            //卖一等于涨停价买点
             quotes.Code = "002713";
             quotes.Name = "新易日升";
             quotes.PreClose = 7.12f;
             quotes.LatestPrice = 7.76f;
             quotes.Buy1 = 7.80f;
             quotes.Buy1Vol = 10000;
             quotes.Sell1 = 7.83f;
             quotes.HighLimit = 7.83f;
             quotes.Money = 5000 * 10000;
             quotes.MoneyCtrl = 4000;
             quotes.PositionCtrl = 0.1f;

             buyStrategy.Buy(quotes, accounts, null);
            Thread.Sleep(3000);
            //买一量小于1500万买点
            quotes = new Quotes();
            quotes.Code = "002713";
            quotes.Name = "新易日升";
            quotes.PreClose = 7.12f;
            quotes.LatestPrice = 7.83f;
            quotes.Buy1 = 7.83f;
            quotes.Buy1Vol = 390000;
            quotes.Sell1 = 0f;
            quotes.HighLimit = 7.83f;
            quotes.Money = 5000 * 10000;
            quotes.MoneyCtrl = 4000;
            quotes.PositionCtrl = 0.1f;

            buyStrategy.Buy(quotes, accounts, null);
            Thread.Sleep(3000);

            //封死涨停
            quotes = new Quotes();
            quotes.Code = "002713";
            quotes.Name = "新易日升";
            quotes.PreClose = 7.12f;
            quotes.LatestPrice = 7.83f;
            quotes.Buy1 = 7.83f;
            quotes.Buy1Vol = 4917000;
            quotes.Sell1 = 0f;
            quotes.HighLimit = 7.83f;
            quotes.Money = 5000 * 10000;
            quotes.MoneyCtrl = 4000;
            quotes.PositionCtrl = 0.1f;

            buyStrategy.Buy(quotes, accounts, null);
            Thread.Sleep(3000);
            //封死涨停
            while (cnt++ < 12)
            {
                quotes = new Quotes();
                quotes.Code = "002713";
                quotes.Name = "新易日升";
                quotes.PreClose = 7.12f;
                quotes.LatestPrice = 7.83f;
                quotes.Buy1 = 7.83f;
                quotes.Buy1Vol = 14917000;
                quotes.Sell1 = 0f;
                quotes.HighLimit = 7.83f;
                quotes.Money = 5000 * 10000;
                quotes.MoneyCtrl = 4000;
                quotes.PositionCtrl = 0.1f;

                buyStrategy.Buy(quotes, accounts, null);
                Thread.Sleep(3000);
            }
            //封单大幅减少，即将开板
            quotes = new Quotes();
            quotes.Code = "002713";
            quotes.Name = "新易日升";
            quotes.PreClose = 7.12f;
            quotes.LatestPrice = 7.83f;
            quotes.Buy1 = 7.83f;
            quotes.Buy1Vol = 17000;
            quotes.Sell1 = 0f;
            quotes.HighLimit = 7.83f;
            quotes.Money = 5000 * 10000;
            quotes.MoneyCtrl = 4000;
            quotes.PositionCtrl = 0.1f;

            buyStrategy.Buy(quotes, accounts, null);
            Thread.Sleep(3000);
            //开板
            quotes = new Quotes();
            quotes.Code = "002713";
            quotes.Name = "新易日升";
            quotes.PreClose = 7.12f;
            quotes.LatestPrice = 7.82f;
            quotes.Buy1 = 7.82f;
            quotes.Buy1Vol = 7000;
            quotes.Sell1 = 7.83f;
            quotes.HighLimit = 7.83f;
            quotes.Money = 5000 * 10000;
            quotes.MoneyCtrl = 4000;
            quotes.PositionCtrl = 0.1f;

            buyStrategy.Buy(quotes, accounts, null);
            Thread.Sleep(3000);
            cnt = 0;
            //模拟开板下跌
            while (cnt++ < 10)
            {
                quotes = new Quotes();
                quotes.Code = "002713";
                quotes.Name = "新易日升";
                quotes.PreClose = 7.12f;
                quotes.LatestPrice = (float)(7.82 - cnt / 10);
                quotes.Buy1 = quotes.LatestPrice;
                quotes.Buy1Vol = 17000;
                quotes.Sell1 = (float)(quotes.LatestPrice + 0.1);
                quotes.HighLimit = 7.83f;
                quotes.Money = 5000 * 10000;
                quotes.MoneyCtrl = 4000;
                quotes.PositionCtrl = 0.1f;

                buyStrategy.Buy(quotes, accounts, null);
                Thread.Sleep(3000);
            }
            //开板回封
            quotes = new Quotes();
            quotes.Code = "002713";
            quotes.Name = "新易日升";
            quotes.PreClose = 7.12f;
            quotes.LatestPrice = 7.82f;
            quotes.Buy1 = 7.82f;
            quotes.Buy1Vol = 7000;
            quotes.Sell1 = 7.83f;
            quotes.HighLimit = 7.83f;
            quotes.Money = 5000 * 10000;
            quotes.MoneyCtrl = 4000;
            quotes.PositionCtrl = 0.1f;
            //小量回封
            buyStrategy.Buy(quotes, accounts, null);
            Thread.Sleep(3000);
            quotes = new Quotes();
            quotes.Code = "002713";
            quotes.Name = "新易日升";
            quotes.PreClose = 7.12f;
            quotes.LatestPrice = 7.83f;
            quotes.Buy1 = 7.83f;
            quotes.Buy1Vol = 17000;
            quotes.Sell1 = 0f;
            quotes.HighLimit = 7.83f;
            quotes.Money = 5000 * 10000;
            quotes.MoneyCtrl = 4000;
            quotes.PositionCtrl = 0.1f;

            buyStrategy.Buy(quotes, accounts, null);
            Thread.Sleep(3000);
        }

        public void TestSellStrategy(List<Account> accounts)
        {
            Quotes quotes = new Quotes();
            quotes.Code = "300518";
            quotes.Name = "盛迅达";
            quotes.PreClose = 27.44f;
            quotes.Buy1 = 27f;
            quotes.Buy1Vol = 10000;
            quotes.Sell1 = 27.1f;
            quotes.HighLimit = 30f;
            quotes.Money = 5000 * 10000;
            quotes.MoneyCtrl = 1000;
            quotes.PositionCtrl = 0.1f;
            quotes.LatestPrice = 27;
            quotes.Open = 27;
            quotes.LowLimit = 24.7f;

            new SellStrategy().Sell(quotes, accounts, null);
        }
    }
}
