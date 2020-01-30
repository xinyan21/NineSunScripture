using NineSunScripture.model;
using NineSunScripture.trade.structApi.api;
using NineSunScripture.trade.structApi.helper;
using NineSunScripture.util;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
        public static bool IsTest = true;

        //非交易时间策略执行频率，单位ms
        private const short CycleTimeOfNonTrade = 25000;

        //策略交易周期，单位ms，由于采取了推送，这个周期只用来控制太极图的转动
        private const short CycleTimeOfTrade = 1000;

        //更新资金、持仓信息间隔，单位秒
        private const short UpdateFundCycle = 5;

        //回调委托声明成静态变量防止被回收
        private static PriceAPI.PushCallback pushCallback;

        protected ReaderWriterLockSlim rwLockSlimForBuy;

        //主策略周期时间
        private short cycleTime = CycleTimeOfTrade;

        private short queryPriceErrorCnt = 0;
        private bool isHoliday;
        //涨跌停是否初始化（由于推送接口不推送这2个值，所以需要开盘的时候查出来存着）
        private bool isLimitInited = false;

        //stocks是供买入的股票池，stocksForPrice是用来查询行情的股票池（包括卖的）
        private List<Quotes> stocksForPrice;

        private List<Quotes> stocksToBuy;
        private static List<Quotes> stocksToSell;

        private Account mainAcct;
        private List<Account> accounts;
        private Thread mainStrategyThread;

        private HitBoardStrategy hitBoardStrategy;
        private WeakTurnStrongStrategy weakTurnStrongStrategy;
        private BandStrategy bandStrategy;
        private ContBoardSellStrategy contBoardSellStrategy;

        private ITrade callback;
        private IAcctInfoListener fundListener;
        private IShowWorkingSatus showWorkingSatus;
        private DateTime lastFundUpdateTime;

        //逆回购记录，使用日期记录以支持不关策略长时间运行
        private Dictionary<DateTime, bool> reverseRepurchaseRecords;

        //行情订阅状态
        private Dictionary<string, Dictionary<short, bool>> priceSubState;

        //买入状态保护，由于逐笔委托的毫秒级高速并发，为防止误买入一只股票同时只能执行一次策略
        private Dictionary<string, bool> buyProtection;

        public MainStrategy()
        {
            ThreadPool.SetMinThreads(100, 40);
            stocksToBuy = new List<Quotes>();
            stocksForPrice = new List<Quotes>();
            hitBoardStrategy = new HitBoardStrategy();
            contBoardSellStrategy = new ContBoardSellStrategy();
            weakTurnStrongStrategy = new WeakTurnStrongStrategy();
            bandStrategy = new BandStrategy();
            isHoliday = Utils.IsHolidayByDate(DateTime.Now);
            reverseRepurchaseRecords = new Dictionary<DateTime, bool>();
            lastFundUpdateTime = DateTime.Now;
            rwLockSlimForBuy = new ReaderWriterLockSlim();
            pushCallback = OnPushResult;
            priceSubState = new Dictionary<string, Dictionary<short, bool>>();
            buyProtection = new Dictionary<string, bool>();
        }

        private void InitStrategy()
        {
            accounts = AccountHelper.Login(callback);
            if (null == accounts || accounts.Count == 0)
            {
                callback.OnTradeResult(0, "策略启动", "没有可操作的账户", false);
                return;
            }
            mainAcct = accounts[0];
            UpdateTotalAccountInfo(false);
            if (isHoliday && !IsTest)
            {
                callback.OnTradeResult(0, "策略启动", "现在是假期", false);
                return;
            }
            //登录时的持仓股
            List<Quotes> stocksInPosition = AccountHelper.QueryPositionStocks(accounts);
            foreach (Quotes item in stocksInPosition)
            {
                //如果股票池里有持仓股说明可以继续做，没有就不能做InPosition要设置成true
                if (!stocksToSell.Contains(item))
                {
                    item.InPosition = true;
                    //TODO 这里都是没有策略参数的，要注意个股分类问题
                    stocksToSell.Add(item);
                    if (!stocksForPrice.Contains(item))
                    {
                        stocksForPrice.Add(item);
                    }
                }
            }
            if (stocksForPrice.Count > 0)
            {
                for (int i = 0; i < stocksForPrice.Count; i++)
                {
                    EditStockSub(stocksForPrice[i], true, PriceAPI.PushTypeTenGear);
                }
            }
            else
            {
                Logger.Log("无股票可订阅");
            }
            bool isWorkingRight = true;
            while (true)
            {
                Thread.Sleep(cycleTime);
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
                if (9 == DateTime.Now.Hour && 30 == DateTime.Now.Minute && !isLimitInited)
                {
                    InitLimitPrice();
                    isLimitInited = true;
                }
                UpdateTotalAccountInfo(true);
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

        /// <summary>
        /// 此回调在C++线程里面执行
        /// </summary>
        /// <param name="type">功能号</param>
        /// <param name="Result">数据指针</param>
        public void OnPushResult(int type, IntPtr Result)
        {
            rwLockSlimForBuy.EnterWriteLock();
            string code = Marshal.PtrToStringAnsi(Result + 16, 6);
            Quotes stock = stocksForPrice.Find(item => item.Code.Equals(code));
            if (null == stock)
            {
                Logger.Log("OnPushResult> not find stock in stocksForPrice=" + code);
                return;
            }
            //买入保护
            if (buyProtection.ContainsKey(code))
            {
                if (buyProtection[code])
                {
                    return;
                }
            }
            else
            {
                buyProtection.Add(code, true);
            }
            rwLockSlimForBuy.ExitWriteLock();
            //推送过来是数据是十档行情 推送中的十档无换手、总市值、流通值、涨停、跌停
            if (type == 10001)
            {
                Quotes quotes = ApiHelper.ParseStructToQuotes(Result);
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
                Logger.Log(quotes.Code + "的订阅行情：" + quotes.ToString());

                rwLockSlimForBuy.EnterWriteLock();
                buyProtection[code] = true;
                rwLockSlimForBuy.ExitWriteLock();

                quotes.Name = stock.Name;
                Utils.SamplingLogQuotes(quotes);
                quotes.CloneStrategyParams(stock);
                //打板的深圳股票需要在8.5%之上订阅逐笔委托，可以和十档争夺策略执行
                if (Utils.ifNeedToSubOByOPrice(quotes) && priceSubState.ContainsKey(code) &&
                        priceSubState[code].ContainsKey(PriceAPI.PushTypeOByOCommision) &&
                        !priceSubState[code][PriceAPI.PushTypeOByOCommision])
                {
                    EditStockSub(quotes, true, PriceAPI.PushTypeOByOCommision);
                }
                ExeContBoardStrategyByStock(quotes);
            }
            else if (type == 10206)//逐笔委托 推送太快注意代码优化
            {
                OByOCommision commision = ApiHelper.ParseStructToCommision(Result);
                Logger.Log(code + "的逐笔委托：" + commision);
                //逐笔委托只处理特大单，这里还要拿到涨停价
                if (commision.Quantity < 500000 && commision.Quantity * commision.Price < 5000000)
                {
                    return;
                }
                if (commision.Price != stock.HighLimit)
                {
                    return;
                }

                rwLockSlimForBuy.EnterWriteLock();
                buyProtection[code] = true;
                rwLockSlimForBuy.ExitWriteLock();

                //TODO 执行买入，同时设置买入保护状态，防止多次买入，把打板的买入逻辑拆出来
                //还有个问题就是这里没有行情对象，要查个行情出来，或者把打板保存的最新对象拿出来
                //修改买一卖一，然后直接调用打板的方法
                Quotes quotes = hitBoardStrategy.GetLastHistoryQuotesBy(code);
                quotes.Buy1 = stock.HighLimit;
                ExeContBoardStrategyByStock(quotes);

            }
            
            rwLockSlimForBuy.EnterWriteLock();
            buyProtection[code] = false;
            rwLockSlimForBuy.ExitWriteLock();
        }

        /// <summary>
        /// 按个股执行策略
        /// </summary>
        /// <param name="stock">源个股对象，包含操作策略信息</param>
        /// <param name="quotes">行情对象，只包含最新行情</param>
        private void ExeContBoardStrategyByStock(Quotes quotes)
        {
            try
            {
                if (quotes.StockCategory == Quotes.CategoryWeakTurnStrong)
                {
                    if (Quotes.OperationBuy == quotes.Operation)
                    {
                        weakTurnStrongStrategy.Buy(quotes, accounts, callback);
                    }
                    else
                    {
                        contBoardSellStrategy.Sell(quotes, accounts, callback);
                    }
                }
                else if (quotes.StockCategory == Quotes.CategoryBand)
                {
                    bandStrategy.Process(quotes, accounts, callback);
                }
                else if (quotes.StockCategory == Quotes.CategoryDragonLeader)
                {
                    if (stocksToSell.Contains(quotes))
                    {
                        contBoardSellStrategy.Sell(quotes, accounts, callback);
                    }
                }
                else
                {
                    //暂时除了龙头、弱转强和波段，其余都是打板
                    //持仓股回封要买回，所以全部股票都在买的范围，InPosition参数依然有效
                    hitBoardStrategy.Buy(quotes, accounts, callback);
                    if (stocksToSell.Contains(quotes))
                    {
                        contBoardSellStrategy.Sell(quotes, accounts, callback);
                    }
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

        public bool Start(List<Quotes> stocks)
        {
            if (null != stocks && stocks.Count > 0)
            {
                stocksForPrice.Clear();
                //这里需要对股票对象进行分类
                foreach (Quotes item in stocks)
                {
                    if (!stocksForPrice.Contains(item))
                    {
                        stocksForPrice.Add(item);
                    }
                    if (!stocksToBuy.Exists(
                        t => t.Code == item.Code && t.Operation == Quotes.OperationBuy))
                    {
                        stocksToBuy.Add(item);
                    }
                    if (!stocksToSell.Exists(
                        t => t.Code == item.Code && t.Operation == Quotes.OperationSell))
                    {
                        stocksToSell.Add(item);
                    }
                }
            }
            else
            {
                return false;
            }

            mainStrategyThread = new Thread(InitStrategy);
            mainStrategyThread.Start();

            return true;
        }

        /// <summary>
        /// 策略停止后不注销，依然可以人工操作
        /// 注销后停止更新资金信息，不能逆回购，太极图停止转动
        /// </summary>
        public void Stop()
        {
            mainStrategyThread.Abort();
            foreach (var item in priceSubState)
            {
                Dictionary<short, bool> states = item.Value;
                Quotes quotes = stocksForPrice.Find(temp => temp.Code == item.Key);
                if (null == quotes)
                {
                    continue;
                }
                foreach (var stateItem in states)
                {
                    if (states[stateItem.Key])
                    {
                        EditStockSub(quotes, false, stateItem.Key);
                    }
                }
            }
        }

        /// <summary>
        /// 编辑股票行情订阅
        /// </summary>
        /// <param name="quotes">股票对象</param>
        /// <param name="isSubscribe">是否订阅</param>
        /// <param name="priceType">
        /// 行情类型，十档：PriceAPI.PushTypeTenGear；
        /// 逐笔：PriceAPI.PushTypeOBOCommision，默认是十档只有进入打板位置才会触发逐笔订阅</param>
        public void EditStockSub(
            Quotes quotes, bool isSubscribe, short priceType = PriceAPI.PushTypeTenGear)
        {
            if (null == quotes)
            {
                return;
            }
            int rspCode = PriceAPI.HQ_PushData(
                mainAcct.PriceSessionId, priceType, quotes.Code, "", pushCallback, isSubscribe);
            if (null != callback)
            {
                string temp = priceType == 0 ? "十档行情" : "逐笔行情";
                callback.OnTradeResult(
                    rspCode, "订阅【" + quotes.Name + "】" + temp, rspCode > 0 ? "成功" : "失败", false);
            }
            //记录订阅状态
            if (priceSubState.ContainsKey(quotes.Code))
            {
                priceSubState[quotes.Code][priceType] = isSubscribe;
            }
            else
            {
                Dictionary<short, bool> state = new Dictionary<short, bool>();
                state.Add(priceType, isSubscribe);
                priceSubState.Add(quotes.Code, state);
            }
        }

        /// <summary>
        /// 每隔UpdateFundInterval更新一下总账户信息
        /// </summary>
        /// <param name="ctrlFrequency">是否控制频率</param>
        public void UpdateTotalAccountInfo(bool ctrlFrequency)
        {
            if (ctrlFrequency &&
                DateTime.Now.Subtract(lastFundUpdateTime).TotalSeconds < UpdateFundCycle)
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
            lastFundUpdateTime = DateTime.Now;
        }

        /// <summary>
        /// 初始化涨跌停数据
        /// </summary>
        private void InitLimitPrice()
        {
            Task.Run(() =>
            {
                foreach (Quotes item in stocksForPrice)
                {
                    Quotes quotes = TradeAPI.QueryQuotes(mainAcct.TradeSessionId, item.Code);
                    item.HighLimit = quotes.HighLimit;
                    item.LowLimit = quotes.LowLimit;
                }
            });
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
                isLimitInited = false;
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
                && !reverseRepurchaseRecords.ContainsKey(DateTime.Now.Date))
            {
                AccountHelper.AutoReverseRepurchaseBonds(accounts, callback);
                reverseRepurchaseRecords.Add(DateTime.Now.Date, true);
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