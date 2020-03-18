using NineSunScripture.model;
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
    /// 弱转强策略
    /// </summary>
    public class WeakTurnStrongStrategy
    {
        /// <summary>
        /// 最小买一额限制默认是1888万
        /// </summary>
        private const int MinBuy1MoneyCtrl = 1888;

        /// <summary>
        /// 单账户最小可用金额默认为5000
        /// </summary>
        private const int MinTotalAvailableAmt = 5000;

        /// <summary>
        /// 开盘最小涨幅
        /// </summary>
        private const float MinUpRatio = 1.01f;

        /// <summary>
        /// 开盘最大涨幅
        /// </summary>
        private const float MaxUpRatio = 1.06f;

        public void Buy(Quotes quotes, List<Account> accounts, ITrade callback)
        {
            bool isTradeTime = DateTime.Now.Hour == 9 && DateTime.Now.Minute == 30;
            if (!isTradeTime)
            {
                return;
            }
            if (null == accounts || null == quotes)
            {
                return;
            }
            float changeRatio = quotes.Buy1 / quotes.PreClose;
            bool canBuy = changeRatio > MinUpRatio && changeRatio < MaxUpRatio
                && quotes.Buy1 * quotes.Buy1Vol > MinBuy1MoneyCtrl;
            if (!canBuy)
            {
                return;
            }
            Logger.Log("【" + quotes.Name + "】触发弱转强买点");
            //###############以上都是买点判断########################
            //###############下面是买入策略########################
            float positionRatioCtrl = 0;   //买入计划仓位比例
            if (quotes.PositionCtrl > 0)
            {
                positionRatioCtrl = quotes.PositionCtrl;
                Logger.Log("【" + quotes.Name + "】设置仓位控制为" + positionRatioCtrl);
            }
            Funds funds = AccountHelper.QueryTotalFunds(accounts, callback);
            //所有账户总可用金额小于每个账号一手的金额或者小于1万，直接退出
            if (funds.AvailableAmt < MinTotalAvailableAmt * accounts.Count
                || funds.AvailableAmt < quotes.Buy1 * 100 * accounts.Count)
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
            short successCnt = 0;
            List<Task> tasks = new List<Task>();
            List<Account> failAccts = new List<Account>();
            foreach (Account account in accounts)
            {
                tasks.Add(Task.Run(() =>
                {
                    int code = BuyWithAcct(account, quotes, funds, positionRatioCtrl);
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
                }));
                Thread.Sleep(1);
            }//END FOR
            Task.WaitAll(tasks.ToArray());
            if (null != callback && (successCnt + failAccts.Count) > 0)
            {
                string tradeResult = "【" + quotes.Name + "】弱转强买入结果：成功账户"
                    + successCnt + "个，失败账户" + failAccts.Count + "个";
                callback.OnTradeResult(MainStrategy.RspCodeOfUpdateAcctInfo, tradeResult, "", false);
                Utils.LogTradeFailedAccts(tradeResult, failAccts);
            }
        }//END BUY

        private int BuyWithAcct(
            Account account, Quotes quotes, Funds funds, float positionRatioCtrl)
        {
            Order order = new Order();
            order.Code = quotes.Code;
            order.Price = quotes.Sell1;
            account.Funds = TradeAPI.QueryFunds(account.TradeSessionId);
            if (funds.AvailableAmt < MinTotalAvailableAmt || funds.AvailableAmt < quotes.Sell1 * 100)
            {
                Logger.Log("【" + quotes.Name + "】触发买点，账户["
                    + account.FundAcct + "]结束于可用金额不够");
                return 888;
            }
            account.Positions = TradeAPI.QueryPositions(account.TradeSessionId);
            Position position = AccountHelper.GetPositionOf(account.Positions, quotes.Code);
            //################计算买入数量BEGIN#######################
            if (null != position)
            {
                Logger.Log(
                    "【" + quotes.Name + "】触发买点，账户[" + account.FundAcct + "]已经持有");
                return 888;
            }   //END  if (null != position)
            else    //新开仓买入
            {
                //新增账户后之前账户已经持仓的股，新增账户不买（交了很多学费解决的bug）
                if (quotes.InPosition)
                {
                    Logger.Log("【" + quotes.Name + "】触发买点，但由于是持仓股且账户[" + account.FundAcct
                    + "]是新增账户，不买");
                    return 888;
                }
                Logger.Log("【" + quotes.Name + "】触发买点，账户[" + account.FundAcct
                    + "]将新开仓买入");
                //positionRatioCtrl是基于个股仓位风险控制，profitPositionCtrl是基于账户仓位风险控制
                //账户风险控制直接写死在程序里，没毛病，后面改的必要也不大
                float acctPositionCtrl = HitBoardStrategy.GetNewPositionRatio(account);
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
                order.Quantity = ((int)(availableCash / (quotes.Sell1 * 100))) * 100;
                Logger.Log("【" + quotes.Name + "】触发买点，账户["
                    + account.FundAcct + "]经过仓位控制后可买数量为" + order.Quantity + "股");
            }//END else 新开仓买入
            int boughtQuantity = AccountHelper.GetTodayBoughtQuantityOf(
                   account.TradeSessionId, quotes.Code, Order.OperationBuy);
            if (order.Quantity <= boughtQuantity)
            {
                Logger.Log("【" + quotes.Name + "】触发买点，账户["
                   + account.FundAcct + "]结束于计划买入数量>=今天已买数量");
                return 888;
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
                return 888;
            }
            //################计算买入数量END#######################
            //计算出来的数量不够资金买，那么把剩余资金买完
            if (account.Funds.AvailableAmt < order.Quantity * quotes.Sell1)
            {
                order.Quantity = ((int)(account.Funds.AvailableAmt / (quotes.Sell1 * 100))) * 100;
                Logger.Log("【" + quotes.Name + "】触发买点，账户["
                    + account.FundAcct + "]可用金额不够，数量改为" + order.Quantity);
            }
            order.TradeSessionId = account.TradeSessionId;
            ApiHelper.SetShareholderAcct(account, quotes, order);
            int rspCode = TradeAPI.Buy(order);
            string opLog = "资金账号【" + account.FundAcct + "】" + "策略买入【"
                + quotes.Name + "】"
                 + Math.Round(order.Quantity * order.Price / account.Funds.TotalAsset * 100) + "%仓位";
            Logger.Log(opLog);
            return rspCode;
        }
    }//END CLASS
}