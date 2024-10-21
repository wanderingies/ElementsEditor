using System;
using System.Collections.Generic;
using GNET.Common;
using System.Collections;

namespace GNET.Common
{
    public class ByteVector: ArrayList, ICloneable, Marshal
	{                
        public ByteVector()
        {     
        }

        public OctetsStream marshal(OctetsStream os)
        {
            object[] arry = ToArray();
            os.compact_uint32((uint)arry.Length);
            for (int i = 0; i < arry.Length; i++)
            {
                os.marshal((byte)arry[i]);
            }             
            return os;
        }

        public OctetsStream unmarshal(OctetsStream os)
        {
            int size = (int)os.uncompact_uint32();
            for (int i = 0; i < size; i++)
            {
                Add(os.unmarshal_byte());
            }
            return os;
        } 
	}
}
