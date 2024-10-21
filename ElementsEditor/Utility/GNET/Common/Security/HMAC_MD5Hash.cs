using System;
using System.Collections.Generic;
using GNET.Common;

namespace GNET.Common.Security
{
    public sealed class HMAC_MD5Hash : Security
    {
        private Octets k_opad = new Octets(64);
        private MD5Hash md5hash = new MD5Hash();

        internal HMAC_MD5Hash()
        {
            type = 4;
        }

        public override object Clone()
        {
            HMAC_MD5Hash o = new HMAC_MD5Hash();
            o.k_opad.replace(k_opad).reserve(64);
            o.md5hash = (MD5Hash)md5hash.Clone();
            return o;
        }

        public override void SetParameter(Octets param)
        {
            Octets k_ipad = new Octets(64);
            int keylen = param.size();

            if (keylen > 64)
            {
                Octets key = MD5Hash.Digest(param);
                k_ipad.replace(key);
                k_opad.replace(key);
                keylen = key.size();
            }
            else
            {
                k_ipad.replace(param);
                k_opad.replace(param);
            }

            int i = 0;
            for (; i < keylen; i++)
            {
                k_ipad.setByte(i, (byte)(k_ipad.getByte(i) ^ 0x36));
                k_opad.setByte(i, (byte)(k_opad.getByte(i) ^ 0x5c));
            }
            for (; i < 64; i++)
            {
                k_ipad.setByte(i, (byte)0x36);
                k_opad.setByte(i, (byte)0x5c);
            }
            k_ipad.resize(64);
            k_opad.resize(64);            
            md5hash.Update(k_ipad);
        }

        public override Octets Update(Octets o)
        {
            md5hash.Update(o);
            return o;
        }

        public override Octets Final(Octets digest)
        {
            md5hash.Final(digest);
            MD5Hash ctx = new MD5Hash();
            ctx.Update(k_opad);
            ctx.Update(digest);
            return ctx.Final(digest);
        }
    }
}
