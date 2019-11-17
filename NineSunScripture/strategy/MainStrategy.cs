using NineSunScripture.model;
using NineSunScripture.trade.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NineSunScripture.strategy
{
    /// <summary>
    /// 主策略
    /// </summary>
    class MainStrategy
    {
        private String[] stocks;
        private int clientId;
        private Thread mainThread;        public MainStrategy(int clientId, String[] stocks) {
            this.stocks = stocks;
            this.clientId = clientId;
        }
        public void Start() {
            if (null == mainThread) {
                mainThread = new Thread(Process);
            }
            mainThread.Start();
        }

        private void Process() {
            //到时改到总开关里面去
            while (true)
            {
                Thread.Sleep(200);
                if (DateTime.Now.Hour < 9 || DateTime.Now.Hour == 12 || DateTime.Now.Hour >= 15)
                {
                    return;
                }
                if (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 30)
                {
                    return;
                }
                if (DateTime.Now.Hour == 11 && DateTime.Now.Minute > 30)
                {
                    return;
                }
                if (null==stocks || stocks.Length==0)
                {
                    return;
                }
                for (int i = 0; i < stocks.Length; i++)
                {
                    Quotes quotes = TradeAPI.QueryQuotes(clientId, stocks[i]); 
                }
            }

        }

        private void QueryPositions() { 
        }
    }
}
