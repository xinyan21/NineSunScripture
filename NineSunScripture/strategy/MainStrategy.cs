using NineSunScripture.model;
using NineSunScripture.trade;
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
        public const int RspCodeOfUpdateAcctInfo = 8888;

        /// <summary>
        /// 是否是测试状态，实盘的时候改为false
        /// </summary>
        public static bool IsTest = true;
        //非交易时间策略执行频率，单位ms
        //    private const short CycleTimeOfNonTrade = 2500;

        //策略交易周期，单位ms，由于采取了推送，这个周期只用来控制太极图的转动
        private const short CycleTimeOfTrade = 1000;

        //更新资金、持仓信息间隔，单位秒
        private const short UpdateFundCycle = 10;

        //回调委托声明成静态变量防止被回收
        private static PriceAPI.PushCallback pushCallback;

        private List<Account> accounts;

        private BandStrategy bandStrategy;
        private ContBoardSellStrategy contBoardSellStrategy;
        private ConvertibleBondStrategy convertibleBondStrategy;

        private ITrade callback;

        //主策略周期时间
        private short cycleTime = CycleTimeOfTrade;

        private HitBoardStrategy hitBoardStrategy;
        private bool isHoliday;
        private bool isMarketOpen = false;
        private short queryPriceErrorCnt = 0;
        //统计被操作保护过滤的行情次数
        private short pricePushPassCnt = 0;

        private DateTime lastFundUpdateTime;
        private DateTime lastPricePushTime;
        private DateTime lastPriceUpdateTime;
        private Account mainAcct;
        //策略执行状态保护，由于逐笔委托的毫秒级高速并发，为防止误操作一只股票同时只能执行一次策略
        private Dictionary<string, bool> operationProtection;
        //逆回购记录，使用日期记录以支持不关策略长时间运行
        private Dictionary<DateTime, bool> reverseRepurchaseRecords;

        //stocks是供买入的股票池，stocksForPrice是用来查询行情的股票池（包括卖的）
        private List<Quotes> stocksForPrice;
        private List<Quotes> stocksToBuy;
        private List<Quotes> stocksToSell;
        private Thread strategyThread;
        private WeakTurnStrongStrategy weakTurnStrongStrategy;
        private IWorkListener workListener;

        public MainStrategy()
        {
            ThreadPool.SetMinThreads(100, 40);
            stocksToBuy = new List<Quotes>();
            stocksForPrice = new List<Quotes>();
            stocksToSell = new List<Quotes>();
            hitBoardStrategy = new HitBoardStrategy();
            contBoardSellStrategy = new ContBoardSellStrategy();
            convertibleBondStrategy = new ConvertibleBondStrategy();
            weakTurnStrongStrategy = new WeakTurnStrongStrategy();
            bandStrategy = new BandStrategy();
            reverseRepurchaseRecords = new Dictionary<DateTime, bool>();
            operationProtection = new Dictionary<string, bool>();
            isHoliday = Utils.IsHolidayByDate(DateTime.Now);
            lastFundUpdateTime = DateTime.Now;
            lastPriceUpdateTime = DateTime.Now;
            lastPricePushTime = DateTime.MaxValue;
            pushCallback = OnPushResult;
        }

        private void DoStrategy()
        {
            try
            {
                accounts = AccountHelper.Login(callback);
                if (null == accounts || accounts.Count == 0)
                {
                    callback.OnTradeResult(0, "策略启动", "登录账户失败", false);
                    return;
                }
                mainAcct = accounts[0];
                UpdateTotalAccountInfo(false);
                if (isHoliday && !IsTest)
                {
                    callback.OnTradeResult(0, "策略启动", "现在是假期", false);
                    return;
                }
                while (true)
                {
                    Thread.Sleep(cycleTime);
                    if (!IsTest && !IsTradeTime())
                    {
                        if (null != workListener)
                        {
                            workListener.OnImgRotate(-1);
                        }
                        ReverseRepurchaseBonds();
                        continue;
                    }
                    DoConvertibleBondStrategy();

                    RebootBeforeOpenMarket();
                    OpenMarket();
                    CloseMarket(false);
                    CheckPricePush();
                    UpdateTotalAccountInfo(true);
                    if (null != workListener)
                    {
                        workListener.OnImgRotate(1);
                        if (DateTime.Now.Subtract(lastPriceUpdateTime).TotalSeconds
                            >= UpdateFundCycle && IsTradeTime())
                        {
                            workListener.OnPriceChange(stocksForPrice);
                            lastPriceUpdateTime = DateTime.Now;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Exception(e);
                if (null != callback)
                {
                    callback.OnTradeResult(0, "辅助策略线程", e.Message, true);
                }
            }
        }

        public bool Start(List<Quotes> stocks)
        {
            if (!Utils.IsListEmpty(stocks))
            {
                stocksForPrice.Clear();
                //这里需要对股票对象进行分类
                foreach (Quotes item in stocks)
                {
                    if (!stocksForPrice.Contains(item))
                    {
                        stocksForPrice.Add(item);
                    }
                    if (item.Operation == Quotes.OperationBuy
                        && !stocksToBuy.Exists(t => t.Code == item.Code
                        && t.StockCategory == item.StockCategory))
                    {
                        stocksToBuy.Add(item);
                    }
                    if (item.Operation == Quotes.OperationSell
                        && !stocksToSell.Exists(t => t.Code == item.Code))
                    {
                        stocksToSell.Add(item);
                    }
                }
            }
            Stop();
            Thread.Sleep(2000);
            hitBoardStrategy.RestoreOpenBoardCnt();
            strategyThread = new Thread(DoStrategy);
            strategyThread.Start();

            return true;
        }

        /// <summary>
        /// 策略停止后不注销，依然可以人工操作
        /// 注销后停止更新资金信息，不能逆回购，太极图停止转动
        /// </summary>
        public void Stop()
        {
            if (null != strategyThread)
            {
                strategyThread.Abort();
            }
            CloseMarket(true);
            hitBoardStrategy.SaveOpenBoardCnt();
        }

        /// <summary>
        /// 此回调在C++线程里面执行
        /// </summary>
        /// <param name="type">功能号</param>
        /// <param name="result">数据指针</param>
        public void OnPushResult(int type, IntPtr result)
        {
            if (!IsTradeTime())
            {
                Logger.Log("非交易时间推送");
                return;
            }
            string code = Marshal.PtrToStringAnsi(result + 16, 6);
            Quotes stock = stocksForPrice.Find(item => item.Code.Equals(code));
            if (null == stock || 0 == stock.HighLimit)
            {
                Logger.Log("OnPushResult find a stock to InitLimitPrice=" + stock);
                InitLimitPrice(code);
                return;
            }
            Logger.Log("OnPushResult new push =" + stock.Name);
            lock (operationProtection)
            {
                //买入保护
                if (operationProtection.ContainsKey(code))
                {
                    if (operationProtection[code])
                    {
                        pricePushPassCnt++;
                        if (pricePushPassCnt > 10 && null != callback)
                        {
                            pricePushPassCnt = 0;
                            callback.OnTradeResult(
                                0, "被操作保护过滤的行情推送次数超过10次，重启策略", "", true);
                        }
                        Logger.Log("【" + stock.Name + "】被操作保护，推送被过滤");
                        return;
                    }
                }
                else
                {
                    operationProtection.Add(code, false);
                }
            }
            Logger.Log("OnPushResult new push and pass protection=" + stock.Name);
            try
            {
                //推送过来是数据是十档行情 推送中的十档无换手、总市值、流通值、涨停、跌停
                if (type == 10001)
                {
                    OnTenthGearPricePush(result, stock);
                }
                else if (type == 10206)
                {
                    OnOByOCommisionPush(result, stock);
                }
            }
            finally
            {
                lock (operationProtection)
                {
                    operationProtection[code] = false;
                    Logger.Log("【" + stock.Name + "】取消操作保护");
                }
            }
        }

        private void OnTenthGearPricePush(IntPtr result, Quotes stock)
        {
            Quotes quotes = ApiHelper.ParseStructToQuotes(result);
            if (!CheckQuotes(quotes))
            {
                Logger.Log("CheckQuotes failed: " + quotes);
                return;
            }
            //修改行情价格以更新主界面的涨跌幅
            Quotes priceStock = stocksForPrice.Find(item => item.Code.Equals(stock.Code));
            if (null != priceStock)
            {
                if (quotes.Buy1 > 0)
                {
                    priceStock.Buy1 = quotes.Buy1;
                }
                else
                {
                    priceStock.Buy1 = quotes.Sell1;
                }
                quotes.Name = priceStock.Name;
            }

            lock (operationProtection)
            {
                lastPricePushTime = DateTime.Now;
                operationProtection[stock.Code] = true;
                Logger.Log("【" + stock.Name + "】开启操作保护");
            }
            Logger.Log("执行策略");
            //由于一只股票可能要分多个策略买，所以单独用行情里的参数去执行是不行的
            if (stocksToBuy.Contains(quotes))
            {
                foreach (var item in stocksToBuy)
                {
                    if (item.Code.Equals(quotes.Code))
                    {
                        quotes.CloneStrategyParamsFrom(item);
                        //打板的深圳股票需要在8.5%之上订阅逐笔委托，可以和十档争夺策略执行，买入之后取消订阅
                        //由于是3s一推，所以这里不需要担心并发问题
                        if (Utils.IfNeedToSubOByOPrice(quotes))
                        {
                            if (!stock.IsOByOComissionSubscribed)
                            {
                                EditStockSub(stock, true, PriceAPI.PushTypeOByOCommision);
                            }
                        }
                        else
                        {
                            EditStockSub(stock, false, PriceAPI.PushTypeOByOCommision);
                        }
                        ExeContBoardStrategyByStock(quotes);
                    }
                }
            }
            if (stocksToSell.Contains(quotes))
            {
                foreach (var item in stocksToSell)
                {
                    if (item.Code.Equals(quotes.Code))
                    {
                        quotes.CloneStrategyParamsFrom(item);
                        ExeContBoardStrategyByStock(quotes);
                    }
                }
            }
        }

        private void OnOByOCommisionPush(IntPtr Result, Quotes stock)
        {
            List<OByOCommision> commisions = ApiHelper.ParseStructToCommision(Result);
            foreach (var commision in commisions)
            {
                //逐笔委托只处理特大单，这里还要拿到涨停价
                if (commision.Price != stock.HighLimit)
                {
                    return;
                }
                //低于1000手或者低于300万的单子过滤
                if (commision.Quantity < 100000 && commision.Quantity * commision.Price < 2000000)
                {
                    return;
                }
                lock (operationProtection)
                {
                    operationProtection[stock.Code] = true;
                    Logger.Log("【" + stock.Name + "】开启逐笔买入保护");
                }
                //执行买入，同时设置买入保护状态，防止多次买入，把打板的买入逻辑拆出来
                //还有个问题就是这里没有行情对象，要查个行情出来，或者把打板保存的最新对象拿出来
                //修改买一卖一，然后直接调用打板的方法
                Quotes quotes = hitBoardStrategy.GetLastHistoryQuotesBy(stock.Code);
                quotes.Buy1 = stock.HighLimit;
                Logger.Log("【" + quotes.Name + "】触发逐笔委托买点：" + commision);
                ExeContBoardStrategyByStock(quotes);
            }
        }

        /// <summary>
        /// 执行可转债策略
        /// </summary>
        private void DoConvertibleBondStrategy()
        {
            if (DateTime.Now.Second%3==0)
            {
                convertibleBondStrategy.DoStrategy();
            }
        }

        public void AddStock(Quotes quotes)
        {
            if (null == quotes)
            {
                return;
            }

            if (!stocksForPrice.Contains(quotes))
            {
                stocksForPrice.Add(quotes);
                if (IsTradeTime())
                {
                    EditStockSub(quotes, true);
                }
            }
            if (quotes.Operation == Quotes.OperationBuy)
            {
                Quotes stock = stocksToBuy.Find(t => t.Code == quotes.Code
                && t.StockCategory == quotes.StockCategory);
                if (null != stock)
                {
                    stocksToBuy.Remove(stock);
                }
                stocksToBuy.Add(quotes);
            }
            else if (quotes.Operation == Quotes.OperationSell)
            {
                Quotes stock = stocksToSell.Find(t => t.Code == quotes.Code);
                if (null != stock)
                {
                    stocksToSell.Remove(stock);
                }
                stocksToSell.Add(quotes);
            }
        }

        public void ClearStocks()
        {
            UnsubscribeAll();
            stocksForPrice.Clear();
            stocksToBuy.Clear();
            stocksToSell.Clear();
        }

        public void DelStock(Quotes quotes)
        {
            if (null == quotes)
            {
                return;
            }
            if (stocksForPrice.Contains(quotes))
            {
                stocksForPrice.Remove(quotes);
                EditStockSub(quotes, false);
            }
            if (quotes.Operation == Quotes.OperationBuy && stocksToBuy.Exists(
                t => t.Code == quotes.Code && t.Operation == Quotes.OperationBuy
                && t.StockCategory == quotes.StockCategory))
            {
                stocksToBuy.Remove(quotes);
            }
            if (quotes.Operation == Quotes.OperationSell && stocksToSell.Exists(
                t => t.Code == quotes.Code && t.Operation == Quotes.OperationSell))
            {
                stocksToSell.Remove(quotes);
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
            Quotes matchStock = stocksForPrice.Find(item => item.Code == quotes.Code);
            if (null != matchStock)
            {
                if (PriceAPI.PushTypeOByOCommision == priceType
                && matchStock.IsOByOComissionSubscribed == isSubscribe)
                {
                    return;
                }
                if (PriceAPI.PushTypeTenGear == priceType
                    && matchStock.IsTenthGearSubscribed == isSubscribe)
                {
                    return;
                }
            }
            int rspCode = PriceAPI.HQ_PushData(
                mainAcct.PriceSessionId, priceType, quotes.Code, "", pushCallback, isSubscribe);
            if (null != callback)
            {
                string temp = priceType == 0 ? "十档行情" : "逐笔行情";
                string isSub = isSubscribe ? "订阅" : "取消订阅";
                callback.OnTradeResult(rspCode,
                    isSub + "【" + quotes.Name + "】" + temp, rspCode > 0 ? "成功" : "失败", false);
            }
            //记录订阅状态
            if (rspCode > 0)
            {
                if (PriceAPI.PushTypeOByOCommision == priceType)
                {
                    quotes.IsOByOComissionSubscribed = isSubscribe;
                }
                else
                {
                    quotes.IsTenthGearSubscribed = isSubscribe;
                }
            }
        }

        public List<Account> GetAccounts()
        {
            return accounts;
        }

        public bool IsTradeTime()
        {
            DateTime now = DateTime.Now;
            if (now.Hour < 9 || now.Hour > 14)
            {
                return false;
            }
            if (now.Hour == 9 && now.Minute < 26)
            {
                isMarketOpen = false;
                return false;
            }
            if (now.Hour == 11 && now.Minute >= 30)
            {
                return false;
            }
            if (now.Hour == 12)
            {
                return false;
            }
            if (now.Hour == 14 && now.Minute >= 57)
            {
                return false;
            }
            return true;
        }
        public void SellAll(ITrade callBack)
        {
            if (Utils.IsListEmpty(accounts))
            {
                MessageBox.Show("没有可操作的账户");
                return;
            }
            AccountHelper.SellAll(accounts, callBack);
        }

        public void SellStock(Quotes quotes, ITrade callBack)
        {
            if (Utils.IsListEmpty(accounts))
            {
                MessageBox.Show("没有可操作的账户");
                return;
            }
            AccountHelper.SellByRatio(quotes, accounts, callBack, 1);
        }

        public void SetTradeCallback(ITrade callback)
        {
            this.callback = callback;
        }

        public void SetWorkListener(IWorkListener workListener)
        {
            this.workListener = workListener;
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
            if (null != workListener && null != accounts)
            {
                try
                {
                    Account account = new Account();
                    account.Funds = AccountHelper.QueryTotalFunds(accounts, callback);
                    account.Positions = AccountHelper.QueryTotalPositions(accounts, callback);
                    account.CancelOrders = AccountHelper.QueryTotalCancelOrders(accounts, callback);
                    workListener.OnAcctInfoUpdate(account);
                }
                catch (ApiTimeoutException e)
                {
                    Logger.Exception(e);
                    if (null != callback)
                    {
                        callback.OnTradeResult(0, "UpdateTotalAccountInfo", e.Message, true);
                    }
                }
                catch (Exception e)
                {
                    Logger.Exception(e);
                }
            }
            lastFundUpdateTime = DateTime.Now;
        }

        private void CheckPricePush()
        {
            if (!IsTradeTime())
            {
                return;
            }
            double seconds = DateTime.Now.Subtract(lastPricePushTime).TotalSeconds;
            Logger.Log("行情推送间隔=" + seconds);
            //改成70，这样早盘竞价一分钟一推就不会报错了
            if (seconds > 70)
            {
                string log = "70秒未收到行情推送，重启策略";
                Logger.Log(log);
                if (null != callback)
                {
                    callback.OnTradeResult(0, log, "", true);
                }
            }
        }

        private bool CheckQuotes(Quotes quotes)
        {
            //这里要考虑涨跌停的情况，涨停sell1=0，跌停buy1=0
            if (null == quotes || (quotes.Buy1 == 0 && quotes.Sell1 == 0))
            {
                //这个变量不需要lock，因为大于2后必定会触发
                queryPriceErrorCnt++;
                Logger.Log("OnTenthGearPricePush error " + queryPriceErrorCnt);
                if (null != quotes)
                {
                    Logger.Log("OnTenthGearPricePush error " + quotes.ToString());
                }
                else
                {
                    Logger.Log("quotes is null");
                }
                if (queryPriceErrorCnt < 3)
                {
                    return false;
                }
            }
            Utils.SamplingLogQuotes(quotes);
            if (queryPriceErrorCnt > 2)
            {
                Logger.Log("OnTenthGearPricePush error has been occured 3 times, need to reboot");
                if (null != quotes)
                {
                    Logger.Log("queryPriceErrorCnt > 2=" + quotes.ToString());
                }
                if (null != callback)
                {
                    callback.OnTradeResult(0, "检查行情推送", "行情数据异常", true);
                }
                return false;
            }
            if (queryPriceErrorCnt > 0)
            {
                queryPriceErrorCnt = 0;
            }
            return true;
        }

        /// <summary>
        /// 是否是关闭策略
        /// </summary>
        /// <param name="isStop"></param>
        private void CloseMarket(bool isStop)
        {
            if (isStop || (isMarketOpen && !IsTradeTime()))
            {
                UnsubscribeAll();
                //要清空状态，否则关闭重启策略会有问题
                lastPricePushTime = DateTime.MaxValue;
                isMarketOpen = false;
            }
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
                short category = quotes.StockCategory;
                switch (category)
                {
                    case Quotes.CategoryWeakTurnStrong:
                        if (Quotes.OperationBuy == quotes.Operation)
                        {
                            weakTurnStrongStrategy.Buy(quotes, accounts, callback);
                        }
                        else
                        {
                            contBoardSellStrategy.Sell(quotes, accounts, callback);
                        }

                        break;
                    case Quotes.CategoryBand:
                        bandStrategy.Process(accounts, quotes, callback);

                        break;
                    case Quotes.CategoryWindVane:

                        break;
                    case Quotes.CategoryDragonLeader:
                        if (stocksToSell.Contains(quotes))
                        {
                            contBoardSellStrategy.Sell(quotes, accounts, callback);
                        }

                        break;
                    case Quotes.CategoryHitBoard:
                    case Quotes.CategoryLongTerm:
                        Logger.Log("执行打板策略");
                        //持仓股回封要买回，所以全部股票都在买的范围，InPosition参数依然有效
                        hitBoardStrategy.Buy(quotes, accounts, callback);
                        if (stocksToSell.Contains(quotes))
                        {
                            contBoardSellStrategy.Sell(quotes, accounts, callback);
                        }

                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Exception(e);
                callback.OnTradeResult(0, "策略执行发生异常", e.Message, true);
            }
        }

        /// <summary>
        /// 初始化开盘、昨收、涨跌停数据，推送的并没有这些数据
        /// </summary>
        private void InitLimitPrice(string code = null)
        {
            List<Task> tasks = new List<Task>();
            foreach (Quotes item in stocksForPrice)
            {
                if (!string.IsNullOrEmpty(code) && !item.Code.Equals(code))
                {
                    continue;
                }
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        Quotes quotes
                        = PriceAPI.QueryTenthGearPrice(mainAcct.PriceSessionId, item.Code);
                        if (null == quotes)
                        {
                            return;
                        }
                        item.HighLimit = quotes.HighLimit;
                        item.LowLimit = quotes.LowLimit;
                        item.Open = quotes.Open;
                        item.PreClose = quotes.PreClose;
                        item.Buy1 = quotes.Buy1;
                        foreach (var buyItem in stocksToBuy)
                        {
                            //重写了equal方法，只要代码相等就相等
                            if (buyItem.Equals(item))
                            {
                                buyItem.HighLimit = quotes.HighLimit;
                                buyItem.LowLimit = quotes.LowLimit;
                                buyItem.Open = quotes.Open;
                                buyItem.PreClose = quotes.PreClose;
                                buyItem.Buy1 = quotes.Buy1;
                            }
                        }
                        foreach (var sellItem in stocksToSell)
                        {
                            if (sellItem.Equals(item))
                            {
                                sellItem.HighLimit = quotes.HighLimit;
                                sellItem.LowLimit = quotes.LowLimit;
                                sellItem.Open = quotes.Open;
                                sellItem.PreClose = quotes.PreClose;
                                sellItem.Buy1 = quotes.Buy1;
                            }
                        }
                        Logger.Log("InitLimitPric=》 " + item);
                    }
                    catch (ApiTimeoutException e)
                    {
                        if (null != callback)
                        {
                            callback.OnTradeResult(0, "InitLimitPrice", e.Message, true);
                        }
                    }
                }));
                Thread.Sleep(1);
            }
            Task.WaitAll(tasks.ToArray());
        }
        private void OpenMarket()
        {
            if (!isMarketOpen && IsTradeTime())
            {
                pricePushPassCnt = 0;
                isMarketOpen = true;
                InitLimitPrice();
                PrepareStocksAndSubPrice();
            }
        }

        /// <summary>
        /// 开盘前重启策略，防止长时间挂机登录状态失效，DoStrategy的交易时机是9:26开始算起
        /// 而且跟睡眠频率相关
        /// </summary>
        private void RebootBeforeOpenMarket()
        {
            DateTime now = DateTime.Now;
            if (now.Hour == 9 && now.Minute == 27 && now.Second == 0 && null != callback)
            {
                callback.OnTradeResult(0, "开盘前重启策略，防止长时间挂机登录状态失效", "", true);
            }
            if (now.Hour == 12 && now.Minute == 57 && now.Second == 0 && null != callback)
            {
                callback.OnTradeResult(0, "开盘前重启策略，防止长时间挂机登录状态失效", "", true);
            }
        }

        private void PrepareStocksAndSubPrice()
        {
            //登录时的持仓股
            List<Quotes> stocksInPosition = AccountHelper.QueryPositionStocks(accounts, callback);
            foreach (Quotes item in stocksInPosition)
            {
                //如果股票池里有持仓股说明可以继续做，没有就不能做InPosition要设置成true
                Quotes matchItem = stocksToSell.Find(t => t.Code == item.Code);
                if (null == matchItem)
                {
                    item.InPosition = true;
                    item.Operation = Quotes.OperationSell;
                    stocksToSell.Add(item);
                    if (!stocksForPrice.Contains(item))
                    {
                        stocksForPrice.Add(item);
                    }
                }
                else
                {
                    matchItem.InPosition = true;
                }
            }
            if (!Utils.IsListEmpty(stocksForPrice))
            {
                foreach (var item in stocksForPrice)
                {
                    EditStockSub(item, true, PriceAPI.PushTypeTenGear);
                }
            }
            else
            {
                Logger.Log("无股票可订阅");
            }
        }
        /// <summary>
        /// 15:20逆回购
        /// </summary>
        private void ReverseRepurchaseBonds()
        {
            if (DateTime.Now.Hour == 15 && DateTime.Now.Minute == 20
                && !reverseRepurchaseRecords.ContainsKey(DateTime.Now.Date))
            {
                AccountHelper.ReverseRepurchaseBonds(accounts, callback);
                reverseRepurchaseRecords.Add(DateTime.Now.Date, true);
            }
        }

        private void UnsubscribeAll()
        {
            foreach (var stock in stocksForPrice)
            {
                if (stock.IsTenthGearSubscribed)
                {
                    EditStockSub(stock, false, PriceAPI.PushTypeTenGear);
                }
                if (stock.IsOByOComissionSubscribed)
                {
                    EditStockSub(stock, false, PriceAPI.PushTypeOByOCommision);
                }
            }
        }
    }
}