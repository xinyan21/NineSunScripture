using System;
using System.Threading;
using System.Threading.Tasks;
using WatchDog;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            /* string test = "603876	1.000	6.410	0.000	3911378300.000	16.600	0.000	0.000	0.000	0	0.000	17.160	14.040	0.000	25.819	24.667	7138000000.000";
             string[] arrTest = test.Split("\t");
             DateTime start = DateTime.Now;
             Console.WriteLine(start.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
             Thread.Sleep(2000);
             Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
             Console.WriteLine(DateTime.Now.Subtract(start).Milliseconds+"--"+ DateTime.Now.Subtract(start).TotalMilliseconds
                 +"--"+ DateTime.Now.Subtract(start).TotalSeconds);
             //Console.WriteLine(((int)345.2 / 1000) *10);
             */
            /*ProcessWatcher watcher=new ProcessWatcher();
            int code = watcher.init();
            Console.WriteLine("Init watcher, code is " + code);
            watcher.watchStart(1);*/
            ClassTest classTest = new ClassTest();
            Console.WriteLine(classTest.dbl);
            ThreadPool.SetMaxThreads(10, 10);
            Console.Read();
            Task[] tasks = new Task[10];
            for (int j = 0; j < 10; j++)
            {
                int temp = j;
                Thread.Sleep(2);
                tasks[j] = Task.Run(() => Test(temp));
            }
            Task.WaitAll(tasks);
        }
        private static void Test(int i)
        {
            try
            {
                Console.WriteLine($"Task.Run value {i}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Task.Run异常: " + e.Message);
            }
        }

    }
    class ClassTest
    {
        public double dbl;
    }
}
