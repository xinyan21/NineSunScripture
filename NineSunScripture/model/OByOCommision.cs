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
        private int time;
        /// <summary>
        /// 报价分类：1买，2卖
        /// </summary>
        private int category;
        /// <summary>
        /// 委托价格
        /// </summary>
        private double price;
        /// <summary>
        /// 委托量
        /// </summary>
        private double quantity;

        public int Category { get => category; set => category = value; }
        public int Time { get => time; set => time = value; }
        public double Price { get => price; set => price = value; }
        public double Quantity { get => quantity; set => quantity = value; }

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
