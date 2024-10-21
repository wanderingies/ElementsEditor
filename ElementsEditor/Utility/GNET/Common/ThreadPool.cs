using System;
using System.Collections.Generic;
using System.Threading;

namespace GNET.Common
{
	public class ThreadPool
	{
		static LinkedList<Runnable> tasks = new LinkedList<Runnable>();
        static Object thread_count_locker = new Object();
		static int thread_count = 0;
		static int remove_count = 0;

		public static void AddThread (string value)
		{
            //Console.WriteLine("{0} thread={1}", value,thread_count + 1);
            new Thread(() => { ThreadPool.Run(); }).Start();
        }

		public static void RemoveThread ()
		{
            lock (thread_count_locker)
            {
                if (thread_count > remove_count)                                   
                    ++remove_count;
            }
		}

        public static int ThreadCount()
        {
            lock (thread_count_locker)
                return thread_count;
        }

		public static void ShutDown ()
		{
            lock (thread_count_locker)
                remove_count = thread_count;
		}

        public static void AddTask(Runnable r)
        {
            lock (tasks)
            {
                tasks.AddFirst(r);
                Monitor.Pulse(tasks);
            }
        }

        ThreadPool()
        {
            lock (thread_count_locker)
                ++thread_count;
        }

        public static void Run()
        {
            while (true)
            {
                try
                {
                    //bool bSleep = false;
                    Runnable r = null;

                    lock (tasks)
                    {
                        while (tasks.Count == 0)
                            Monitor.Wait(tasks);

                        r = tasks.Last.Value;
                        tasks.RemoveLast();

                        //bSleep = (tasks.Count == 0);
                    }

                    if (r != null)
                        r.run();

                    lock (thread_count_locker)
                    {
                        if (remove_count > 0)
                        {
                            --remove_count;
                            --thread_count;
                            return;
                        }
                    }

                    //if (bSleep)
                    //    Thread.Sleep(10);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
        }
	}
}
