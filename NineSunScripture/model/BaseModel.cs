using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.model
{
    /// <summary>
    /// 基础实体类
    /// </summary>
    public class BaseModel
    {
        public int TradeSessionId;
        public byte[] Result;
        public byte[] ErrorInfo;

        /// <summary>
        /// 给Result和ErrorInfo分配内存
        /// </summary>
        public void AllocateResultMem()
        {
            Result = new byte[1024 * 1024];
            ErrorInfo = new byte[256];
        }
    }
}
