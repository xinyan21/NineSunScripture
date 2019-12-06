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
    class TestTrade
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
            Funds funds;
            BuyStrategy buyStrategy = new BuyStrategy();
            Quotes quotes = new Quotes();
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

            /*buyStrategy.Buy(quotes, accounts, null);
            Thread.Sleep(3000);
            quotes.Code = "002907";
            quotes.Name = "华森制药";
            quotes.PreClose = 17.22f;
            quotes.Buy1 = 18.94f;
            quotes.Buy1Vol = 10000;
            quotes.Sell1 = 0f;
            quotes.HighLimit = 18.94f;
            quotes.Money = 5000 * 10000;
            quotes.MoneyCtrl = 1000;
            quotes.PositionCtrl = 0.1f;

            buyStrategy.Buy(quotes, accounts, null);
            buyStrategy.Buy(quotes, accounts, null);*/
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
