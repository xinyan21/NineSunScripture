using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using NineSunScripture.model;
using NineSunScripture.trade.helper;

namespace NineSunScripture.trade.api
{
    /// <summary>
    /// 交易API，附带五档行情API
    /// </summary>
    public class TradeAPI
    {
        //dll必须放在程序同一目录下面，否则调用会报错
        private const string dllPath = "ths.dll";
        //功能：登录 成功返回clientID	失败返回0
        //Qsid,//券商id 					这个值可以在IP文件中查看
        //Host,//券商服务器IP
        //Port,//券商服务器端口
        //Version,//同花顺客户端版本号		目前通用版本号E065.20.92			目前发现，海通需要使用老版本号E065.18.77
        //AccountType,//账户类型 0x20自动判断(有些券商可能不支持) 0x30资金账户 0x31深圳账户 0x32上海账户 0x33基金账户 0x34深圳B股 0x35上海B股 0x6B客户号
        //Account,//券商账号“账号类型需要与上面的对应”
        //Password,//券商密码
        //comm_password,//通讯密码	可空
        //dommac 是否随机MAC 假=取本机MAC  真=每次登录都随机MAC   正常情况下写假 	变态测试时最好写真
        //errInfo//此 API 执行返回后，如果出错，保存了错误信息说明。一般要分配 256 字节的空间。没出错时为空字符串
        [DllImport(@dllPath, EntryPoint = "Logon", CallingConvention = CallingConvention.Winapi)]
        public extern static int Logon(int qsid, string host, short port, string version, short accountType,
            string account, string password, string commPassword, bool dommac, byte[] errInfo);
        //功能：查询各种交易数据
        //clientID,//客户端 ID
        //category,//查询信息的种类 0资金	1股份 2当日委托	3当日成交 4可撤单 5股东账户 
        //result, //内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n 字符分割，列数据之间通过\t 分隔。一般要分配 1024 * 1024 字节的空间。出错时为空字符串。
        //errInfo//此 API 执行返回后，如果出错，保存了错误信息说明。一般要分配 256 字节的空间。没出错时为空字符串。
        [DllImport(@dllPath, EntryPoint = "QueryData", CallingConvention = CallingConvention.Winapi)]
        public extern static int QueryData(int clientID, int category, byte[] result, byte[] errInfo);
        //功能：查行情 券商提供的行情虽然只有5档 但是速度快
        //clientID,//客户端ID
        //zqdm,//股票代码
        //result,//内保存了返回的查询数据
        //errInfo//执行返回后，如果出错，保存了错误信息说明
        [DllImport(@dllPath, EntryPoint = "QueryHQ", CallingConvention = CallingConvention.Winapi)]
        public extern static int QueryHQ(int clientID, string gddm, byte[] result, byte[] errInfo);
        //功能：委托下单
        //clientID,//客户端ID
        //category,//委托种类 0买入 1卖出
        //gddm,//股东账号；交易上海股票填上海的股东账号，交易深圳的股票填入深圳的股东账号。 正常情况下留空（留空自动判断）
        //zqdm,//证券代码
        //price,//委托价格
        //quantity,//委托数量
        //result,//内保存了返回的查询数据, 含有委托编号数据 出错时为空字符串。
        //errInfo//如果出错，保存了错误信息说明。一般要分配 256 字节的空间
        [DllImport(@dllPath, EntryPoint = "SendOrder", CallingConvention = CallingConvention.Winapi)]
        public extern static int SendOrder(int clientID, int category, string gddm, string zqdm,
            float price, int quantity, byte[] result, byte[] errInfo);
        //功能：取消订单
        //clientID,//客户端ID
        //gddm,//股东账号 必写
        //OrderID,//表示要撤的目标委托的编号
        //result,//内保存了返回的查询数据
        //errInfo//执行返回后，如果出错，保存了错误信息说明
        [DllImport(@dllPath, EntryPoint = "CancelOrder", CallingConvention = CallingConvention.Winapi)]
        public extern static int CancelOrder(int clientID, string gddm, string orderID, byte[] result, byte[] errInfo);
        //功能：退出登录	无返回值
        //clientID,//客户端ID
        [DllImport(@dllPath, EntryPoint = "Logoff", CallingConvention = CallingConvention.Winapi)]
        public extern static void Logoff(int clientID);

        /// <summary>
        /// 查询资金
        /// </summary>
        /// <param name="clientID">客户端id</param>
        /// <returns></returns>
        public static Funds QueryFunds(int clientID)
        {
            Funds funds = new Funds();
            int code = QueryData(clientID, 0, funds.Result, funds.ErrorInfo);
            if (code > 0)
            {
                String[] temp = ApiHelper.ParseResult(funds.Result);
                funds.TotalAsset = Double.Parse(temp[0]);
                funds.FundBalance = Double.Parse(temp[1]);
                funds.FrozenAmt = Double.Parse(temp[2]);
                funds.AvailableAmt = Double.Parse(temp[4]);
            }
            return funds;
        }

        /// <summary>
        ///  查询券商自带行情
        /// </summary>
        /// <param name="clientID">客户端id</param>
        /// <param name="code">股票代码</param>
        /// <returns></returns>
        public static Quotes QueryQuotes(int clientID, String code)
        {
            Quotes quotes = new Quotes();
            int rspCode = QueryHQ(clientID, code, quotes.Result, quotes.ErrorInfo);
            if (rspCode > 0)
            {
                String[] temp = ApiHelper.ParseResult(quotes.Result);
                quotes.Code = temp[0];
                quotes.Name = temp[1];
                quotes.PreClose = float.Parse(temp[2]);
                quotes.LatestPrice = float.Parse(temp[3]);
                quotes.HighLimit = float.Parse(temp[4]);
                quotes.LowLimit = float.Parse(temp[5]);
                quotes.High = float.Parse(temp[6]);
                quotes.Low = float.Parse(temp[7]);
                quotes.Money = double.Parse(temp[8]);
                quotes.Volume = int.Parse(temp[9]);
                quotes.Open = float.Parse(temp[10]);
                quotes.Sell1 = float.Parse(temp[11]);
                quotes.Sell2 = float.Parse(temp[12]);
                quotes.Sell3 = float.Parse(temp[13]);
                quotes.Sell4 = float.Parse(temp[14]);
                quotes.Sell5 = float.Parse(temp[15]);
                quotes.Buy1 = float.Parse(temp[16]);
                quotes.Buy2 = float.Parse(temp[17]);
                quotes.Buy3 = float.Parse(temp[18]);
                quotes.Buy4 = float.Parse(temp[19]);
                quotes.Buy5 = float.Parse(temp[20]);
                quotes.Sell1Vol = int.Parse(temp[21].Replace(".000", ""));
                quotes.Sell2Vol = int.Parse(temp[22].Replace(".000", ""));
                quotes.Sell3Vol = int.Parse(temp[23].Replace(".000", ""));
                quotes.Sell4Vol = int.Parse(temp[24].Replace(".000", ""));
                quotes.Sell5Vol = int.Parse(temp[25].Replace(".000", ""));
                quotes.Buy1Vol = int.Parse(temp[26].Replace(".000", ""));
                quotes.Buy2Vol = int.Parse(temp[27].Replace(".000", ""));
                quotes.Buy3Vol = int.Parse(temp[28].Replace(".000", ""));
                quotes.Buy4Vol = int.Parse(temp[29].Replace(".000", ""));
                quotes.Buy5Vol = int.Parse(temp[30].Replace(".000", ""));
            }
            return quotes;
        }

        /// <summary>
        /// 委托买入
        /// </summary>
        /// <param name="order">委托单对象</param>
        /// <returns>响应码</returns>
        public static int Buy(Order order)
        {
            int rspId = SendOrder(order.ClientId, Order.CategoryBuy, order.Code, order.ShareholderAcct,
                order.Price, order.Quantity, order.Result, order.ErrorInfo);
            return rspId;
        }

        /// <summary>
        /// 委托卖出
        /// </summary>
        /// <param name="order">委托单对象</param>
        /// <returns>响应码</returns>
        public static int Sell(Order order)
        {
            int rspId = SendOrder(order.ClientId, Order.CategorySell, order.Code, order.ShareholderAcct,
                order.Price, order.Quantity, order.Result, order.ErrorInfo);
            return rspId;
        }

        /// <summary>
        /// 撤销委托
        /// </summary>
        /// <param name="order">委托对象</param>
        /// <returns>响应码</returns>
        public static int CancelOrder(Order order)
        {
            return CancelOrder(order.ClientId, order.Code, order.OrderId, order.Result, order.ErrorInfo);
        }
    }
}
