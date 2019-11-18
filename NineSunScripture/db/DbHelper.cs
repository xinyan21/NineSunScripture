using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.db
{
    class DbHelper
    {
        private const String DbFileName = "NineSunScriptrue.db";
        public DbHelper()
        {
            SQLiteHelper.CreateDBFile(DbFileName);
        }
    }
}
