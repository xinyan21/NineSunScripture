using NineSunScripture.model;
using NineSunScripture.trade.persistence;
using NineSunScripture.util;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Text;

namespace NineSunScripture.strategy
{
    /// <summary>
    /// 可转债策略
    /// </summary>
    class ConvertibleBondStrategy
    {
        private double asset = 50000;
        private string priceUrl = "http://hq.sinajs.cn/list=";
        private bool isBidBought = false;
        private ITrade callback;

        private Http http;
        //标的最高价字典；key=Name，value=行情对象，里面的time保留了时间
        private Dictionary<string, Quotes> highPiceDic;
        private Dictionary<string, Quotes> buyRecordDic;
        private Dictionary<string, Quotes> sellRecordDic;
        private Dictionary<string, Quotes> lastTickDic;
        private Dictionary<string, double> boughtValue;     //已买市值

        public ConvertibleBondStrategy(ITrade callback)
        {
            this.callback = callback;
            http = new Http();
            highPiceDic = new Dictionary<string, Quotes>();
            buyRecordDic = new Dictionary<string, Quotes>();
            sellRecordDic = new Dictionary<string, Quotes>();
            lastTickDic = new Dictionary<string, Quotes>();
            boughtValue = new Dictionary<string, double>();
            Init();
        }


        public void DoStrategy()
        {
            if (!IsTradeTime())
            {
                return;
            }
            List<Quotes> quotes = QueryPrice();
            if (Utils.IsListEmpty(quotes))
            {
                Logger.Log("可转债行情查询为空");
                return;
            }
            try
            {
                UpdateHighPrice(quotes);
                Sell(quotes);
                Buy(quotes);
                UpdateLastTick(quotes);
                DateTime now = DateTime.Now;
                if (now.Hour > 10 && now.Minute == 0 && now.Second == 0)
                {
                    Logger.Log("可转债当日盈亏比例为" + (asset / 50000 - 1) * 100 + "%");
                }
            }
            catch (Exception e)
            {
                Logger.Log("可转债策略异常>" + e.Message);
                Logger.Exception(e);
            }
        }

        private void Buy(List<Quotes> quotes)
        {
            if (asset < asset / 5)
            {
                return;
            }
            if (IsOpenMarketBidTime())
            {
                BidTimeBuy(quotes);
                return;
            }
            //暂时只买10点前的，后面再加火箭发射的买点
            if (DateTime.Now.Hour >= 10)
            {
                return;
            }
            //除了竞价，其它的玩不了，要被止损玩死，除非是热点的几个
           /* foreach (var item in quotes)
            {
                if (item.Buy1 / item.PreClose < 1.06)
                {
                    break;
                }
                if (buyRecordDic.ContainsKey(item.Name))
                {
                    continue;
                }
                if (!lastTickDic.ContainsKey(item.Name))
                {
                    continue;
                }
                if (item.Buy1 / item.PreClose >= 1.06
                    && lastTickDic[item.Name].Buy1 / item.PreClose < 1.06)
                {
                    SimulateBuy(item);
                }
            }*/
        }

        private void Sell(List<Quotes> quotes)
        {
            if (0 == buyRecordDic.Count)
            {
                return;
            }
            SellBondsBoughtBefor935(quotes);
            foreach (var key in buyRecordDic.Keys)
            {
                Quotes buyQuote = buyRecordDic[key];
                Quotes quote = quotes.Find(q => q.Name == key);
                if (null == quote)
                {
                    continue;
                }
                if ((quote.Buy1 - buyQuote.Buy1) / quote.PreClose > 0.1)
                {
                    Logger.Log("可转债止盈卖出1/3>" + quote.Name);
                    SimulateSell(quote);
                    continue;
                }
                if ((quote.Buy1 - buyQuote.Buy1) / quote.PreClose > 0.2)
                {
                    Logger.Log("可转债止盈卖出1/2>" + quote.Name);
                    SimulateSell(quote);
                    continue;
                }
                if ((quote.Buy1 - buyQuote.Buy1) / quote.PreClose > 0.3)
                {
                    Logger.Log("可转债止盈清仓>" + quote.Name);
                    SimulateSell(quote);
                    continue;
                }
                if ((quote.Buy1 - buyQuote.Buy1) / quote.PreClose < -0.015)
                {
                    Logger.Log("可转债止损卖出>" + quote.Name);
                    SimulateSell(quote);
                    continue;
                }
                if ((quote.Buy1 - buyQuote.Buy1) <= (highPiceDic[key].Buy1 - buyQuote.Buy1) / 2)
                {
                    Logger.Log("可转债回落超过一半最高涨幅卖出>" + quote.Name);
                    SimulateSell(quote);
                    continue;
                }
            }
        }

        private void BidTimeBuy(List<Quotes> quotes)
        {
            if (isBidBought)
            {
                return;
            }
            //按涨幅排序买前面4个，每个1/4仓位
            Quotes bond = quotes[0];
            if (bond.Buy1 / bond.PreClose >= 1.06)
            {
                SimulateBuy(bond);
                Logger.Log("可转债竞价买入>" + bond.Name);
            }
            bond = quotes[1];
            if (bond.Buy1 / bond.PreClose >= 1.06)
            {
                SimulateBuy(bond);
                Logger.Log("可转债竞价买入>" + bond.Name);
            }
            bond = quotes[2];
            if (bond.Buy1 / bond.PreClose >= 1.06)
            {
                SimulateBuy(bond);
                Logger.Log("可转债竞价买入>" + bond.Name);
            }
            bond = quotes[3];
            if (bond.Buy1 / bond.PreClose >= 1.06)
            {
                SimulateBuy(bond);
                Logger.Log("可转债竞价买入>" + bond.Name);
            }
            isBidBought = true;
        }

        private void SellBondsBoughtBefor935(List<Quotes> quotes)
        {
            if (0 == buyRecordDic.Count)
            {
                return;
            }
            List<string> soldBonds = new List<string>();
            foreach (var key in buyRecordDic.Keys)
            {
                Quotes buyQuote = buyRecordDic[key];
                Quotes quote = quotes.Find(q => q.Name == key);
                if (null == quote)
                {
                    Logger.Log("Name not find in quotes> " + key);
                    continue;
                }
                DateTime buyTime = buyQuote.Time;
                if (9 == buyTime.Hour && buyTime.Minute <= 31 && quote.Buy1 < buyQuote.Buy1)
                {
                    double secsAfterHighUpdated = DateTime.Now.Subtract(buyTime).TotalSeconds;
                    if (secsAfterHighUpdated >= 60)
                    {
                        Logger.Log("可转债卖出9：30及之前买入的>" + quote.Name);
                        SimulateSell(quote);
                    }
                }
            }
            if (soldBonds.Count > 0)
            {
                foreach (var item in soldBonds)
                {
                    buyRecordDic.Remove(item);
                    asset += boughtValue[item];
                    boughtValue.Remove(item);
                }
            }
        }

        private void SimulateBuy(Quotes quote)
        {
            if (buyRecordDic.ContainsKey(quote.Name) || asset < asset / 5)
            {
                return;
            }
            buyRecordDic.Add(quote.Name, quote);
            boughtValue.Add(quote.Name, asset / 4);
            asset -= asset / 4;
            string log = "可转债模拟买入>" + quote.Name;
            Logger.Log(log);
            if (null != callback)
            {
                callback.OnTradeResult(MainStrategy.RspCodeOfUpdateAcctInfo, log, "", false);
            }
        }

        private void SimulateSell(Quotes quote)
        {
            if (sellRecordDic.ContainsKey(quote.Name))
            {
                return;
            }
            sellRecordDic.Add(quote.Name, quote);
            Quotes buyQuote = buyRecordDic[quote.Name];
            float profitPct = (quote.Buy1 - buyQuote.Buy1) / buyQuote.Buy1;
            boughtValue[quote.Name] *= profitPct;
            asset += boughtValue[quote.Name];

            boughtValue.Remove(quote.Name);
            Logger.Log("可转债模拟卖出>" + quote.Name);
        }

        private void Init()
        {
            List<string> bonds = JsonDataHelper.Instance.GetConvertibleBonds();
            if (null == bonds || bonds.Count == 0)
            {
                Logger.Log("可转债代码列表为空，策略初始化失败");
                return;
            }
            priceUrl = priceUrl + PackCodeParam(bonds);
        }

        private void UpdateHighPrice(List<Quotes> quotes)
        {
            if (0 == highPiceDic.Count)
            {
                return;
            }
            foreach (var item in highPiceDic.Keys)
            {
                Quotes quote = quotes.Find(q => q.Name == highPiceDic[item].Name);
                if (null != quote && quote.Buy1 > highPiceDic[item].Buy1)
                {
                    highPiceDic[item] = quote;
                }
            }
        }

        private void UpdateLastTick(List<Quotes> quotes)
        {
            foreach (var item in quotes)
            {
                if (item.Buy1 / item.PreClose < 1.03)
                {
                    break;
                }
                if (!lastTickDic.ContainsKey(item.Name))
                {
                    lastTickDic.Add(item.Name, item);
                }
                else
                {
                    lastTickDic[item.Name] = item;
                }
            }
        }

        private string PackCodeParam(List<string> codes)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in codes)
            {
                if (sb.Length > 0)
                {
                    sb.Append(",");
                }
                sb.Append(item);
            }
            return sb.ToString();
        }

        private List<Quotes> QueryPrice()
        {
            //新浪接口使用的GB2312编码
            string data = http.Get(priceUrl, "", 2000, Encoding.GetEncoding(936));
            if (string.IsNullOrEmpty(data))
            {
                Logger.Log("QueryPrice 接口返回空值->");
                return null;
            }
            List<Quotes> quotes = new List<Quotes>();
            try
            {
                string[] priceList = data.Split(';');
                if (null == priceList || priceList.Length == 0)
                {
                    Logger.Log("QueryPrice 值分割结果为空->");
                    return null;
                }
                for (int i = 0; i < priceList.Length; i++)
                {
                    if (priceList[i].Length < 50)
                    {
                        continue;
                    }
                    Quotes quote = new Quotes();
                    string[] price = priceList[i].Split('=')[1].Split(',');
                    quote.Name = @price[0];
                    quote.Open = float.Parse(@price[1]);
                    quote.PreClose = float.Parse(@price[2]);
                    quote.LatestPrice = float.Parse(@price[3]);
                    quote.High = float.Parse(@price[4]);
                    quote.Low = float.Parse(@price[5]);
                    quote.Buy1 = float.Parse(@price[6]);
                    quote.Sell1 = float.Parse(@price[7]);
                    quote.Volume = int.Parse(@price[8]);
                    quote.Money = float.Parse(@price[9]);
                    quote.Time = DateTime.Now;

                    quotes.Add(quote);
                }
                quotes.Sort((left, right) =>
                {
                    if (left.Buy1 / left.PreClose <= right.Buy1 / right.PreClose)
                        return 1;
                    else
                        return -1;
                });
            }
            catch (Exception e)
            {
                Logger.Log("QueryPrice exception->" + e.Message);
                Logger.Exception(e);
            }


            return quotes;
        }

        private bool IsTradeTime()
        {
            DateTime now = DateTime.Now;
            if (now.Hour < 9 || now.Hour > 14)
            {
                return false;
            }
            if (now.Hour == 9 && now.Minute < 24)
            {
                return false;
            }
            if (now.Hour == 11 && now.Minute >= 30)
            {
                return false;
            }
            if (now.Hour == 12)
            {
                return false;
            }
            return true;
        }

        private bool IsOpenMarketBidTime()
        {
            DateTime now = DateTime.Now;
            //可转债策略只有在3的倍数秒才会执行
            if (now.Hour == 9 && now.Minute == 24 && now.Second == 56)
            {
                return true;
            }
            return false;
        }

        private bool IsCloseMarketBidTime()
        {
            DateTime now = DateTime.Now;
            if (now.Hour == 14 && now.Minute == 57)
            {
                return true;
            }
            return false;
        }
    }
}
