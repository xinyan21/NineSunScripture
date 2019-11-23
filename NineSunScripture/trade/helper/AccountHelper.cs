﻿using NineSunScripture.model;
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
        /// <param name="trade">回调接口</param>
        public static List<Account> Login(ITrade trade)
        {
            List<Account> accounts = new AcctDbHelper().GetAccounts();
            if (null == accounts || accounts.Count == 0)
            {
                return null;
            }
            foreach (Account account in accounts)
            {
                int userId = TradeAPI.Logon(account.BrokerId, account.BrokerServerIP,
                    account.BrokerServerPort, account.VersionOfTHS, account.AcctType,
                    account.FundAcct, account.Password, account.CommPwd,
                    account.IsRandomMac, account.ErrorInfo);
                if (userId > 0)
                {
                    account.ClientId = userId;
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
            return accounts;
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

        /// <summary>
        /// 查询所有账户总持仓：个股数据全部汇总到一起成为一个账户
        /// </summary>
        /// <param name="accounts">账户数组</param>
        /// <returns></returns>
        public static List<Position> QueryPositions(List<Account> accounts)
        {
            List<Position> positions = new List<Position>();
            List<Position> allPositions = new List<Position>(); //所有账户持仓原始数据
            foreach (Account account in accounts)
            {
                allPositions.AddRange(TradeAPI.QueryPositions(account.ClientId));
            }
            List<Position> temp = allPositions.Distinct().ToList();
            foreach (Position item in temp)
            {
                Position position = new Position();
                position.Code = item.Code;
                position.Name = item.Name;
                position.Price = item.Price;
                positions.Add(position);
            }
            //把所有个股持仓数据分别汇总
            foreach (Position item in allPositions)
            {
                foreach (Position item2 in positions)
                {
                    if (item.Code == item2.Code)
                    {
                        item2.AvailableQuantity += item.AvailableQuantity;
                        item2.AvgCost += item.AvgCost;
                        item2.FrozenQuantity += item.FrozenQuantity;
                        item2.ProfitAndLoss += item.ProfitAndLoss;
                        item2.ProfitAndLossPct += item.ProfitAndLossPct;
                        item2.QuantityBalance += item.QuantityBalance;
                    }
                }
            }

            return positions;
        }

        /// <summary>
        /// 查询所有账户总资金
        /// </summary>
        /// <param name="accounts">账户列表</param>
        /// <returns></returns>
        public static Funds QueryTotalFunds(List<Account> accounts)
        {
            Funds funds = new Funds();
            foreach (Account account in accounts)
            {
                Funds temp = TradeAPI.QueryFunds(account.ClientId);
                funds.AvailableAmt += temp.AvailableAmt;
                funds.FrozenAmt += temp.FrozenAmt;
                funds.FundBalance += temp.FundBalance;
                funds.TotalAsset += temp.TotalAsset;
            }

            return funds;
        }
    }
}
