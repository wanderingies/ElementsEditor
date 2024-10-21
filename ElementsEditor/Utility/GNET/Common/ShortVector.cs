using System;
using System.Collections.Generic;
using GNET.Common;
using System.Collections;

namespace GNET.Common
{
    public class ShortVector: ArrayList, ICloneable, Marshal
	{
        public ShortVector()
        {     
        }

        public override Object Clone()
        {
            try
            {
                ShortVector obj = new ShortVector();
                foreach (short ele in this)
                {
                    obj.Add(ele);
                }
                return obj;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public OctetsStream marshal(OctetsStream os)
        {
            Object[] arry = ToArray();
            os.compact_uint32((uint)arry.Length);
            for (int i = 0; i < arry.Length; i++)
            {
                os.marshal((short)arry[i]);
            }             
            return os;
        }

        public OctetsStream unmarshal(OctetsStream os)
        {
            int size = (int)os.uncompact_uint32();
            for (int i = 0; i < size; i++)
            {
                Add(os.unmarshal_short());
            }
            return os;
        } 
	}
}
