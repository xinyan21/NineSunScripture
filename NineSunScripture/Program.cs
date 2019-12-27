using NineSunScripture.util.log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NineSunScripture
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //处理未捕获的异常
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            //处理UI线程异常
            Application.ThreadException
                += new System.Threading.ThreadExceptionEventHandler(ApplicationThreadException);
            //处理非UI线程异常
            AppDomain.CurrentDomain.UnhandledException
                += new UnhandledExceptionEventHandler(UnhandledException);
            Application.Run(new MainForm());
        }

        static void UnhandledException(
            object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Log("【致命错误】发生未捕获的异常", LogType.Error);
            Logger.Log("【致命错误】IsTerminating = " + e.IsTerminating, LogType.Error);
            Logger.Log(e.ExceptionObject.ToString(), LogType.Error);
            Logger.Log("已经记录未捕获的异常，即将重启程序");
            //放到捕获事件的处理代码后，重启程序，需要时加上重启的参数
            CmdStartCTIProc(Application.ExecutablePath, "cmd params");
        }

        static void ApplicationThreadException(
            object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Logger.Log("【致命错误】发生UI线程异常", LogType.Error);
            Logger.Exception(e.Exception);
            Logger.Log("已经记录未处理异常，即将重启程序", LogType.Error);
            //放到捕获事件的处理代码后，重启程序，需要时加上重启的参数
            CmdStartCTIProc(Application.ExecutablePath, "cmd params");
        }

        /// <summary>
        /// 在命令行窗口中执行
        /// </summary>
        /// <param name="sExePath"></param>
        /// <param name="sArguments"></param>
        static void CmdStartCTIProc(string sExePath, string sArguments)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            p.Start();
            p.StandardInput.WriteLine(sExePath + " " + sArguments);
            p.StandardInput.WriteLine("exit");
            p.Close();

            System.Threading.Thread.Sleep(2000);//必须等待，否则重启的程序还未启动完成；根据情况调整等待时间
        }
    }
}
