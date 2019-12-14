using NineSunScripture.model;
using NineSunScripture.trade.api;
using NineSunScripture.trade.helper;
using NineSunScripture.util;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.strategy
{
    /// <summary>
    /// 买策略
    /// </summary>
    class BuyStrategy
    {
        /// <summary>
        /// 默认成交额限制是7000万，低于的过滤
        /// </summary>
        private const int DefaultMoneyCtrl = 7000;
        /// <summary>
        /// 最小买一涨幅
        /// </summary>
        private const float MinBuy1Ratio = 1.085f;
        /// <summary>
        /// 最大买一额限制默认是1500万
        /// </summary>
        private const int MaxBuy1MoneyCtrl = 1500;
        private Dictionary<string, DateTime> openBoardTime = new Dictionary<string, DateTime>();
        private Dictionary<string, Queue<Quotes>> historyTicks
            = new Dictionary<string, Queue<Quotes>>();

        public void Buy(Quotes quotes, List<Account> accounts, ITrade callback)
        {
            float highLimit = quotes.HighLimit;
            float open = quotes.Open;
            string code = quotes.Code;
            if (!historyTicks.ContainsKey(code))
            {
                historyTicks.Add(code, new Queue<Quotes>(60));
            }
            if (historyTicks[code].Count == 60)
            {
                historyTicks[code].Dequeue();
            }
            //由于逻辑关系会return，数据必须在进来时就放进去，上一个取倒数第二就行了
            historyTicks[code].Enqueue(quotes);

            if ((DateTime.Now.Hour == 14 && DateTime.Now.Minute > 30 || DateTime.Now.Hour > 14)
                && !MainStrategy.IsTest)
            {
                return;
            }
            if (quotes.LatestPrice < quotes.PreClose * 1.08)
            {
                return;
            }
            Quotes[] ticks = historyTicks[code].ToArray();
            Quotes lastTickQuotes = null;
            if (ticks.Length >= 2)
            {
                lastTickQuotes = ticks[ticks.Length - 2];
            }
            float positionRatioCtrl = 0.33f;   //买入计划仓位比例
            //记录开板时间
            bool isBoardLastTick = null != lastTickQuotes && lastTickQuotes.LatestPrice == highLimit;
            bool isNotBoardThisTick = quotes.Buy1 < highLimit && 0 != quotes.Buy1;
            if (isBoardLastTick && isNotBoardThisTick && !openBoardTime.ContainsKey(code))
            {
                Logger.Log(quotes.Name + "开板");
                openBoardTime.Add(code, DateTime.Now);
            }
            //重置开板时间，为了防止信号出现后重置导致下面买点判断失效，需要等连续2个tick涨停才重置
            bool isBoardThisTick = quotes.LatestPrice == highLimit;
            if (!isBoardLastTick && isBoardThisTick && openBoardTime.ContainsKey(code))
            {
                Logger.Log(quotes.Name + "回封");
                openBoardTime.Remove(code);
            }
            //成交额小于7000万过滤
            //买入计划里设置了成交额（单位为万）限制，这里就要判断
            bool isMoneyNotQuolified = quotes.MoneyCtrl > 0
                && quotes.Money < quotes.MoneyCtrl * 10000;
            //买入计划的成交额限制与7000万限制只要满足一个即可买入
            if (isMoneyNotQuolified && quotes.Money < DefaultMoneyCtrl * 10000)
            {
                return;
            }
            //买一小于MinBuy1Ratio的直线拉板，过滤
            if (quotes.Buy1 < quotes.PreClose * MinBuy1Ratio)
            {
                return;
            }
            //已经涨停且封单大于MaxBuy1MoneyCtrl，过滤
            if (quotes.Buy1 == highLimit)
            {
                if (quotes.Buy1Vol * highLimit > MaxBuy1MoneyCtrl * 10000)
                {
                    return;
                }
                else
                {
                    if (null != lastTickQuotes && lastTickQuotes.Buy1 == highLimit)
                    {
                        return;
                    }
                }
            }
            //开板时间小于30秒，过滤
            if (openBoardTime.ContainsKey(code))
            {
                int openBoardInterval = (int)(DateTime.Now - openBoardTime[code]).TotalSeconds;
                if (openBoardInterval < 30)
                {
                    Logger.Log("openBoardInterval->" + openBoardInterval);
                    return;
                }
            }
            else
            {
                //涨停开盘，没开板，过滤
                if (open == highLimit)
                {
                    return;
                }
            }
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
                Funds funds = AccountHelper.QueryTotalFunds(accounts);
                //所有账户总可用金额小于每个账号一手的金额或者小于1万，直接退出
                if (funds.AvailableAmt < 10000 || funds.AvailableAmt < highLimit * 100 * accounts.Count)
                {
                    Logger.Log("【" + quotes.Name + "】触发买点，结束于总金额不够一万或总账户每户一手");
                    return;
                }
                //可用金额不够用，撤销所有可撤单
                if (funds.AvailableAmt < positionRatioCtrl * funds.TotalAsset)
                {
                    //这里取消撤单后，后面要重新查询资金，否则白撤
                    CancelOrdersCanCancel(accounts, quotes, callback);
                }
                Order order = new Order();
                order.Code = code;
                order.Price = highLimit;
                foreach (Account account in accounts)
                {
                    account.Funds = TradeAPI.QueryFunds(account.TradeSessionId);
                    if (funds.AvailableAmt < 5000 || funds.AvailableAmt < highLimit * 100)
                    {
                        Logger.Log("【" + quotes.Name + "】触发买点，账户["
                            + account.FundAcct + "]结束于可用金额不够");
                        continue;
                    }
                    account.Positions = TradeAPI.QueryPositions(account.TradeSessionId);
                    Position position = AccountHelper.GetPositionOf(account.Positions, code);
                    //################计算买入数量BEGIN#######################
                    if (null != position)
                    {
                        Logger.Log(
                            "【" + quotes.Name + "】触发买点，账户[" + account.FundAcct + "]已经持有");
                        //查询已卖数量得到买入数量（可用大于0说明今天之前买的，这种情况只回补仓位）
                        Logger.Log("【" + quotes.Name + "】触发买点，账户["
                        + account.FundAcct + "]的可用余额大于0，为" + position.AvailableBalance);
                        int sellQuantity = getTodayTransactionQuantityOf(
                            account.TradeSessionId, code, Order.OperationSell);
                        //这里还需要查询当日已买，已经买了就return
                        if (open == highLimit)
                        {
                            order.Quantity = sellQuantity;
                            Logger.Log("【" + quotes.Name + "】触发买点，账户["
                        + account.FundAcct + "]设置买回数量为卖掉的全部" + order.Quantity);
                        }
                        else
                        {
                            order.Quantity = Utils.FixQuantity(sellQuantity / 2);
                            Logger.Log("【" + quotes.Name + "】触发买点，账户["
                        + account.FundAcct + "]设置买回数量为卖掉的1/2=" + order.Quantity);
                        }
                    }   //END  if (null != position)
                    else    //新开仓买入
                    {
                        //新增账户后之前账户已经持仓的股，新增账户不买（交了很多学费解决的bug）
                        if (quotes.InPosition)
                        {
                            Logger.Log("【" + quotes.Name + "】触发买点，但由于是持仓股且账户[" + account.FundAcct
                            + "]是新增账户，不买");
                            return;
                        }
                        Logger.Log("【" + quotes.Name + "】触发买点，账户[" + account.FundAcct
                            + "]将新开仓买入");
                        //positionRatioCtrl是基于个股仓位风险控制，profitPositionCtrl是基于账户仓位风险控制
                        //账户风险控制直接写死在程序里，没毛病，后面改的必要也不大
                        float acctPositionCtrl = getNewPositionRatio(account);
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
                           + account.FundAcct + "]的买入金额设置为仓位控制后的" + availableCash + "元");
                        }
                        //数量是整百整百的
                        order.Quantity = ((int)(availableCash / (highLimit * 100))) * 100;
                        Logger.Log("【" + quotes.Name + "】触发买点，账户["
                            + account.FundAcct + "]经过仓位控制后可买数量为" + order.Quantity + "股");
                    }//END else 新开仓买入
                    int boughtQuantity = getTodayTransactionQuantityOf(
                           account.TradeSessionId, code, Order.OperationBuy);
                    if (order.Quantity <= boughtQuantity)
                    {
                        Logger.Log("【" + quotes.Name + "】触发买点，账户["
                           + account.FundAcct + "]结束于计划买入数量>=今天已买数量");
                        return;
                    }
                    else
                    {
                        order.Quantity -= boughtQuantity;
                    }
                    //检查委托，如果已经委托，但是没成交，要减去已经委托数量
                    List<Order> orders =
                        AccountHelper.GetOrdersCanCancelOf(account.TradeSessionId, code);
                    int orderedQauntity = 0;
                    if (orders.Count > 0)
                    {
                        foreach (Order item in orders)
                        {
                            if (item.Operation.Contains("买入"))
                            {
                                orderedQauntity += item.Quantity;
                            }
                        }
                    }
                    if (orderedQauntity > 0)
                    {
                        //买入数量要减去已经委托数量
                        order.Quantity = order.Quantity - orderedQauntity;
                        Logger.Log("【" + quotes.Name + "】触发买点，账户[" + account.FundAcct
                            + "]已下单数量为" + orderedQauntity + "，减去后为" + order.Quantity);
                    }
                    if (order.Quantity <= 0)
                    {
                        Logger.Log("【" + quotes.Name + "】触发买点，账户["
                            + account.FundAcct + "]结束于出去委托数量后可买数量为0");
                        return;
                    }
                    //################计算买入数量END####################### 
                    //计算出来的数量不够资金买，那么把剩余资金买完
                    if (account.Funds.AvailableAmt < order.Quantity * highLimit)
                    {
                        order.Quantity = ((int)(account.Funds.AvailableAmt / (highLimit * 100))) * 100;
                        Logger.Log("【" + quotes.Name + "】触发买点，账户["
                            + account.FundAcct + "]可用金额不够，数量改为" + order.Quantity);
                    }
                    order.TradeSessionId = account.TradeSessionId;
                    ApiHelper.SetShareholderAcct(account, quotes, order);
                    int rspCode = TradeAPI.Buy(order);
                    string opLog = "资金账号【" + account.FundAcct + "】" + "策略买入【"
                        + quotes.Name + "】"
                        + (order.Quantity * order.Price).ToString("0.00####") + "万元";
                    Logger.Log(opLog);
                    if (null != callback)
                    {
                        string errInfo = ApiHelper.ParseErrInfo(account.ErrorInfo);
                        callback.OnTradeResult(rspCode, opLog, errInfo, false);
                    }
                }//END FOR ACCOUNT
            }
        }

        /// <summary>
        /// 获取新开仓单股仓位>>新账户按盈利计算单股仓位，10%内1/3，10%-20%间1/2，30%以上满仓
        /// </summary>
        /// <param name="fundAcct">资金账号</param>
        /// <returns>仓位比例</returns>
        private float getNewPositionRatio(Account account)
        {
            account.Funds = TradeAPI.QueryFunds(account.TradeSessionId);
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
        /// 获取持仓股，当天已清仓的个股也会在里面，但是可用数量为0
        /// </summary>
        /// <param name="accounts">已登录账户</param>
        /// <returns></returns>
        private String getPositionStock(List<Account> accounts)
        {
            string stocks = "";
            List<Position> positions = AccountHelper.QueryTotalPositions(accounts);
            if (null == positions || positions.Count == 0)
            {
                return stocks;
            }
            foreach (Position item in positions)
            {
                if (!stocks.Contains(item.Code))
                {
                    stocks += item.Code;
                }
            }
            return stocks;
        }

        /// <summary>
        /// 撤销不是quotes的买单以便回笼资金开新仓
        /// </summary>
        /// <param name="accounts">账户对象数组</param>
        /// <param name="quotes">股票对象</param>
        public static void CancelOrdersCanCancel(
            List<Account> accounts, Quotes quotes, ITrade callback)
        {
            foreach (Account account in accounts)
            {
                List<Order> orders = TradeAPI.QueryOrdersCanCancel(account.TradeSessionId);
                if (null == orders || orders.Count == 0)
                {
                    continue;
                }
                foreach (Order order in orders)
                {
                    if (order.Code != quotes.Code && order.Operation.Contains(Order.OperationBuy))
                    {
                        order.TradeSessionId = account.TradeSessionId;
                        int rspCode = TradeAPI.CancelOrder(order);
                        string opLog = "资金账号【" + account.FundAcct + "】" + "撤销【"
                            + quotes.Name + "】委托->"
                            + (order.Quantity * order.Price).ToString("0.00####") + "万元";
                        Logger.Log(opLog);
                        if (null != callback)
                        {
                            string errInfo = ApiHelper.ParseErrInfo(order.ErrorInfo);
                            callback.OnTradeResult(rspCode, opLog, errInfo, false);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获取当天成交的code股票数量
        /// </summary>
        /// <param name="sessionId">登录账号的ID</param>
        /// <param name="code">股票代码</param>
        /// <returns></returns>
        private int getTodayTransactionQuantityOf(int sessionId, string code, string op)
        {
            int quantity = 0;
            List<Order> todayTransactions = TradeAPI.QueryTodayTransaction(sessionId);
            if (null == todayTransactions || todayTransactions.Count == 0)
            {
                return quantity;
            }
            foreach (Order order in todayTransactions)
            {
                if (order.Code == code && order.Operation.Contains(op))
                {
                    quantity += order.Quantity;
                }
            }

            return quantity;
        }



        /// <summary>
        /// 3:25可用资金自动国债逆回购，只买131810深市一天期，每股100
        /// </summary>
        /// <param name="accounts"></param>
        private void AutoReverseRepurchaseBonds(List<Account> accounts, ITrade callback)
        {
            Account mainAcct = null;
            if (accounts.Count > 0)
            {
                mainAcct = accounts[0];
            }
            Order order = new Order();
            order.Code = "131810";
            Quotes quotes = TradeAPI.QueryQuotes(mainAcct.TradeSessionId, order.Code);
            order.Price = quotes.Buy1;
            foreach (Account account in accounts)
            {
                order.TradeSessionId = account.TradeSessionId;
                ApiHelper.SetShareholderAcct(account, quotes, order);
                double availableCash = account.Funds.AvailableAmt;
                order.Quantity = (int)(availableCash / 1000 * 10);
                int rspCode = TradeAPI.Buy(order);
                string opLog
                    = "资金账号【" + account.FundAcct + "】" + "逆回购"
                    + (int)(availableCash / 1000) * 1000;
                Logger.Log(opLog);
                if (null != callback)
                {
                    string errInfo = ApiHelper.ParseErrInfo(order.ErrorInfo);
                    callback.OnTradeResult(rspCode, opLog, errInfo, false);
                }
            }//END FOR
        }//END METHOD
    }
}
