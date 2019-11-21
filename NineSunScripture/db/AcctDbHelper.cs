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

    class AcctDbHelper : DbHelper
    {
        private const String CreateAcctTable =
            "CREATE TABLE IF NOT EXISTS t_acct(id integer NOT NULL PRIMARY KEY AUTOINCREMENT " +
            "UNIQUE, brokerId integer,  brokerServerIp varchar(20), brokerServerPort varchar(6), " +
            "versionOfTHS varchar(10), acctType varchar(6), commPwd varchar(10), fundAcct varchar(10)," +
            "password varchar(6), shShareholderAcct varchar(10), szShareholderAcct varchar(10)," +
            "initTotalAsset inteter)";
        public AcctDbHelper()
        {
            SQLiteHelper.ExecuteNonQuery(CreateAcctTable);
        }
        public bool AddAccount(Account account)
        {
            string sql = "INSERT INTO t_acct(brokerId, brokerServerIp, brokerServerPort, versionOfTHS," +
                " acctType, commPwd, fundAcct, password) VALUES(@brokerId, @brokerServerIp, " +
                "@brokerServerPort, @versionOfTHS, @acctType, @commPwd, @fundAcct, @password)";
            //TODO 不知道param里面要不要传数据类型，有问题再加
            SQLiteParameter[] parameters = new SQLiteParameter[]{
             new SQLiteParameter("@brokerId",account.BrokerId),
           new SQLiteParameter("@brokerServerIp",account.BrokerServerIP),
           new SQLiteParameter("@brokerServerPort",account.BrokerServerPort),
           new SQLiteParameter("@versionOfTHS",account.VersionOfTHS),
           new SQLiteParameter("@acctType",account.AcctType),
           new SQLiteParameter("@commPwd",account.CommPwd),
           new SQLiteParameter("@fundAcct",account.FundAcct),
           new SQLiteParameter("@password",account.Password)};
            int cnt = SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return cnt == 1 ? true : false;
        }
        public void DelAccount(String fundAcct)
        {
            string sql = "DELETE FROM t_acct WHERE fundAcct=" + fundAcct;
            SQLiteHelper.ExecuteNonQuery(sql, null);
        }
        public bool EditAccount(Account account)
        {
            string sql = "UPDATE t_acct SET brokerId=@brokerId, brokerServerIp=@brokerServerIp," +
                " brokerServerPort=@brokerServerPort, versionOfTHS=@versionOfTHS, acctType=@acctType," +
                    "commPwd=@commPwd, fundAcct=@fundAcct, password=@password";
            SQLiteParameter[] parameters = new SQLiteParameter[]{
             new SQLiteParameter("@brokerId",account.BrokerId),
           new SQLiteParameter("@brokerServerIp",account.BrokerServerIP),
           new SQLiteParameter("@brokerServerPort",account.BrokerServerPort),
           new SQLiteParameter("@versionOfTHS",account.VersionOfTHS),
           new SQLiteParameter("@acctType",account.AcctType),
           new SQLiteParameter("@commPwd",account.CommPwd),
           new SQLiteParameter("@fundAcct",account.FundAcct),
           new SQLiteParameter("@password",account.Password)};
            int cnt = SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return cnt == 1 ? true : false;
        }

        /// <summary>
        /// 编辑初始总资产
        /// </summary>
        /// <param name="account">账户对象</param>
        /// <returns></returns>
        public bool EditInitTotalAsset(Account account)
        {
            if (null == account)
            {
                return false;
            }
            string sql = "UPDATE t_acct SET initTotalAsset=@initTotalAsset WHERE " +
                "fundAcct=@fundAcct";
            SQLiteParameter[] parameters = new SQLiteParameter[]{
           new SQLiteParameter("@initTotalAsset",account.InitTotalAsset),
            new SQLiteParameter("@fundAcct",account.FundAcct)};
            int cnt = SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return cnt == 1 ? true : false;
        }

        public List<Account> GetAccounts()
        {
            //TODO 不行就换下面的SQL
            string sql = "SELECT * FROM t_acct";
            //string sql = "SELECT brokerId, brokerServerIp, brokerServerPort, versionOfTHS, acctType," +
            //     "commPwd, macAddr, fundAcct, password FROM t_acct";
            DataTable dt = SQLiteHelper.GetDataTable(sql, null);
            List<Account> accounts = new List<Account>();
            if (dt.Rows.Count > 0)
            {
                Account acct;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    acct = DataRowToModel(dt.Rows[i]);
                    if (null != acct)
                    {
                        accounts.Add(acct);
                    }
                }
            }
            return accounts;
        }

        public Account DataRowToModel(DataRow row)
        {
            Account acct = new Account();
            if (null != row)
            {
                if (null != row["brokerId"])
                {
                    acct.BrokerId = int.Parse(row["brokerId"].ToString());
                }
                if (null != row["brokerServerIp"])
                {
                    acct.BrokerServerIP = row["brokerServerIp"].ToString();
                }
                if (null != row["brokerServerPort"])
                {
                    acct.BrokerServerPort = short.Parse(row["brokerServerPort"].ToString());
                }
                if (null != row["versionOfTHS"])
                {
                    acct.VersionOfTHS = row["versionOfTHS"].ToString();
                }
                if (null != row["acctType"])
                {
                    acct.AcctType = short.Parse(row["acctType"].ToString());
                }
                if (null != row["commPwd"])
                {
                    acct.CommPwd = row["commPwd"].ToString();
                }
                if (null != row["initTotalAsset"])
                {
                    acct.InitTotalAsset = int.Parse(row["initTotalAsset"].ToString());
                }
                if (null != row["fundAcct"])
                {
                    acct.FundAcct = row["fundAcct"].ToString();
                }
                if (null != row["password"])
                {
                    acct.Password = row["password"].ToString();
                }
            }
            return acct;
        }
    }
}
