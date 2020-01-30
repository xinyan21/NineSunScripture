using NineSunScripture.model;
using NineSunScripture.util.log;
using System;
using System.Runtime.InteropServices;

namespace NineSunScripture.trade.structApi.api
{
    /// <summary>
    /// 行情专用API
    /// </summary>
    public static class PriceAPI
    {
        /// <summary>
        /// 十档行情推送
        /// </summary>
        public const short PushTypeTenGear = 0;

        /// <summary>
        /// 逐笔委托推送
        /// </summary>
        public const short PushTypeOByOCommision = 1;

        //dll必须放在程序同一目录下面，否则调用会报错
        private const string dllPath = "StructApi.dll";

        //功能：行情登录 登录成功返回sessionId 失败返回0
        //account 登录同花顺的手机号
        //password 密码
        //errInfo 登录失败时返回的提示
        [DllImport(@dllPath,
            EntryPoint = "HQ_Logon",    CallingConvention = CallingConvention.Winapi)]
        public static extern int HQ_Logon(string account, string password, IntPtr errInfo);

        //功能：退出登录	无返回值
        //sessionId,//客户端ID
        [DllImport(@dllPath,
            EntryPoint = "HQ_Logoff", CallingConvention = CallingConvention.Winapi)]
        public static extern void HQ_Logoff(int sessionId);

        //功能：查询各类行情数据
        //sessionId	登录成功时返回值
        //category 查询类型
        //gpdm		股票代码	0十档行情价 1逐笔成交明细 2买列队数据 3卖列队数据
        //name		股票名称 正常情况下可空		不可空的情况有：ST股 退市股
        //PtrResult	个别类型数据需要解析  可参考演示中的代码
        [DllImport(@dllPath,
            EntryPoint = "HQ_QueryData", CallingConvention = CallingConvention.Winapi)]
        public static extern int HQ_QueryData(
            int sessionId, int category, string gpdm, string name, IntPtr PtrResult, IntPtr errInfo);

        public delegate void PushCallback(int type, IntPtr Ptr);

        [DllImport(@dllPath, 
            EntryPoint = "HQ_PushData", CallingConvention = CallingConvention.Winapi)]
        public static extern int HQ_PushData(
            int ClientID, int Category, string Gddm, string name, PushCallback Callback, bool state);

        /// <summary>
        /// 十档行情，支持level2高速行情（没有成交额），这2个接口都没有股票名称，日
        /// </summary>
        /// <param name="priceSessionId">行情会话Id</param>
        /// <param name="code">股票代码</param>
        /// <returns></returns>
        public static Quotes QueryTenthGearPrice(int priceSessionId, string code)
        {
            Quotes quotes = new Quotes();
            quotes.AllocCoTaskMem();

            try
            {
                int rspCode
                    = HQ_QueryData(priceSessionId, 1, code, "", quotes.PtrResult, quotes.PtrErrorInfo);
                /* ApiHelper.HandleTimeOut(quotes.PtrErrorInfo);
                 quotes.StrErrorInfo = ApiHelper.ParseErrInfo(quotes.PtrErrorInfo);
                 if (rspCode > 0)
                 {
                     十档行情结构体 price =(十档行情结构体)
                         Marshal.PtrToStructure(quotes.PtrResult + 32, typeof(十档行情结构体));
                     quotes.Code = code;
                     quotes.Open = (float)price.开盘;
                     quotes.LatestPrice = (float)price.最新;
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
                 }
                 else
                 {
                     Logger.Log("QueryTenthGearPrice error：" + quotes.StrErrorInfo);
                     return null;
                 }*/
            }
            catch (Exception e)
            {
                Logger.Log("QueryTenthGearPrice exception：" + quotes.StrErrorInfo);
                Logger.Exception(e, quotes.StrErrorInfo);
                throw;
            }
            finally
            {
                quotes.FreeCoTaskMem();
            }
            /*if (MainStrategy.IsTest
                   && quotes.Code.Equals("300643") && DateTime.Now.Second == 30)
               {
                   quotes = TradeTestCase.ConstructHitBoardData(quotes);
                   Logger.Log("ConstructHitBoardData for 300643 is ready>" + quotes.ToString());
               }
               if (MainStrategy.IsTest
              && quotes.Code.Equals("002351") && DateTime.Now.Second == 0)
            {
                quotes = TradeTestCase.ConstructHitBoardData(quotes);
                Logger.Log("ConstructHitBoardData for 300643 is ready>" + quotes.ToString());
            }*/
            quotes.Code = "000004";
            quotes.HighLimit = 24.55f;
            quotes.Open = 22;
            quotes.Buy1 = 22.3f;
            quotes.PreClose = 22.3f;
            quotes.Name = "国农科技";
            quotes.Buy1Vol = 100000;
            quotes.Sell1 = 22.32f;
            quotes.Sell1Vol = 10000;
            quotes.Sell2 = 22.33f;
            quotes.Sell2Vol = 10000;
            quotes.Money = 100000000;
            quotes.LatestPrice = quotes.Sell1;
            return quotes;
        }
    }
}