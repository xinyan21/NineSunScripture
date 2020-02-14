using System;
using System.Text;

namespace NineSunScripture.model
{
    /// <summary>
    /// 行情实体类
    /// </summary>
    public class Quotes : BaseModel, IEquatable<Quotes>
    {
        /// <summary>
        /// 【买入策略】常驻股票池中的股票
        /// </summary>
        public const short CategoryLongTerm = 0;

        /// <summary>
        /// 【买入策略】 龙头股票池中的股票
        /// </summary>
        public const short CategoryDragonLeader = 1;

        /// <summary>
        ///  【买入策略】最新股票池中的股票
        /// </summary>
        public const short CategoryLatest = 2;

        /// <summary>
        /// 【买入策略】 弱转强股票池中的股票
        /// </summary>
        public const short CategoryWeakTurnStrong = 3;

        /// <summary>
        ///  【买入策略】波段股票池中的股票
        /// </summary>
        public const short CategoryBand = 4;

        /// <summary>
        /// 【卖出策略】持仓股票池中的股票，用作卖点监控
        /// </summary>
        public const short CategoryPosition = 5;

        public const short OperationBuy = 1;
        public const short OperationSell = 2;

        private string code;
        private string name;

        //昨收
        private float preClose;

        //最新价
        private float latestPrice;

        //涨停价
        private float highLimit;

        //跌停价
        private float lowLimit;

        private float high;
        private float low;

        //成交金额
        private double money;

        //成交量
        private int volume;

        private float open;

        //卖一到卖五
        private float sell1;

        private float sell2;
        private float sell3;
        private float sell4;
        private float sell5;

        //买一到买五
        private float buy1;

        private float buy2;
        private float buy3;
        private float buy4;
        private float buy5;

        //对应的委托量
        private int sell1Vol;

        private int sell2Vol;
        private int sell3Vol;
        private int sell4Vol;
        private int sell5Vol;
        private int buy1Vol;
        private int buy2Vol;
        private int buy3Vol;
        private int buy4Vol;
        private int buy5Vol;

        /// <summary>
        /// 仓位控制，用于买入策略
        /// </summary>
        private float positionCtrl;

        /// <summary>
        /// 买入前的成交额限制，用于买入策略（单位为万）
        /// </summary>
        private int moneyCtrl;

        /// <summary>
        /// 股票类型：打板（常驻0、龙头1、最新2），弱转强（3），波段（4）
        /// </summary>
        private short stockCategory = 2;

        /// <summary>
        /// 操作方向：OperationBuy, OperationSell。于此区分买入和卖出的股票池
        /// </summary>
        private short operation;

        /// <summary>
        /// 连扳数
        /// </summary>
        private short contBoards;

        /// <summary>
        /// 是否是持仓股，默认不是，此属性用来解决新增账户后持仓不一致导致的开仓问题
        /// 值为true的时候不能新开仓
        /// </summary>
        private bool inPosition = false;

        private bool isDragonLeader = false;

        private float avgCost;

        /// <summary>
        /// 止盈比例
        /// </summary>
        private float stopWinPct;

        /// <summary>
        /// 止盈价格
        /// </summary>
        private float stopWinPrice;

        /// <summary>
        /// 止损比例
        /// </summary>
        private float stopLossPct;

        /// <summary>
        /// 止损价格
        /// </summary>
        private float stopLossPrice;

        /// <summary>
        /// 十档行情是否已经订阅
        /// </summary>
        private bool isTenthGearSubscribed;

        /// <summary>
        /// 逐笔委托行情是否已经订阅
        /// </summary>
        private bool isOByOComissionSubscribed;

        public string Code { get => code; set => code = value; }
        public string Name { get => name; set => name = value; }
        public float PreClose { get => preClose; set => preClose = value; }
        public float LatestPrice { get => latestPrice; set => latestPrice = value; }
        public float HighLimit { get => highLimit; set => highLimit = value; }
        public float LowLimit { get => lowLimit; set => lowLimit = value; }
        public float High { get => high; set => high = value; }
        public float Low { get => low; set => low = value; }
        public double Money { get => money; set => money = value; }
        public int Volume { get => volume; set => volume = value; }
        public float Open { get => open; set => open = value; }
        public float Sell1 { get => sell1; set => sell1 = value; }
        public float Sell2 { get => sell2; set => sell2 = value; }
        public float Sell3 { get => sell3; set => sell3 = value; }
        public float Sell4 { get => sell4; set => sell4 = value; }
        public float Sell5 { get => sell5; set => sell5 = value; }
        public float Buy1 { get => buy1; set => buy1 = value; }
        public float Buy2 { get => buy2; set => buy2 = value; }
        public float Buy3 { get => buy3; set => buy3 = value; }
        public float Buy5 { get => buy5; set => buy5 = value; }
        public float Buy4 { get => buy4; set => buy4 = value; }
        public int Sell1Vol { get => sell1Vol; set => sell1Vol = value; }
        public int Sell2Vol { get => sell2Vol; set => sell2Vol = value; }
        public int Sell3Vol { get => sell3Vol; set => sell3Vol = value; }
        public int Sell4Vol { get => sell4Vol; set => sell4Vol = value; }
        public int Sell5Vol { get => sell5Vol; set => sell5Vol = value; }
        public int Buy1Vol { get => buy1Vol; set => buy1Vol = value; }
        public int Buy2Vol { get => buy2Vol; set => buy2Vol = value; }
        public int Buy3Vol { get => buy3Vol; set => buy3Vol = value; }
        public int Buy4Vol { get => buy4Vol; set => buy4Vol = value; }
        public int Buy5Vol { get => buy5Vol; set => buy5Vol = value; }
        public float PositionCtrl { get => positionCtrl; set => positionCtrl = value; }
        public int MoneyCtrl { get => moneyCtrl; set => moneyCtrl = value; }
        public short StockCategory { get => stockCategory; set => stockCategory = value; }
        public short Operation { get => operation; set => operation = value; }
        public short ContBoards { get => contBoards; set => contBoards = value; }
        public bool InPosition { get => inPosition; set => inPosition = value; }
        public bool IsDragonLeader { get => isDragonLeader; set => isDragonLeader = value; }
        public float AvgCost { get => avgCost; set => avgCost = value; }
        public float StopWinPct { get => stopWinPct; set => stopWinPct = value; }
        public float StopWinPrice { get => stopWinPrice; set => stopWinPrice = value; }
        public float StopLossPct { get => stopLossPct; set => stopLossPct = value; }
        public float StopLossPrice { get => stopLossPrice; set => stopLossPrice = value; }
        public bool IsTenthGearSubscribed
        {
            get => isTenthGearSubscribed; set => isTenthGearSubscribed = value;
        }
        public bool IsOByOComissionSubscribed
        {
            get => isOByOComissionSubscribed; set => isOByOComissionSubscribed = value;
        }

        public bool Equals(Quotes other)
        {
            if (null == other)
            {
                return false;
            }
            return Code == other.Code;
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
            sb.Append("#Open=").Append(Open);
            sb.Append("#HighLimit=").Append(HighLimit);
            sb.Append("#LowLimit=").Append(LowLimit);
            sb.Append("#LatestPrice= ").Append(LatestPrice);
            sb.Append("#Money=").Append(Money);
            sb.Append("#Sell1=").Append(Sell1);
            sb.Append("#Sell1Vol=").Append(Sell1Vol);
            sb.Append("#Buy1=").Append(Buy1);
            sb.Append("#Buy1Vol=").Append(Buy1Vol);
            sb.Append("#PositionCtrl=").Append(PositionCtrl);
            sb.Append("#MoneyCtrl=").Append(MoneyCtrl);
            sb.Append("#StockCategory=").Append(StockCategory);
            sb.Append("#InPosition=").Append(InPosition);
            sb.Append("#IsDragonLeader=").Append(IsDragonLeader);
            return sb.ToString();
        }

        /// <summary>
        /// 复制策略参数
        /// </summary>
        /// <param name="quotes">源股票对象，附带操作计划</param>
        public void CloneStrategyParamsFrom(Quotes quotes)
        {
            if (null == quotes)
            {
                return;
            }
            HighLimit = quotes.HighLimit;
            LowLimit = quotes.LowLimit;
            InPosition = quotes.InPosition;
            IsDragonLeader = quotes.IsDragonLeader;
            PositionCtrl = quotes.PositionCtrl;
            MoneyCtrl = quotes.MoneyCtrl;
            ContBoards = quotes.ContBoards;
            StockCategory = quotes.StockCategory;
            Operation = quotes.Operation;
            StopWinPct = quotes.StopWinPct;
            StopWinPrice = quotes.StopWinPrice;
            StopLossPct = quotes.StopLossPct;
            StopLossPrice = quotes.StopLossPrice;
            AvgCost = quotes.AvgCost;
            IsOByOComissionSubscribed = quotes.IsOByOComissionSubscribed;
            IsTenthGearSubscribed = quotes.IsTenthGearSubscribed;
        }
    }
}