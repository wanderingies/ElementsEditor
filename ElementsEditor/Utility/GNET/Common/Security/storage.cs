using System;
using System.Collections.Generic;
using System.Text;

namespace GNET.Common.Security
{
    public class storage
    {
        public class Compress : ICloneable
        {
            enum MPPC { CTRL_OFF_EOB = 0, MPPC_HIST_LEN = 8192 }

            private byte[] history = new byte[(int)MPPC.MPPC_HIST_LEN];
            private uint histptr = 0;
            private uint[] hash = new uint[256];
            uint legacy_in = 0;

            private void putbits(byte[] buf, ref uint pos, uint val, uint n, ref uint l)
            {
                l += n;
                int t = System.Net.IPAddress.HostToNetworkOrder((int)(val << (32 - (int)l))) | buf[pos];
                Array.Copy(BitConverter.GetBytes(t), 0, buf, pos, 4);
                pos += l >> 3;
                l &= 7;
            }

            private void putlit(byte[] buf, ref uint pos, uint c, ref uint l)
            {
                if (c < 0x80)
                    putbits(buf, ref pos, c, 8, ref l);
                else
                    putbits(buf, ref pos, c & 0x7f | 0x100, 9, ref l);
            }

            private void putoff(byte[] buf, ref uint pos, uint off, ref uint l)
            {
                if (off < 64)
                    putbits(buf, ref pos, 0x3c0 | off, 10, ref l);
                else if (off < 320)
                    putbits(buf, ref pos, 0xe00 | (off - 64), 12, ref l);
                else
                    putbits(buf, ref pos, 0xc000 | (off - 320), 16, ref l);
            }

            private bool compare_short(uint p, uint s)
            {
                return history[p] == history[s] && history[p + 1] == history[s + 1];
            }

            private void compress_block(byte[] obuf, ref uint pos, uint isize)
            {
                uint r = histptr + isize;
                uint s = histptr;
                uint l = 0;
                obuf[pos] = 0;

                while (r - s > 2)
                {
                    uint p = hash[history[s]];
                    hash[history[s]] = s;
                    if (p >= s)
                    {
                        putlit(obuf, ref pos, history[histptr++], ref l);
                        s = histptr;
                    }
                    else if (!compare_short(p, s++))
                    {
                        putlit(obuf, ref pos, history[histptr++], ref l);
                    }
                    else if (history[p + 1] != history[++s])
                    {
                        putlit(obuf, ref pos, history[histptr++], ref l);
                        s = histptr;
                    }
                    else
                    {
                        for (p++, s++; s < r && history[p] == history[s]; p++, s++) ;
                        uint len = s - histptr;
                        histptr = s;
                        putoff(obuf, ref pos, s - p, ref l);

                        if (len < 4)
                            putbits(obuf, ref pos, 0, 1, ref l);
                        else if (len < 8)
                            putbits(obuf, ref pos, 0x08 | (len & 0x03), 4, ref l);
                        else if (len < 16)
                            putbits(obuf, ref pos, 0x30 | (len & 0x07), 6, ref l);
                        else if (len < 32)
                            putbits(obuf, ref pos, 0xe0 | (len & 0x0f), 8, ref l);
                        else if (len < 64)
                            putbits(obuf, ref pos, 0x3c0 | (len & 0x1f), 10, ref l);
                        else if (len < 128)
                            putbits(obuf, ref pos, 0xf80 | (len & 0x3f), 12, ref l);
                        else if (len < 256)
                            putbits(obuf, ref pos, 0x3f00 | (len & 0x7f), 14, ref l);
                        else if (len < 512)
                            putbits(obuf, ref pos, 0xfe00 | (len & 0xff), 16, ref l);
                        else if (len < 1024)
                            putbits(obuf, ref pos, 0x3fc00 | (len & 0x1ff), 18, ref l);
                        else if (len < 2048)
                            putbits(obuf, ref pos, 0xff800 | (len & 0x3ff), 20, ref l);
                        else if (len < 4096)
                            putbits(obuf, ref pos, 0x3ff000 | (len & 0x7ff), 22, ref l);
                        else if (len < (uint)MPPC.MPPC_HIST_LEN)
                            putbits(obuf, ref pos, 0xffe000 | (len & 0xfff), 24, ref l);
                    }
                }

                switch (r - s)
                {
                    case 2:
                        putlit(obuf, ref pos, history[histptr++], ref l);
                        putlit(obuf, ref pos, history[histptr++], ref l);
                        break;
                    case 1:
                        putlit(obuf, ref pos, history[histptr++], ref l);
                        break;
                }
                putoff(obuf, ref pos, (uint)MPPC.CTRL_OFF_EOB, ref l);
                if (l != 0)
                    putbits(obuf, ref pos, 0, 8 - l, ref l);
                legacy_in = 0;
            }

            public Compress() { }

            public Object Clone()
            {
                Compress o = new Compress();
                o.histptr = histptr;
                o.legacy_in = legacy_in;
                Array.Copy(history, o.history, history.Length);
                Array.Copy(hash, o.hash, hash.Length);
                return o;
            }

            public Octets Update(Octets oin)
            {
                Octets oout = new Octets();
                uint ipos = 0, opos = 0;
                byte[] ibuf = oin.Buffer();
                uint isize = (uint)oin.size();
                uint remain = (uint)MPPC.MPPC_HIST_LEN - histptr - legacy_in;

                if (isize >= remain)
                {
                    oout.resize((int)(isize + legacy_in) * 9 / 8 + 6);
                    byte[] obuf = oout.Buffer();
                    Array.Copy(ibuf, ipos, history, histptr + legacy_in, remain);
                    isize -= remain;
                    ipos += remain;
                    compress_block(obuf, ref opos, remain + legacy_in);
                    histptr = 0;

                    for (; isize >= (uint)MPPC.MPPC_HIST_LEN;
                        isize -= (uint)MPPC.MPPC_HIST_LEN, ipos += (uint)MPPC.MPPC_HIST_LEN)
                    {
                        Array.Copy(ibuf, ipos, history, histptr, (int)MPPC.MPPC_HIST_LEN);
                        compress_block(obuf, ref opos, (uint)MPPC.MPPC_HIST_LEN);
                        histptr = 0;
                    }
                    oout.resize((int)opos);
                }

                Array.Copy(ibuf, ipos, history, histptr + legacy_in, isize);
                legacy_in += isize;
                return oin.swap(oout);
            }

            public Octets Final(Octets oin)
            {
                if (oin.size() == 0 && legacy_in == 0)
                    return oin;

                Octets oout = Update(oin);
                int osize = oout.size();
                oout.reserve(osize + (int)legacy_in * 9 / 8 + 6);
                byte[] obuf = oout.Buffer();
                uint opos = (uint)osize;
                compress_block(obuf, ref opos, legacy_in);
                oout.resize((int)opos);
                return oin.swap(oout);
            }
        }

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

            public Object Clone()
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
    }
}
