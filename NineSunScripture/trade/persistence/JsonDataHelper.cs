using Newtonsoft.Json;
using NineSunScripture.model;
using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.trade.persistence
{
    /// <summary>
    /// 将账户和股票持久化到json文件，方便使用
    /// </summary>
    public static class JsonDataHelper
    {
        private static string baseDir = Environment.CurrentDirectory + @"\data\";
        private static string acctsFilePath = baseDir + "accounts.json";
        private static string stocksFilePath = baseDir + "stocks.json";

        /// <summary>
        /// 获取所有账号，账号由人工编辑json文件，方便快捷
        /// </summary>
        /// <returns></returns>
        public static List<Account> GetAccounts()
        {
            List<Account> accounts = null;
            try
            {
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

        public static void InitTotalAsset(Account account)
        {
            List<Account> accounts = GetAccounts();
            if (null == accounts || null == account)
            {
                return;
            }
            Account temp = accounts.Find(item => account.FundAcct == item.FundAcct);
            if (null == temp)
            {
                Logger.Log("Set account init total asset  failed, cannot found this account. " + account);
                return;
            }
            temp.InitTotalAsset = account.InitTotalAsset;
            SaveAccounts(accounts);
        }

        /// <summary>
        /// 获取股票池所有股票
        /// </summary>
        /// <returns></returns>
        public static List<Quotes> GetStocks()
        {
            List<Quotes> stocks = null;
            try
            {
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

        public static List<Quotes> GetStocksByOperation(short operation)
        {
            List<Quotes> stocks = GetStocks();
            if (null == stocks || stocks.Count == 0)
            {
                return null;
            }
            List<Quotes> result = stocks.FindAll(temp => temp.Operation == operation);
            return result;
        }

        public static List<Quotes> GetStocksByCatgory(short operation, short category)
        {
            List<Quotes> stocks = GetStocks();
            if (null == stocks || stocks.Count == 0)
            {
                return null;
            }
            List<Quotes> result = stocks.FindAll(
                temp => temp.StockCategory == category && temp.Operation == operation);
            return result;
        }

        /// <summary>
        /// 增加股票quotes到股票池
        /// </summary>
        /// <param name="quotes">个股</param>
        public static void AddStock(Quotes quotes)
        {
            List<Quotes> stocks = GetStocks();
            if (null == stocks)
            {
                stocks = new List<Quotes>();
            }
            if (!stocks.Exists(
                temp => temp.Code == quotes.Code && temp.Operation == quotes.Operation &&
                 temp.StockCategory == quotes.StockCategory))
            {
                stocks.Add(quotes);
            }
            SaveStocks(stocks);
        }

        public static void DelStockByCode(string code, short operation)
        {
            List<Quotes> stocks = GetStocks();
            if (null == stocks || stocks.Count == 0)
            {
                return;
            }
            stocks.RemoveAll(temp => temp.Code == code && temp.Operation == operation);
            SaveStocks(stocks);
        }

        public static void DelStocksByCategory(short operation, short category)
        {
            List<Quotes> stocks = GetStocks();
            if (null == stocks || stocks.Count == 0)
            {
                return;
            }
            stocks.RemoveAll(
                temp => temp.StockCategory == category && temp.Operation == operation);
            SaveStocks(stocks);
        }

        public static void ClearStocks()
        {
            SaveStocks(new List<Quotes>());
        }

        private static void SaveStocks(List<Quotes> quotes)
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

        private static void SaveAccounts(List<Account> accounts)
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
    }
}
