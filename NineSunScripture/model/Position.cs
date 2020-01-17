using System;
using System.Text;

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
        /// 股票余额=持有数量
        /// </summary>
        public int StockBalance;

        /// <summary>
        /// 可用余额=可以卖的股票数
        /// </summary>
        public int AvailableBalance;

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

        /// <summary>
        /// 市值
        /// </summary>
        public float MarketValue;

        /// <summary>
        /// 市场
        /// </summary>
        public String Market;

        public String ShareholderAcct;

        public bool Equals(Position other)
        {
            return this.Code.Equals(other.Code);
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
            sb.Append("#MarketValue= ").Append(MarketValue);
            sb.Append("#ProfitAndLoss=").Append(ProfitAndLoss);
            sb.Append("#StockBalance=").Append(StockBalance);
            sb.Append("#AvailableBalance=").Append(AvailableBalance);
            sb.Append("#ProfitAndLossPct=").Append(ProfitAndLossPct);
            sb.Append("#AvgCost=").Append(AvgCost);
            return sb.ToString();
        }
    }
}