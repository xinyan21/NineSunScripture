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
        private const int IntervalOfNonTrade = 200;
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
        private ITrade callback;
        private IAcctInfoListener fundListener;

        public MainStrategy(List<Quotes> stocks, ITrade callback)
        {
            this.stocks = stocks;
            this.callback = callback;
            buyStrategy = new BuyStrategy();
            sellStrategy = new SellStrategy();
        }
        public bool Start()
        {
            if (null == stocks || stocks.Count == 0)
            {
                MessageBox.Show("没有可操作的股票");
                return false;
            }
            if (null == mainThread)
            {
                mainThread = new Thread(Process);
            }
            strategySwitch = true;
            mainThread.Start();

            return true;
        }
        public void Stop()
        {
            strategySwitch = false;
        }

        private void Process()
        {
            accounts = AccountHelper.Login(callback);
            if (null == accounts || accounts.Count == 0)
            {
                MessageBox.Show("没有可操作的账户");
                return;
            }
            mainAcct = accounts[0];
            while (strategySwitch)
            {
                Thread.Sleep(sleepInterval);
                UpdateFundsInfo();
                /*if (!IsTradeTime())
                {
                    continue;
                }*/
                Quotes quotes;
                lock (stocks)
                {
                    for (int i = 0; i < stocks.Count; i++)
                    {
                        try
                        {
                            quotes = TradeAPI.QueryQuotes(mainAcct.ClientId, stocks[i].Code);
                            buyStrategy.Buy(quotes, accounts, callback);
                            sellStrategy.Sell(quotes, accounts, callback);
                        }
                        catch (Exception e)
                        {
                            Logger.exception(e);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 每隔10s更新一下账户信息
        /// </summary>
        private void UpdateFundsInfo()
        {
            if (DateTime.Now.Second % 3 != 0)
            {
                return;
            }
            if (null != fundListener)
            {
                Account account = new Account();
                account.Funds = AccountHelper.QueryTotalFunds(accounts);
                account.Positions = AccountHelper.QueryPositions(accounts);
                fundListener.onAcctInfoListen(account);
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

        public void SellAll(ITrade callBack)
        {
            if (null == accounts || accounts.Count == 0)
            {
                MessageBox.Show("没有可操作的账户");
                return;
            }
            SellStrategy.SellAll(accounts, callBack);
        }

        public void updateStocks(List<Quotes> quotes)
        {
            //策略是在线程里跑的，修改前要加锁
            lock (stocks)
            {
                this.stocks.Clear();
                this.stocks.AddRange(quotes);
            }
        }

        public void setFundListener(IAcctInfoListener fundListener)
        {
            this.fundListener = fundListener;
        }
    }
}
