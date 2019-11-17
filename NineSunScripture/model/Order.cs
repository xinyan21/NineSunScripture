using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.model
{
    /// <summary>
    /// 订单
    /// </summary>
    public class Order : BaseModel
    {
        public const short CategoryBuy = 0;
        public const short CategorySell = 1;

        public String OrderId;
        public short Category;//委托种类 0买入 1卖出
        //股东账号；交易上海股票填上海的股东账号，交易深圳的股票填入深圳的股东账号。
        //正常情况下留空（留空自动判断）
        public String ShareholderAcct;
        public float Price;
        public int Quantity;//委托数量
        public String Code;
    }
}
