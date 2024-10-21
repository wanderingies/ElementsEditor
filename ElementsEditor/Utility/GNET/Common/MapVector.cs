using GNET.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace GNET.Common
{
    public class MapVector<TKey, TValue> : Dictionary<TKey, TValue>, ICloneable, Marshal
    {
        public object Clone()
        {
            return MemberwiseClone();
        }

        public OctetsStream marshal(OctetsStream os)
        {
            throw new NotImplementedException();
        }

        public OctetsStream unmarshal(OctetsStream os)
        {
            throw new NotImplementedException();
        }
    }
}
