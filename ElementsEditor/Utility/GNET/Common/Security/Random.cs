using System;
using System.Collections.Generic;
using GNET.Common;

namespace GNET.Common.Security
{
    public sealed class Random : Security
    {
        private static System.Random r = new System.Random();

        internal Random() { type = 0; }

        public override Octets Update(Octets o)
        {
            r.NextBytes(o.Buffer());
            return o;
        }

        public override Object Clone()
        {
            return this;
        }
    }
}
