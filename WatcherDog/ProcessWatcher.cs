using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
namespace WatchDog
{
    class ProcessWatcher
    {
        /// <summary>
        /// 进程列表
        /// </summary>
        public List<MyProcess> myProcessList;
        private Thread watchThread;
        private int watchWaitingTime = 3;


        /// <summary>
        /// 初始化
        /// -1 process.mj 文件不存在
        /// 0 看护进程不存在
        /// 1 成功
        /// 2 部分进程路径不正确
        /// </summary>
        /// <returns></returns>
        public int init()
        {
            if (!System.IO.File.Exists("process.txt")) return -1;
            string[] processPath = System.IO.File.ReadAllLines("process.txt", Encoding.Default);
            int count = 0;
            myProcessList = new List<MyProcess>();
            foreach (string path in processPath)
            {
                if (System.IO.File.Exists(@path))
                {
                    count++;
                    MyProcess mp = new MyProcess(@path);
                    myProcessList.Add(mp);
                }
            }
            if (count == 0) return 0;
            if (count == processPath.Length) return 1;
            return 2;
        }
        /// <summary>
        /// 启动守护
        /// 
        /// </summary>
        /// <param name="sleepTime">等待时间</param>
        /// <returns></returns>
        public int watchStart(int sleepTime)
        {
            watchWaitingTime = sleepTime;
            watchStop();
            watchThread = new Thread(watch);
            watchThread.Start();
            return 0;
        }
        /// <summary>
        /// 关闭守护
        /// </summary>
        /// <returns></returns>
        public int watchStop()
        {
            try
            {
                watchThread.Abort();
            }
            catch
            { }
            return 0;
        }
        /// <summary>
        /// 守护线程，死循环
        /// </summary>
        private void watch()
        {
            while (true)
            {
                if (myProcessList == null) return;
                if (myProcessList.Count < 1) return;
                foreach (MyProcess mp in myProcessList)
                {
                    if (!mp.isAlive())
                    {
                        //Thread.Sleep(1000);
                        //if (!mp.isAlive()) mp.start();
                        mp.start();
                    }
                }
                Thread.Sleep(watchWaitingTime * 1000);
            }
        }
        /// <summary>
        /// 全部重启，如果已经启动则先关闭
        /// </summary>
        public void startAll()
        {
            if (myProcessList == null) return;
            if (myProcessList.Count < 1) return;
            foreach (MyProcess mp in myProcessList)
            {
                if (!mp.isAlive()) mp.start();
            }
        }
        /// <summary>
        /// 关闭所有守护进程
        /// </summary>
        public void stopAll()
        {
            if (myProcessList == null) return;
            if (myProcessList.Count < 1) return;
            foreach (MyProcess mp in myProcessList)
            {
                mp.stop();
            }
        }
        /// <summary>
        /// 进程状态
        /// 1 显示界面
        /// 2 影藏界面
        /// 3 重启
        /// 4 停止
        /// </summary>
        /// <param name="state"></param>
        /// <param name="name"></param>
        public void setProcessState(int state, string name)
        {
            foreach (ProcessWatcher.MyProcess p in myProcessList)
            {
                if (p.Name == name)
                {
                    switch (state)
                    {
                        case 3:
                            p.start();
                            break;
                        case 4:
                            p.stop();
                            break;
                    }
                    break;
                }
            }
        }
        /// <summary>
        /// 判断某个线程是否存在
        /// </summary>
        /// <param name="name">线程名字</param>
        /// <returns></returns>
        public Boolean processIsAlive(string name)
        {
            if (myProcessList == null) return false;
            if (myProcessList.Count < 1) return false;
            foreach (MyProcess mp in myProcessList)
            {
                if (mp.Name == name)
                {
                    return mp.isAlive();
                }
            }
            return false;
        }

        public class MyProcess
        {

            private string name;
            private string path;
            private IntPtr ptrHide;
            //private Process process;
            public MyProcess(string path)
            {
                string[] s = path.Split('\\');
                this.name = s[s.Length - 1];
                this.name = name.Substring(0, name.Length - 4);
                this.path = path;
                ptrHide = IntPtr.Zero;
            }
            /// <summary>
            /// 进程名字
            /// </summary>
            public string Name
            {
                get { return name; }
            }
            /// <summary>
            /// 进程路径
            /// </summary>
            public string Path
            {
                get { return path; }
            }
            /// <summary>
            /// 进程状态
            /// </summary>
            /// <returns></returns>
            public Boolean isAlive()
            {
                Process p = process();
                if (p == null) return false;
                if (p.Responding == true) return true;
                return !p.HasExited;
                //try
                //{
                //    return process().Responding;
                //}
                //catch
                //{
                //    return false;
                //}
            }
            /// <summary>
            /// 启动，如果已经启动，则关闭后再启动
            /// </summary>
            public void start()
            {
                stop();
                Thread.Sleep(500);
                Process.Start(path);
                writeLog("启动程序" + name);
            }
            /// <summary>
            /// 关闭
            /// </summary>
            public void stop()
            {
                try
                {
                    process().Kill();
                    writeLog("关闭程序" + name);
                }
                catch
                { }
            }
            /// <summary>
            /// 获取进程
            /// </summary>
            /// <returns></returns>
            private Process process()
            {
                Process[] proc = Process.GetProcessesByName(name);
                if (proc.Length > 0)
                {
                    return proc[0];
                }
                return null;
            }
            /// <summary>
            /// 锁
            /// </summary>
            readonly static ReaderWriterLockSlim _rw = new ReaderWriterLockSlim();
            /// <summary>
            /// 写日志文件
            /// </summary>
            /// <param name="str_msg"></param>
            public static void writeLog(string str_msg)
            {
                if (System.IO.Directory.Exists("logW") == false) System.IO.Directory.CreateDirectory("logW");
                string msg = DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + ":" + str_msg + "\r\n";
                _rw.EnterWriteLock();
                System.IO.File.AppendAllText("logW\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", msg);
                _rw.ExitWriteLock();
                string[] s = System.IO.Directory.GetFiles("logW");
                if (s.Length > 7)
                {
                    System.IO.File.Delete(s[0]);
                }
            }
        }
    }
}