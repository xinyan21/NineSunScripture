using NineSunScripture.model;
using NineSunScripture.trade.api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineSunScripture.strategy
{
    /// <summary>
    /// 主策略
    /// </summary>
    class MainStrategy
    {
        private const int IntervalOfNonTrade = 3000;    
        private const int IntervalOfTrade = 200;
        //交易时间睡眠间隔为200ms，非交易时间为3s
        private int sleepInterval = IntervalOfTrade;
        private String[] stocks;
        private int clientId;
        private Thread mainThread;


        public MainStrategy(int clientId, String[] stocks)
        {
            this.stocks = stocks;
            this.clientId = clientId;
        }
        public void Start()
        {
            if (null == stocks || stocks.Length == 0)
            {
                MessageBox.Show("没有可操作的股票");
                return;
            }
            if (null == mainThread)
            {
                mainThread = new Thread(Process);
            }
            mainThread.Start();
        }

        private void Process()
        {
            //到时改到总开关里面去
            while (true)
            {
                Thread.Sleep(sleepInterval);
                if (!IsTradeTime())
                {
                    continue;
                }
                for (int i = 0; i < stocks.Length; i++)
                {
                    Quotes quotes = TradeAPI.QueryQuotes(clientId, stocks[i]);
                }
            }

        }

        private bool IsTradeTime() {
            if (DateTime.Now.Hour < 9 || DateTime.Now.Hour == 12 || DateTime.Now.Hour >= 15)
            {
                if (IntervalOfNonTrade != sleepInterval)
                {
                    sleepInterval = IntervalOfNonTrade;
                }
                return false;
            }
            if (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 29)
            {
                if (IntervalOfNonTrade != sleepInterval)
                {
                    sleepInterval = IntervalOfNonTrade;
                }
                return false;
            }
            if (DateTime.Now.Hour == 11 && DateTime.Now.Minute > 30)
            {
                if (IntervalOfNonTrade != sleepInterval)
                {
                    sleepInterval = IntervalOfNonTrade;
                }
                return false;
            }
            if (IntervalOfTrade != sleepInterval)
            {
                sleepInterval = IntervalOfTrade;
            }
            return true;
        }

        private void QueryPositions()
        {
        }
    }
}
