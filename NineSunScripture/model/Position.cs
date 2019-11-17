using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.model
{
    /// <summary>
    /// 持仓实体类 
    /// 证券代码\t证券名称\t股票余额\t可用余额\t冻结数量\t参考盈亏\t成本价
    /// \t盈亏比例\t市价\t市值\t交易市场\t股东账户
    /// </summary>
    class Position
    {
        public String Code;
        public String Name;
        public int StockBalance;
        public int StockAvailable;
        public int FrozenQuantity;
        public float ProfitAndLoss;
        public float AvgCost;
        public float ProfitAndLossPct;
        public float Price;
        public String Market;
        public String ShareholderAcct;
    }
}
