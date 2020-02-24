using NineSunScripture.model;
using NineSunScripture.trade;
using NineSunScripture.trade.persistence;
using NineSunScripture.trade.structApi.api;
using NineSunScripture.trade.structApi.helper;
using NineSunScripture.util;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NineSunScripture.strategy
{
    /// <summary>
    /// 打板策略
    /// </summary>
    public class HitBoardStrategy : Strategy
    {
        /// <summary>
        /// 默认成交额限制是5000万，低于的过滤（之前设置的7000万是打3板用的，在2板上不适用）
        /// 改成5000万后，3板就要手动设置下成交额限制了
        /// </summary>
        private const int DefaultMoneyCtrl = 5000;

        /// <summary>
        /// 最小买一涨幅
        /// </summary>
        private const float MinBuy1Ratio = 1.085f;

        /// <summary>
        /// 回封买回比例
        /// </summary>
        private const float BuyBackRatio = 2 / 3;

        /// <summary>
        /// 单账户最小可用金额默认为5000
        /// </summary>
        private const int MinTotalAvailableAmt = 5000;

        /// <summary>
        ///下降期T字板 最小开板时间
        /// </summary>
        private const int MinOpenBoardTimeOfDown = 26;

        /// <summary>
        ///上升期T字板 最小开板时间
        /// </summary>
        private const int MinOpenBoardTimeOfUp = 12;

        /// <summary>
        /// 最大买一额限制默认是1888万
        /// </summary>
        private const int MaxBuy1MoneyCtrl = 1888;


        /// <summary>
        /// 最大卖一额限制默认是1888万
        /// </summary>
        private const int MaxSellMoneyCtrl = 1888;

        static ReaderWriterLockSlim rwLockSlimForHistory = new ReaderWriterLockSlim();
        static ReaderWriterLockSlim rwLockSlimForOpenBoard = new ReaderWriterLockSlim();

        //由于是每只股票一个线程，这些字典都是根据股票来区分的，所以不会出现并发问题
        /// <summary>
        /// 开板时间
        /// </summary>
        private Dictionary<string, DateTime> openBoardTime = new Dictionary<string, DateTime>();

        /// <summary>
        /// 开板次数
        /// </summary>
        private Dictionary<string, short> openBoardCnt = new Dictionary<string, short>();

        public void Buy(Quotes quotes, List<Account> accounts, ITrade callback)
        {
            if (!BasicCheck(quotes, accounts))
            {
                return;
            }
            float highLimit = quotes.HighLimit;
            float open = quotes.Open;
            string code = quotes.Code;
            float positionRatioCtrl = 1 / 4;   //买入计划仓位比例

            rwLockSlimForHistory.EnterWriteLock();
            if (!historyTicks.ContainsKey(code))
            {
                historyTicks.Add(code, new Queue<Quotes>(DefaultHistoryTickCnt));
            }
            if (historyTicks[code].Count == DefaultHistoryTickCnt)
            {
                historyTicks[code].Dequeue();
            }
            //由于逻辑关系会return，数据必须在进来时就放进去，上一个取倒数第二就行了
            historyTicks[code].Enqueue(quotes);
            rwLockSlimForHistory.ExitWriteLock();

            //买一小于MinBuy1Ratio的直线拉板，过滤
            if (quotes.Buy1 < quotes.PreClose * MinBuy1Ratio ||
                quotes.Buy2 < quotes.PreClose * MinBuy1Ratio)
            {
                return;
            }
            rwLockSlimForHistory.EnterReadLock();
            Quotes[] ticks = historyTicks[code].ToArray();
            rwLockSlimForHistory.ExitReadLock();
            Quotes lastTickQuotes = null;
            if (ticks.Length >= 2)
            {
                lastTickQuotes = ticks[ticks.Length - 2];
            }
            if (!CheckOpenBoards(quotes, lastTickQuotes, open, highLimit))
            {
                Logger.Log("【" + quotes.Name + "】CheckOpenBoards return ");
                return;
            }
            if (!CheckMoney(accounts, quotes, lastTickQuotes, highLimit, callback))
            {
                Logger.Log("【" + quotes.Name + "】CheckMoney return ");
                return;
            }
            TriggerBuy(accounts, quotes, highLimit, positionRatioCtrl, callback);
        }

        private bool BasicCheck(Quotes quotes, List<Account> accounts)
        {
            if (null == quotes || null == accounts)
            {
                return false;
            }
            //9:30之前不打板
            if (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 30 && !MainStrategy.IsTest)
            {
                return false;
            }
            if ((DateTime.Now.Hour == 14 && DateTime.Now.Minute > 30 || DateTime.Now.Hour > 14)
               && !MainStrategy.IsTest)
            {
                return false;
            }
            return true;
        }

        private bool CheckOpenBoards(
            Quotes quotes, Quotes lastTickQuotes, float open, float highLimit)
        {
            string code = quotes.Code;
            //记录开板时间，这里得用买1来判断封板和开板，即使涨停，但是买1不是涨停价也不算板
            //注意买一和最新价的区别，最新价即使涨停，但是买一不是涨停就没有封住，板就是封住
            bool isBoardLastTick
                = null != lastTickQuotes && lastTickQuotes.Buy1 == highLimit;
            bool isNotBoardThisTick = quotes.Buy1 < highLimit && 0 != quotes.Buy1;
            if (isBoardLastTick && isNotBoardThisTick && !openBoardTime.ContainsKey(code))
            {
                SetOpenBoardState(quotes);
            }
            int minOpenBoardTime = MinOpenBoardTimeOfDown;
            if (Utils.IsUpPeriod())
            {
                minOpenBoardTime = MinOpenBoardTimeOfUp;
            }
            //重置开板时间，为了防止信号出现后重置导致下面买点判断失效，需要等连续2个tick涨停才重置
            bool isBoardThisTick = quotes.Buy1 == highLimit;
            if (isBoardLastTick && isBoardThisTick && openBoardTime.ContainsKey(code))
            {
                int openBoardInterval
                    = (int)DateTime.Now.Subtract(openBoardTime[code]).TotalSeconds;
                Logger.Log("【" + quotes.Name + "】回封");
                openBoardTime.Remove(code);

                //重置后因为触发了买点需要判断下开板时间是否足够
                if (openBoardInterval < minOpenBoardTime)
                {
                    Logger.Log("【" + quotes.Name
                        + "】开板时间（已回封），此次开板时间为" + openBoardInterval + "s");
                    return false;
                }
            }
            rwLockSlimForOpenBoard.EnterReadLock();
            if (openBoardCnt.ContainsKey(code) && openBoardCnt[code] > 3)
            {
                Logger.Log("【" + quotes.Name + "】开板次数大于3次，过滤");
                return false;
            }
            rwLockSlimForOpenBoard.ExitReadLock();
            //开板时间小于30秒，过滤。如果没触发卖2买点，直接回封，这里的判断是没用的
            //因为上面回封后会重置开板时间，所以上面加上了30s的判断
            if (openBoardTime.ContainsKey(code))
            {
                int openBoardInterval
                    = (int)DateTime.Now.Subtract(openBoardTime[code]).TotalSeconds;
                if (openBoardInterval < minOpenBoardTime)
                {
                    Logger.Log("【" + quotes.Name + "】开板时间->" + openBoardInterval + "s");
                    return false;
                }
            }
            else if (open == highLimit)
            {
                //特殊情况是开盘卖一是涨停，而上面是用的买一做判断，所以会漏掉，这里要加上
                if (quotes.Buy1 != highLimit && !openBoardTime.ContainsKey(code))
                {
                    SetOpenBoardState(quotes);
                }
                else
                {
                    Logger.Log("【" + quotes.Name + "】未开板，过滤");
                    //涨停开盘，没开板，过滤
                    return false;
                }
            }
            return true;
        }

        private void SetOpenBoardState(Quotes quotes)
        {
            rwLockSlimForOpenBoard.EnterWriteLock();
            openBoardTime.Add(quotes.Code, DateTime.Now);
            if (!openBoardCnt.ContainsKey(quotes.Code))
            {
                openBoardCnt.Add(quotes.Code, 1);
            }
            else
            {
                openBoardCnt[quotes.Code] += 1;
            }
            rwLockSlimForOpenBoard.ExitWriteLock();
            Logger.Log("【" + quotes.Name + "】开板");
        }

        private static bool CheckMoney(List<Account> accounts,
            Quotes quotes, Quotes lastTickQuotes, float highLimit, ITrade callback)
        {
            //已经涨停且封单大于MaxBuy1MoneyCtrl，过滤
            if (quotes.Buy1 == highLimit)
            {
                if (quotes.Buy1Vol * highLimit > MaxBuy1MoneyCtrl * 10000)
                {
                    return false;
                }
                else if (null == lastTickQuotes || lastTickQuotes.Buy1 == highLimit)
                {
                    //涨停开盘或者上一个tick已经涨停，过滤
                    return false;
                }
            }
            if (quotes.Sell1 == highLimit
                && quotes.Sell1Vol * highLimit > MaxSellMoneyCtrl * 10000)
            {
                Logger.Log("【" + quotes.Name + "】板上货太多，过滤");
                return false;
            }
            if (quotes.Sell2 == highLimit
                && quotes.Sell2Vol * highLimit > MaxSellMoneyCtrl * 10000)
            {
                Logger.Log("【" + quotes.Name + "】板上货太多，过滤");
                return false;
            }
            //买入计划里设置了成交额（单位为万）限制，那么成交额就要大于限制的成交额
            //没有设置大于默认的成交额限制即可
            bool isMoneyQuolified = false;
            Quotes forMoney = null;
            try
            {
                forMoney = TradeAPI.QueryQuotes(accounts[0].TradeSessionId, quotes.Code);
            }
            catch (ApiTimeoutException e)
            {
                ApiHelper.HandleCriticalException(e, e.Message, callback);
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
            if (null != forMoney)
            {
                quotes.Money = forMoney.Money;
            }
            if (quotes.MoneyCtrl > 0 && quotes.Money > quotes.MoneyCtrl * 10000)
            {
                isMoneyQuolified = true;
            }
            else if (quotes.MoneyCtrl == 0 && quotes.Money > DefaultMoneyCtrl * 10000)
            {
                isMoneyQuolified = true;
            }
            return isMoneyQuolified;
        }

        private void TriggerBuy(List<Account> accounts,
            Quotes quotes, float highLimit, float positionRatioCtrl, ITrade callback)
        {
            //买一是涨停价、卖一或者卖二是涨停价符合买点
            if (quotes.Buy1 == highLimit || quotes.Sell1 == highLimit || quotes.Sell2 == highLimit)
            {
                Logger.Log("【" + quotes.Name + "】触发买点");
                //###############以上都是买点判断########################
                //###############下面是买入策略########################
                if (quotes.PositionCtrl > 0)
                {
                    positionRatioCtrl = quotes.PositionCtrl;
                    Logger.Log("【" + quotes.Name + "】设置仓位控制为" + positionRatioCtrl);
                }
                Funds funds = AccountHelper.QueryTotalFunds(accounts, callback);
                //所有账户总可用金额小于每个账号一手的金额或者小于1万，直接退出
                if (funds.AvailableAmt <= MinTotalAvailableAmt * accounts.Count
                    || funds.AvailableAmt <= highLimit * 100 * accounts.Count)
                {
                    Logger.Log("【" + quotes.Name
                        + "】触发买点，结束于总金额不够一万或总账户每户一手");
                    return;
                }
                //可用金额不够用，撤销所有可撤单
                if (funds.AvailableAmt < positionRatioCtrl * funds.TotalAsset)
                {
                    //这里取消撤单后，后面要重新查询资金，否则白撤
                    AccountHelper.CancelOrdersCanCancel(accounts, quotes, callback);
                }
                short successCnt = 0;
                List<Task> tasks = new List<Task>();
                List<Account> failAccts = new List<Account>();
                foreach (Account account in accounts)
                {
                    //每个账户开个线程去处理，账户间同时操作，效率提升大大的
                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            int code = BuyWithAcct(account, quotes, positionRatioCtrl, callback);
                            lock (failAccts)
                            {
                                if (code <= 0)
                                {
                                    failAccts.Add(account);
                                }
                                else if (code != 888)
                                {
                                    successCnt++;
                                }
                            }
                        }
                        catch (ApiTimeoutException e)
                        {
                            ApiHelper.HandleCriticalException(e, e.Message, callback);
                        }
                        catch (Exception e)
                        {
                            Logger.Exception(e);
                        }
                    }));
                    Thread.Sleep(1);
                }
                Task.WaitAll(tasks.ToArray());
                if (null != callback && (successCnt + failAccts.Count) > 0)
                {
                    string tradeResult = "【" + quotes.Name + "】打板买入结果：成功账户"
                        + successCnt + "个，失败账户" + failAccts.Count + "个";
                    callback.OnTradeResult(
                        MainStrategy.RspCodeOfUpdateAcctInfo, tradeResult, "", false);
                    Utils.LogTradeFailedAccts(tradeResult, failAccts);
                }
            }
        }

        private int BuyWithAcct(
            Account account, Quotes quotes, float positionRatioCtrl, ITrade callback)
        {
            float highLimit = quotes.HighLimit;
            float open = quotes.Open;
            string code = quotes.Code;
            Order order = new Order();
            order.Code = code;
            order.Price = highLimit;
            account.Funds = TradeAPI.QueryFunds(account.TradeSessionId);
            //可用金额或者一手价值小于MinTotalAvailableAmt，过滤
            if (account.Funds.AvailableAmt <= MinTotalAvailableAmt
                || account.Funds.AvailableAmt <= highLimit * 100)
            {
                Logger.Log("【" + quotes.Name + "】触发买点，账户["
                    + account.FundAcct + "]结束于可用金额不够，余额为"
                    + account.Funds.AvailableAmt);
                return 888;
            }
            //################计算买入数量BEGIN#######################
            //新增账户后之前账户已经持仓的股，新增账户不买（交了很多学费解决的bug）
            //持仓股都合并在一起，有的买了有的没买，没买的卖出数量是0，也没必要买入了
            //增加买回限制条件上升期和设置是否买回设置
            if (quotes.InPosition && quotes.IsBuyBackWhenReboard && Utils.IsUpPeriod())
            {
                Logger.Log(
                    "【" + quotes.Name + "】触发买点，账户[" + account.FundAcct + "]已经持有");
                //查询已卖数量得到买入数量（可用大于0说明今天之前买的，这种情况只回补仓位）
                int sellQuantity = AccountHelper.GetTodayBoughtQuantityOf(
                    account.TradeSessionId, quotes.Code, Order.OperationSell);
                //因为1/2板会在7%止盈，上板的时候全部打回，T字板也全部打回
                if (open == highLimit || quotes.ContBoards < 3)
                {
                    order.Quantity = sellQuantity;
                    Logger.Log("【" + quotes.Name + "】触发买点，账户["
                + account.FundAcct + "]设置买回数量为卖掉的全部" + order.Quantity);
                }
                else
                {
                    order.Quantity = Utils.FixQuantity((int)(sellQuantity * BuyBackRatio));
                    Logger.Log("【" + quotes.Name + "】触发买点，账户["
                + account.FundAcct + "]设置买回数量为卖掉的1/2=" + order.Quantity);
                }
                if (order.Quantity == 0)
                {
                    Logger.Log("【" + quotes.Name + "】触发买点，账户["
                + account.FundAcct + "]买回数量为0，过滤");
                    return 888;
                }
            }   //END  if (null != position)
            else    //新开仓买入
            {
                Logger.Log("【" + quotes.Name
                    + "】触发买点，账户[" + account.FundAcct + "]将新开仓买入");
                //positionRatioCtrl是基于个股仓位风险控制，profitPositionCtrl是基于账户仓位风险控制
                //账户风险控制直接写死在程序里，没毛病，后面改的必要也不大
                float acctPositionCtrl = GetNewPositionRatio(account);
                double availableCash = account.Funds.AvailableAmt;
                if (acctPositionCtrl <= positionRatioCtrl)
                {
                    positionRatioCtrl = acctPositionCtrl;
                    Logger.Log("【" + quotes.Name + "】触发买点，账户[" + account.FundAcct
                        + "]的仓位控制为账户级风险控制，仓位为" + acctPositionCtrl);
                }
                if (availableCash > account.Funds.TotalAsset * positionRatioCtrl)
                {
                    availableCash = account.Funds.TotalAsset * positionRatioCtrl;
                    Logger.Log("【" + quotes.Name + "】触发买点，账户["
                   + account.FundAcct + "]的买入金额设置为仓位控制后的"
                   + Math.Round(availableCash / 10000, 2) + "万元");
                }
                //数量是整百整百的
                order.Quantity = ((int)(availableCash / (highLimit * 100))) * 100;
                Logger.Log("【" + quotes.Name + "】触发买点，账户["
                    + account.FundAcct + "]经过仓位控制后可买数量为" + order.Quantity + "股");
            }//END else 新开仓买入
            if (!CheckQuantity(account, quotes, order, callback))
            {
                return 888;
            }
            //################计算买入数量END#######################
            //计算出来的数量不够资金买，那么把剩余资金买完
            order.TradeSessionId = account.TradeSessionId;
            ApiHelper.SetShareholderAcct(account, quotes, order);
            int rspCode = TradeAPI.Buy(order);
            string opLog = "资金账号【" + account.FundAcct + "】" + "策略买入【"
                + quotes.Name + "】"
                + Math.Round(order.Quantity * order.Price / account.Funds.TotalAsset * 100) + "%仓位";
            string tradeResult
                = rspCode > 0 ? "#成功" : "#失败：" + order.StrErrorInfo;
            Logger.Log(opLog + tradeResult);

            return rspCode;
        }

        private bool CheckQuantity(Account account, Quotes quotes, Order order, ITrade callback)
        {
            int boughtQuantity = 0;
            try
            {
                boughtQuantity = AccountHelper.GetTodayBoughtQuantityOf(
                         account.TradeSessionId, quotes.Code, Order.OperationBuy);
            }
            catch (ApiTimeoutException e)
            {
                ApiHelper.HandleCriticalException(e, e.Message, callback);
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
            if (order.Quantity <= boughtQuantity)
            {
                Logger.Log("【" + quotes.Name + "】触发买点，账户["
                   + account.FundAcct + "]结束于计划买入数量>=今天已买数量");
                return false;
            }
            else
            {
                order.Quantity -= boughtQuantity;
            }
            //检查委托，如果已经委托，但是没成交，要减去已经委托数量
            List<Order> orders =
                AccountHelper.GetOrdersCanCancelOf(account.TradeSessionId, quotes.Code);
            int orderedQauntity = 0;
            if (orders.Count > 0)
            {
                foreach (Order item in orders)
                {
                    if (item.Operation.Contains("买入"))
                    {
                        //委托数量要扣掉已经撤单的数量
                        orderedQauntity += item.Quantity;
                        orderedQauntity -= item.CanceledQuantity;
                    }
                }
            }
            if (orderedQauntity > 0)
            {
                //买入数量要减去已经委托数量
                order.Quantity -= orderedQauntity;
                Logger.Log("【" + quotes.Name + "】触发买点，账户[" + account.FundAcct
                    + "]已下单数量为" + orderedQauntity + "，减去后为" + order.Quantity);
            }
            if (order.Quantity <= 0)
            {
                Logger.Log("【" + quotes.Name + "】触发买点，账户["
                    + account.FundAcct + "]结束于出去委托数量后可买数量为0");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取新开仓单股仓位>>新账户按盈利计算单股仓位，10%内1/3，10%-20%间1/2，30%以上满仓
        /// </summary>
        /// <param name="fundAcct">资金账号</param>
        /// <returns>仓位比例</returns>
        public static float GetNewPositionRatio(Account account)
        {
            if (null == account || 0 == account.InitTotalAsset)
            {
                return 1 / 3f;
            }
            double totalProfitPct = account.Funds.TotalAsset / account.InitTotalAsset;
            if (totalProfitPct < 1.1)
            {
                return 1 / 3f;
            }
            else if (totalProfitPct < 1.2)
            {
                return 1 / 2f;
            }
            else if (totalProfitPct < 1.3)
            {
                return 0.8f;
            }
            else
            {
                return 1f;
            }
        }

        /// <summary>
        /// 获取code最新的行情
        /// </summary>
        /// <param name="code">股票代码</param>
        /// <returns></returns>
        public Quotes GetLastHistoryQuotesBy(string code)
        {
            Quotes[] ticks = historyTicks[code].ToArray();
            Quotes lastTickQuotes = null;
            if (null != ticks && ticks.Length >= 2)
            {
                lastTickQuotes = ticks[ticks.Length - 1];
            }
            return lastTickQuotes;
        }

        public void RestoreOpenBoardCnt()
        {
            Dictionary<string, short> reservedCnt = JsonDataHelper.Instance.GetOpenBoardCnt();
            if (null != reservedCnt)
            {
                openBoardCnt = reservedCnt;
                Logger.Log("RestoreOpenBoardCnt=" + reservedCnt.ToString());
            }
        }

        public void SaveOpenBoardCnt()
        {
            JsonDataHelper.Instance.SaveOpenBoardCnt(openBoardCnt);
            Logger.Log("SaveOpenBoardCnt=" + openBoardCnt.ToString());
        }
    }
}