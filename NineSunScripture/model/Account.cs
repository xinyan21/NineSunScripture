using System;
using System.Collections.Generic;
using System.Text;

namespace NineSunScripture.model
{
    /// <summary>
    /// 账户类
    /// </summary>
    public class Account : BaseModel
    {
        private short acctType;

        //Qsid,//券商id 					这个值可以在IP文件中查看
        //Host,//券商服务器IP
        //Port,//券商服务器端口
        //Version,//同花顺客户端版本号		目前通用版本号E065.20.92	目前发现，海通需要使用老版本号E065.18.77
        //AccountType,//账户类型 0x20自动判断(有些券商可能不支持) 0x30资金账户 0x31深圳账户
        //0x32上海账户 0x33基金账户 0x34深圳B股 0x35上海B股 0x6B客户号
        //Account,//券商账号“账号类型需要与上面的对应”
        //Password,//券商密码
        //comm_password,//通讯密码	可空
        //dommac 是否随机MAC 假=取本机MAC  真=每次登录都随机MAC   正常情况下写假 	变态测试时最好写真
        private int brokerId;

        private string brokerName;
        private string brokerServerIP;
        private short brokerServerPort;
        /// <summary>
        /// 撤单
        /// </summary>
        private List<Order> cancelOrders;

        private string commPwd;
        private string fundAcct;
        private string fundPassword;
        private Funds funds;
        private int initTotalAsset;
        private bool isRandomMac = true;
        private List<Position> positions;
        //资金账号
        private string priceAcct;

        //行情账号
        private string pricePassword;

        /// <summary>
        /// 行情会话id
        /// </summary>
        private int priceSessionId;

        private string salesDepartId;
        //初始总资金，作为单股开仓仓位依据
        private List<ShareHolderAcct> shareHolderAccts;

        private string shShareholderAcct;
        //上海股东账号
        private string szShareholderAcct;

        private string versionOfTHS;

        public short AcctType { get => acctType; set => acctType = value; }

        //深圳股东账户
        public int BrokerId { get => brokerId; set => brokerId = value; }
        public string BrokerName { get => brokerName; set => brokerName = value; }
        public string BrokerServerIP { get => brokerServerIP; set => brokerServerIP = value; }
        public short BrokerServerPort { get => brokerServerPort; set => brokerServerPort = value; }
        public string CommPwd { get => commPwd; set => commPwd = value; }
        public string FundAcct { get => fundAcct; set => fundAcct = value; }
        public string FundPassword { get => fundPassword; set => fundPassword = value; }
        public int InitTotalAsset { get => initTotalAsset; set => initTotalAsset = value; }
        public bool IsRandomMac { get => isRandomMac; set => isRandomMac = value; }
        public string PriceAcct { get => priceAcct; set => priceAcct = value; }
        public string PricePassword { get => pricePassword; set => pricePassword = value; }
        public int PriceSessionId { get => priceSessionId; set => priceSessionId = value; }
        public string SalesDepartId { get => salesDepartId; set => salesDepartId = value; }
        public string VersionOfTHS { get => versionOfTHS; set => versionOfTHS = value; }
        public Funds Funds { get => funds; set => funds = value; }
        public List<Position> Positions { get => positions; set => positions = value; }
        public List<Order> CancelOrders { get => cancelOrders; set => cancelOrders = value; }
        public List<ShareHolderAcct> ShareHolderAccts
        {
            get => shareHolderAccts; set => shareHolderAccts = value;
        }

        public string ShShareholderAcct
        {
            get => shShareholderAcct; set => shShareholderAcct = value;
        }

        public string SzShareholderAcct
        {
            get => szShareholderAcct; set => szShareholderAcct = value;
        }

        public bool Equals(Account other)
        {
            if (null == other)
            {
                return false;
            }
            return FundAcct == other.FundAcct;
        }

        //重写Equals和GetHashCode方法可以在List里面使用Contains方法
        public override int GetHashCode()
        {
            return FundAcct.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#FundAcct=").Append(FundAcct);
            sb.Append("#BrokerName=").Append(BrokerName);
            sb.Append("#BrokerServerIP= ").Append(BrokerServerIP);
            sb.Append("#InitTotalAsset=").Append(InitTotalAsset);
            return sb.ToString();
        }
    }
}