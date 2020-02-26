using NineSunScripture.model;
using NineSunScripture.trade.structApi.helper;
using NineSunScripture.util.log;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using static NineSunScripture.trade.structApi.ApiDataStruct;

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

        private static ReaderWriterLockSlim rwls = new ReaderWriterLockSlim();

        /// <summary>
        /// 十档行情，没有股票名称
        /// </summary>
        /// <param name="priceSessionId">行情会话Id</param>
        /// <param name="code">股票代码</param>
        /// <returns></returns>
        public static Quotes QueryTenthGearPrice(int priceSessionId, string code)
        {
            Quotes quotes = new Quotes();
            //有的接口就是不能用封装在对象里面的那种形式，只能采取下面这种不封装的形式
            rwls.EnterWriteLock();
            IntPtr result = Marshal.AllocCoTaskMem(1024 * 1024);
            IntPtr errorInfo = Marshal.AllocCoTaskMem(256);
            rwls.ExitWriteLock();
            try
            {
                int rspCode
                   = HQ_QueryData(priceSessionId, 0, code, "", result, errorInfo);
                ApiHelper.HandleApiException(errorInfo);
                quotes.StrErrorInfo = ApiHelper.ParseErrInfo(errorInfo);
                if (rspCode > 0)
                {
                    十档行情结构体 price = (十档行情结构体)
                        Marshal.PtrToStructure(result + 32, typeof(十档行情结构体));
                    quotes.Code = code;
                    quotes.Open = (float)price.开盘;
                    quotes.Buy1 = (float)price.最新;
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
                }
            }
            catch (Exception e)
            {
                Logger.Log("QueryTenthGearPrice exception：" + quotes.StrErrorInfo);
                Logger.Exception(e, quotes.StrErrorInfo);
                throw;
            }
            finally
            {
                rwls.EnterWriteLock();
                Marshal.FreeCoTaskMem(result);
                Marshal.FreeCoTaskMem(errorInfo);
                rwls.ExitWriteLock();
            }
            return quotes;
        }
    }
}