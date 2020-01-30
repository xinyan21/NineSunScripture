using NineSunScripture.model;
using System.Collections.Generic;

namespace NineSunScripture.strategy
{
    public class Strategy
    {
        /// <summary>
        /// 默认历史数据保存数量，3s一个Tick，40根是2分钟数据
        /// </summary>
        protected const short DefaultHistoryTickCnt = 40;

        /// <summary>
        /// 历史数据
        /// </summary>
        protected Dictionary<string, Queue<Quotes>> historyTicks
            = new Dictionary<string, Queue<Quotes>>();
    }
}