using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.model
{
    /// <summary>
    /// 资金
    /// </summary>
    public class Funds : BaseModel
    {
        //总资产
        public double TotalAsset;
        //资金余额
        public double FundBalance;
        //冻结金额
        public double FrozenAmt;
        //可用金额
        public double AvailableAmt;
    }
}
