using NineSunScripture.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.strategy
{
    public class Strategy
    {
        /// <summary>
        /// 默认历史数据保存数量为，由一分钟除睡眠间隔算出，也就是保留一分钟的历史数据
        /// </summary>
        protected const short DefaultHistoryTickCnt = 300;
        /// <summary>
        /// 历史数据
        /// </summary>
        protected Dictionary<string, Queue<Quotes>> historyTicks
            = new Dictionary<string, Queue<Quotes>>();
    }
}
