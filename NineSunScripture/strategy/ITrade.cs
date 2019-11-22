using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.strategy
{
    public interface ITrade
    {
        void OnTradeResult(int rspCode, String msg, String errInfo);
    }
}
