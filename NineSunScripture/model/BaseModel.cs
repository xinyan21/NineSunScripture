using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace NineSunScripture.model
{
    /// <summary>
    /// 基础实体类
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// 【极其重要】
        /// 内存分配属于公共资源，并发分配内存会引发堆损坏，
        /// 这是一直莫名崩溃且找无法定位异常的罪魁祸首，分配内存的时候要把代码块锁定
        /// 而且这里要声明成静态的，这样所有子类都只有一个锁，不然每个子类都自己一个锁毫无意义
        /// </summary>
        private static ReaderWriterLockSlim rwls = new ReaderWriterLockSlim();

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
            rwls.EnterWriteLock();
            PtrResult = Marshal.AllocCoTaskMem(256);
            PtrErrorInfo = Marshal.AllocCoTaskMem(1024 * 1024);
            rwls.ExitWriteLock();
        }

        /// <summary>
        /// 释放结构体版本内存
        /// </summary>
        public void FreeCoTaskMem()
        {
            if (null != PtrResult)
            {
                Marshal.FreeCoTaskMem(PtrResult);
            }
            if (null != PtrErrorInfo)
            {
                Marshal.FreeCoTaskMem(PtrErrorInfo);
            }
        }
    }
}