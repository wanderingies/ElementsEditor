using System;
using System.Collections.Generic;
using GNET.Common;

namespace GNET.Common.Security
{
    public abstract class Security : ICloneable
    {
        private static readonly Dictionary<string, Security> map = new Dictionary<string, Security>();
        protected int type;

        static Security()
        {
            Security instance = new Random();
            map.Add("RANDOM", instance);
            map.Add("0", instance);
            instance = new NullSecurity();
            map.Add("NULLSECURITY", instance);
            map.Add("1", instance);
            instance = new ARCFourSecurity();
            map.Add("ARCFOURSECURITY", instance);
            map.Add("2", instance);
            instance = new MD5Hash();
            map.Add("MD5HASH", instance);
            map.Add("3", instance);
            instance = new HMAC_MD5Hash();
            map.Add("HMAC_MD5HASH", instance);
            map.Add("4", instance);
            instance = new CompressARCFourSecurity();
            map.Add("COMPRESSARCFOURSECURITY", instance);
            map.Add("5", instance);
            instance = new DecompressARCFourSecurity();
            map.Add("DECOMPRESSARCFOURSECURITY", instance);
            map.Add("6", instance);
            instance = new DecompressSecurity();
            map.Add("DECOMPRESSSECURITY", instance);
            map.Add("7", instance);
            instance = new CompressSecurity();
            map.Add("COMPRESSSECURITY", instance);
            map.Add("8", instance);
        }

        public virtual void SetParameter(Octets o) { }
        public virtual void GetParameter(Octets o) { }
        public virtual Octets Update(Octets o) { return o; }
        public virtual Octets Final(Octets o) { return o; }

        public abstract object Clone();

        public static Security Create(string name)
        {
            Security stub;
            if (map.TryGetValue(name.ToUpper(), out stub))
            {
                return (Security)stub.Clone();
            }

            return new NullSecurity();
        }

        public static Security Create(int type)
        {
            return Create(type.ToString());
        }
    }
}
