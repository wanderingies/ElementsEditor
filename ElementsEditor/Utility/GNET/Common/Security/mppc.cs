using GNET.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace GNET.Common.Security
{
    public class mppc
    {
        static CompressSecurity compressSecurity
            = new CompressSecurity();

        static DecompressSecurity decompressSecurity
            = new DecompressSecurity();

        static int compressBound(int sourcelen)
        {
            return (((sourcelen * 9) / 8) + 1) + 2 + 3;
        }

        public static void Compress(Octets os_src,ref Octets os_com)
        {
            int len_src = os_src.size();
            int len_com = compressBound(len_src);

            os_com.reserve(len_com);
            os_com = compressSecurity.Final(os_src);

            OctetsStream os =new OctetsStream();
            os.reserve(2 * sizeof(int) + len_com + os.size());
            os.compact_sint32(len_src).compact_sint32(len_com);
            //os << CompactUINT(len_src) << CompactUINT(len_com);
            os.push_bytes(os_com.Buffer(), len_com);
            //os.push_byte((const char*)os_com.begin(), len_com );
            os_com.swap(os);
        }
    }
}
