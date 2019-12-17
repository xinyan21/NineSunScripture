using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace NineSunScripture.util.log
{
    class MultiThreadLogger
    {
        #region 私有变量
        /// <summary>
        /// 线程安全队列
        /// </summary>
        private static readonly ConcurrentQueue<LogMessage> _que;

        /// <summary>
        /// 信号
        /// </summary>
        private static readonly ManualResetEvent _mre;

        /// <summary>
        /// 日志写锁
        /// </summary>
        private static readonly ReaderWriterLockSlim _lock;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        static MultiThreadLogger()
        {
            _que = new ConcurrentQueue<LogMessage>();
            _mre = new ManualResetEvent(false);
            _lock = new ReaderWriterLockSlim();
            Task.Run(() => Initialize());
        }
        #endregion

        #region 信息日志
        /// <summary>
        /// 信息日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="args">字符串格式化参数</param>
        public static void Info(string message, params object[] args)
        {
            var sf = new StackTrace(true).GetFrame(1);
            var logMessage = new LogMessage
            {
                Level = LogLevel.Info,
                Message = string.Format(Regex.Replace(
                    message?.Replace("{", "{{").Replace("}", "}}") ?? "", @"{{(\d+)}}", "{$1}"), args),
                StackFrame = sf
            };
            _que.Enqueue(logMessage);
            _mre.Set();
        }
        #endregion

        #region 行情日志
        /// <summary>
        /// 行情日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="args">字符串格式化参数</param>
        public static void Quotes(string message, params object[] args)
        {
            var sf = new StackTrace(true).GetFrame(1);
            var logMessage = new LogMessage
            {
                Level = LogLevel.Quotes,
                Message = string.Format(Regex.Replace(
                    message?.Replace("{", "{{").Replace("}", "}}") ?? "", @"{{(\d+)}}", "{$1}"), args),
                StackFrame = sf
            };
            _que.Enqueue(logMessage);
            _mre.Set();
        }
        #endregion

        #region 错误日志
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="ex">错误Exception</param>
        /// <param name="message">自定义信息</param>
        /// <param name="args">字符串格式化参数</param>
        public static void Error(string message, params object[] args)
        {
            var sf = new StackTrace(true).GetFrame(1);
            var logMessage = new LogMessage
            {
                Level = LogLevel.Error,
                StackFrame = sf,
                Message = string.Format(Regex.Replace(
                    message?.Replace("{", "{{").Replace("}", "}}") ?? "", @"{{(\d+)}}", "{$1}"), args),
            };
            _que.Enqueue(logMessage);
            _mre.Set();
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="ex">错误Exception</param>
        /// <param name="message">自定义信息</param>
        /// <param name="args">字符串格式化参数</param>
        public static void Error(Exception ex, string message = "", params object[] args)
        {
            StackFrame sf = null;
            if (ex != null)
            {
                var frames = new StackTrace(ex, true).GetFrames();
                sf = frames?[frames.Length - 1];
            }
            else
            {
                sf = new StackTrace(true).GetFrame(1);
            }
            var logMessage = new LogMessage
            {
                Level = LogLevel.Error,
                Exception = ex,
                Message = string.Format(Regex.Replace(
                    message?.Replace("{", "{{").Replace("}", "}}") ?? "", @"{{(\d+)}}", "{$1}"), args),
                StackFrame = sf
            };
            _que.Enqueue(logMessage);
            _mre.Set();
        }
        #endregion

        #region 私有方法/实体
        #region 日志初始化
        /// <summary>
        /// 日志初始化
        /// </summary>
        private static void Initialize()
        {
            while (true)
            {
                //等待信号通知
                _mre.WaitOne();
                //写入日志
                Write();
                //重新设置信号
                _mre.Reset();
                Thread.Sleep(10);
            }
        }
        #endregion

        #region 写入日志
        /// <summary>
        /// 写入日志
        /// </summary>
        private static void Write()
        {
            //获取物理路径
            var infoDir = GetPhysicalPath(ConfigurationManager.AppSettings["logInfo"] ?? @"Logs\Info");
            var errorDir = GetPhysicalPath(ConfigurationManager.AppSettings["logError"] ?? @"Logs\Error");
            var quotesDir = GetPhysicalPath(@"Logs\Quotes");
            //根据当天日期创建日志文件
            var fileName = $"{DateTime.Now.ToString("yyyy-MM-dd")}.log";
            var infoPath = infoDir + fileName;
            var errorPath = errorDir + fileName;
            var quotesPath = quotesDir + fileName;
            try
            {
                //进入写锁
                _lock.EnterWriteLock();
                //判断目录是否存在，不存在则重新创建
                if (!Directory.Exists(infoDir)) Directory.CreateDirectory(infoDir);
                if (!Directory.Exists(errorDir)) Directory.CreateDirectory(errorDir);
                if (!Directory.Exists(quotesDir)) Directory.CreateDirectory(quotesDir);
                //创建StreamWriter
                StreamWriter swInfo = null;
                StreamWriter swError = null;
                StreamWriter swQuotes = null;
                if (_que?.ToList().Exists(o => o.Level == LogLevel.Info) == true)
                {
                    swInfo = new StreamWriter(infoPath, true, Encoding.UTF8);
                }
                if (_que?.ToList().Exists(o => o.Level == LogLevel.Error) == true)
                {
                    swError = new StreamWriter(errorPath, true, Encoding.UTF8);
                }
                if (_que?.ToList().Exists(o => o.Level == LogLevel.Quotes) == true)
                {
                    swQuotes = new StreamWriter(quotesPath, true, Encoding.UTF8);
                }
                //判断日志队列中是否有内容，从列队中获取内容，并删除列队中的内容
                while (_que?.Count > 0 && _que.TryDequeue(out LogMessage logMessage))
                {
                    var sf = logMessage.StackFrame;
                    //Quotes
                    if (swQuotes != null && logMessage.Level == LogLevel.Quotes)
                    {
                        string msg
                            = DateTime.Now.ToString("HH: mm: ss.ffff") + $"：{logMessage.Message}";
                        swQuotes.WriteLine(msg);
                    }
                    //Info
                    if (swInfo != null && logMessage.Level == LogLevel.Info)
                    {
                        string msg 
                            = DateTime.Now.ToString("HH: mm: ss.ffff") + $"：{logMessage.Message}";
                        swInfo.WriteLine(msg);
                        /*swInfo.WriteLine($"[级别：Info]");
                        swInfo.WriteLine($"[时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")}]");
                        swInfo.WriteLine($"[类名：{sf?.GetMethod().DeclaringType.FullName}]");
                        swInfo.WriteLine($"[方法：{sf?.GetMethod().Name}]");
                        swInfo.WriteLine($"[行号：{sf?.GetFileLineNumber()}]");
                        swInfo.WriteLine($"[内容：{logMessage.Message}]");
                        swInfo.WriteLine("------------------------------------------------------------------------------------------");*/
                        //swInfo.WriteLine(string.Empty);
                    }
                    //Error
                    if (swError != null && logMessage.Level == LogLevel.Error)
                    {
                        swError.WriteLine($"[级别：Error]");
                        swError.WriteLine($"[时间：{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")}]");
                        swError.WriteLine($"[类名：{sf?.GetMethod().DeclaringType.FullName}]");
                        swError.WriteLine($"[方法：{sf?.GetMethod().Name}]");
                        swError.WriteLine($"[行号：{sf?.GetFileLineNumber()}]");
                        if (!string.IsNullOrEmpty(logMessage.Message))
                        {
                            swError.WriteLine($"[内容：{logMessage.Message}]");
                        }
                        if (logMessage.Exception != null)
                        {
                            swError.WriteLine($"[异常：{logMessage.Exception.ToString()}]");
                        }
                        swError.WriteLine("------------------------------------------------------------------------------------------");
                        //swError.WriteLine(string.Empty);
                    }
                }
                //释放并关闭资源
                if (swInfo != null)
                {
                    swInfo.Close();
                    swInfo.Dispose();
                }
                if (swError != null)
                {
                    swError.Close();
                    swError.Dispose();
                }
                if (swQuotes != null)
                {
                    swQuotes.Close();
                    swQuotes.Dispose();
                }
            }
            finally
            {
                //退出写锁
                _lock.ExitWriteLock();
            }
        }
        #endregion

        #region 获取物理路径
        /// <summary>
        /// 获取物理路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GetPhysicalPath(string path)
        {
            var physicalPath = System.Environment.CurrentDirectory;
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Replace("~", "").Replace("/", @"\").TrimStart('\\').TrimEnd('\\');
                var start = path.LastIndexOf('\\') + 1;
                var length = path.Length - start;
                physicalPath = Path.Combine(
                    physicalPath, path.Substring(start, length).Contains(".") ? path : path + @"\");
            }
            return physicalPath;
        }
        #endregion

        #region 日志实体
        /// <summary>
        /// 日志级别
        /// </summary>
        private enum LogLevel
        {
            Info,
            Error,
            Quotes
        }

        /// <summary>
        /// 消息实体
        /// </summary>
        private class LogMessage
        {
            /// <summary>
            /// 日志级别
            /// </summary>
            public LogLevel Level { get; set; }

            /// <summary>
            /// 消息内容
            /// </summary>
            public string Message { get; set; }

            /// <summary>
            /// 异常对象
            /// </summary>
            public Exception Exception { get; set; }

            /// <summary>
            /// 堆栈帧信息
            /// </summary>
            public StackFrame StackFrame { get; set; }
        }
        #endregion
        #endregion
    }
}
