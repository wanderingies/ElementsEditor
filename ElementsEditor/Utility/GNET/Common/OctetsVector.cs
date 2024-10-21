using System;
using System.Collections.Generic;
using GNET.Common;
using System.Collections;

namespace GNET.Common
{
    public class OctetsVector: ArrayList, ICloneable, Marshal
	{
        public OctetsVector()
        {     
        }

        public override Object Clone()
        {
            try
            {
                OctetsVector v = (OctetsVector)base.Clone();
                return v;
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
                os.marshal((Octets)arry[i]);
            }             
            return os;
        }

        public OctetsStream unmarshal(OctetsStream os)
        {
            int size = (int)os.uncompact_uint32();
            for (int i = 0; i < size; i++)
            {
                Add(os.unmarshal_Octets());
            }
            return os;
        } 
	}
}
