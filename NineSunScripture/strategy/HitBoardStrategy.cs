using NineSunScripture.model;
using NineSunScripture.trade.api;
using NineSunScripture.trade.helper;
using NineSunScripture.util;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.strategy
{
    /// <summary>
    /// 打板策略
    /// </summary>
    class HitBoardStrategy:Strategy
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
        /// 最大买一额限制默认是1888万
        /// </summary>
        private const int MaxBuy1MoneyCtrl = 1888;
        /// <summary>
        /// 单账户最小可用金额默认为5000
        /// </summary>
        private const int MinTotalAvailableAmt = 5000;
        /// <summary>
        /// 最大卖一额限制默认是1888万
        /// </summary>
        private const int MaxSellMoneyCtrl = 1888;
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
            //9:30之前不打板
            if (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 30 && !MainStrategy.IsTest)
            {
                return;
            }

            float highLimit = quotes.HighLimit;
            float open = quotes.Open;
            string code = quotes.Code;
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

            if ((DateTime.Now.Hour == 14 && DateTime.Now.Minute > 30 || DateTime.Now.Hour > 14)
                && !MainStrategy.IsTest)
            {
                return;
            }
            if (quotes.Buy1 < quotes.PreClose * 1.08)
            {
                return;
            }
            Quotes[] ticks = historyTicks[code].ToArray();
            Quotes lastTickQuotes = null;
            if (ticks.Length >= 2)
            {
                lastTickQuotes = ticks[ticks.Length - 2];
            }
            float positionRatioCtrl = 0;   //买入计划仓位比例
            //记录开板时间，这里得用买1来判断封板和开板，即使涨停，但是买1不是涨停价也不算板
            //注意买一和最新价的区别，最新价即使涨停，但是买一不是涨停就没有封住，板就是封住
            bool isBoardLastTick
                = null != lastTickQuotes && lastTickQuotes.Buy1 == highLimit;
            bool isNotBoardThisTick = quotes.Buy1 < highLimit && 0 != quotes.Buy1;
            if (isBoardLastTick && isNotBoardThisTick && !openBoardTime.ContainsKey(code))
            {
                openBoardTime.Add(code, DateTime.Now);
                if (!openBoardCnt.ContainsKey(code))
                {
                    openBoardCnt.Add(code, 1);
                }
                else
                {
                    openBoardCnt[code] += 1;
                }
                Logger.Log("【" + quotes.Name + "】开板");
            }
            //重置开板时间，为了防止信号出现后重置导致下面买点判断失效，需要等连续2个tick涨停才重置
            bool isBoardThisTick = quotes.Buy1 == highLimit;
            if (!isBoardLastTick && isBoardThisTick && openBoardTime.ContainsKey(code))
            {
                int openBoardInterval
                    = (int)DateTime.Now.Subtract(openBoardTime[code]).TotalSeconds;
                Logger.Log("【" + quotes.Name + "】回封");
                openBoardTime.Remove(code);
                //重置后因为触发了买点需要判断下开板时间是否足够
                if (openBoardInterval < 30)
                {
                    Logger.Log(
                        "【" + quotes.Name + "】开板时间（已回封），此次开板时间为" + openBoardInterval + "s");
                    return;
                }
            }
            if (openBoardCnt.ContainsKey(code) && openBoardCnt[code] > 2)
            {
                Logger.Log("【" + quotes.Name + "】开板次数大于2次，过滤");
                return;
            }
            //开板时间小于30秒，过滤。如果没触发卖2买点，直接回封，这里的判断是没用的，因为上面回封后会重置开板时间
            //所以上面加上了30s的判断
            if (openBoardTime.ContainsKey(code))
            {
                int openBoardInterval
                    = (int)DateTime.Now.Subtract(openBoardTime[code]).TotalSeconds;
                if (openBoardInterval < 30)
                {
                    Logger.Log("【" + quotes.Name + "】开板时间->" + openBoardInterval + "s");
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
            //成交额小于7000万过滤
            //买入计划里设置了成交额（单位为万）限制，这里就要判断
            bool isMoneyQuolified = quotes.MoneyCtrl > 0
                && quotes.Money > quotes.MoneyCtrl * 10000;
            //买入计划的成交额限制与默认的7000万限制都不和要求就过滤
            if (!isMoneyQuolified && quotes.Money < DefaultMoneyCtrl * 10000)
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
                    //涨停开盘或者上一个tick已经涨停，过滤
                    if (null == lastTickQuotes || lastTickQuotes.Buy1 == highLimit)
                    {
                        return;
                    }
                }
            }
            if (quotes.Sell1 == highLimit
                && quotes.Sell1Vol * highLimit > MaxSellMoneyCtrl * 10000)
            {
                Logger.Log("【" + quotes.Name + "】板上货太多，过滤");
                return;
            }
            if (quotes.Sell2 == highLimit
                && quotes.Sell2Vol * highLimit > MaxSellMoneyCtrl * 10000)
            {
                Logger.Log("【" + quotes.Name + "】板上货太多，过滤");
                return;
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
                if (funds.AvailableAmt < MinTotalAvailableAmt * accounts.Count
                    || funds.AvailableAmt < highLimit * 100 * accounts.Count)
                {
                    Logger.Log("【" + quotes.Name + "】触发买点，结束于总金额不够一万或总账户每户一手");
                    return;
                }
                //可用金额不够用，撤销所有可撤单
                if (funds.AvailableAmt < positionRatioCtrl * funds.TotalAsset)
                {
                    //这里取消撤单后，后面要重新查询资金，否则白撤
                    AccountHelper.CancelOrdersCanCancel(accounts, quotes, callback);
                }
                foreach (Account account in accounts)
                {
                    //每个账户开个线程去处理，账户间同时操作，效率提升大大的
                    Task.Run(() => BuyWithAcct(account, quotes, positionRatioCtrl, callback));
                }//END FOR ACCOUNT
            }
        }

        private void BuyWithAcct(Account account, Quotes quotes, float positionRatioCtrl, ITrade callback)
        {
            float highLimit = quotes.HighLimit;
            float open = quotes.Open;
            string code = quotes.Code;
            Order order = new Order();
            order.Code = code;
            order.Price = highLimit;
            StringBuilder sbOpLog = new StringBuilder("买入简报：");
            account.Funds = TradeAPI.QueryFunds(account.TradeSessionId);
            if (account.Funds.AvailableAmt < MinTotalAvailableAmt
                || account.Funds.AvailableAmt < highLimit * 100)
            {
                Logger.Log("【" + quotes.Name + "】触发买点，账户["
                    + account.FundAcct + "]结束于可用金额不够");
                return;
            }
            account.Positions = TradeAPI.QueryPositions(account.TradeSessionId);
            //################计算买入数量BEGIN#######################
            //新增账户后之前账户已经持仓的股，新增账户不买（交了很多学费解决的bug）
            //持仓股都合并在一起，有的买了有的没买，没买的卖出数量是0，也没必要买入了
            if (quotes.InPosition)
            {
                Logger.Log(
                    "【" + quotes.Name + "】触发买点，账户[" + account.FundAcct + "]已经持有");
                //查询已卖数量得到买入数量（可用大于0说明今天之前买的，这种情况只回补仓位）
                int sellQuantity = GetTodayTransactionQuantityOf(
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
                if (order.Quantity == 0)
                {
                    Logger.Log("【" + quotes.Name + "】触发买点，账户["
                + account.FundAcct + "]买回数量为0，过滤");
                    return;
                }
            }   //END  if (null != position)
            else    //新开仓买入
            {
                Logger.Log("【" + quotes.Name + "】触发买点，账户["
                    + account.FundAcct + "]将新开仓买入");
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
            int boughtQuantity = GetTodayTransactionQuantityOf(
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
                order.Quantity -= orderedQauntity;
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
            order.TradeSessionId = account.TradeSessionId;
            ApiHelper.SetShareholderAcct(account, quotes, order);
            int rspCode = TradeAPI.Buy(order);
            string opLog = "资金账号【" + account.FundAcct + "】" + "策略买入【"
                + quotes.Name + "】"
                + Math.Round(order.Quantity * order.Price / 10000, 2) + "万元";
            string tradeResult
                = rspCode > 0 ? "#成功" : "#失败：" + ApiHelper.ParseErrInfo(order.ErrorInfo);
            Logger.Log(opLog + tradeResult);
            sbOpLog.Append("\n\n").Append(opLog).Append(tradeResult);
            if (null != callback)
            {
                callback.OnTradeResult(rspCode, sbOpLog.ToString(), "", false);
            }
        }

        /// <summary>
        /// 获取新开仓单股仓位>>新账户按盈利计算单股仓位，10%内1/3，10%-20%间1/2，30%以上满仓
        /// </summary>
        /// <param name="fundAcct">资金账号</param>
        /// <returns>仓位比例</returns>
        public static float GetNewPositionRatio(Account account)
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
        private String GetPositionStock(List<Account> accounts)
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
        /// 获取当天成交的code股票数量
        /// </summary>
        /// <param name="sessionId">登录账号的ID</param>
        /// <param name="code">股票代码</param>
        /// <returns></returns>
        public static int GetTodayTransactionQuantityOf(int sessionId, string code, string op)
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
    }
}
