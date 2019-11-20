using NineSunScripture.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.trade.helper
{
    public class ApiHelper
    {
        /// <summary>
        /// 解析接口结果字符串
        /// </summary>
        /// <param name="data">接口返回的字节流</param>
        /// <returns>结果分割后的字符数组</returns>
        public static String[] ParseResult(byte[] data)
        {
            String result = Encoding.Default.GetString(data).TrimEnd('\0');
            result = result.Substring(result.IndexOf("\n") + 1);
            String[] temp = result.Split(new String[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
            return temp;
        }

        /// <summary>
        /// 解析接口多个结果字符串
        /// </summary>
        /// <param name="data">接口返回的字节流</param>
        /// <returns>结果分割后的二维字符数组</returns>
        public static String[][] ParseResults(byte[] data)
        {
            String result = Encoding.Default.GetString(data).TrimEnd('\0');
            result = result.Substring(result.IndexOf("\n") + 1);
            String[] rows = result.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string[][] temp = new string[][] { };
            for (int i = 0; i < rows.Length; i++)
            {
                temp[i] = result.Split(new String[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
            }
            return temp;
        }

        public static String ParseErrInfo(byte[] errorInfo)
        {
            return Encoding.Default.GetString(errorInfo).TrimEnd('\0');
        }

        /// <summary>
        /// 通过股票代码得到对应的股东账号
        /// </summary>
        /// <param name="acct">账号对象</param>
        /// <param name="code">股票代码</param>
        /// <returns>股东账号</returns>
        public static String getShareholderByStockCode(Account acct, String code)
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
    }
}
