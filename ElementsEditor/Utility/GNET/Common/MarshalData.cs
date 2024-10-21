using System;
using System.Collections.Generic;
using GNET.Common;

namespace GNET.Common
{
    public abstract partial class MarshalData : ICloneable, Marshal	
	{		
		public abstract Object Clone ();
		public abstract OctetsStream marshal (OctetsStream os);
		public abstract OctetsStream unmarshal (OctetsStream os);
	}
}
