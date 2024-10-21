using System;
using System.Collections.Generic;
using FlowGroup.Crypto;
using GNET.Common;

namespace GNET.Common.Security
{
    public sealed class MD5Hash : Security
    {
        private readonly MD5 md5 = MD5.Create();
        private Octets context = new Octets();

        internal MD5Hash()
        {
            type = 3;
        }

        public override object Clone()
        {
            MD5Hash m = new MD5Hash();
            m.context.replace(context);
            return m;
        }

        public override Octets Update(Octets o)
        {
            context.insert(context.size(), o);
            return o;
        }

        public override Octets Final(Octets digest)
        {
            digest.replace(md5.ComputeHash(context.getBytes()));
            context.clear();
            return digest;
        }

        public static Octets Digest(Octets o)
        {
            try
            {
                return new Octets(MD5.Create().ComputeHash(o.getBytes()));
            }
            catch (Exception) { }
            return new Octets();
        }
    }
}
