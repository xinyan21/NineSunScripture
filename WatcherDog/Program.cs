using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using WatchDog;

namespace WatcherDog
{
    class Program
    {
        public  List<string> testList=new List<string>();
        private ReaderWriterLockSlim lockSlim=new ReaderWriterLockSlim();
        static void Main(string[] args)
        {
            /*ProcessWatcher watcher = new ProcessWatcher();
            int code = watcher.init();
            Console.WriteLine("Init watcher, code is " + code);
            watcher.watchStart(3);*/
            // Process.Start(@"D:\NineSun\NineSunScripture.exe");
            Program program = new Program();
            program.testList.Add("a");
            program.testList.Add("b");
            program.testList.Add("c");
            program.testList.Add("d");
            for (int i = 0; i < 1000; i++)
            {
                program.testList.Add("d"+i);
            }
            new Thread(program.TestRead).Start();
            new Thread(program.TestWrite).Start();
        }

        private void TestRead()
        {
            while (true)
            {
                lockSlim.EnterReadLock();
                for (int i = 0; i < testList.Count; i++)
                {
                    Console.WriteLine(testList[i]);
                }
                lockSlim.ExitReadLock();
                Thread.Sleep(1000);
            }
        }

        private void TestWrite()
        {
            Thread.Sleep(11);
            lockSlim.EnterWriteLock();
            testList.Clear();
            testList.Add("c");
            testList.Add("d");
            lockSlim.ExitWriteLock();
        }
    }
}
