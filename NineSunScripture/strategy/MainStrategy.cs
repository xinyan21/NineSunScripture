using NineSunScripture.model;
using NineSunScripture.trade.api;
using NineSunScripture.trade.helper;
using NineSunScripture.util;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
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
        public static bool IsTest = false;
        //非交易周期
        private const short CycleTimeOfNonTrade = 25000;
        //交易周期
        private const short CycleTimeOfTrade = 300;
        //更新资金、持仓信息间隔，单位秒
        private const short UpdateFundInterval = 5;
        //周期时间
        private short cycleTime = CycleTimeOfTrade;
        private short queryPriceErrorCnt = 0;
        private bool isHoliday;

        private Account mainAcct;
        private List<Account> accounts;
        //stocks是供买入的股票池，stocksForPrice是用来查询行情的股票池（包括卖的）
        private List<Quotes> stocksToBuy, stocksForPrice;
        private List<Quotes> dragonLeaders;
        private List<Quotes> weakTurnStrongStocks;
        private List<Quotes> bandStocks;
        private Thread mainStrategyThread;
        //private Thread bandStrategyThread;
        private HitBoardStrategy hitBoardStrategy;
        private WeakTurnStrongStrategy weakTurnStrongStrategy;
        //private BandStrategy bandStrategy;
        private ContBoardSellStrategy contBoardSellStrategy;
        private ITrade callback;
        private IAcctInfoListener fundListener;
        private IShowWorkingSatus showWorkingSatus;
        private DateTime lastCycleBeginTime;
        private DateTime lastUpdateFundTime;
        //逆回购记录，使用日期记录以支持不关策略长时间运行
        private Dictionary<DateTime, bool> reverseRepurchaseBondsRecords;

        public MainStrategy()
        {
            this.stocksToBuy = new List<Quotes>();
            this.stocksForPrice = new List<Quotes>();
            this.weakTurnStrongStocks = new List<Quotes>();
            this.bandStocks = new List<Quotes>();
            this.hitBoardStrategy = new HitBoardStrategy();
            this.contBoardSellStrategy = new ContBoardSellStrategy();
            this.weakTurnStrongStrategy = new WeakTurnStrongStrategy();
            //this.bandStrategy = new BandStrategy();
            this.isHoliday = Utils.IsHolidayByDate(DateTime.Now);
            this.reverseRepurchaseBondsRecords = new Dictionary<DateTime, bool>();
            this.lastUpdateFundTime = DateTime.Now;
            this.lastCycleBeginTime = DateTime.Now;
            //ThreadPool.SetMaxThreads(100, 10);
            ThreadPool.SetMinThreads(100, 40);
        }

        private void Process()
        {
            accounts = AccountHelper.Login(callback);
            if (null == accounts || accounts.Count == 0)
            {
                callback.OnTradeResult(0, "策略启动", "没有可操作的账户", false);
                return;
            }
            UpdateTotalAccountInfo(false);
            if (isHoliday && !IsTest)
            {
                callback.OnTradeResult(0, "策略启动", "现在是假期", false);
                return;
            }
            stocksForPrice.Clear();
            stocksForPrice.AddRange(stocksToBuy);
            //登录时的持仓股
            List<Quotes> positionStocks = AccountHelper.QueryPositionStocks(accounts);
            foreach (Quotes item in positionStocks)
            {
                //龙头只有持仓里可能有
                if (null != dragonLeaders && dragonLeaders.Contains(item))
                {
                    item.IsDragonLeader = true;
                }
                //如果股票池里有持仓股说明可以继续做，没有就不能做InPosition要设置成true
                if (!stocksToBuy.Contains(item))
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
                int lastCycleCostTime
                    = (int)DateTime.Now.Subtract(lastCycleBeginTime).TotalMilliseconds;
                if (IsTest)
                {
                    Logger.Log("Cycle begin---------- last cycle cost " + lastCycleCostTime + "ms");
                }
                //处理时间超过一半的睡眠时间就不睡了，否则实际频率会降低很大
                int sleepTime = cycleTime - lastCycleCostTime;
                if (sleepTime > 0)
                {
                    Thread.Sleep(sleepTime);
                    Logger.Log("Sleep " + sleepTime + "ms");
                }
                lastCycleBeginTime = DateTime.Now;
                if (!IsTest && !IsTradeTime())
                {
                    if (null != showWorkingSatus)
                    {
                        showWorkingSatus.RotateStatusImg(-1);
                    }
                    Logger.Log("Not trade time, pass");
                    ReverseRepurchaseBonds();
                    continue;
                }
                if (null == stocksForPrice || stocksForPrice.Count == 0)
                {
                    Logger.Log("No stocks to Query");
                    continue;
                }
                UpdateTotalAccountInfo(true);
                Task[] tasks = new Task[stocksForPrice.Count];
                for (int i = 0; i < stocksForPrice.Count; i++)
                {
                    Quotes quotes = stocksForPrice[i];
                    //每只股票开一个线程去处理
                    tasks[i] = Task.Run(() => ExeContBoardStrategyByStock(quotes, positionStocks));
                    Thread.Sleep(3);
                }//END FOR
                Task.WaitAll(tasks);
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

        private void ExeContBoardStrategyByStock(Quotes stock, List<Quotes> positionStocks)
        {
            try
            {
                if (IsTest)
                {
                    Logger.Log("【" + stock.Name + "】 Task begin with id=" + Task.CurrentId);
                }
                DateTime startTime = DateTime.Now;
                Quotes quotes = PriceAPI.QueryTenthGearPrice(mainAcct.PriceSessionId, stock.Code);
                if (null == quotes || quotes.LatestPrice == 0)
                {
                    //这个变量不需要lock，因为大于2后必定会触发
                    queryPriceErrorCnt++;
                    Logger.Log("QueryTenthGearPrice error " + queryPriceErrorCnt);
                    if (queryPriceErrorCnt < 3)
                    {
                        return;
                    }
                }
                if (queryPriceErrorCnt > 2)
                {
                    Logger.Log("QueryTenthGearPrice error has been occured 3 times, need to reboot");
                    if (null != quotes)
                    {
                        Logger.Log(quotes.ToString(), LogType.Quotes);
                    }
                    callback.OnTradeResult(0, "调用行情接口", "行情接口异常", true);
                    return;
                }
                if (queryPriceErrorCnt > 0)
                {
                    queryPriceErrorCnt = 0;
                }
                quotes.Name = stock.Name;
                Utils.SamplingLogQuotes(quotes);
                SetTradeParams(stock, quotes);
                if (quotes.StockCategory == Quotes.CategoryWeakTurnStrong)
                {
                    weakTurnStrongStrategy.Buy(quotes, accounts, callback);
                }
                else
                {
                    //暂时除了弱转强和波段，其余都是打板
                    //持仓股回封要买回，所以全部股票都在买的范围
                    hitBoardStrategy.Buy(quotes, accounts, callback);
                }
                //TODO 所有持仓都用连板卖策略是不对的，后面需要完善
                if (positionStocks.Contains(quotes))
                {
                    contBoardSellStrategy.Sell(quotes, accounts, callback);
                }
                if (IsTest)
                {
                    Logger.Log("【" + quotes.Name + "】 cost time "
                    + DateTime.Now.Subtract(startTime).TotalMilliseconds
                    + "ms with id " + Task.CurrentId);
                }
            }
            catch (ThreadAbortException)
            {
                Logger.Log("----------策略运行线程终止------------");
            }
            catch (Exception e)
            {
                Logger.Exception(e);
                callback.OnTradeResult(0, "策略执行发生异常", e.Message, true);
            }
        }

        public bool Start()
        {
            mainStrategyThread = new Thread(Process);
            mainStrategyThread.Start();

            return true;
        }

        /// <summary>
        /// 策略停止后不注销，依然可以人工操作
        /// </summary>
        public void Stop()
        {
            mainStrategyThread.Abort();
        }

        /// <summary>
        /// 设置交易计划，以便在策略里面直接拿到龙头、仓位和成交量限制值
        /// </summary>
        /// <param name="source">源股票对象</param>
        /// <param name="quotes">股票对象</param>
        private void SetTradeParams(Quotes source, Quotes quotes)
        {
            quotes.PositionCtrl = source.PositionCtrl;
            quotes.MoneyCtrl = source.MoneyCtrl;
            quotes.InPosition = source.InPosition;
            quotes.IsDragonLeader = source.IsDragonLeader;
            quotes.StockCategory = source.StockCategory;
        }

        /// <summary>
        /// 每隔UpdateFundInterval更新一下总账户信息
        /// </summary>
        /// <param name="ctrlFrequency">是否控制频率</param>
        public void UpdateTotalAccountInfo(bool ctrlFrequency)
        {
            if (ctrlFrequency &&
                DateTime.Now.Subtract(lastUpdateFundTime).TotalSeconds < UpdateFundInterval)
            {
                return;
            }
            if (null != fundListener && null != accounts)
            {
                Task.Run(() =>
                {
                    Account account = new Account
                    {
                        Funds = AccountHelper.QueryTotalFunds(accounts),
                        Positions = AccountHelper.QueryTotalPositions(accounts),
                        CancelOrders = AccountHelper.QueryTotalCancelOrders(accounts)
                    };
                    fundListener.OnAcctInfoListen(account);
                });
                Thread.Sleep(8);
            }
            lastUpdateFundTime = DateTime.Now;
        }

        public bool IsTradeTime()
        {
            if (DateTime.Now.Hour < 9 || DateTime.Now.Hour > 14)
            {
                SetSleepIntervalOfNonTrade();
                return false;
            }
            if (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 24)
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
            if (CycleTimeOfTrade != cycleTime)
            {
                cycleTime = CycleTimeOfTrade;
            }
            return true;
        }

        /// <summary>
        /// 15:20逆回购
        /// </summary>
        private void ReverseRepurchaseBonds()
        {
            if (DateTime.Now.Hour == 15 && DateTime.Now.Minute == 20
                && !reverseRepurchaseBondsRecords.ContainsKey(DateTime.Now.Date))
            {
                AccountHelper.AutoReverseRepurchaseBonds(accounts, callback);
                reverseRepurchaseBondsRecords.Add(DateTime.Now.Date, true);
            }
        }

        private void SetSleepIntervalOfNonTrade()
        {
            if (CycleTimeOfNonTrade != cycleTime)
            {
                cycleTime = CycleTimeOfNonTrade;
            }
        }

        public void SellAll(ITrade callBack)
        {
            if (null == accounts || accounts.Count == 0)
            {
                MessageBox.Show("没有可操作的账户");
                return;
            }
            ContBoardSellStrategy.SellAll(accounts, callBack);
        }

        public void SellStock(Quotes quotes, ITrade callBack)
        {
            if (null == accounts || accounts.Count == 0)
            {
                MessageBox.Show("没有可操作的账户");
                return;
            }
            ContBoardSellStrategy.SellByRatio(quotes, accounts, callBack, 1);
        }

        public void UpdateStocks(
            List<Quotes> hitBoard, List<Quotes> weakTurnStrong, List<Quotes> band)
        {
            if (null != hitBoard)
            {
                this.stocksToBuy.Clear();
                this.stocksToBuy.AddRange(hitBoard);
            }
            if (null != weakTurnStrong)
            {
                this.weakTurnStrongStocks.Clear();
                this.weakTurnStrongStocks.AddRange(weakTurnStrong);
            }
            if (null != band)
            {
                this.bandStocks.Clear();
                this.bandStocks.AddRange(band);
            }
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
