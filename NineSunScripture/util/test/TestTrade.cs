using NineSunScripture.model;
using NineSunScripture.trade.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.util.test
{
    class TestTrade
    {
        public void TestBuy(List<Account> accounts)
        {
            Order order = new Order();
            order.ClientId = accounts[0].ClientId;
            order.Category = Order.CategoryBuy;
            order.Code = "300341";
            order.Price = 13.95f;
            order.Quantity = 200;
            order.ShareholderAcct = accounts[0].SzShareholderAcct;
            TradeAPI.Buy(order);
        }
    }
}
