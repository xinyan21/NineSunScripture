using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.model
{
    /// <summary>
    /// 账户类
    /// </summary>
    public class Account : BaseModel
    {
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
        public int BrokerId;
        public string BrokerName;
        public String BrokerServerIP;
        public short BrokerServerPort;
        public String VersionOfTHS;
        public short AcctType;
        public String CommPwd;
        public bool IsRandomMac = false;
        public String FundAcct;//资金账号
        public String Password;
        public String ShShareholderAcct;//上海股东账号
        public String SzShareholderAcct;//深圳股东账户
        public int InitTotalAsset;    //初始总资金，作为单股开仓仓位依据

        public Funds Funds;
        public List<Position> Positions;
    }
}
