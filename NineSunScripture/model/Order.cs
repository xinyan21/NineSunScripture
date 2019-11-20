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
        public const string OperationBuy = "证券买入";
        public const string OperationSell = "证券卖出";

        public String OrderId;
        /// <summary>
        /// 委托种类 0买入 1卖出
        /// </summary>
        public short Category;
        /// <summary>
        ///股东账号；交易上海股票填上海的股东账号，交易深圳的股票填入深圳的股东账号。
        ///正常情况下留空（留空自动判断），撤销委托的时候必带
        /// </summary>
        public String ShareholderAcct = "";
        public float Price;
        /// <summary>
        /// 委托、持仓数量
        /// </summary>
        public int Quantity;
        public String Code;

        //冗余字段：用于当日成交、撤单等查询
        public String Name;
        public String Time;
        /// <summary>
        /// 操作：证券买入|证券卖出
        /// </summary>
        public String Operation;
    }
}
