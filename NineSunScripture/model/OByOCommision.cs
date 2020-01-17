using System.Text;

namespace NineSunScripture.model
{
    /// <summary>
    /// 逐笔委托
    /// </summary>
    public class OByOCommision : BaseModel
    {
        /// <summary>
        ///时间戳
        /// </summary>
        public int Time;
        /// <summary>
        /// 报价分类：1买，2卖
        /// </summary>
        public int Category;
        /// <summary>
        /// 委托价格
        /// </summary>
        public double Price;
        /// <summary>
        /// 委托量
        /// </summary>
        public double Quantity;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("#Time=").Append(Time);
            sb.Append("#Category=").Append(Category);
            sb.Append("#Price=").Append(Price);
            sb.Append("#Quantity=").Append(Quantity);
            return sb.ToString();
        }
    }
}
