using System;

namespace GNET.Common.Security
{
    public sealed class ARCFourSecurity : Security
    {        
        private byte index1;
        private byte index2;
        private readonly byte[] perm;

        internal ARCFourSecurity()
        {
            type = 2;
            perm = new byte[256];
        }

        public override object Clone()
        {
            ARCFourSecurity o = new ARCFourSecurity();
            Array.Copy(perm, 0, o.perm, 0, 256);
            return o;
        }

        public override void SetParameter(Octets o)
        {
            byte shift = 0;
            int keylen = o.size();
            
            for (int i = 0; i < 256; i++) 
                perm[i] = (byte)i;            

            for (int i = 0; i < 256; i++)
            {
                var a = o.getByte(i % keylen);
                shift += (byte)((a + perm[i]) % 256);

                byte k = perm[i];
                perm[i] = perm[shift];
                perm[shift] = k;
            }
        }

        public override Octets Update(Octets o)
        {
            int len = o.size();
            for (int i = 0; i < len; i++)
            {
                index1++;
                var a = perm[index1];

                index2 += a;
                var b = perm[index2];

                perm[index2] = a;
                perm[index1] = b;

                var c = (byte)((a + b) % 256);
                var d = perm[c];

                o.setByte(i, (byte)(o.getByte(i) ^ d));
            }

            return o;
        }
    }
}
