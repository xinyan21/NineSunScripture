﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string OrderId;
        /// <summary>
        /// 委托种类 0买入 1卖出
        /// </summary>
        public short Category;
        /// <summary>
        ///股东账号；交易上海股票填上海的股东账号，交易深圳的股票填入深圳的股东账号。
        ///正常情况下留空（留空自动判断），撤销委托的时候必带
        /// </summary>
        public string ShareholderAcct = "";
        public float Price;
        /// <summary>
        /// 委托、持仓数量
        /// </summary>
        public int Quantity;
        public string Code;

        //冗余字段：用于最新成交、撤单等查询
        public string Name;
        public string Time;
        /// <summary>
        /// 操作：证券买入|证券卖出
        /// </summary>
        public string Operation;
        /// <summary>
        /// 成交数量
        /// </summary>
        public int TransactionQuantity;
        /// <summary>
        /// 成交均价
        /// </summary>
        public float TransactionPrice;
        /// <summary>
        /// 撤销数量
        /// </summary>
        public int CanceledQuantity;

        public bool Equals(Order other)
        {
            return this.Code == other.Code;
        }

        //重写Equals和GetHashCode方法可以在List里面使用Contains方法
        public override int GetHashCode()
        {
            return this.Code.GetHashCode();
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
