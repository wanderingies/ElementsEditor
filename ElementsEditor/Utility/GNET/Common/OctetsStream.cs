using System;
using System.Diagnostics;
using System.Text;

namespace GNET.Common
{
    public class OctetsStream : Octets
    {
        private static readonly int MAXSPARE = 16384;
        private int pos = 0;
        private int tranpos = 0;

        public OctetsStream()
        {
        }

        public OctetsStream(int size)
            : base(size)
        {
        }

        public OctetsStream(Octets o)
            : base(o)
        {
        }

        public static OctetsStream wrap(Octets o)
        {
            OctetsStream os = new OctetsStream();
            os.swap(o);
            return os;
        }

        public new OctetsStream Clone()
        {
            OctetsStream os = new OctetsStream((Octets)base.Clone());
            os.pos = pos;
            os.tranpos = pos;
            return os;
        }

        public bool eos()
        {
            return pos == size();
        }

        public int position(int pos)
        {
            this.pos = pos;
            return this.pos;
        }

        public int position()
        {
            return pos;
        }

        public int remain()
        {
            return size() - pos;
        }

        public OctetsStream marshal(sbyte x)
        {
            push_back((byte)x);
            return this;
        }

        public void marshal_sbyte(sbyte x)
        {
            marshal(x);
        }

        public OctetsStream marshal(byte x)
        {
            push_back(x);
            return this;
        }

        public void marshal_byte(byte x)
        {
            marshal(x);
        }

        public OctetsStream marshal(bool b)
        {
            push_back((byte)(b ? 1 : 0));
            return this;
        }

        public void marshal_boolean(bool x)
        {
            marshal(x);
        }

        public OctetsStream marshal(short x)
        {
            return marshal((ushort)x);
        }

        public void marshal_short(short x)
        {
            marshal(x);
        }

        public OctetsStream marshal(ushort x)
        {
            return
                   marshal((byte)(x >> 8)).
                   marshal((byte)(x));
        }

        public void marshal_ushort(ushort x)
        {
            marshal(x);
        }

        public OctetsStream marshal(int x)
        {
            return marshal((uint)x);
        }

        public void marshal_int(int x)
        {
            marshal(x);
        }

        public OctetsStream marshal(uint x)
        {
            return
                marshal((byte)(x >> 24)).
                marshal((byte)(x >> 16)).
                marshal((byte)(x >> 8)).
                marshal((byte)(x));
        }

        public void marshal_uint(uint x)
        {
            marshal(x);
        }

        public OctetsStream marshal(long x)
        {
            return marshal((ulong)x);
        }

        public void marshal_long(long x)
        {
            marshal(x);
        }

        public OctetsStream marshal(ulong x)
        {
            return
                marshal((byte)(x >> 56)).
                marshal((byte)(x >> 48)).
                marshal((byte)(x >> 40)).
                marshal((byte)(x >> 32)).
                marshal((byte)(x >> 24)).
                marshal((byte)(x >> 16)).
                marshal((byte)(x >> 8)).
                marshal((byte)(x));
        }

        public void marshal_ulong(ulong x)
        {
            marshal(x);
        }

        public OctetsStream marshal(float x)
        {
            return marshal(BitConverter.ToInt32(BitConverter.GetBytes(x), 0));
        }

        public void marshal_float(float x)
        {
            marshal(x);
        }

        public OctetsStream marshal(double x)
        {
            return marshal(BitConverter.ToInt64(BitConverter.GetBytes(x), 0));
        }

        public void marshal_double(double x)
        {
            marshal(x);
        }

        public OctetsStream compact_uint32(uint x)
        {
            //if (x < 0x40) return marshal((sbyte)x);
            if (x < 0x80) return marshal((sbyte)x);
            else if (x < 0x4000) return marshal((ushort)(x | 0x8000));
            else if (x < 0x20000000) return marshal((uint)(x | 0xc0000000));
            marshal(unchecked((sbyte)0xe0));
            return marshal(x);
        }

        public void lua_compact_uint32(uint x)
        {
            compact_uint32(x);
        }

        public OctetsStream compact_sint32(int x)
        {
            if (x >= 0)
            {
                if (x < 0x40) return marshal((byte)x);
                else if (x < 0x2000) return marshal((short)(x | 0x8000));
                else if (x < 0x10000000) return marshal((int)((uint)x | 0xc0000000));
                marshal((byte)0xe0);
                return marshal(x);
            }
            if (-x > 0)
            {
                x = -x;
                if (x < 0x40) return marshal((byte)(x | 0x40));
                else if (x < 0x2000) return marshal((short)(x | 0xa000));
                else if (x < 0x10000000) return marshal((int)((uint)x | 0xd0000000));
                marshal((byte)0xf0);
                return marshal(x);
            }
            marshal((byte)0xf0);
            return marshal(x);
        }

        public void lua_compact_sint32(int x)
        {
            compact_sint32(x);
        }

        public OctetsStream marshal(Marshal m)
        {
            return m.marshal(this);
        }

        public OctetsStream marshal(Octets o)
        {
            if (null != o)
            {
                compact_sint32(o.size());
                insert(size(), o);
            }
            return this;
        }

        public void marshal_Octets(Octets o)
        {
            marshal(o);
        }

        public OctetsStream marshal(String str)
        {
            return marshal(str, null);
        }

        public OctetsStream marshal(String str, String charset)
        {
            try
            {
                if (charset == null)
                {
                    marshal(Encoding.Default.GetBytes(str));
                }
                else
                {
                    marshal(Encoding.GetEncoding(charset).GetBytes(str));
                }
            }
            catch (Exception e)
            {
                throw new SystemException(e.Message);
            }
            return this;
        }

        public OctetsStream Begin() { tranpos = pos; return this; }
        public OctetsStream Rollback() { pos = tranpos; return this; }
        public OctetsStream Commit()
        {
            if (pos >= MAXSPARE)
            {
                erase(0, pos);
                pos = 0;
            }
            return this;
        }

        public byte unmarshal_byte()
        {
            if (pos + 1 > size()) throw new MarshalException();
            return getByte(pos++);
        }

        public sbyte unmarshal_sbyte()
        {
            if (pos + 1 > size()) throw new MarshalException();
            return (sbyte)getByte(pos++);
        }

        public bool unmarshal_boolean()
        {
            return unmarshal_byte() == 1;
        }

        public short unmarshal_short()
        {
            return (short)(unmarshal_ushort());
        }

        public ushort unmarshal_ushort()
        {
            if (pos + 2 > size()) throw new MarshalException();
            byte b0 = getByte(pos++);
            byte b1 = getByte(pos++);
            return (ushort)(((b0 & 0xff) << 8) | (b1 & 0xff));
        }

        public int unmarshal_int()
        {
            if (pos + 4 > size()) throw new MarshalException();
            byte b0 = getByte(pos++);
            byte b1 = getByte(pos++);
            byte b2 = getByte(pos++);
            byte b3 = getByte(pos++);
            return ((
                ((b0 & 0xff) << 24) |
                ((b1 & 0xff) << 16) |
                ((b2 & 0xff) << 8) |
                ((b3 & 0xff) << 0)));
        }

        public int unmarshalInt(int pos = 0)
        {
            if (pos + 4 > size())
                throw new MarshalException();

            return BitConverter.ToInt32(getBytes(), pos);
        }

        public uint unmarshal_uint()
        {
            if (pos + 4 > size()) throw new MarshalException();
            byte b0 = getByte(pos++);
            byte b1 = getByte(pos++);
            byte b2 = getByte(pos++);
            byte b3 = getByte(pos++);
            return (uint)((
                ((b0 & 0xff) << 24) |
                ((b1 & 0xff) << 16) |
                ((b2 & 0xff) << 8) |
                ((b3 & 0xff) << 0)));
        }

        public long unmarshal_long()
        {
            return (long)(unmarshal_ulong());
        }

        public ulong unmarshal_ulong()
        {
            if (pos + 8 > size()) throw new MarshalException();
            byte b0 = getByte(pos++);
            byte b1 = getByte(pos++);
            byte b2 = getByte(pos++);
            byte b3 = getByte(pos++);
            byte b4 = getByte(pos++);
            byte b5 = getByte(pos++);
            byte b6 = getByte(pos++);
            byte b7 = getByte(pos++);
            return (ulong)((((long)b0 & 0xff) << 56) |
                (((long)b1 & 0xff) << 48) |
                (((long)b2 & 0xff) << 40) |
                (((long)b3 & 0xff) << 32) |
                (((long)b4 & 0xff) << 24) |
                (((long)b5 & 0xff) << 16) |
                (((long)b6 & 0xff) << 8) |
                (((long)b7 & 0xff) << 0));
        }

        public float unmarshal_float()
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(unmarshal_int()), 0);
        }

        public double unmarshal_double()
        {
            return BitConverter.ToDouble(BitConverter.GetBytes(unmarshal_long()), 0);
        }

        public uint uncompact_uint32()
        {
            if (pos == size()) throw new MarshalException();
            switch (getByte(pos) & 0xe0)
            {
                case 0xe0:
                    unmarshal_byte();
                    return unmarshal_uint();
                case 0xc0:
                    return unmarshal_uint() & (int)~0xc0000000;
                case 0xa0:
                case 0x80:
                    return (uint)(unmarshal_ushort() & ~0x8000);
            }
            return (uint)unmarshal_byte();
        }

        public int uncompact_sint32()
        {
            if (pos == size()) throw new MarshalException();
            switch (getByte(pos) & 0xf0)
            {
                case 0xf0:
                    unmarshal_sbyte();
                    return -unmarshal_int();
                case 0xe0:
                    unmarshal_sbyte();
                    return unmarshal_int();
                case 0xd0:
                    return (int)(-(unmarshal_int() & ~0xd0000000));
                case 0xc0:
                    return (int)(unmarshal_int() & ~0xc0000000);
                case 0xb0:
                case 0xa0:
                    return -((ushort)unmarshal_short() & ~0xa000);
                case 0x90:
                case 0x80:
                    return (ushort)unmarshal_short() & ~0x8000;
                case 0x70:
                case 0x60:
                case 0x50:
                case 0x40:
                    return -(unmarshal_sbyte() & ~0x40);
            }
            return unmarshal_sbyte();
        }

        public Octets unmarshal_Octets()
        {
            int size = (int)uncompact_uint32();

            if (size < 0)
            {
                Console.Error.WriteLine("uncompact_octets: uncompact size = " + size);
                throw new MarshalException();
            }
            if (pos + size > this.size())
            {
                Console.Error.WriteLine("uncompact_octets: pos = " + pos + " size =" + size);
                throw new MarshalException();
            }

            Octets o = new Octets(this, pos, size);
            pos += size;
            return o;
        }

        public OctetsStream unmarshal_OctetsStream()
        {
            int size = (int)uncompact_uint32();

            if (size < 0)
            {
                Console.WriteLine("unmarshal_OctetsStream: uncompact size = " + size);
                throw new MarshalException();
            }
            if (pos + size > this.size())
            {
                Console.WriteLine("unmarshal_OctetsStream: pos = " + pos + " size =" + size);
                throw new MarshalException();
            }

            OctetsStream o = new OctetsStream();
            o.replace(this, pos, size);
            pos += size;
            return o;
        }

        public byte[] unmarshal_bytes()
        {
            int size = (int)uncompact_uint32();
            if (pos + size > this.size()) throw new MarshalException();
            byte[] copy = new byte[size];
            Array.Copy(Buffer(), pos, copy, 0, size);
            pos += size;
            return copy;
        }

        public OctetsStream marshal(byte[] bytes)
        {
            compact_sint32(bytes.Length);
            insert(size(), bytes);
            return this;
        }

        public OctetsStream push_bytes(byte[] bytes)
        {
            insert(size(), bytes);
            return this;
        }

        public OctetsStream push_bytes(byte[] bytes, int len)
        {
            insert(size(), bytes, 0, len);
            return this;
        }

        public OctetsStream unmarshal(Octets os)
        {
            int size = (int)uncompact_uint32();
            if (pos + size > this.size()) throw new MarshalException();
            os.replace(this, pos, size);
            pos += size;
            return this;
        }

        public OctetsStream unmarshal(Octets os, int size)
        {
            if (pos + size > this.size()) throw new MarshalException();
            os.replace(this, pos, size);
            pos += size;
            return this;
        }

        public String unmarshal_String()
        {
            return unmarshal_String(null);
        }

        public String unmarshal_String(String charset)
        {
            try
            {
                int size = (int)uncompact_uint32();
                if (pos + size > this.size()) throw new MarshalException();
                int cur = pos;
                pos += size;
                return (charset == null)
                    ? String.Copy(getString(cur, size))
                    : String.Copy(getString(cur, size, charset));
            }
            catch (Exception e)
            {
                throw new SystemException(e.Message);
            }
        }

        public OctetsStream unmarshal(Marshal m)
        {
            return m.unmarshal(this);
        }

        /*
        public static void Main(String[] args)
        {
            try
            {
                OctetsStream os = new OctetsStream();

                //Octets x = new Octets("abcdef");
                //os.marshal(x);
                os.compact_sint32(-368123456);
                //os.marshal((float)2.5);

                //Console.WriteLine(os.unmarshal_Octets().getString());
                Console.WriteLine(os.uncompact_sint32());
                //Console.WriteLine(os.unmarshal_float());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
        */
    }
}
