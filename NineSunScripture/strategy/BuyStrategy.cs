using NineSunScripture.model;
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
    /// 买策略
    /// </summary>
    class BuyStrategy
    {
        private Dictionary<string, DateTime> openBoardTime = new Dictionary<string, DateTime>();
        public void Buy(Quotes quotes, List<Account> accounts, ITrade trade)
        {
            if (DateTime.Now.Hour == 14 && DateTime.Now.Minute > 30)
            {
                return;
            }
            float highLimit = quotes.HighLimit;
            float open = quotes.Open;
            string code = quotes.Code;
            if (open == highLimit && quotes.Sell1 != highLimit
                && !openBoardTime.ContainsKey(code))
            {
                openBoardTime.Add(code, DateTime.Now);
            }
            if (quotes.Buy1 == highLimit && quotes.Buy2Vol * highLimit > 1500 * 10000)
            {
                return;
            }
            if (quotes.Buy1 < quotes.PreClose * 1.07)
            {
                return;
            }
            if (quotes.Buy1 == highLimit || quotes.Sell1 == highLimit || quotes.Sell2 == highLimit)
            {
                if (open == highLimit)
                {
                    int backBoardInterval = (int)(DateTime.Now - openBoardTime[code])
                        .TotalSeconds;
                    if (backBoardInterval < 30)
                    {
                        return;
                    }
                }
                float positionRatio = 1f;
                //TODO 持仓股回封单独买
                if (getPositionStock(accounts).Contains(code) && open != highLimit)
                {
                    positionRatio = 0.5f;
                    return;
                }
                Order order = new Order();
                order.Code = code;
                order.Price = highLimit;
                foreach (Account account in accounts)
                {
                    order.ClientId = account.ClientId;
                    order.Quantity = (int)(account.Funds.AvailableAmt / highLimit);
                    int rspCode = TradeAPI.Buy(order);
                    trade.OnTradeResult(rspCode, ApiHelper.ParseErrInfo(order.ErrorInfo));
                }
            }
        }


        /// <summary>
        /// 获取持仓股
        /// </summary>
        /// <param name="accounts">已登录账户</param>
        /// <returns></returns>
        private String getPositionStock(List<Account> accounts)
        {
            string stocks = "";
            foreach (Account account in accounts)
            {
                List< Position> positions = account.Positions;

            }
            return stocks;
        }
    }
}
