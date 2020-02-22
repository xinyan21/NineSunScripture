using Newtonsoft.Json;
using NineSunScripture.model;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace NineSunScripture.trade.persistence
{
    /// <summary>
    /// 将账户和股票持久化到json文件，方便使用，
    /// 为防止多线程调用导致数据错误，特使用线程安全的单例模式
    /// </summary>
    public sealed class JsonDataHelper
    {
        private static string baseDir = Environment.CurrentDirectory + @"\data\";
        private static string acctsFilePath = baseDir + "accounts.json";
        private static string stocksFilePath = baseDir + "stocks.json";
        private static string settingsFilePath = baseDir + "settings.json";
        private static string openBoardCntFilePath = baseDir + "openBoardCnt.json";

        private Dictionary<string, string> settings;
        private static readonly Lazy<JsonDataHelper> lazy
            = new Lazy<JsonDataHelper>(() => new JsonDataHelper());

        public static JsonDataHelper Instance { get { return lazy.Value; } }

        public Dictionary<string, string> Settings { get => settings; set => settings = value; }

        private JsonDataHelper()
        {
            Settings = GetSettings();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Dictionary<string, short> GetOpenBoardCnt()
        {
            Dictionary<DateTime, Dictionary<string, short>> data
            = new Dictionary<DateTime, Dictionary<string, short>>();
            try
            {
                if (!File.Exists(openBoardCntFilePath))
                {
                    File.Create(openBoardCntFilePath);
                }
                data = JsonConvert.DeserializeObject<
                    Dictionary<DateTime, Dictionary<string, short>>>(File.ReadAllText(openBoardCntFilePath));
                if (null != data && data.ContainsKey(DateTime.Now.Date))
                {
                    return data[DateTime.Now.Date];
                }
            }
            catch (Exception e)
            {
                Logger.Log("GetSettings exception: " + e.Message);
                Logger.Exception(e);
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private Dictionary<string, string> GetSettings()
        {
            Dictionary<string, string> settings = null;
            try
            {
                if (!File.Exists(settingsFilePath))
                {
                    File.Create(settingsFilePath);
                }
                settings = JsonConvert
                    .DeserializeObject<Dictionary<string, string>>(File.ReadAllText(settingsFilePath));
                if (null == settings)
                {
                    settings = new Dictionary<string, string>();
                }
            }
            catch (Exception e)
            {
                Logger.Log("GetSettings exception: " + e.Message);
                Logger.Exception(e);
            }
            return settings;
        }

        /// <summary>
        /// 获取所有账号，账号由人工编辑json文件，方便快捷
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<Account> GetAccounts()
        {
            List<Account> accounts = null;
            try
            {
                if (!File.Exists(acctsFilePath))
                {
                    File.Create(acctsFilePath);
                }
                accounts
                    = JsonConvert.DeserializeObject<List<Account>>(File.ReadAllText(acctsFilePath));
            }
            catch (Exception e)
            {
                Logger.Log("GetAccounts exception: " + e.Message);
                Logger.Exception(e);
            }
            return accounts;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void InitTotalAsset(Account account)
        {
            List<Account> accounts = GetAccounts();
            if (null == accounts || null == account)
            {
                return;
            }
            Account item = accounts.Find(i => account.FundAcct == i.FundAcct);
            if (null == item)
            {
                Logger.Log("Set account init total asset  failed, cannot found this account. " + account);
                return;
            }
            item.InitTotalAsset = account.InitTotalAsset;
            SaveAccounts(accounts);
        }

        /// <summary>
        /// 获取股票池所有股票
        /// </summary>
        /// <returns></returns>
        public List<Quotes> GetStocks()
        {
            List<Quotes> stocks = null;
            try
            {
                if (!File.Exists(stocksFilePath))
                {
                    File.Create(stocksFilePath);
                }
                stocks
                    = JsonConvert.DeserializeObject<List<Quotes>>(File.ReadAllText(stocksFilePath));
            }
            catch (Exception e)
            {
                Logger.Exception(e);
                Logger.Log("GetStocks exception: " + e.Message);
            }
            return stocks;
        }

        public List<Quotes> GetStocksByOperation(short operation)
        {
            List<Quotes> stocks = GetStocks();
            if (null == stocks || stocks.Count == 0)
            {
                return null;
            }
            List<Quotes> result = stocks.FindAll(item => item.Operation == operation);
            return result;
        }

        public List<Quotes> GetStocksByCatgory(short operation, short category)
        {
            List<Quotes> stocks = GetStocks();
            if (null == stocks || stocks.Count == 0)
            {
                return null;
            }
            List<Quotes> result = stocks.FindAll(
                item => item.StockCategory == category && item.Operation == operation);
            return result;
        }

        /// <summary>
        /// 增加股票quotes到股票池
        /// </summary>
        /// <param name="quotes">个股</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddStock(Quotes quotes)
        {
            List<Quotes> stocks = GetStocks();
            if (null == stocks)
            {
                stocks = new List<Quotes>();
            }
            if (!stocks.Exists(
                item => item.Code == quotes.Code && item.Operation == quotes.Operation &&
                 item.StockCategory == quotes.StockCategory))
            {
                stocks.Add(quotes);
            }
            SaveStocks(stocks);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DelStockByCode(string code, short operation)
        {
            List<Quotes> stocks = GetStocks();
            if (null == stocks || stocks.Count == 0)
            {
                return;
            }
            stocks.RemoveAll(item => item.Code == code && item.Operation == operation);
            SaveStocks(stocks);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DelStocksByCategory(short operation, short category)
        {
            List<Quotes> stocks = GetStocks();
            if (null == stocks || stocks.Count == 0)
            {
                return;
            }
            stocks.RemoveAll(
                item => item.StockCategory == category && item.Operation == operation);
            SaveStocks(stocks);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void ClearStocks()
        {
            SaveStocks(new List<Quotes>());
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void SaveStocks(List<Quotes> quotes)
        {
            try
            {
                if (!Directory.Exists(baseDir))
                {
                    Directory.CreateDirectory(baseDir);
                }
                using (StreamWriter file = File.CreateText(stocksFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, quotes);
                }
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void SaveAccounts(List<Account> accounts)
        {
            try
            {
                if (!Directory.Exists(baseDir))
                {
                    Directory.CreateDirectory(baseDir);
                }
                using (StreamWriter file = File.CreateText(acctsFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, accounts);
                }
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SaveSettings(Dictionary<string, string> settings)
        {
            try
            {
                if (!Directory.Exists(baseDir))
                {
                    Directory.CreateDirectory(baseDir);
                }
                using (StreamWriter file = File.CreateText(settingsFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, settings);
                }
                Settings = settings;
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SaveOpenBoardCnt(Dictionary<string, short> cntDic)
        {
            if (null == cntDic || cntDic.Count == 0)
            {
                return;
            }
            Dictionary<DateTime, Dictionary<string, short>> data
                = new Dictionary<DateTime, Dictionary<string, short>>();
            data.Add(DateTime.Now.Date, cntDic);
            try
            {
                if (!Directory.Exists(baseDir))
                {
                    Directory.CreateDirectory(baseDir);
                }
                using (StreamWriter file = File.CreateText(openBoardCntFilePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, data);
                }
            }
            catch (Exception e)
            {
                Logger.Exception(e);
            }
        }
    }
}
