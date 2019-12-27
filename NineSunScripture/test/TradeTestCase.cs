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
            int rspId = TradeAPI.Sell(order);
            if (rspId > 0)
            {
                MessageBox.Show("sell success");
            }
            else
            {
                MessageBox.Show("sell failed, " + ApiHelper.ParseErrInfo(order.ErrorInfo));
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

        public void TestBuyStrategy(List<Account> accounts, ITrade callback)
        {
            int cnt = 0;
            HitBoardStrategy buyStrategy = new HitBoardStrategy();
            Quotes LatestPrice = TradeAPI.QueryQuotes(accounts[0].TradeSessionId, "300278");
            //进入视野
            Quotes quotes = new Quotes();
            quotes.Name = LatestPrice.Name;
            quotes.Code = LatestPrice.Code;
            quotes.Buy1 = (float)Math.Round(LatestPrice.PreClose * 1.09f, 2);
            quotes.Buy1Vol = 10000;
            quotes.Money = 20000 * 10000;
            quotes.Sell1 = (float)Math.Round(LatestPrice.PreClose * 1.091f, 2);
            quotes.Sell1Vol = 200000;
            quotes.Open = (float)Math.Round(LatestPrice.PreClose * 1.03f, 2);
            quotes.HighLimit = (float)Math.Round(LatestPrice.PreClose * 1.1f, 2);
            quotes.PositionCtrl = 0.1f;
            buyStrategy.Buy(quotes, accounts, callback);
            //触发买点
            quotes = new Quotes();
            quotes.Name = LatestPrice.Name;
            quotes.Code = LatestPrice.Code;
            quotes.Buy1 = (float)Math.Round(LatestPrice.PreClose * 1.099f, 2);
            quotes.Buy1Vol = 10000;
            quotes.Money = 20000 * 10000;
            quotes.Sell1 = (float)Math.Round(LatestPrice.PreClose * 1.1f, 2);
            quotes.Sell1Vol = 200000;
            quotes.Open = (float)Math.Round(LatestPrice.PreClose * 1.03f, 2);
            quotes.HighLimit = (float)Math.Round(LatestPrice.PreClose * 1.1f, 2);
            quotes.PositionCtrl = 0.1f;
            buyStrategy.Buy(quotes, accounts, callback);
            //封死
            quotes = new Quotes();
            quotes.Name = LatestPrice.Name;
            quotes.Code = LatestPrice.Code;
            quotes.Buy1 = (float)Math.Round(LatestPrice.PreClose * 1.1f, 2);
            quotes.Buy1Vol = 10000000;
            quotes.Money = 20000 * 10000;
            quotes.Sell1 = 0;
            quotes.Sell1Vol = 0;
            quotes.Open = (float)Math.Round(LatestPrice.PreClose * 1.03f, 2);
            quotes.HighLimit = (float)Math.Round(LatestPrice.PreClose * 1.1f, 2);
            quotes.PositionCtrl = 0.1f;
            buyStrategy.Buy(quotes, accounts, callback);
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
