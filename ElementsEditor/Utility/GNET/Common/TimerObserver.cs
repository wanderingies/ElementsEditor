using System;
using System.Threading;

namespace GNET.Common
{
    public class TimerObserver : Observable
    {
        private static readonly TimerObserver instance = new TimerObserver();

        private Timer timer;
        private static long now = DateTime.Now.Millisecond;

        public static TimerObserver GetInstance()
        {
            return instance;
        }

        public TimerObserver()
        {
            timer = new Timer(
                (object state) =>
                    {
                        instance.changed = true;
                        instance.notifyObservers();
                    },
                    null,
                    0,
                    1
                );
        }

        public void StopTimer()
        {
            timer.Dispose();
        }

        public class WatchDog
        {
            private long t = now;
            public long GetTime() { return now; }
            public long Elapse() { return now - t; }
            public void Reset() { t = now; }
        }
    }
}
