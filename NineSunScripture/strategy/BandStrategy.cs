using NineSunScripture.model;
using NineSunScripture.trade.structApi.helper;
using System.Collections.Generic;

namespace NineSunScripture.strategy
{
    /// <summary>
    /// 波段策略
    /// </summary>
    internal class BandStrategy
    {
        public void Process(List<Account> accounts, Quotes quotes, ITrade callback)
        {
            if (null == accounts || null == quotes)
            {
                return;
            }
            if (quotes.StopWinPrice > 0 && quotes.Buy1 > quotes.StopWinPrice)
            {
                StopWin(accounts, quotes, callback);
            }
            if (quotes.StopLossPrice > 0 && quotes.Buy1 < quotes.StopLossPrice)
            {
                StopLoss(accounts, quotes, callback);
            }
        }

        private void StopWin(List<Account> accounts, Quotes quotes, ITrade callback)
        {
            AccountHelper.SellByRatio(quotes, accounts, callback, 1);
        }

        private void StopLoss(List<Account> accounts, Quotes quotes, ITrade callback)
        {
            AccountHelper.SellByRatio(quotes, accounts, callback, 1);
        }

        private void Buy(List<Account> accounts, Quotes quotes, ITrade callback)
        {
        }
    }
}