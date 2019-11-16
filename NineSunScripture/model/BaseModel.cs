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
        public int clientId;
        public byte[] result = new byte[1024 * 1024];
        public byte[] errorInfo = new byte[256];
    }
}
