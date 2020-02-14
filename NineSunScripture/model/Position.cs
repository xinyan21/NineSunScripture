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
        private string code;
        private string name;

        /// <summary>
        /// 股票余额=持有数量
        /// </summary>
        private int stockBalance;

        /// <summary>
        /// 可用余额=可以卖的股票数
        /// </summary>
        private int availableBalance;

        /// <summary>
        /// 冻结数量
        /// </summary>
        private int frozenQuantity;

        /// <summary>
        /// 参考盈亏
        /// </summary>
        private float profitAndLoss;

        private float avgCost;

        /// <summary>
        /// 盈亏比例
        /// </summary>
        private float profitAndLossPct;

        private float price;

        /// <summary>
        /// 市值
        /// </summary>
        private float marketValue;

        /// <summary>
        /// 市场
        /// </summary>
        private string market;

        private string shareholderAcct;

        public string Code { get => code; set => code = value; }
        public string Name { get => name; set => name = value; }
        public int StockBalance { get => stockBalance; set => stockBalance = value; }
        public int AvailableBalance { get => availableBalance; set => availableBalance = value; }
        public int FrozenQuantity { get => frozenQuantity; set => frozenQuantity = value; }
        public float ProfitAndLoss { get => profitAndLoss; set => profitAndLoss = value; }
        public float AvgCost { get => avgCost; set => avgCost = value; }
        public float ProfitAndLossPct { get => profitAndLossPct; set => profitAndLossPct = value; }
        public float Price { get => price; set => price = value; }
        public float MarketValue { get => marketValue; set => marketValue = value; }
        public string Market { get => market; set => market = value; }
        public string ShareholderAcct { get => shareholderAcct; set => shareholderAcct = value; }

        public bool Equals(Position other)
        {
            if (null == other)
            {
                return false;
            }
            return Code.Equals(other.Code);
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