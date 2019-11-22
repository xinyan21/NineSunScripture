using NineSunScripture.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using NineSunScripture.trade.api;
using NineSunScripture.strategy;
using NineSunScripture.db;
using NineSunScripture.util.log;

namespace NineSunScripture.trade.helper
{
    public class AccountHelper
    {
        /// <summary>
        /// 登录所有账户，首次在客户端登录会记录初始总资金到数据库
        /// </summary>
        /// <param name="accounts"></param>
        /// <param name="trade"></param>
        public static void Login(List<Account> accounts, ITrade trade)
        {
            if (null == accounts || accounts.Count == 0)
            {
                return;
            }
            foreach (Account account in accounts)
            {
                int userId = TradeAPI.Logon(account.BrokerId, account.BrokerServerIP,
                    account.BrokerServerPort, account.VersionOfTHS, account.AcctType,
                    account.FundAcct, account.Password, account.CommPwd,
                    account.IsRandomMac, account.ErrorInfo);
                if (userId > 0)
                {
                    account.Funds = TradeAPI.QueryFunds(userId);
                    account.Positions = TradeAPI.QueryPositions(userId);
                    Logger.log("资金账号" + account.FundAcct + "登录成功，ID为" + userId);
                }
                else
                {
                    string opLog = "资金账号" + account.FundAcct + "登录失败，信息："
                        + ApiHelper.ParseErrInfo(account.ErrorInfo);
                    Logger.log(opLog);
                    trade.OnTradeResult(1, opLog, ApiHelper.ParseErrInfo(account.ErrorInfo));
                }
                if (account.InitTotalAsset == 0)
                {
                    new AcctDbHelper().EditInitTotalAsset(account);
                }
            }
        }

        /// <summary>
        /// 获取单个账户里stock的持仓信息，一只股票只有一个持仓对象
        /// </summary>
        /// <param name="positions">账户所有持仓</param>
        /// <param name="stock">股票代码</param>
        /// <returns>stock的持仓对象</returns>
        public static Position GetPositionOf(List<Position> positions, string stock)
        {
            foreach (Position position in positions)
            {
                if (position.Code == stock)
                {
                    return position;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取总账户stock的持仓信息
        /// </summary>
        /// <param name="accounts">账户列表</param>
        /// <param name="stock">股票代码</param>
        /// <returns>stock的持仓对象</returns>
        public static Position GetPositionOf(List<Account> accounts, string stock)
        {
            Position position = null;
            foreach (Account account in accounts)
            {
                Position temp = GetPositionOf(account.Positions, stock);
                if (null == position)
                {
                    continue;
                }
                position.AvailableQuantity += temp.AvailableQuantity;
                position.AvgCost = (position.AvgCost + temp.AvgCost) / 2;
                position.FrozenQuantity += temp.FrozenQuantity;
                position.ProfitAndLoss += temp.ProfitAndLoss;
                position.ProfitAndLossPct = (position.ProfitAndLossPct + temp.ProfitAndLossPct) / 2;
            }
            return position;
        }
    }
}
