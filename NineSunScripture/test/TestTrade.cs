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
            if (DateTime.Now.Second % 10 == 0)
            {
                funds = AccountHelper.QueryTotalFunds(accounts);
                Thread.Sleep(200);
            }
            Quotes quotes = new Quotes();
            quotes.Code = "002652";
            quotes.Name = "扬子新材";
            quotes.PreClose = 4f;
            quotes.Buy1 = 4.4f;
            quotes.Buy1Vol = 10000;
            quotes.Sell1 = 0f;
            quotes.HighLimit = 4.4f;
            quotes.Money = 5000 * 10000;
            quotes.MoneyCtrl = 4000;
            quotes.PositionCtrl = 0.1f;

            buyStrategy.Buy(quotes, accounts, null);
            buyStrategy.Buy(quotes, accounts, null);
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
            buyStrategy.Buy(quotes, accounts, null);
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
