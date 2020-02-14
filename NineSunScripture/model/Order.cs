using System;
using System.Text;

namespace NineSunScripture.model
{
    /// <summary>
    /// 订单
    /// </summary>
    public class Order : BaseModel, IEquatable<Order>
    {
        //下面2个用作下单分类
        public const short CategoryBuy = 0;

        public const short CategorySell = 1;

        //下面2个用作查询分类
        public const string OperationBuy = "买入";

        public const string OperationSell = "卖出";

        /// <summary>
        /// 撤销数量
        /// </summary>
        private int canceledQuantity;

        /// <summary>
        /// 委托种类 0买入 1卖出
        /// </summary>
        private short category;

        private string code;
        //冗余字段：用于最新成交、撤单等查询
        private string name;

        /// <summary>
        /// 操作：证券买入|证券卖出
        /// </summary>
        private string operation;

        private string orderId;
        private float price;

        /// <summary>
        /// 委托、持仓数量
        /// </summary>
        private int quantity;

        /// <summary>
        ///股东账号；交易上海股票填上海的股东账号，交易深圳的股票填入深圳的股东账号。
        ///正常情况下留空（留空自动判断），撤销委托的时候必带
        /// </summary>
        private string shareholderAcct = "";
        private string time;
        /// <summary>
        /// 成交均价
        /// </summary>
        private float transactionPrice;

        /// <summary>
        /// 成交数量
        /// </summary>
        private int transactionQuantity;

        public int CanceledQuantity { get => canceledQuantity; set => canceledQuantity = value; }
        public short Category { get => category; set => category = value; }
        public string Code { get => code; set => code = value; }
        public string Name { get => name; set => name = value; }
        public string Operation { get => operation; set => operation = value; }
        public string OrderId { get => orderId; set => orderId = value; }
        public float Price { get => price; set => price = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public string ShareholderAcct { get => shareholderAcct; set => shareholderAcct = value; }
        public string Time { get => time; set => time = value; }
        public float TransactionPrice { get => transactionPrice; set => transactionPrice = value; }
        public int TransactionQuantity
        {
            get => transactionQuantity; set => transactionQuantity = value;
        }
        public bool Equals(Order other)
        {
            if (null == other)
            {
                return false;
            }
            return Code == other.Code;
        }

        //重写Equals和GetHashCode方法可以在List里面使用Contains方法
        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#Code=").Append(Code);
            sb.Append("#Name=").Append(Name);
            sb.Append("#Operation= ").Append(Operation);
            sb.Append("#Price=").Append(Price);
            sb.Append("#Quantity=").Append(Quantity);
            sb.Append("#TransactionQuantity=").Append(TransactionQuantity);
            sb.Append("#TransactionPrice=").Append(TransactionPrice);
            sb.Append("#CanceledQuantity=").Append(CanceledQuantity);
            return sb.ToString();
        }
    }
}