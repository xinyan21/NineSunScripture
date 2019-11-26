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
        public const string ReserveStocks = "000001|000002|000005|300233|600235";
        private const int IntervalOfNonTrade = 3000;
        //没有level2没必要设置太低
        private const int IntervalOfTrade = 1000;
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

        public MainStrategy()
        {
            this.stocks = new List<Quotes>();
            this.buyStrategy = new BuyStrategy();
            this.sellStrategy = new SellStrategy();
        }
        public bool Start()
        {
            if (null == stocks || stocks.Count == 0)
            {
                string hint = "没有可操作的股票";
                MessageBox.Show(hint);
                Logger.log(hint);
                return false;
            }
            mainThread = new Thread(Process);
            strategySwitch = true;
            mainThread.Start();

            return true;
        }
        public void Stop()
        {
            strategySwitch = false;
            if (accounts.Count > 0)
            {
                foreach (Account account in accounts)
                {
                    TradeAPI.Logoff(account.ClientId);
                }
            }
        }

        private void Process()
        {
            accounts = AccountHelper.Login(callback);
            if (null == accounts || accounts.Count == 0)
            {
                MessageBox.Show("没有可操作的账户");
                return;
            }
            UpdateFundsInfo(false);
            List<Quotes> positionStocks = AccountHelper.QueryPositionStocks(accounts);
            //TODO 实盘账户使用代码
            //stocks.AddRange(positionStocks);
            //TODO 模拟账户使用代码BEGIN
            foreach (Quotes quote in positionStocks)
            {
                if (ReserveStocks.Contains(quote.Code))
                {
                    continue;
                }
                stocks.Add(quote);
            }
            //TODO 模拟账户使用代码END
            //TODO 买了行情协议的时候主账户直接写死在程序里好点
            mainAcct = accounts[0];
            while (strategySwitch)
            {
                Thread.Sleep(sleepInterval);
                UpdateFundsInfo(true);
                if (!IsTradeTime())
                {
                    continue;
                }
                Quotes quotes;
                lock (stocks)
                {
                    for (int i = 0; i < stocks.Count; i++)
                    {
                        try
                        {
                            quotes = TradeAPI.QueryQuotes(mainAcct.ClientId, stocks[i].Code);
                            SetBuyPlan(quotes);
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
        /// 设置买入计划，以便在买策略里面直接拿到仓位和成交量限制值
        /// </summary>
        /// <param name="quotes">行情对象</param>
        private void SetBuyPlan(Quotes quotes)
        {
            foreach (Quotes item in stocks)
            {
                if (item.Code == quotes.Code)
                {
                    quotes.PositionCtrl = item.PositionCtrl;
                    quotes.MoneyCtrl = item.MoneyCtrl;
                    break;
                }
            }
        }

        /// <summary>
        /// 每隔10s更新一下账户信息
        /// </summary>
        /// <param name="ctrlFrequency">是否控制频率</param>
        private void UpdateFundsInfo(bool ctrlFrequency)
        {
            if (ctrlFrequency && DateTime.Now.Second % 3 != 0)
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
            this.stocks.Clear();
            this.stocks.AddRange(quotes);
        }

        public void setFundListener(IAcctInfoListener fundListener)
        {
            this.fundListener = fundListener;
        }

        public void setTradeCallback(ITrade callback)
        {
            this.callback = callback;
        }
    }
}
