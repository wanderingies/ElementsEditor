using System;
using System.Collections.Generic;

namespace GNET.Common
{
    public interface Marshal
    {
        OctetsStream marshal(OctetsStream os);
	    OctetsStream unmarshal(OctetsStream os);
    }
}
