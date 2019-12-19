using NineSunScripture.model;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.trade.helper
{
    public static class ApiHelper
    {
        /// <summary>
        /// 解析接口结果字符串
        /// </summary>
        /// <param name="data">接口返回的字节流</param>
        /// <returns>结果分割后的字符数组</returns>
        public static String[] ParseResult(byte[] data)
        {
            String result = Encoding.Default.GetString(data).TrimEnd('\0');
            Logger.Log(result, LogType.Quotes);
            result = result.Substring(result.IndexOf("\n") + 1);
            String[] temp = result.Split(new String[] { "\t" }, StringSplitOptions.None);
            return temp;
        }

        /// <summary>
        /// 解析错误信息
        /// </summary>
        /// <param name="data">源数据</param>
        /// <returns></returns>
        public static String ParseErrInfo(byte[] data)
        {
            return Encoding.Default.GetString(data).TrimEnd('\0');
        }

        /// <summary>
        /// 解析接口多个结果字符串
        /// </summary>
        /// <param name="data">接口返回的字节流</param>
        /// <returns>结果分割后的二维字符数组</returns>
        public static String[,] ParseResults(byte[] data)
        {
            String result = Encoding.Default.GetString(data).TrimEnd('\0');
            result = result.Substring(result.IndexOf("\n") + 1);
            String[] rows = result.Split(new String[] { "\n" },
                StringSplitOptions.None);
            int cols = rows[0].Split(new String[] { "\t" },
                StringSplitOptions.None).Length;
            string[,] temp = new string[rows.Length, cols];
            for (int i = 0; i < rows.Length; i++)
            {
                string[] items = rows[i].Split(new String[] { "\t" },
                    StringSplitOptions.None);
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
        public static String GetShareholderByStockCode(Account acct, String code)
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
        public static void HandleTimeOut(byte[] errInfo)
        {
            if (null == errInfo)
            {
                return;
            }
            string strErr = ParseErrInfo(errInfo);
            if (!string.IsNullOrEmpty(strErr) && strErr.Contains("超时"))
            {
                throw new Exception(strErr);
            }
        }
    }
}
