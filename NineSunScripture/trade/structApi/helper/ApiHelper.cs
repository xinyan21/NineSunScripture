using NineSunScripture.model;
using NineSunScripture.strategy;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
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
        /// 设置订单的股东代码
        /// </summary>
        /// <param name="account">账号对象</param>
        /// <param name="quotes">股票对象</param>
        /// <param name="order">订单对象</param>
        public static void SetShareholderAcct(Account account, Quotes quotes, Order order)
        {
            if (null == quotes || null == account || null == order)
            {
                return;
            }
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
        public static void HandleApiException(IntPtr errInfo)
        {
            if (null == errInfo)
            {
                return;
            }
            string strErr = ParseErrInfo(errInfo);
            if (!string.IsNullOrEmpty(strErr)
                && (strErr.Contains("超时") || strErr.Equals("ClientID_错误")))
            {
                if (callApiTimeOutCnt > 3)
                {
                    Logger.Log("HandleApiException->" + strErr);
                    throw new ApiTimeoutException(strErr);
                }
                callApiTimeOutCnt += 1;
            }
            else
            {
                callApiTimeOutCnt = 0;
            }
        }

        /// <summary>
        /// 处理严重异常
        /// </summary>
        /// <param name="e">异常对象</param>
        /// <param name="msg">异常信息</param>
        /// <param name="callback">运行日志回调</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void HandleCriticalException(Exception e, string errorInfo, ITrade callback)
        {
            if (null != e)
            {
                Logger.Exception(e);
            }
            if (null != callback)
            {
                callback.OnTradeResult(0, "", errorInfo, true);
            }
        }

        public static Quotes ParseStructToQuotes(IntPtr data)
        {
            if (null == data)
            {
                return null;
            }
            Quotes quotes = new Quotes();
            string code = Marshal.PtrToStringAnsi(data + 16, 6);
            十档行情结构体 price = (十档行情结构体)
                Marshal.PtrToStructure(data + 32, typeof(十档行情结构体));
            quotes.Code = code;
            quotes.Open = (float)price.开盘;
            quotes.Buy1 = (float)(price.最新);
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

        public static List<OByOCommision> ParseStructToCommision(IntPtr data)
        {
            if (null == data)
            {
                return null;
            }
            List<OByOCommision> commisions = new List<OByOCommision>();
            int listSize = Marshal.ReadInt32(data + 4);
            int structSize = Marshal.ReadInt32(data + 12);
            for (int i = 0; i < listSize; i++)
            {
                OByOCommision com = new OByOCommision();
                逐笔委托结构体 item = (逐笔委托结构体)
                    Marshal.PtrToStructure(data + 32 + i * structSize, typeof(逐笔委托结构体));
                com.Time = item.时间;
                com.Category = item.类型;
                com.Price = item.委托价;
                com.Quantity = item.委托量;
                Logger.Log("逐笔委托数组item" + i + "：" + com);
            }

            return commisions;
        }
    }
}