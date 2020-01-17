using System;
using System.Runtime.InteropServices;

namespace NineSunScripture.model
{
    /// <summary>
    /// 基础实体类
    /// </summary>
    public class BaseModel
    {
        public int TradeSessionId;

        //字符串版本结果接收数组
        public byte[] Result;

        public byte[] ErrorInfo;

        //结构体版本结果接收数组，此内存要手动释放
        public IntPtr PtrResult;

        public IntPtr PtrErrorInfo;

        //为了方便管理内存释放，新增字符串错误信息存放字段
        public string StrErrorInfo;

        /// <summary>
        /// 给Result和ErrorInfo分配内存，此内存是托管内存，由垃圾回收器自动管理
        /// </summary>
        public void AllocateResultMem()
        {
            Result = new byte[1024 * 1024];
            ErrorInfo = new byte[256];
        }

        /// <summary>
        /// 分配结构体版本内存
        /// </summary>
        public void AllocCoTaskMem()
        {
            PtrResult = Marshal.AllocCoTaskMem(256);
            PtrErrorInfo = Marshal.AllocCoTaskMem(1024 * 1024);
        }

        /// <summary>
        /// 释放结构体版本内存
        /// </summary>
        public void FreeCoTaskMem()
        {
            Marshal.FreeCoTaskMem(PtrResult);
            Marshal.FreeCoTaskMem(PtrErrorInfo);
        }
    }
}