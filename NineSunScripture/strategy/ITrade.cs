using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.strategy
{
    public interface ITrade
    {
        void OnTradeResult(int code, String msg);
    }
}
