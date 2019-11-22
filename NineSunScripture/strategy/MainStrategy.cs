using NineSunScripture.model;
using NineSunScripture.trade.api;
using NineSunScripture.trade.helper;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineSunScripture.strategy
{
    /// <summary>
    /// 主策略
    /// </summary>
    public class MainStrategy
    {
        private const int IntervalOfNonTrade = 3000;
        private const int IntervalOfTrade = 200;
        //交易时间睡眠间隔为200ms，非交易时间为3s
        private int sleepInterval = IntervalOfTrade;
        private int tryLoginCnt = 0;
        private bool hasReverseRepurchaseBonds = false; //是否已经逆回购
        private bool strategySwitch = true; //策略开关：true开，false关
        private Account mainAcct;
        private List<Account> accounts;
        private List<Quotes> stocks;
        private Thread mainThread;
        private BuyStrategy buyStrategy;
        private SellStrategy sellStrategy;
        private ITrade onTradeResult;

        public MainStrategy(List<Account> accounts, List<Quotes> stocks, ITrade onTradeResult)
        {
            this.accounts = accounts;
            this.stocks = stocks;
            this.onTradeResult = onTradeResult;
            if (accounts.Count > 0)
            {
                mainAcct = accounts[0];
            }
        }
        public void Start()
        {
            if (null == stocks || stocks.Count == 0)
            {
                MessageBox.Show("没有可操作的股票");
                return;
            }
            if (null == accounts || accounts.Count == 0)
            {
                MessageBox.Show("没有可操作的账户");
                return;
            }
            if (null == mainThread)
            {
                mainThread = new Thread(Process);
            }
            strategySwitch = true;
            mainThread.Start();
        }
        public void Stop()
        {
            strategySwitch = false;
        }

        private void Process()
        {
            AccountHelper.Login(accounts, onTradeResult);
            //到时改到总开关里面去
            while (strategySwitch)
            {
                Thread.Sleep(sleepInterval);
                if (!IsTradeTime())
                {
                    continue;
                }
                Quotes quotes;
                for (int i = 0; i < stocks.Count; i++)
                {
                    try
                    {
                        quotes = TradeAPI.QueryQuotes(mainAcct.ClientId, stocks[i].Code);
                        buyStrategy.Buy(quotes, accounts, onTradeResult);
                        sellStrategy.Sell(quotes, accounts, onTradeResult);
                    }
                    catch (Exception e)
                    {
                        Logger.exception(e);
                    }
                }
            }

        }

        private bool IsTradeTime()
        {
            if (DateTime.Now.Hour < 9 || DateTime.Now.Hour == 12 || DateTime.Now.Hour >= 15)
            {
                if (IntervalOfNonTrade != sleepInterval)
                {
                    sleepInterval = IntervalOfNonTrade;
                }
                return false;
            }
            if (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 29)
            {
                if (IntervalOfNonTrade != sleepInterval)
                {
                    sleepInterval = IntervalOfNonTrade;
                }
                return false;
            }
            if (DateTime.Now.Hour == 11 && DateTime.Now.Minute > 30)
            {
                if (IntervalOfNonTrade != sleepInterval)
                {
                    sleepInterval = IntervalOfNonTrade;
                }
                return false;
            }
            if (IntervalOfTrade != sleepInterval)
            {
                sleepInterval = IntervalOfTrade;
            }
            return true;
        }

        public void OnTradeResult(int code, string msg)
        {
            if (code < 0 && tryLoginCnt < 3)
            {
            }
        }
    }
}
