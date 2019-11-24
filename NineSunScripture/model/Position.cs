﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.model
{
    /// <summary>
    /// 持仓实体类 
    /// 证券代码\t证券名称\t持仓数量\t可用数量\t冻结数量\t参考盈亏\t成本价
    /// \t盈亏比例\t市价\t市值\t交易市场\t股东账户
    /// </summary>
    public class Position : BaseModel, IEquatable<Position>
    {
        public String Code;
        public String Name;
        /// <summary>
        /// 持仓数量
        /// </summary>
        public int QuantityBalance;
        /// <summary>
        /// 可用数量
        /// </summary>
        public int AvailableQuantity;
        /// <summary>
        /// 冻结数量
        /// </summary>
        public int FrozenQuantity;
        /// <summary>
        /// 参考盈亏
        /// </summary>
        public float ProfitAndLoss;
        public float AvgCost;
        /// <summary>
        /// 盈亏比例
        /// </summary>
        public float ProfitAndLossPct;
        public float Price;
        public String Market;
        public String ShareholderAcct;

        public bool Equals(Position other)
        {
            return this.Code.Equals(other.Code);
        }
    }
}
