using System;

namespace GNET.Common
{
    public interface Observer
    {
        void update(Observable o, object arg);
    }
}
