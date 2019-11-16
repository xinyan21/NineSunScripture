using NineSunScripture.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.trade.helper
{
    class ApiHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static String[] parseResult(byte[] data) {
            String result = Encoding.Default.GetString(data).TrimEnd('\0');
            result = result.Substring(result.IndexOf("\n") + 1);
            String[] temp = result.Split(new String[] { "\t" }, StringSplitOptions.RemoveEmptyEntries);
            return temp;
        }
    }
}
