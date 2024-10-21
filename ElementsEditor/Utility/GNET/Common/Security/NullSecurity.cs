using System;
using System.Collections.Generic;
using GNET.Common;

namespace GNET.Common.Security
{
    public sealed class NullSecurity : Security
    {
        internal NullSecurity() { type = 1; }

        public override object Clone()
        {
            return this;
        }
    }
}
