using NineSunScripture.model;
using NineSunScripture.trade.structApi.helper;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace C_sharp演示
{
    public class TestApi
    {
        [DllImport(@"StructApi.dll", EntryPoint = "Logon", CallingConvention = CallingConvention.Winapi)]
        public extern static int Logon(int Qsid, string Host, short Port, string Version, string YybId, short AccountType, string Account, string Password, string comm_password, bool dommac, IntPtr ErrInfo);
        [DllImport(@"StructApi.dll", EntryPoint = "QueryData", CallingConvention = CallingConvention.Winapi)]
        public extern static int QueryData(int ClientID, int Category, IntPtr Result, IntPtr ErrInfo);
        [DllImport(@"StructApi.dll", EntryPoint = "QueryHQ", CallingConvention = CallingConvention.Winapi)]
        public extern static int QueryHQ(int ClientID, string Gddm, IntPtr Result, IntPtr ErrInfo);
        [DllImport(@"StructApi.dll", EntryPoint = "QueryHistoryData", CallingConvention = CallingConvention.Winapi)]
        public extern static int QueryHistoryData(int ClientID, int Category, string BeginDate, string EndDate, IntPtr Result, IntPtr ErrInfo);
        [DllImport(@"StructApi.dll", EntryPoint = "HQ_Logon", CallingConvention = CallingConvention.Winapi)]
        public extern static int HQ_Logon(string acct, string pwd, IntPtr ErrInfo);
        [DllImport(@"StructApi.dll", EntryPoint = "HQ_QueryData", CallingConvention = CallingConvention.Winapi)]
        public extern static int HQ_QueryData(int ClientID, int Category, string Gddm, string name, IntPtr Result, IntPtr ErrInfo);
        public delegate void THS推送回调(int type, IntPtr Ptr);
        [DllImport(@"StructApi.dll", EntryPoint = "HQ_PushData", CallingConvention = CallingConvention.Winapi)]
        public extern static int HQ_PushData(int ClientID, int Category, string Gddm, string name, THS推送回调 Callback, bool state);
        private static THS推送回调 Callback;

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
                    Logger.Log(funds.ToString());
                }
                else
                {
                    //NO DATA
                    Logger.Log("QueryFunds：" + ApiHelper.ParseErrInfo(funds.PtrErrorInfo));
                }
            }
            finally
            {
                funds.FreeCoTaskMem();
            }
            return funds;
        }
        public static List<Position> QueryPositions(int sessionId)
        {
            List<Position> positions = new List<Position>();
            Position position;
            Position positionResult = new Position();
            positionResult.AllocCoTaskMem();
            try
            {
                int code = QueryData(sessionId, 1, positionResult.PtrResult, positionResult.PtrErrorInfo);
                ApiHelper.HandleTimeOut(positionResult.PtrErrorInfo);
                if (code > 0)
                {
                    int positionLength = Marshal.ReadInt32(positionResult.PtrResult + 4);
                    int structLength = Marshal.ReadInt32(positionResult.PtrResult + 12);
                    IntPtr dataPtr = positionResult.PtrResult + 32;

                    for (int i = 0; i < positionLength; i++)
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
            }
            finally
            {
                positionResult.FreeCoTaskMem();
            }
            return positions;
        }
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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 资产结构体
        {
            public double 总资产;
            public double 资金余额;
            public double 冻结金额;
            public double 可取金额;
            public double 可用金额;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 持仓股票结构体
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券代码;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券名称;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 交易市场;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 股东账户;
            public double 参考盈亏;
            public double 盈亏比例;
            public double 成本价;
            public double 市价;
            public double 市值;
            public int 股票余额;
            public int 可用余额;
            public int 冻结数量;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 当日成交结构体
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 成交时间;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券代码;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券名称;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 合同编号;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 操作;
            public double 成交均价;
            public double 成交金额;
            public int 成交数量;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 当日委托结构体
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 委托时间;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券代码;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券名称;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 合同编号;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 交易市场;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 股东账户;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 操作;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 备注;
            public double 委托价格;
            public double 成交均价;
            public int 委托数量;
            public int 成交数量;
            public int 撤单数量;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 可申购新股结构体
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 新股代码;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 新股名称;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 申购日期;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 交易市场;
            public double 申购价格;
            public int 申购上限;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 新股配售额度结构体
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 资金账户;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 股东账户;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 股东类型;
            public int 可用余额;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 配号结构体
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 成交日期;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 交易市场;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券代码;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券名称;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 起始配号;
            public int 成交笔数;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 中签结构体
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 成交日期;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券代码;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券名称;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 交易市场;
            public double 成交均价;
            public int 发生数量;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 证券行情结构体
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券代码;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券名称;
            public double 开盘价;
            public double 昨日价;
            public double 最新价;
            public double 涨停价;
            public double 跌停价;
            public double 最高价;
            public double 最低价;
            public double 成交额;
            public double 卖一价;
            public double 卖二价;
            public double 卖三价;
            public double 卖四价;
            public double 卖五价;
            public double 买一价;
            public double 买二价;
            public double 买三价;
            public double 买四价;
            public double 买五价;
            public int 成交量;
            public int 卖一量;
            public int 卖二量;
            public int 卖三量;
            public int 卖四量;
            public int 卖五量;
            public int 买一量;
            public int 买二量;
            public int 买三量;
            public int 买四量;
            public int 买五量;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 历史委托结构体
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 委托日期;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 委托时间;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券代码;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券名称;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 合同编号;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 交易市场;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 股东账户;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 操作;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 备注;
            public double 委托价格;
            public double 成交均价;
            public int 委托数量;
            public int 成交数量;
            public int 撤单数量;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 历史成交结构体
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 成交日期;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 成交时间;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券代码;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券名称;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 合同编号;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 成交编号;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 交易市场;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 股东账户;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 操作;
            public double 成交均价;
            public double 成交金额;
            public double 发生金额;
            public double 其他杂费;
            public double 手续费;
            public double 印花税;
            public double 佣金;
            public int 股票余额;
            public int 成交数量;
            public int 撤单数量;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 资金流水结构体
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 成交日期;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券代码;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券名称;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 交易市场;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 股东账户;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 货币单位;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 操作;
            public double 成交均价;
            public double 成交金额;
            public double 发生金额;
            public double 本次金额;
            public int 成交数量;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 交割单结构体
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 成交日期;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 成交时间;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券代码;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 证券名称;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 成交编号;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 合同编号;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 股东账户;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 交易市场;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 操作;
            public double 成交均价;
            public double 成交金额;
            public double 发生金额;
            public double 本次金额;
            public double 其他杂费;
            public double 印花税;
            public double 过户费;
            public double 佣金;
            public int 成交数量;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 股东账户结构体
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 股东代码;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 帐号类别;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
            public string 股东姓名;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 十档行情结构体
        {
            public double 昨收;
            public double 开盘;
            public double 最新;
            public double 总量;
            public double 换手;
            public double 涨停;
            public double 跌停;
            public double 总市值;
            public double 流通值;
            public double 买一价;
            public double 买二价;
            public double 买三价;
            public double 买四价;
            public double 买五价;
            public double 买六价;
            public double 买七价;
            public double 买八价;
            public double 买九价;
            public double 买十价;
            public double 买一量;
            public double 买二量;
            public double 买三量;
            public double 买四量;
            public double 买五量;
            public double 买六量;
            public double 买七量;
            public double 买八量;
            public double 买九量;
            public double 买十量;
            public double 卖一价;
            public double 卖二价;
            public double 卖三价;
            public double 卖四价;
            public double 卖五价;
            public double 卖六价;
            public double 卖七价;
            public double 卖八价;
            public double 卖九价;
            public double 卖十价;
            public double 卖一量;
            public double 卖二量;
            public double 卖三量;
            public double 卖四量;
            public double 卖五量;
            public double 卖六量;
            public double 卖七量;
            public double 卖八量;
            public double 卖九量;
            public double 卖十量;
            public double 主动买入特大单量;
            public double 主动卖出特大单量;
            public double 主动买入大单量;
            public double 主动卖出大单量;
            public double 主动买入中单量;
            public double 主动卖出中单量;
            public double 主动买入小单量;
            public double 主动卖出小单量;
            public double 被动买入特大单量;
            public double 被动卖出特大单量;
            public double 被动买入大单量;
            public double 被动卖出大单量;
            public double 被动买入中单量;
            public double 被动卖出中单量;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 成交明细结构体
        {
            public int 时间;
            public int 分类;
            public double 价格;
            public double 总量;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct 逐笔委托结构体
        {
            public int 时间;//时间戳
            public int 类型;//1买 2卖
            public double 委托价;
            public double 委托量;
        };
        /*
         * C#中IntPtr的使用
         在C#中IntPtr类型相当于C或C++中的指针,只不过在C#对指针做了封装.这个类型不属于托管类型,所以在使用前需要用
         Marshal.AllocCoTaskMem(int Bytes)来分配内存,这里的Bytes是你要用的指针的字节数.在使用后需要用
         Marshal.FreeCoTaskMem(IntPtr ptr)来释放内存空间,否则也会像C或C++那样出现内存泄露.
         */
        public static void Test()
        {
            //下面申请的两个内存指针不会被C#回收   如线程中使用一定要释放
            IntPtr ErrInfo = Marshal.AllocCoTaskMem(256);
            IntPtr Result = Marshal.AllocCoTaskMem(1024 * 1024);
            //交易接口测试==================================================================================================================
            int Qsid = 0;
            string Host = "trade.10jqka.com.cn";
            short Port = 8002;
            string Version = "E065.18.77";
            string YybId = "";
            short AccountType = 0x30;
            string Account = "24509958";
            string password = "3594035x";
            string comm_password = "";
            int ID = Logon(Qsid, Host, Port, Version, YybId, AccountType, Account, password, comm_password, false, ErrInfo);

            if (ID > 0)
            {
                Logger.Log("\n登录成功 ID:" + ID);
                int b = 0;
                Logger.Log("资产测试========================;");
                for (int x = 0; x < 20; x++)
                {
                    Thread.Sleep(1000);
                    b = QueryData(ID, 0, Result, ErrInfo);
                    if (b > 0)
                    {
                        资产结构体 资产 = (资产结构体)Marshal.PtrToStructure(Result + 32, typeof(资产结构体));
                        Logger.Log("资产.总资产=" + 资产.总资产);
                    }
                    else
                    {
                        Logger.Log(Marshal.PtrToStringAnsi(ErrInfo));
                    }
                    Logger.Log("n持仓股票测试========================;");
                    b = QueryData(ID, 1, Result, ErrInfo);
                    if (b > 0)
                    {
                        // int id = Marshal.ReadInt32(Result);
                        int 结构体数组成员数 = Marshal.ReadInt32(Result + 4);
                        // int 结构体成员数 = Marshal.ReadInt32(Result+ 8);
                        int 结构体字节数 = Marshal.ReadInt32(Result + 12);

                        IntPtr 数据指针 = Result + 32;

                        for (int i = 0; i < 结构体数组成员数; i++)
                        {
                            持仓股票结构体 持仓股票 = (持仓股票结构体)Marshal.PtrToStructure(数据指针, typeof(持仓股票结构体));

                            Logger.Log("n持仓股票.证券名称================;" + 持仓股票.证券名称);
                            数据指针 += 结构体字节数;
                        }
                    }
                    else
                    {
                        Logger.Log(Marshal.PtrToStringAnsi(ErrInfo));
                    }
                }
            }
            else
            {
                Logger.Log(Marshal.PtrToStringAnsi(ErrInfo));
            }

        }
        private static void 回调函数(int type, IntPtr Result)//Result 运行完此函数将自动释放   
        {
            // Console.WriteLine(type);
            if (type == 10001)//推送过来是数据是十档行情 推送中的十档无换手、总市值、流通值、涨停、跌停
            {
                Console.WriteLine("\n接收到十档行情推送数据=====================================================");
                // int id = Marshal.ReadInt32(Result);
                // int 结构体数组成员数 = Marshal.ReadInt32(Result + 4);
                // int 结构体成员数 = Marshal.ReadInt32(Result + 8);
                //int 结构体字节数 = Marshal.ReadInt32(Result + 12);
                string 股票代码 = Marshal.PtrToStringAnsi(Result + 16, 6);
                Console.WriteLine(股票代码);
                十档行情结构体 十档行情 = (十档行情结构体)Marshal.PtrToStructure(Result + 32, typeof(十档行情结构体));

                Console.WriteLine(十档行情.卖十价);
                Console.WriteLine(十档行情.卖十量);
            }
            else if (type == 10206)//逐笔委托 推送太快注意代码优化
            {
                Console.WriteLine("\n逐笔委托推送数据=====================================================");
                // int id = Marshal.ReadInt32(Result);
                // int 结构体数组成员数 = Marshal.ReadInt32(Result+4);
                // int 结构体成员数 = Marshal.ReadInt32(Result+ 8);
                //int 结构体字节数 = Marshal.ReadInt32(Result + 12);
                string 股票代码 = Marshal.PtrToStringAnsi(Result + 16, 6);
                Console.WriteLine(股票代码);
                逐笔委托结构体 逐笔委托 = (逐笔委托结构体)Marshal.PtrToStructure(Result + 32, typeof(逐笔委托结构体));
                Console.WriteLine(逐笔委托.时间);
                Console.WriteLine(逐笔委托.类型);
                Console.WriteLine(逐笔委托.委托价);
                Console.WriteLine(逐笔委托.委托量);

            }
        }
    }
}

