using System;
using System.Collections.Generic;


namespace GNET.Common
{
    public class WatchDog
    {
        private long t = 0;
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public WatchDog() { t = GetTime(); }

        public static long GetTime() { return (long)(DateTime.UtcNow - epoch).TotalSeconds; }
        public long Elapse() { return GetTime() - t; }
        public void Reset() { t = GetTime(); }
    }
}
