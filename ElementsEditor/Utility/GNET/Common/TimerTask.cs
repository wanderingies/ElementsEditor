using System;
using System.Collections.Generic;
using System.Text;

namespace GNET.Common
{
    public class TimerTask : Observer
    {
        private static TimerTask instance = new TimerTask();
        private SortedDictionary<long, LinkedList<Runnable>> tasks
            = new SortedDictionary<long, LinkedList<Runnable>>();

        private long elapse = 0;
        private Object ilock = new Object();

        private TimerTask()
        {
            TimerObserver.GetInstance().addObserver(this);
        }

        public void update(Observable o, Object arg)
        {
            lock (ilock)
            {
                ++elapse;
                int size = tasks.Count;
                long[] keys = new long[size];
                tasks.Keys.CopyTo(keys, 0);
                
                foreach (long exectime in keys)
                {
                    if (exectime > elapse)
                        break;
                    LinkedList<Runnable> ll = tasks[exectime];
                    foreach (Runnable t in ll)
                    {
                        ThreadPool.AddTask(t);
                    }
                    ll.Clear();
                    tasks.Remove(exectime);
                }
            }
        }

        public void AddTask(Runnable task, long waitsecds)
        {
            lock (ilock)
            {
                long timestamp = waitsecds + elapse;
                LinkedList<Runnable> tasklist;
                if (tasks.TryGetValue(timestamp, out tasklist))
                {
                    tasklist.AddLast(task);
                }
                else
                {
                    tasklist = new LinkedList<Runnable>();
                    tasklist.AddLast(task);
                    tasks.Add(timestamp, tasklist);
                }
            }
        }

        public static void AddTimerTask(Runnable task, long waitsecds)
        {
            instance.AddTask(task, waitsecds);
        }
    }
}
