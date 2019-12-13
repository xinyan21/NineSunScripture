using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NineSunScripture.util.log
{
    public class Logger
    {
        #region Instance
        private static object logLock;
        private static Logger _instance;
        private static string logFileName;
        private static string excFileName;
        private static string quotesFileName;
        private Logger() { }

        /// <summary>
        /// Logger instance
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Logger();
                    logLock = new object();
                    logFileName = "debug.log";
                    excFileName = "exception.log";
                    quotesFileName = "quotes.log";
                }
                return _instance;
            }
        }
        #endregion

        /// <summary>
        /// Write log to log file
        /// </summary>
        /// <param name="logContent">Log content</param>
        /// <param name="logType">Log type</param>
        public static void Log(string logContent, LogType logType = LogType.Info)
        {
            if (LogType.Error==logType)
            {
                MultiThreadLogger.Error(logContent);
            }
            else
            {
                MultiThreadLogger.Info(logContent);
            }
        }

        /// <summary>
        /// Write exception to log file
        /// </summary>
        /// <param name="exception">Exception</param>
        public static void Exception(Exception exception, string specialText = null)
        {
            MultiThreadLogger.Error(exception, specialText);
        }

        /// <summary>
        /// Write log to log file
        /// </summary>
        /// <param name="logContent">Log content</param>
        /// <param name="logType">Log type</param>
        private void WriteLog(string logContent, LogType logType = LogType.Info)
        {
            try
            {
                string basePath = System.Environment.CurrentDirectory;
                if (!Directory.Exists(basePath + "\\Log"))
                {
                    Directory.CreateDirectory(basePath + "\\Log");
                }

                string dataString = DateTime.Now.ToString("yyyy-MM-dd");
                if (!Directory.Exists(basePath + "\\Log\\" + dataString))
                {
                    Directory.CreateDirectory(basePath + "\\Log\\" + dataString);
                }

                string[] logText = new string[] { DateTime.Now.ToString("hh:mm:ss") + ": "
                    + logType.ToString() + ": " + logContent };

                string fileName = logFileName;
                if (logType == LogType.Error)
                {
                    fileName = excFileName;
                }
                else if (logType == LogType.Quotes)
                {
                    fileName = quotesFileName;
                }
                lock (logLock)
                {
                    File.AppendAllLines(basePath + "\\Log\\" + dataString + "\\" + fileName, logText);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Write exception to log file
        /// </summary>
        /// <param name="exception">Exception</param>
        private void WriteException(Exception exception, string specialText = null)
        {
            if (exception != null)
            {
                Type exceptionType = exception.GetType();
                string text = string.Empty;
                if (!string.IsNullOrEmpty(specialText))
                {
                    text = text + specialText + Environment.NewLine;
                }
                text = "Exception: " + exceptionType.Name + Environment.NewLine;
                text += "               " + "Message: " + exception.Message + Environment.NewLine;
                text += "               " + "Source: " + exception.Source + Environment.NewLine;
                text += "               " + "StackTrace: " + exception.StackTrace + Environment.NewLine;
                WriteLog(text, LogType.Error);
            }
        }
    }
    public enum LogType
    {
        All,
        Info,
        Debug,
        Success,
        Failure,
        Warning,
        Error,
        Quotes
    }
}
