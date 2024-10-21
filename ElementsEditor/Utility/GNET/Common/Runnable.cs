using System;
using System.Collections.Generic;
using System.Threading;

namespace GNET.Common
{
    public abstract class Runnable
    {        
        public Runnable() {}        
        public abstract void run();
    }
}
