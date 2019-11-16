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
    public class Quotes : BaseModel
    {
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

    }
}
