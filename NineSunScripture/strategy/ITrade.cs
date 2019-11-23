using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.strategy
{
    public interface ITrade
    {
        /// <summary>
        /// 交易结果回调
        /// </summary>
        /// <param name="rspCode">响应码</param>
        /// <param name="msg">成功消息</param>
        /// <param name="errInfo">错误消息</param>
        void OnTradeResult(int rspCode, String msg, String errInfo);
    }
}
