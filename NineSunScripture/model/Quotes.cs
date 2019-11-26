using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.model
{
    /// <summary>
    /// 行情实体类
    /// </summary>
    public class Quotes : BaseModel, IEquatable<Quotes>
    {
        /// <summary>
        /// 常驻股票池中的股票
        /// </summary>
        public  const short CategoryLongTerm = 0;
        /// <summary>
        /// 龙头股票池中的股票
        /// </summary>
        public const short CategoryDragonLeader = 1;
        /// <summary>
        /// 最新股票池中的股票
        /// </summary>
        public const short CategoryLatest = 2;

        public String Code;
        public String Name;
        //昨收
        public float PreClose;
        //最新价
        public float LatestPrice;
        //涨停价
        public float HighLimit;
        //跌停价
        public float LowLimit;
        public float High;
        public float Low;
        //成交金额
        public double Money;
        //成交量
        public int Volume;
        public float Open;
        public float Sell1;
        public float Sell2;
        public float Sell3;
        public float Sell4;
        public float Sell5;
        public float Buy1;
        public float Buy2;
        public float Buy3;
        public float Buy4;
        public float Buy5;
        public int Sell1Vol;
        public int Sell2Vol;
        public int Sell3Vol;
        public int Sell4Vol;
        public int Sell5Vol;
        public int Buy1Vol;
        public int Buy2Vol;
        public int Buy3Vol;
        public int Buy4Vol;
        public int Buy5Vol;

        /// <summary>
        /// 仓位控制，用于买入策略
        /// </summary>
        public float PositionCtrl;
        /// <summary>
        /// 买入前的成交额限制，用于买入策略
        /// </summary>
        public int MoneyCtrl;
        /// <summary>
        /// 股票类型：常驻0、龙头1、最新2
        /// </summary>
        public short StockCategory;

        public bool Equals(Quotes other)
        {
            return this.Code == other.Code;
        }
        //重写Equals和GetHashCode方法可以在List里面使用Contains方法
        public override int GetHashCode()
        {
            return this.Code.GetHashCode();
        }
    }
}
