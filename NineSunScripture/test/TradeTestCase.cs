using NineSunScripture.model;
using NineSunScripture.strategy;
using NineSunScripture.trade.api;
using NineSunScripture.trade.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NineSunScripture.util.test
{
    class TradeTestCase
    {
        public void TestBuy(List<Account> accounts)
        {
            Order order = new Order();
            order.SessionId = accounts[0].SessionId;
            order.Category = Order.CategoryBuy;
            order.Code = "300341";
            order.Price = 13.95f;
            order.Quantity = 200;
            order.ShareholderAcct = accounts[0].SzShareholderAcct;
            TradeAPI.Buy(order);
        }

        public void TestBuyStrategy(List<Account> accounts)
        {
            int cnt = 0;
            BuyStrategy buyStrategy = new BuyStrategy();
            Quotes quotes = new Quotes();
            //卖一等于涨停价买点
            /* quotes.Code = "002713";
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

             buyStrategy.Buy(quotes, accounts, null);*/
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
            while (cnt++ < 10)
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
            quotes.Code = "002907";
            quotes.Name = "华森制药";
            quotes.PreClose = 17.22f;
            quotes.Buy1 = 18f;
            quotes.Buy1Vol = 10000;
            quotes.Sell1 = 0f;
            quotes.HighLimit = 18.94f;
            quotes.Money = 5000 * 10000;
            quotes.MoneyCtrl = 1000;
            quotes.PositionCtrl = 0.1f;
            quotes.LatestPrice = 18;
            quotes.Open = 18.2f;
            quotes.LowLimit = 15.5f;

            new SellStrategy().Sell(quotes, accounts, null);
        }
    }
}
