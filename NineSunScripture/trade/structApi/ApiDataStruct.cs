using System.Runtime.InteropServices;

namespace NineSunScripture.trade.structApi
{
    internal class ApiDataStruct
    {
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
        public struct N档委托结构体
        {
            public double 委托价;
            public double 委托量;
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
    }
}