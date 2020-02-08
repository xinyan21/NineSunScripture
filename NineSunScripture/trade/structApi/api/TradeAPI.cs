using NineSunScripture.model;
using NineSunScripture.trade.structApi.helper;
using NineSunScripture.util;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using static NineSunScripture.trade.structApi.ApiDataStruct;

namespace NineSunScripture.trade.structApi.api
{
    /// <summary>
    /// 交易API，附带五档行情API
    /// </summary>
    public class TradeAPI
    {
        //dll必须放在程序同一目录下面，否则调用会报错
        private const string dllPath = "StructApi.dll";

        //功能：登录 成功返回sessionId	失败返回0
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
        [DllImport(@"StructApi.dll", EntryPoint = "Logon", CallingConvention = CallingConvention.Winapi)]
        public extern static int Logon(int Qsid, string Host, short Port, string Version, string YybId,
            short AccountType, string Account, string Password, string comm_password,
            bool dommac, IntPtr ErrInfo);

        //功能：查询各种交易数据
        //sessionId,//客户端 ID
        //category,//查询信息的种类 0资金	1股份 2最新委托	3最新成交 4可撤单 5股东账户
        //result, //内保存了返回的查询数据, 形式为表格数据，行数据之间通过\n 字符分割，列数据之间通过\t 分隔。一般要分配 1024 * 1024 字节的空间。出错时为空字符串。
        //errInfo//此 API 执行返回后，如果出错，保存了错误信息说明。一般要分配 256 字节的空间。没出错时为空字符串。
        [DllImport(@"StructApi.dll", EntryPoint = "QueryData", CallingConvention = CallingConvention.Winapi)]
        public extern static int QueryData(int ClientID, int Category, IntPtr Result, IntPtr ErrInfo);

        //功能：查行情 券商提供的行情虽然只有5档 但是速度快
        //sessionId,//客户端ID
        //zqdm,//股票代码
        //result,//内保存了返回的查询数据
        //errInfo//执行返回后，如果出错，保存了错误信息说明
        [DllImport(@"StructApi.dll", EntryPoint = "QueryHQ", CallingConvention = CallingConvention.Winapi)]
        public extern static int QueryHQ(int ClientID, string Gddm, IntPtr Result, IntPtr ErrInfo);

        //功能：委托下单
        //sessionId,//客户端ID
        //category,//委托种类 0买入 1卖出
        //gddm,//股东账号；交易上海股票填上海的股东账号，交易深圳的股票填入深圳的股东账号。 正常情况下留空（留空自动判断）
        //zqdm,//证券代码
        //price,//委托价格
        //quantity,//委托数量
        //result,//内保存了返回的查询数据, 含有委托编号数据 出错时为空字符串。
        //errInfo//如果出错，保存了错误信息说明。一般要分配 256 字节的空间
        [DllImport(@"StructApi.dll", EntryPoint = "SendOrder", CallingConvention = CallingConvention.Winapi)]
        public extern static int SendOrder(int sessionId, int category, string gddm, string zqdm,
            float price, int quantity, IntPtr result, IntPtr errInfo);

        //功能：取消订单
        //sessionId,//客户端ID
        //gddm,//股东账号 必写
        //OrderID,//表示要撤的目标委托的编号
        //result,//内保存了返回的查询数据
        //errInfo//执行返回后，如果出错，保存了错误信息说明
        [DllImport(@"StructApi.dll", EntryPoint = "CancelOrder", CallingConvention = CallingConvention.Winapi)]
        public extern static int CancelOrder(int sessionId, string gddm, string orderID, IntPtr result, IntPtr errInfo);

        //功能：退出登录	无返回值
        //sessionId,//客户端ID
        [DllImport(@"StructApi.dll", EntryPoint = "Logoff", CallingConvention = CallingConvention.Winapi)]
        public extern static void Logoff(int sessionId);

        private static ReaderWriterLockSlim rwls = new ReaderWriterLockSlim();

        /// <summary>
        /// 查询资金
        /// </summary>
        /// <param name="sessionId">会话id</param>
        /// <returns></returns>
        public static Funds QueryFunds(int sessionId)
        {
            Funds funds = new Funds();
            funds.AllocCoTaskMem();
            try
            {
                int code = QueryData(sessionId, 0, funds.PtrResult, funds.PtrErrorInfo);
                ApiHelper.HandleTimeOut(funds.PtrErrorInfo);
                if (code > 0)
                {
                    资产结构体 fund
                        = (资产结构体)Marshal.PtrToStructure(funds.PtrResult + 32, typeof(资产结构体));
                    funds.TotalAsset = fund.总资产;
                    funds.FundBalance = fund.资金余额;
                    funds.FrozenAmt = fund.冻结金额;
                    funds.AvailableAmt = fund.可用金额;
                }
                else
                {
                    Logger.Log("QueryFunds：" + ApiHelper.ParseErrInfo(funds.PtrErrorInfo));
                }
            }
            finally
            {
                funds.FreeCoTaskMem();
            }
            return funds;
        }

        /// <summary>
        /// 查询单个账户持仓
        /// </summary>
        /// <param name="sessionId">登录返回的客户Id</param>
        /// <returns></returns>
        public static List<Position> QueryPositions(int sessionId)
        {
            List<Position> positions = new List<Position>();
            Position position;
            rwls.EnterWriteLock();
            IntPtr result = Marshal.AllocCoTaskMem(1024 * 1024);
            IntPtr errorInfo = Marshal.AllocCoTaskMem(256);
            rwls.ExitWriteLock();
            try
            {
                int code = QueryData(sessionId, 1, result, errorInfo);
                ApiHelper.HandleTimeOut(errorInfo);
                if (code > 0)
                {
                    int listLength = Marshal.ReadInt32(result + 4);
                    int structLength = Marshal.ReadInt32(result + 12);
                    IntPtr dataPtr = result + 32;

                    for (int i = 0; i < listLength; i++)
                    {
                        持仓股票结构体 temp
                        = (持仓股票结构体)Marshal.PtrToStructure(dataPtr, typeof(持仓股票结构体));
                        position = new Position();
                        position.Code = temp.证券代码;
                        position.Name = temp.证券名称;
                        position.StockBalance = temp.股票余额;
                        position.AvailableBalance = temp.可用余额;
                        position.FrozenQuantity = temp.冻结数量;
                        position.ProfitAndLoss = (float)temp.参考盈亏;
                        position.AvgCost = (float)temp.成本价;
                        position.ProfitAndLossPct = (float)temp.盈亏比例;
                        position.Price = (float)temp.市价;
                        position.MarketValue = (float)temp.市值;


                        positions.Add(position);
                        dataPtr += structLength;
                    }
                }
                else
                {
                    Logger.Log("QueryPositions：" + ApiHelper.ParseErrInfo(errorInfo));
                }
            }
            finally
            {
                rwls.EnterWriteLock();
                Marshal.FreeCoTaskMem(result);
                Marshal.FreeCoTaskMem(errorInfo);
                rwls.ExitWriteLock();
            }
            return positions;
        }

        /// <summary>
        /// 查询最新成交
        /// </summary>
        /// <param name="sessionId">登录返回的客户Id</param>
        /// <returns></returns>
        public static List<Order> QueryTodayTransaction(int sessionId)
        {
            List<Order> orders = new List<Order>();
            Order order;
            Order orderResult = new Order();
            orderResult.AllocCoTaskMem();
            try
            {
                int code = QueryData(sessionId, 3, orderResult.PtrResult, orderResult.PtrErrorInfo);
                ApiHelper.HandleTimeOut(orderResult.PtrErrorInfo);
                if (code > 0)
                {
                    int listLength = Marshal.ReadInt32(orderResult.PtrResult + 4);
                    int structLength = Marshal.ReadInt32(orderResult.PtrResult + 12);
                    IntPtr dataPtr = orderResult.PtrResult + 32;

                    for (int i = 0; i < listLength; i++)
                    {
                        当日成交结构体 temp
                            = (当日成交结构体)Marshal.PtrToStructure(dataPtr, typeof(当日成交结构体));
                        order = new Order();
                        order.Time = temp.成交时间;
                        order.Code = temp.证券代码;
                        order.Name = temp.证券名称;
                        order.Operation = temp.操作;
                        order.Quantity = temp.成交数量;
                        order.OrderId = temp.合同编号;
                        orders.Add(order);

                        dataPtr += structLength;
                    }
                }
            }
            finally
            {
                orderResult.FreeCoTaskMem();
            }

            return orders;
        }

        /// <summary>
        /// 查询可撤销委托
        /// </summary>
        /// <param name="sessionId">登录返回的客户Id</param>
        /// <returns></returns>
        public static List<Order> QueryOrdersCanCancel(int sessionId)
        {
            List<Order> orders = new List<Order>();
            Order order = null;
            Order orderResult = new Order();
            orderResult.AllocCoTaskMem();
            try
            {
                int code = QueryData(sessionId, 4, orderResult.PtrResult, orderResult.PtrErrorInfo);
                ApiHelper.HandleTimeOut(orderResult.PtrErrorInfo);
                if (code > 0)
                {
                    int listLength = Marshal.ReadInt32(orderResult.PtrResult + 4);
                    int structLength = Marshal.ReadInt32(orderResult.PtrResult + 12);
                    IntPtr dataPtr = orderResult.PtrResult + 32;

                    for (int i = 0; i < listLength; i++)
                    {
                        当日委托结构体 temp
                            = (当日委托结构体)Marshal.PtrToStructure(dataPtr, typeof(当日委托结构体));
                        order = new Order();
                        order.Time = temp.委托时间;
                        order.Code = temp.证券代码;
                        order.Name = temp.证券名称;
                        order.Operation = temp.操作;
                        order.Quantity = temp.委托数量;
                        order.OrderId = temp.合同编号;
                        order.TransactionQuantity = temp.成交数量;
                        order.CanceledQuantity = temp.撤单数量;
                        order.Price = (float)temp.委托价格;
                        order.TransactionPrice = (float)temp.成交均价;
                        order.ShareholderAcct = temp.股东账户;
                        orders.Add(order);

                        dataPtr += structLength;
                    }
                }
            }
            finally
            {
                orderResult.FreeCoTaskMem();
            }
            return orders;
        }

        /// <summary>
        /// 查询股东账号
        /// </summary>
        /// <param name="sessionId">登录返回的客户Id</param>
        /// <returns></returns>
        public static List<ShareHolderAcct> QueryShareHolderAccts(int sessionId)
        {
            List<ShareHolderAcct> accounts = new List<ShareHolderAcct>();
            ShareHolderAcct account = new ShareHolderAcct();
            account.AllocCoTaskMem();
            try
            {
                int code = QueryData(sessionId, 5, account.PtrResult, account.PtrErrorInfo);
                ApiHelper.HandleTimeOut(account.PtrErrorInfo);
                if (code > 0)
                {
                    int listLength = Marshal.ReadInt32(account.PtrResult + 4);
                    int structLength = Marshal.ReadInt32(account.PtrResult + 12);
                    IntPtr dataPtr = account.PtrResult + 32;

                    for (int i = 0; i < listLength; i++)
                    {
                        股东账户结构体 temp
                            = (股东账户结构体)Marshal.PtrToStructure(dataPtr, typeof(股东账户结构体));
                        account = new ShareHolderAcct();
                        account.code = temp.股东代码;
                        account.category = temp.帐号类别;
                        account.name = temp.股东姓名;
                        accounts.Add(account);

                        dataPtr += structLength;
                    }
                }
                else
                {
                    Logger.Log("QueryShareHolderAccts：" + ApiHelper.ParseErrInfo(account.PtrErrorInfo));
                }
            }
            finally
            {
                account.FreeCoTaskMem();
            }
            return accounts;
        }

        /// <summary>
        ///  查询券商交易自带行情（交易行情数据问题很多，这个方法一天要崩好多次）
        /// </summary>
        /// <param name="tradeSessionId">交易会话id</param>
        /// <param name="code">股票代码</param>
        /// <returns></returns>
        public static Quotes QueryQuotes(int tradeSessionId, string code)
        {
            Quotes quotes = new Quotes();
            quotes.AllocCoTaskMem();
            try
            {
                int rspCode = QueryHQ(tradeSessionId, code, quotes.PtrResult, quotes.PtrErrorInfo);
                ApiHelper.HandleTimeOut(quotes.PtrErrorInfo);
                if (rspCode > 0)
                {
                    证券行情结构体 price =
                  (证券行情结构体)Marshal.PtrToStructure(quotes.PtrResult + 32, typeof(证券行情结构体));
                    quotes.Code = price.证券代码;
                    quotes.Name = price.证券名称;
                    quotes.Open = (float)price.开盘价;
                    quotes.LatestPrice = (float)price.最新价;
                    quotes.Volume = price.成交量;
                    quotes.Money = price.成交额;
                    quotes.HighLimit = (float)price.涨停价;
                    quotes.LowLimit = (float)price.跌停价;
                    quotes.High = (float)price.最高价;
                    quotes.Low = (float)price.最低价;
                    quotes.Sell1 = (float)price.卖一价;
                    quotes.Sell2 = (float)price.卖二价;
                    quotes.Sell3 = (float)price.卖三价;
                    quotes.Buy1 = (float)price.买一价;
                    quotes.Buy2 = (float)price.买二价;
                    quotes.Buy3 = (float)price.买三价;
                    quotes.Sell1Vol = price.卖一量;
                    quotes.Sell2Vol = price.卖二量;
                    quotes.Sell3Vol = price.卖三量;
                    quotes.Buy1Vol = price.买一量;
                    quotes.Buy2Vol = price.买二量;
                    quotes.Buy3Vol = price.买三量;
                    quotes.PreClose = (float)price.昨日价;
                }
                else
                {
                    Logger.Log("QueryQuotes：" + ApiHelper.ParseErrInfo(quotes.PtrErrorInfo));
                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.Log("QueryQuotes解析异常：" + ApiHelper.ParseErrInfo(quotes.PtrResult));
                Logger.Exception(e, ApiHelper.ParseErrInfo(quotes.PtrResult));
                throw;
            }
            finally
            {
                quotes.FreeCoTaskMem();
            }
            Utils.SamplingLogQuotes(quotes);
            return quotes;
        }

        /// <summary>
        /// 委托买入
        /// </summary>
        /// <param name="order">委托单对象</param>
        /// <returns>响应码</returns>
        public static int Buy(Order order)
        {
            if (null == order)
            {
                return -1;
            }
            order.AllocCoTaskMem();
            int rspId;
            try
            {
                rspId = SendOrder(order.TradeSessionId, Order.CategoryBuy, order.ShareholderAcct,
                   order.Code, order.Price, order.Quantity, order.PtrResult, order.PtrErrorInfo);
                ApiHelper.HandleTimeOut(order.PtrErrorInfo);
                order.StrErrorInfo = ApiHelper.ParseErrInfo(order.PtrErrorInfo);
            }
            finally
            {
                order.FreeCoTaskMem();
            }
            return rspId;
        }

        /// <summary>
        /// 委托卖出
        /// </summary>
        /// <param name="order">委托单对象</param>
        /// <returns>响应码</returns>
        public static int Sell(Order order)
        {
            if (null == order)
            {
                return -1;
            }
            order.AllocCoTaskMem();
            int rspId;
            try
            {
                rspId = SendOrder(order.TradeSessionId, Order.CategorySell, order.ShareholderAcct,
                  order.Code, order.Price, order.Quantity, order.PtrResult, order.PtrErrorInfo);
                ApiHelper.HandleTimeOut(order.PtrErrorInfo);
                order.StrErrorInfo = ApiHelper.ParseErrInfo(order.PtrErrorInfo);
            }
            finally
            {
                order.FreeCoTaskMem();
            }
            return rspId;
        }

        /// <summary>
        /// 撤销委托
        /// </summary>
        /// <param name="order">委托对象</param>
        /// <returns>响应码</returns>
        public static int CancelOrder(Order order)
        {
            if (null == order)
            {
                return -1;
            }
            order.AllocCoTaskMem();
            int rspId;
            try
            {
                rspId = CancelOrder(order.TradeSessionId, order.ShareholderAcct,
                  order.OrderId, order.PtrResult, order.PtrErrorInfo);
                ApiHelper.HandleTimeOut(order.PtrErrorInfo);
                order.StrErrorInfo = ApiHelper.ParseErrInfo(order.PtrErrorInfo);
            }
            finally
            {
                order.FreeCoTaskMem();
            }
            return rspId;
        }
    }
}