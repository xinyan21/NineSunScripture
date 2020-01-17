﻿using NineSunScripture.model;
using NineSunScripture.strategy;
using NineSunScripture.util.log;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static NineSunScripture.trade.structApi.ApiDataStruct;

namespace NineSunScripture.trade.structApi.helper
{
    public static class ApiHelper
    {
        /// <summary>
        /// 调用API超时次数，超过3次重启策略
        /// </summary>
        private static int callApiTimeOutCnt = 0;

        /// <summary>
        /// 解析错误信息
        /// </summary>
        /// <param name="data">源数据</param>
        /// <returns></returns>
        public static string ParseErrInfo(IntPtr data)
        {
            if (null == data)
            {
                return "";
            }
            return Marshal.PtrToStringAnsi(data);
        }

        /// <summary>
        /// 通过股票代码得到对应的股东账号
        /// </summary>
        /// <param name="acct">账号对象</param>
        /// <param name="code">股票代码</param>
        /// <returns>股东账号</returns>
        public static string GetShareholderByStockCode(Account acct, string code)
        {
            if (code.StartsWith("6"))
            {
                return acct.ShShareholderAcct;
            }
            else
            {
                return acct.SzShareholderAcct;
            }
        }

        /// <summary>
        /// 设置订单的股东代码
        /// </summary>
        /// <param name="account">账号对象</param>
        /// <param name="quotes">股票对象</param>
        /// <param name="order">订单对象</param>
        public static void SetShareholderAcct(Account account, Quotes quotes, Order order)
        {
            if (quotes.Code.StartsWith("6"))
            {
                order.ShareholderAcct = account.ShShareholderAcct;
            }
            else
            {
                order.ShareholderAcct = account.SzShareholderAcct;
            }
        }

        /// <summary>
        /// 处理接口超时问题，若超时抛出异常，主策略捕获后重启策略
        /// </summary>
        /// <param name="errInfo">错误源数据</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void HandleTimeOut(IntPtr errInfo)
        {
            if (null == errInfo)
            {
                return;
            }
            string strErr = ParseErrInfo(errInfo);
            if (!string.IsNullOrEmpty(strErr) && strErr.Contains("超时"))
            {
                if (callApiTimeOutCnt++ > 3)
                {
                    Logger.Log("接口超时>" + ParseErrInfo(errInfo));
                    throw new Exception(strErr);
                }
            }
            else
            {
                callApiTimeOutCnt = 0;
            }
        }

        public static Quotes ParseStructToQuotes(IntPtr data)
        {
            if (null==data)
            {
                return null;
            }
            Quotes quotes = new Quotes();
            string code = Marshal.PtrToStringAnsi(data + 16, 6);
            十档行情结构体 price = (十档行情结构体)
                         Marshal.PtrToStructure(data + 32, typeof(十档行情结构体));
            quotes.Code = code;
            quotes.Open = (float)price.开盘;
            quotes.LatestPrice = (float)price.卖一价;
            quotes.Volume = (int)price.总量;
            quotes.HighLimit = (float)price.涨停;
            quotes.LowLimit = (float)price.跌停;
            quotes.Sell1 = (float)price.卖一价;
            quotes.Sell2 = (float)price.卖二价;
            quotes.Sell3 = (float)price.卖三价;
            quotes.Buy1 = (float)price.买一价;
            quotes.Buy2 = (float)price.买二价;
            quotes.Buy3 = (float)price.买三价;
            quotes.Sell1Vol = (int)price.卖一量;
            quotes.Sell2Vol = (int)price.卖二量;
            quotes.Sell3Vol = (int)price.卖三量;
            quotes.Buy1Vol = (int)price.买一量;
            quotes.Buy2Vol = (int)price.买二量;
            quotes.Buy3Vol = (int)price.买三量;
            quotes.PreClose = (float)price.昨收;

            return quotes;
        }

        public static OByOCommision ParseStructToCommision(IntPtr data)
        {
            if (null == data)
            {
                return null;
            }
            OByOCommision commision = new OByOCommision();
            逐笔委托结构体 temp
                   = (逐笔委托结构体)Marshal.PtrToStructure(data + 32, typeof(逐笔委托结构体));
            commision.Time = temp.时间;
            commision.Category = temp.类型;
            commision.Price = temp.委托价;
            commision.Quantity = temp.委托量;

            return commision;
        }
    }
}