﻿namespace NineSunScripture.strategy
{
    public interface ITrade
    {
        /// <summary>
        /// 交易结果回调
        /// </summary>
        /// <param name="rspCode">响应码</param>
        /// <param name="msg">成功消息</param>
        /// <param name="errInfo">错误消息</param>
        /// <param name="needReboot">如果调用失败是否重启策略</param>
        void OnTradeResult(int rspCode, string msg, string errInfo, bool needReboot);
    }
}