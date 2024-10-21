using System;

namespace GNET.Common.Security
{
    public class Decompress : ICloneable
    {
        enum MPPC { CTRL_OFF_EOB = 0, MPPC_HIST_LEN = 8192 }

        private byte[] history = new byte[(int)MPPC.MPPC_HIST_LEN];
        private uint histptr = 0;
        uint l = 0, adjust_l = 0, blen = 0, blen_totol = 0;
        uint rptr, adjust_rptr;
        Octets legacy_in = new Octets();
        byte[] lbuf = null;

        private bool passbits(uint n)
        {
            l += n;
            blen += n;
            if (blen < blen_totol)
                return true;

            l = adjust_l;
            rptr = adjust_rptr;
            return false;
        }

        private uint fetch()
        {
            rptr += l >> 3;
            l &= 7;
            return (uint)(System.Net.IPAddress.HostToNetworkOrder(BitConverter.ToInt32(lbuf, (int)rptr)) << (int)l);
        }

        internal Decompress() { lbuf = legacy_in.Buffer(); }

        public object Clone()
        {
            Decompress o = new Decompress();
            o.histptr = histptr;
            o.l = l;
            o.adjust_l = adjust_l;
            o.blen = blen;
            o.blen_totol = blen_totol;
            o.rptr = rptr;
            o.adjust_rptr = adjust_rptr;
            o.legacy_in.replace(legacy_in);
            o.lbuf = o.legacy_in.Buffer();
            Array.Copy(history, o.history, history.Length);
            return o;
        }

        internal void LameCopy(byte[] arry, int dst, int src, int len)
        {
            if (dst - src > 3)
            {
                while (len > 3)
                {
                    byte b0 = arry[src++];
                    byte b1 = arry[src++];
                    byte b2 = arry[src++];
                    byte b3 = arry[src++];

                    arry[dst++] = b0;
                    arry[dst++] = b1;
                    arry[dst++] = b2;
                    arry[dst++] = b3;
                    len = len - 4;
                }
            }
            while (len > 0)
            {
                arry[dst++] = arry[src++];
                len--;
            }
        }

        internal Octets Update(Octets oin)
        {
            legacy_in.insert(legacy_in.size(), oin);
            blen_totol = (uint)(legacy_in.size() * 8 - l);
            legacy_in.reserve(legacy_in.size() + 3);
            lbuf = legacy_in.Buffer();
            rptr = 0;
            blen = 7;
            Octets oout = oin;
            oout.clear();
            uint histhead = histptr;

            while (blen_totol > blen)
            {
                adjust_l = l;
                adjust_rptr = rptr;
                uint val = fetch();

                if (val < 0x80000000)
                {
                    if (!passbits(8))
                        break;
                    history[histptr++] = (byte)(val >> 24);
                    continue;
                }
                if (val < 0xc0000000)
                {
                    if (!passbits(9))
                        break;
                    history[histptr++] = (byte)(((val >> 23) | 0x80) & 0xff);
                    continue;
                }

                uint len = 0, off = 0;
                if (val >= 0xf0000000)
                {
                    if (!passbits(10))
                        break;
                    off = (val >> 22) & 0x3f;
                    if (off == (uint)MPPC.CTRL_OFF_EOB)
                    {
                        uint advance = 8 - (l & 7);
                        if (advance < 8)
                            if (!passbits(advance))
                                break;
                        oout.insert(oout.size(), history, (int)histhead, (int)(histptr - histhead));
                        if (histptr == (uint)MPPC.MPPC_HIST_LEN)
                            histptr = 0;
                        histhead = histptr;
                        continue;
                    }
                }
                else if (val >= 0xe0000000)
                {
                    if (!passbits(12))
                        break;
                    off = ((val >> 20) & 0xff) + 64;
                }
                else if (val >= 0xc0000000)
                {
                    if (!passbits(16))
                        break;
                    off = ((val >> 16) & 0x1fff) + 320;
                }

                val = fetch();
                if (val < 0x80000000)
                {
                    if (!passbits(1))
                        break;
                    len = 3;
                }
                else if (val < 0xc0000000)
                {
                    if (!passbits(4))
                        break;
                    len = 4 | ((val >> 28) & 3);
                }
                else if (val < 0xe0000000)
                {
                    if (!passbits(6))
                        break;
                    len = 8 | ((val >> 26) & 7);
                }
                else if (val < 0xf0000000)
                {
                    if (!passbits(8))
                        break;
                    len = 16 | ((val >> 24) & 15);
                }
                else if (val < 0xf8000000)
                {
                    if (!passbits(10))
                        break;
                    len = 32 | ((val >> 22) & 0x1f);
                }
                else if (val < 0xfc000000)
                {
                    if (!passbits(12))
                        break;
                    len = 64 | ((val >> 20) & 0x3f);
                }
                else if (val < 0xfe000000)
                {
                    if (!passbits(14))
                        break;
                    len = 128 | ((val >> 18) & 0x7f);
                }
                else if (val < 0xff000000)
                {
                    if (!passbits(16))
                        break;
                    len = 256 | ((val >> 16) & 0xff);
                }
                else if (val < 0xff800000)
                {
                    if (!passbits(18))
                        break;
                    len = 0x200 | ((val >> 14) & 0x1ff);
                }
                else if (val < 0xffc00000)
                {
                    if (!passbits(20))
                        break;
                    len = 0x400 | ((val >> 12) & 0x3ff);
                }
                else if (val < 0xffe00000)
                {
                    if (!passbits(22))
                        break;
                    len = 0x800 | ((val >> 10) & 0x7ff);
                }
                else if (val < 0xfff00000)
                {
                    if (!passbits(24))
                        break;
                    len = 0x1000 | ((val >> 8) & 0xfff);
                }
                else
                {
                    l = adjust_l;
                    rptr = adjust_rptr;
                    break;
                }

                if (histptr < off || histptr + len > (uint)MPPC.MPPC_HIST_LEN)
                    break;
                // Array.Copy(history, histptr - off, history, histptr, histptr - histhead);
                LameCopy(history, (int)histptr, (int)(histptr - off), (int)len);
                // Array.Copy(history, histptr - off, history, histptr, len);
                histptr += len;
            }

            oout.insert(oout.size(), history, (int)histhead, (int)(histptr - histhead));
            legacy_in.erase(0, (int)rptr);
            return oout;
        }
    }

    public sealed class DecompressARCFourSecurity : Security
    {
        ARCFourSecurity arc4 = new ARCFourSecurity();
        Decompress decompress = new Decompress();

        internal DecompressARCFourSecurity()
        {
            type = 6;
        }

        public override object Clone()
        {
            DecompressARCFourSecurity o = new DecompressARCFourSecurity();
            o.arc4 = (ARCFourSecurity)arc4.Clone();
            o.decompress = (Decompress)decompress.Clone();
            return o;
        }

        public override void SetParameter(Octets param)
        {
            arc4.SetParameter(param);
        }

        public override Octets Update(Octets o)
        {
            decompress.Update(arc4.Update(o));
            return o;
        }
    }

    public sealed class DecompressSecurity : Security
    {
        Decompress decompress = new Decompress();

        internal DecompressSecurity()
        {
            type = 7;
        }

        public override object Clone()
        {
            DecompressSecurity o = new DecompressSecurity();
            o.decompress = (Decompress)decompress.Clone();
            return o;
        }

        public override void SetParameter(Octets o)
        {
        }

        public override Octets Update(Octets o)
        {
            decompress.Update(o);
            return o;
        }
    }
}
