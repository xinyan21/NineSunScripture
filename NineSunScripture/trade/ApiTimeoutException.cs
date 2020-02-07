using System;

namespace NineSunScripture.trade
{
    /// <summary>
    /// 交易接口超时异常
    /// </summary>
    [Serializable]
    public class ApiTimeoutException : Exception
    {
        public ApiTimeoutException(string message) : base(message)
        {
        }

        public ApiTimeoutException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ApiTimeoutException()
        {
        }

        protected ApiTimeoutException(
            System.Runtime.Serialization.SerializationInfo serializationInfo, 
            System.Runtime.Serialization.StreamingContext streamingContext)
        {
            throw new NotImplementedException();
        }
    }
}
