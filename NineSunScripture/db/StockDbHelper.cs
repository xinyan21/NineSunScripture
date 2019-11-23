using NineSunScripture.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.db
{
    class StockDbHelper
    {
        private const String CreateAcctTable =
            "CREATE TABLE IF NOT EXISTS t_stock(id integer NOT NULL PRIMARY KEY AUTOINCREMENT " +
            "UNIQUE, code char(6), name char(4), category integer, positionCtrl char(4), moneyCtrl  integer)";
        public StockDbHelper()
        {
            SQLiteHelper.ExecuteNonQuery(CreateAcctTable);
        }
        public bool AddStock(Quotes quotes)
        {
            string sql = "INSERT INTO t_stock(code, name, category, positionCtrl, moneyCtrl)" +
                " VALUES(@code, @name, @category, @positionCtrl, @moneyCtrl)";
            SQLiteParameter[] parameters = new SQLiteParameter[]{
               new SQLiteParameter("@code", quotes.Code),
               new SQLiteParameter("@name", quotes.Name),
               new SQLiteParameter("@category", quotes.StockCategory),
               new SQLiteParameter("@positionCtrl", quotes.PositionCtrl),
               new SQLiteParameter("@moneyCtrl", quotes.MoneyCtrl)};
            int cnt = SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return cnt == 1 ? true : false;
        }
        /// <summary>
        /// 根据分类和代码删除某个股票池的股票
        /// </summary>
        /// <param name="category">分类</param>
        /// <param name="code">股票代码</param>
        /// <returns></returns>
        public bool DelStockBy(short category, String code)
        {
            string sql = "DELETE FROM t_stock WHERE code=@code AND category=@category";
            SQLiteParameter[] parameters = new SQLiteParameter[]{
            new SQLiteParameter("@code",code),
            new SQLiteParameter("@category",category)};
            int cnt = SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return cnt == 1 ? true : false;
        }
        /// <summary>
        /// 删除一个分类的股票池
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public bool DelAllBy(short category)
        {
            string sql = "DELETE FROM t_stock WHERE category=@category";
            SQLiteParameter[] parameters = new SQLiteParameter[]{
             new SQLiteParameter("@category",category) };
            int cnt = SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return cnt > 0 ? true : false;
        }

        /// <summary>
        /// 获取category分类的股票池
        /// </summary>
        /// <param name="category">分类码</param>
        /// <returns></returns>
        public List<Quotes> GetStocksBy(short category)
        {
            string sql = "SELECT * FROM t_stock WHERE category=@category";
            SQLiteParameter[] parameters = new SQLiteParameter[]{
             new SQLiteParameter("@category",category) };
            DataTable dt = SQLiteHelper.GetDataTable(sql, parameters);
            List<Quotes> quotes = new List<Quotes>();
            if (dt.Rows.Count > 0)
            {
                Quotes quote;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    quote = DataRowToModel(dt.Rows[i]);
                    if (null != quote)
                    {
                        quotes.Add(quote);
                    }
                }
            }
            return quotes;
        }


        public Quotes DataRowToModel(DataRow row)
        {
            Quotes quotes = new Quotes();
            if (null != row)
            {
                if (null != row["code"])
                {
                    quotes.Code = row["code"].ToString();
                }
                if (null != row["name"])
                {
                    quotes.Name = row["name"].ToString();
                }
                if (null != row["category"])
                {
                    quotes.StockCategory = short.Parse(row["category"].ToString());
                }
                if (null != row["positionCtrl"])
                {
                    quotes.PositionCtrl = float.Parse(row["positionCtrl"].ToString());
                }
                if (null != row["moneyCtrl"])
                {
                    quotes.MoneyCtrl = int.Parse(row["moneyCtrl"].ToString());
                }
            }


                    return quotes;
        }
    }
}
