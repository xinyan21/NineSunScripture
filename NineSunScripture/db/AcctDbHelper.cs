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
            "CREATE TABLE IF NOT EXISTS t_acct(id integer NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
            "brokerId integer,  brokerServerIp varchar(20), brokerServerPort varchar(6), versionOfTHS varchar(10)," +
            "acctType varchar(6), commPwd varchar(10), macAddr varchar(20), fundAcct varchar(10)," +
            "password varchar(6), shShareholderAcct varchar(10), szShareholderAcct varchar(10))";
        public AcctDbHelper()
        {
            SQLiteHelper.ExecuteNonQuery(CreateAcctTable);
        }
        public bool AddAccount(Account account)
        {
            string sql = "INSERT INTO t_acct(brokerId, brokerServerIp, brokerServerPort, versionOfTHS, acctType," +
                "commPwd, macAddr, fundAcct, password) VALUES(@brokerId, @brokerServerIp, @brokerServerPort, " +
                "@versionOfTHS, @acctType, @commPwd, @macAddr, @fundAcct, @password)";
            SQLiteParameter[] parameters = new SQLiteParameter[]{
             new SQLiteParameter("@brokerId",account.BrokerId),
           new SQLiteParameter("@brokerServerIp",account.BrokerServerIP),
           new SQLiteParameter("@brokerServerPort",account.BrokerServerPort),
           new SQLiteParameter("@versionOfTHS",account.VersionOfTHS),
           new SQLiteParameter("@acctType",account.AcctType),
           new SQLiteParameter("@commPwd",account.CommPwd),
            new SQLiteParameter("@macAddr",account.MacAddr),
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
                    "commPwd=@commPwd, macAddr=@macAddr, fundAcct=@fundAcct, password=@password";
            SQLiteParameter[] parameters = new SQLiteParameter[]{
             new SQLiteParameter("@brokerId",account.BrokerId),
           new SQLiteParameter("@brokerServerIp",account.BrokerServerIP),
           new SQLiteParameter("@brokerServerPort",account.BrokerServerPort),
           new SQLiteParameter("@versionOfTHS",account.VersionOfTHS),
           new SQLiteParameter("@acctType",account.AcctType),
           new SQLiteParameter("@commPwd",account.CommPwd),
            new SQLiteParameter("@macAddr",account.MacAddr),
           new SQLiteParameter("@fundAcct",account.FundAcct),
           new SQLiteParameter("@password",account.Password)};
            int cnt = SQLiteHelper.ExecuteNonQuery(sql, parameters);
            return cnt == 1 ? true : false;
        }

        public List<Account> GetAccounts()
        {
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
                    acct.BrokerId = int.Parse(row["brokerServerIp"].ToString());
                }
                if (null != row["brokerServerPort"])
                {
                    acct.BrokerId = int.Parse(row["brokerServerPort"].ToString());
                }
                if (null != row["versionOfTHS"])
                {
                    acct.BrokerId = int.Parse(row["versionOfTHS"].ToString());
                }
                if (null != row["acctType"])
                {
                    acct.BrokerId = int.Parse(row["acctType"].ToString());
                }
                if (null != row["commPwd"])
                {
                    acct.BrokerId = int.Parse(row["commPwd"].ToString());
                }
                if (null != row["macAddr"])
                {
                    acct.BrokerId = int.Parse(row["macAddr"].ToString());
                }
                if (null != row["fundAcct"])
                {
                    acct.BrokerId = int.Parse(row["fundAcct"].ToString());
                }
                if (null != row["password"])
                {
                    acct.BrokerId = int.Parse(row["password"].ToString());
                }
            }
            return acct;
        }
    }
}
