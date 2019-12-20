using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using NineSunScripture.model;
using NineSunScripture.util.log;
using NineSunScripture.trade.helper;
using NineSunScripture.strategy;
using System.Runtime.CompilerServices;

namespace NineSunScripture.trade.api
{
    /// <summary>
    /// 行情专用API
    /// </summary>
    public class PriceAPI
    {
        //dll必须放在程序同一目录下面，否则调用会报错
        private const string dllPath = "ths.dll";
        //功能：行情登录 登录成功返回sessionId 失败返回0
        //account 登录同花顺的手机号
        //password 密码
        //errInfo 登录失败时返回的提示
        [DllImport(@dllPath, EntryPoint = "HQ_Logon", CallingConvention = CallingConvention.Winapi)]
        public extern static int HQ_Logon(string account, string password, byte[] errInfo);
        //功能：退出登录	无返回值
        //sessionId,//客户端ID
        [DllImport(@dllPath, EntryPoint = "HQ_Logoff", CallingConvention = CallingConvention.Winapi)]
        public extern static void HQ_Logoff(int sessionId);
        //功能：查询各类行情数据
        //sessionId	登录成功时返回值
        //category 查询类型 
        //gpdm		股票代码	0十档行情价 1逐笔成交明细 2买列队数据 3卖列队数据
        //name		股票名称 正常情况下可空		不可空的情况有：ST股 退市股     
        //Result	个别类型数据需要解析  可参考演示中的代码
        [DllImport(@dllPath, EntryPoint = "HQ_QueryData", CallingConvention = CallingConvention.Winapi)]
        public extern static int HQ_QueryData(int sessionId, int category, string gpdm,
            string name, byte[] Result, byte[] errInfo);

        /// <summary>
        /// 【同步方法】十档行情，支持level2高速行情
        /// </summary>
        /// <param name="priceSessionId">行情会话Id</param>
        ///   /// <param name="tradeSessionId">交易会话Id</param>
        /// <param name="code">股票代码</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static Quotes QueryTenthGearPrice(int priceSessionId, int tradeSessionId, string code)
        {
            Quotes quotes = null;
            //由于股票名称在十档里没有，所以先查五档，之后用十档的数据覆盖
            quotes = TradeAPI.QueryQuotes(tradeSessionId, code);
            if (null == quotes)
            {
                return null;
            }
            int rspCode = HQ_QueryData(priceSessionId, 0, code, "", quotes.Result, quotes.ErrorInfo);
            ApiHelper.HandleTimeOut(quotes.ErrorInfo);
            if (rspCode > 0)
            {
                try
                {
                    //代码\t开盘\t最新\t总量\t买价\t买量\t买二\t买二量\t买三\t买三量\t卖价\t卖量\t卖二\t卖二量\t卖三\t卖三量\t涨停\t跌停
                    //\t买六\t买六量\t卖六\t卖六量\t买七\t买七量\t卖七\t卖七量\t买八\t买八量\t卖八\t卖八量\t买九\t买九量\t卖九\t卖九量
                    //\t买十\t买十量\t卖十\t卖十量\t买四\t买四量\t卖四\t卖四量\t买五\t买五量\t卖五\t卖五量\t昨收
                    String[] temp = ApiHelper.ParseResult(quotes.Result);
                    if (temp.Length < 47)
                    {
                        Logger.Log("【重要】HQ_QueryData returns wrong data, start using trade price!");
                        return quotes;
                    }
                    quotes.Code = temp[0];
                    quotes.Open = float.Parse(temp[1]);
                    quotes.LatestPrice = float.Parse(temp[2]);
                    quotes.Volume = int.Parse(temp[3]);
                    quotes.HighLimit = float.Parse(temp[16]);
                    quotes.LowLimit = float.Parse(temp[17]);
                    quotes.Sell1 = float.Parse(temp[10]);
                    quotes.Sell2 = float.Parse(temp[12]);
                    quotes.Sell3 = float.Parse(temp[14]);
                    quotes.Buy1 = float.Parse(temp[4]);
                    quotes.Buy2 = float.Parse(temp[6]);
                    quotes.Buy3 = float.Parse(temp[8]);
                    quotes.Sell1Vol = int.Parse(temp[11]);
                    quotes.Sell2Vol = int.Parse(temp[13]);
                    quotes.Sell3Vol = int.Parse(temp[15]);
                    quotes.Buy1Vol = int.Parse(temp[5]);
                    quotes.Buy2Vol = int.Parse(temp[7]);
                    quotes.Buy3Vol = int.Parse(temp[9]);
                    quotes.PreClose = float.Parse(temp[46]);
                }
                catch (Exception e)
                {
                    Logger.Log("QueryTenthGearPrice exception：" + ApiHelper.ParseErrInfo(quotes.Result));
                    Logger.Exception(e, ApiHelper.ParseErrInfo(quotes.Result));
                    throw e;
                }
            }
            else
            {
                Logger.Log("QueryTenthGearPrice error：" + ApiHelper.ParseErrInfo(quotes.ErrorInfo));
                return null;
            }
            return quotes;
        }
    }
}
