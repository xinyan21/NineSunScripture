using NineSunScripture.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace NineSunScripture.trade.helper
{
    class AccountHelper
    {
        //AppDomain.CurrentDomain.BaseDirectory
        public static void Login()
        {

        }
        public static bool checkIfPositionHas(Account account, string stock)
        {
            return true;
        }

        /// <summary>
        /// 获取stock的持仓信息
        /// </summary>
        /// <param name="positions">账户所有持仓</param>
        /// <param name="stock">股票代码</param>
        /// <returns>stock的持仓对象</returns>
        public static Position GetPositionOf(List<Position> positions, string stock) {
            foreach (Position position in positions)
            {
                if (position.Code==stock)
                {
                    return position;
                }
            }
            return null;        
        }

        public static Position GetPositionOf(List<Account> accounts, string stock) {
            Position position = null;
            foreach (Account account in accounts)
            {
                position = GetPositionOf(account.Positions,stock);
                if (null!=position)
                {
                    return position;
                }
            }
            return position;
        }
    }
}
