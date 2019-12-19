using NineSunScripture.model;
using NineSunScripture.trade.api;
using NineSunScripture.trade.helper;
using NineSunScripture.util;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Threading;
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
        public static bool IsTest = false;
        private const int SleepIntervalOfNonTrade = 25000;
        //没有level2没必要设置太低
        private const int SleepIntervalOfTrade = 200;

        private int sleepInterval = SleepIntervalOfTrade;
        private short fundUpdateCtrl = 0;
        private short queryPriceErrorCnt = 0;

        private Account mainAcct;
        private List<Account> accounts;
        //stocks是供买入的股票池，stocksForPrice是用来查询行情的股票池（包括卖的）
        private List<Quotes> stocksToBuy, stocksForPrice;
        private List<Quotes> dragonLeaders;
        private Thread strategyThread;
        private BuyStrategy buyStrategy;
        private SellStrategy sellStrategy;
        private ITrade callback;
        private IAcctInfoListener fundListener;
        private IShowWorkingSatus showWorkingSatus;
        private bool isHoliday;

        public MainStrategy()
        {
            this.stocksToBuy = new List<Quotes>();
            this.stocksForPrice = new List<Quotes>();
            this.buyStrategy = new BuyStrategy();
            this.sellStrategy = new SellStrategy();
            this.isHoliday = Utils.IsHolidayByDate(DateTime.Now);
        }

        private void Process()
        {
            accounts = AccountHelper.Login(callback);
            if (null == accounts || accounts.Count == 0)
            {
                callback.OnTradeResult(0, "策略启动", "没有可操作的账户", false);
                return;
            }
            UpdateFundsInfo(false);
            if (isHoliday && !IsTest)
            {
                callback.OnTradeResult(0, "策略启动", "现在是假期", false);
                return;
            }
            stocksForPrice.AddRange(stocksToBuy);
            List<Quotes> positionStocks = AccountHelper.QueryPositionStocks(accounts);
            foreach (Quotes item in positionStocks)
            {
                //龙头只有持仓里可能有
                if (null != dragonLeaders && dragonLeaders.Contains(item))
                {
                    item.IsDragonLeader = true;
                }
                //如果股票池里有持仓股说明可以继续做，没有就不能做InPosition要设置成true
                if (!stocksForPrice.Contains(item))
                {
                    item.InPosition = true;
                    stocksForPrice.Add(item);
                }
            }
            mainAcct = accounts[0];
            bool isWorkingRight = true;
            queryPriceErrorCnt = 0;
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
                    Logger.Log("Not trade time, pass");
                    continue;
                }
                if (null == stocksForPrice || stocksForPrice.Count == 0)
                {
                    Logger.Log("No stocks to Query");
                    continue;
                }
                Quotes quotes = null;
                for (int i = 0; i < stocksForPrice.Count; i++)
                {
                    try
                    {
                        quotes = PriceAPI.QueryTenthGearPrice(
                            mainAcct.PriceSessionId, mainAcct.TradeSessionId, stocksForPrice[i].Code);
                        if (null == quotes || quotes.LatestPrice == 0 || string.IsNullOrEmpty(quotes.Name))
                        {
                            queryPriceErrorCnt++;
                            Logger.Log("QueryTenthGearPrice error " + queryPriceErrorCnt);
                            if (queryPriceErrorCnt < 3)
                            {
                                continue;
                            }
                        }
                        if (queryPriceErrorCnt > 2)
                        {
                            Logger.Log("QueryTenthGearPrice error has been occured 3 times, need to reboot");
                            if (null != quotes)
                            {
                                Logger.Log(quotes.ToString(), LogType.Quotes);
                            }
                            isWorkingRight = false;
                            callback.OnTradeResult(0, "调用行情接口", "行情接口异常", true);
                            return;
                        }
                        queryPriceErrorCnt = 0;
                        Utils.SamplingLogQuotes(quotes);
                        SetTradeParams(quotes);
                        if (positionStocks.Contains(quotes))
                        {
                            sellStrategy.Sell(quotes, accounts, callback);
                        }
                        //持仓股回封要买回，所以全部股票都在买的范围
                        buyStrategy.Buy(quotes, accounts, callback);
                    }
                    catch (ThreadAbortException)
                    {
                        Logger.Log("----------策略运行线程终止------------");
                    }
                    catch (Exception e)
                    {
                        isWorkingRight = false;
                        Logger.Exception(e);
                        callback.OnTradeResult(0, "策略执行发生异常", e.Message, true);
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

            if (!Utils.DetectTHSDll())
            {
                MessageBox.Show("dll不存在，不能启动策略->" + System.Environment.CurrentDirectory);
                return false;
            }

            strategyThread = new Thread(Process);
            strategyThread.Start();

            return true;
        }

        public void Stop()
        {
            strategyThread.Abort();
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
        /// 设置交易计划，以便在策略里面直接拿到龙头、仓位和成交量限制值
        /// </summary>
        /// <param name="quotes">行情对象</param>
        private void SetTradeParams(Quotes quotes)
        {
            Quotes source = stocksForPrice.Find(item => item.Code == quotes.Code);
            if (null == source)
            {
                return;
            }
            quotes.PositionCtrl = source.PositionCtrl;
            quotes.MoneyCtrl = source.MoneyCtrl;
            quotes.InPosition = source.InPosition;
            quotes.IsDragonLeader = source.IsDragonLeader;
        }

        /// <summary>
        /// 每隔15个心跳更新一下账户信息
        /// </summary>
        /// <param name="ctrlFrequency">是否控制频率</param>
        public void UpdateFundsInfo(bool ctrlFrequency)
        {
            if (ctrlFrequency && fundUpdateCtrl++ % 15 != 0)
            {
                if (fundUpdateCtrl > 15)
                {
                    fundUpdateCtrl = 0;
                }
                return;
            }
            if (null != fundListener && null != accounts)
            {
                foreach (Account item in accounts)
                {
                    //更新已登录账户的持仓，供交易查询用
                    item.Positions = TradeAPI.QueryPositions(item.TradeSessionId);
                }
                Account account = new Account();
                account.Funds = AccountHelper.QueryTotalFunds(accounts);
                account.Positions = AccountHelper.QueryTotalPositions(accounts);
                account.CancelOrders = AccountHelper.QueryTotalCancelOrders(accounts);
                fundListener.OnAcctInfoListen(account);
            }
        }

        public bool IsTradeTime()
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
            if (DateTime.Now.Hour == 14 && DateTime.Now.Minute >= 56)
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
            this.stocksToBuy.Clear();
            this.stocksToBuy.AddRange(quotes);
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

        public void SetDragonLeaders(List<Quotes> dragonLeaders)
        {
            this.dragonLeaders = dragonLeaders;
        }
    }
}
