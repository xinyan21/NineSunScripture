using NineSunScripture.model;
using NineSunScripture.trade.api;
using NineSunScripture.trade.helper;
using NineSunScripture.util;
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
        /// <summary>
        /// 是否是测试状态，实盘的时候改为false
        /// </summary>
        public const bool IsTest = false;
        private const int SleepIntervalOfNonTrade = 25000;
        //没有level2没必要设置太低
        private const int SleepIntervalOfTrade = 200;

        private int sleepInterval = SleepIntervalOfTrade;
        private int tryLoginCnt = 0;
        private bool hasReverseRepurchaseBonds = false; //是否已经逆回购
        private short fundUpdateCtrl = 0;

        private Account mainAcct;
        private List<Account> accounts;
        private List<Quotes> stocks;
        private Thread mainThread;
        private BuyStrategy buyStrategy;
        private SellStrategy sellStrategy;
        private ITrade callback;
        private IAcctInfoListener fundListener;
        private IShowWorkingSatus showWorkingSatus;
        private bool isHoliday;

        public MainStrategy()
        {
            this.stocks = new List<Quotes>();
            this.buyStrategy = new BuyStrategy();
            this.sellStrategy = new SellStrategy();
            this.isHoliday = Utils.IsHolidayByDate(DateTime.Now);
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
            if (isHoliday && !IsTest)
            {
                return;
            }
            List<Quotes> positionStocks = AccountHelper.QueryPositionStocks(accounts);
            stocks.AddRange(positionStocks);
            mainAcct = accounts[0];
            bool isWorkingRight = true;
            while (true)
            {
                Thread.Sleep(sleepInterval);
                UpdateFundsInfo(true);
                if (!IsTest && !IsTradeTime())
                {
                    if (null != showWorkingSatus)
                    {
                        showWorkingSatus.RotateStatusImg(-1);
                    }
                    continue;
                }
                if (null == stocks || stocks.Count == 0)
                {
                    continue;
                }
                Quotes quotes;
                for (int i = 0; i < stocks.Count; i++)
                {
                    try
                    {
                        quotes = PriceAPI.QueryTenthGearPrice(
                            mainAcct.PriceSessionId, mainAcct.TradeSessionId, stocks[i].Code);
                        Utils.SamplingLogQuotes(quotes);
                        if (quotes.LatestPrice == 0)
                        {
                            Logger.log(quotes.ToString(), LogType.Quotes);
                            isWorkingRight = false;
                            callback.OnTradeResult(0, "策略执行发生异常", "行情接口返回0");
                            return;
                        }
                        SetBuyPlan(quotes);
                        buyStrategy.Buy(quotes, accounts, callback);
                        sellStrategy.Sell(quotes, accounts, callback);
                    }
                    catch (ThreadAbortException)
                    {
                        Logger.log("----------策略运行线程终止------------");
                    }
                    catch (Exception e)
                    {
                        isWorkingRight = false;
                        Logger.exception(e);
                        if (null != callback)
                        {
                            callback.OnTradeResult(0, "策略执行发生异常", e.Message);
                        }
                    }
                    finally
                    {
                        quotes = null;
                    }
                }//END FOR
                if (null != showWorkingSatus)
                {
                    if (isWorkingRight)
                    {
                        showWorkingSatus.RotateStatusImg(1);
                    }
                    else
                    {
                        showWorkingSatus.RotateStatusImg(-1);
                    }
                }
            }
        }

        public bool Start()
        {
            mainThread = new Thread(Process);
            mainThread.Start();

            return true;
        }

        public void Stop()
        {
            mainThread.Abort();
            if (null != accounts && accounts.Count > 0)
            {
                PriceAPI.HQ_Logoff(accounts[0].PriceSessionId);
                foreach (Account account in accounts)
                {
                    TradeAPI.Logoff(account.TradeSessionId);
                }
                accounts.Clear();
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
        /// 每隔3个心跳更新一下账户信息
        /// </summary>
        /// <param name="ctrlFrequency">是否控制频率</param>
        public void UpdateFundsInfo(bool ctrlFrequency)
        {
            if (ctrlFrequency && fundUpdateCtrl++ % 3 != 0)
            {
                if (fundUpdateCtrl > 3)
                {
                    fundUpdateCtrl = 0;
                }
                return;
            }
            if (null != fundListener && null != accounts)
            {
                Account account = new Account();
                account.Funds = AccountHelper.QueryTotalFunds(accounts);
                account.Positions = AccountHelper.QueryPositions(accounts);
                account.CancelOrders = AccountHelper.QueryTotalCancelOrders(accounts);
                fundListener.OnAcctInfoListen(account);
            }
            //调用接口要有时间间隔
            Thread.Sleep(100);
        }

        private bool IsTradeTime()
        {
            if (DateTime.Now.Hour < 9 || DateTime.Now.Hour > 14)
            {
                SetSleepIntervalOfNonTrade();
                return false;
            }
            if (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 29)
            {
                SetSleepIntervalOfNonTrade();
                return false;
            }
            if (DateTime.Now.Hour == 12 && DateTime.Now.Minute < 59)
            {
                SetSleepIntervalOfNonTrade();
                return false;
            }
            if (DateTime.Now.Hour == 11 && DateTime.Now.Minute >= 30)
            {
                SetSleepIntervalOfNonTrade();
                return false;
            }
            if (DateTime.Now.Hour == 14 && DateTime.Now.Minute >= 57)
            {
                SetSleepIntervalOfNonTrade();
                return false;
            }
            if (SleepIntervalOfTrade != sleepInterval)
            {
                sleepInterval = SleepIntervalOfTrade;
            }
            return true;
        }

        private void SetSleepIntervalOfNonTrade()
        {
            if (SleepIntervalOfNonTrade != sleepInterval)
            {
                sleepInterval = SleepIntervalOfNonTrade;
            }
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

        public void SellStock(Quotes quotes, ITrade callBack)
        {
            if (null == accounts || accounts.Count == 0)
            {
                MessageBox.Show("没有可操作的账户");
                return;
            }
            SellStrategy.SellByRatio(quotes, accounts, callBack, 1);
        }

        public void UpdateStocks(List<Quotes> quotes)
        {
            this.stocks.Clear();
            this.stocks.AddRange(quotes);
        }

        public void SetFundListener(IAcctInfoListener fundListener)
        {
            this.fundListener = fundListener;
        }

        public void SetTradeCallback(ITrade callback)
        {
            this.callback = callback;
        }

        public void SetShowWorkingStatus(IShowWorkingSatus showWorkingSatus)
        {
            this.showWorkingSatus = showWorkingSatus;
        }

        public List<Account> GetAccounts()
        {
            return accounts;
        }
    }
}
