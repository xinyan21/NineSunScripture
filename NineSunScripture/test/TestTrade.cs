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

        public void TestBuyStrategy()
        {
            List<Account> accounts = AccountHelper.Login(null);
            int cnt = 0;
            Funds funds;
            BuyStrategy buyStrategy = new BuyStrategy();
            if (DateTime.Now.Second % 10 == 0)
            {
                funds = AccountHelper.QueryTotalFunds(accounts);
                Thread.Sleep(200);
            }
            Quotes quotes = new Quotes();
            quotes.Code = "002442";
            quotes.Name = "龙星化工";
            quotes.PreClose = 5.36f;
            quotes.Buy1 = 5.8f;
            quotes.Buy1Vol = 10000;
            quotes.Sell1 = 5.9f;
            quotes.HighLimit = 5.9f;
            quotes.Money = 5000 * 10000;
            quotes.MoneyCtrl = 4000;
            quotes.PositionCtrl = 0.33f;

            buyStrategy.Buy(quotes, accounts, null);

            Thread.Sleep(3000);
            quotes.Code = "002907";
            quotes.Name = "华森制药";
            quotes.PreClose = 14.23f;
            quotes.Buy1 = 15.64f;
            quotes.Buy1Vol = 10000;
            quotes.Sell1 = 15.65f;
            quotes.HighLimit = 15.65f;
            quotes.Money = 5000 * 10000;
            quotes.MoneyCtrl = 1000;
            quotes.PositionCtrl = 0.33f;

            buyStrategy.Buy(quotes, accounts, null);
        }
    }
}
