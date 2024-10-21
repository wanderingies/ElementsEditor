using GNET.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GNET.Common
{
    public class UIntVector : ArrayList, ICloneable, Marshal
    {
        public UIntVector()
        { }

        public OctetsStream marshal(OctetsStream os)
        {
            Object[] arry = ToArray();
            os.compact_uint32((uint)arry.Length);
            for (int i = 0; i < arry.Length; i++)
            {
                os.marshal((uint)arry[i]);
            }
            return os;
        }

        public OctetsStream unmarshal(OctetsStream os)
        {
            int size = (int)os.uncompact_uint32();
            for (int i = 0; i < size; i++)
            {
                Add(os.unmarshal_uint());
            }
            return os;
        }
    }
}
