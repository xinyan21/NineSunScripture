using NineSunScripture.model;
using NineSunScripture.util.log;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace NineSunScripture.trade.helper
{
    public static class ApiHelper
    {
        /// <summary>
        /// 调用API超时次数，超过3次重启策略
        /// </summary>
        private static int callApiTimeOutCnt = 0;

        /// <summary>
        /// 解析接口结果字符串
        /// </summary>
        /// <param name="data">接口返回的字节流</param>
        /// <returns>结果分割后的字符数组</returns>
        public static string[] ParseResult(byte[] data)
        {
            if (null == data)
            {
                return null;
            }
            string result = Encoding.Default.GetString(data).TrimEnd('\0');
            result = result.Substring(result.IndexOf("\n") + 1);
            string[] temp = result.Split(new string[] { "\t" }, StringSplitOptions.None);
            return temp;
        }

        /// <summary>
        /// 解析错误信息
        /// </summary>
        /// <param name="data">源数据</param>
        /// <returns></returns>
        public static string ParseErrInfo(byte[] data)
        {
            if (null == data)
            {
                return "";
            }
            return Encoding.Default.GetString(data).TrimEnd('\0');
        }

        /// <summary>
        /// 解析接口多个结果字符串
        /// </summary>
        /// <param name="data">接口返回的字节流</param>
        /// <returns>结果分割后的二维字符数组</returns>
        public static string[,] ParseResults(byte[] data)
        {
            if (null == data)
            {
                return null;
            }
            string result = Encoding.Default.GetString(data).TrimEnd('\0');
            result = result.Substring(result.IndexOf("\n") + 1);
            string[] rows = result.Split(new string[] { "\n" }, StringSplitOptions.None);
            int cols = rows[0].Split(new string[] { "\t" },
                StringSplitOptions.None).Length;
            string[,] temp = new string[rows.Length, cols];
            for (int i = 0; i < rows.Length; i++)
            {
                string[] items = rows[i].Split(new string[] { "\t" }, StringSplitOptions.None);
                for (int j = 0; j < items.Length; j++)
                {
                    temp[i, j] = items[j];
                }
            }
            return temp;
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
        public static void HandleTimeOut(byte[] errInfo)
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
    }
}
