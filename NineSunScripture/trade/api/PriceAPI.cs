using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NineSunScripture.trade.api
{
    /// <summary>
    /// 行情专用API
    /// </summary>
    public class PriceAPI
    {
        //dll必须放在程序同一目录下面，否则调用会报错
        private const string dllPath = "ths.dll";
        //功能：行情登录 登录成功返回clientID 失败返回0
        //account 登录同花顺的手机号
        //password 密码
        //errInfo 登录失败时返回的提示
        [DllImport(@dllPath, EntryPoint = "HQ_Logon", CallingConvention = CallingConvention.Winapi)]
        public extern static int HQ_Logon(string account, string password, byte[] errInfo);
        //功能：退出登录	无返回值
        //clientID,//客户端ID
        [DllImport(@dllPath, EntryPoint = "HQ_Logoff", CallingConvention = CallingConvention.Winapi)]
        public extern static void HQ_Logoff(int clientID);
        //功能：查询各类行情数据
        //clientID	登录成功时返回值
        //category 查询类型 
        //gpdm		股票代码	0十档行情价 1逐笔成交明细 2买列队数据 3卖列队数据
        //name		股票名称 正常情况下可空		不可空的情况有：ST股 退市股     
        //Result	个别类型数据需要解析  可参考演示中的代码
        [DllImport(@dllPath, EntryPoint = "HQ_QueryData", CallingConvention = CallingConvention.Winapi)]
        public extern static int HQ_QueryData(int clientID, int category, string gpdm,
            string name, byte[] Result, byte[] errInfo);
    }
}
