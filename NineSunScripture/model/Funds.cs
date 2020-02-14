using System.Text;

namespace NineSunScripture.model
{
    /// <summary>
    /// 资金
    /// </summary>
    public class Funds : BaseModel
    {
        //总资产
        private double totalAsset;

        //资金余额
        private double fundBalance;

        //冻结金额
        private double frozenAmt;

        //可用金额
        private double availableAmt;

        public double TotalAsset { get => totalAsset; set => totalAsset = value; }
        public double FundBalance { get => fundBalance; set => fundBalance = value; }
        public double FrozenAmt { get => frozenAmt; set => frozenAmt = value; }
        public double AvailableAmt { get => availableAmt; set => availableAmt = value; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#TotalAsset=").Append(TotalAsset);
            sb.Append("#FundBalance=").Append(FundBalance);
            sb.Append("#FrozenAmt= ").Append(FrozenAmt);
            sb.Append("#AvailableAmt=").Append(AvailableAmt);
            return sb.ToString();
        }
    }
}