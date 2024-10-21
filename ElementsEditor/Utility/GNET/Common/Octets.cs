using System;
using System.Collections.Generic;
using System.Text;

namespace GNET.Common
{
    public class Octets : ICloneable, IComparable<Octets>
    {
        private static readonly int DEFAULT_SIZE = 128;
        private static Encoding DEFAULT_CHARSET = Encoding.UTF8;
        private byte[] _buffer = null;
        private int count = 0;


        private byte[] roundup(int size)
        {
            int capacity = 16;
            while (size > capacity) capacity <<= 1;
            return new byte[capacity];
        }

        public void reserve(int size)
        {
            if (_buffer == null)
            {
                _buffer = roundup(size);
            }
            else if (size > _buffer.Length)
            {
                byte[] tmp = roundup(size);
                Array.Copy(_buffer, 0, tmp, 0, count);
                _buffer = tmp;
            }
        }

        public Octets replace(byte[] data, int pos, int size)
        {
            reserve(size);
            try
            {
                Array.Copy(data, pos, _buffer, 0, size);
                count = size;
                return this;
            }
            catch (Exception e)
            {
                if (data == null)
                    Console.Error.WriteLine("Octets::replace: Exception! data is null");
                else
                {
                    Console.Error.WriteLine("Octets.replace: Exception! pos = " + pos + " size = " + size);

                    StringBuilder sb = new StringBuilder(data.Length);
                    foreach (byte b in data)
                        sb.Append($"{b} ");

                    Console.Error.WriteLine(sb.ToString());
                }

                throw e;
            }
        }

        public Octets replace(Octets data, int pos, int size)
        {
            return replace(data._buffer, pos, size);
        }

        public Octets replace(byte[] data)
        {
            return replace(data, 0, data.Length);
        }

        public Octets replace(Octets data)
        {
            return replace(data._buffer, 0, data.count);
        }

        public Octets()
        {
            reserve(DEFAULT_SIZE);
        }

        public Octets(int size)
        {
            reserve(size);
        }

        public Octets(Octets rhs)
        {
            replace(rhs);
        }

        public Octets(byte[] rhs)
        {
            replace(rhs);
        }

        public Octets(String str, Encoding encoding)
        {
            replace(encoding.GetBytes(str));
        }

        public Octets(String str)
        {
            replace(DEFAULT_CHARSET.GetBytes(str));
        }

        private Octets(byte[] bytes, int length)
        {
            this._buffer = bytes;
            this.count = length;
        }

        public static Octets wrap(byte[] bytes, int length)
        {
            return new Octets(bytes, length);
        }

        public static Octets wrap(byte[] bytes)
        {
            return wrap(bytes, bytes.Length);
        }

        public static Octets wrap(String str, String encoding)
        {
            try
            {
                return wrap(Encoding.GetEncoding(encoding).GetBytes(str));
            }
            catch (ArgumentException x)
            {
                throw x;
            }
        }

        public Octets(byte[] rhs, int pos, int size)
        {
            replace(rhs, pos, size);
        }

        public Octets(Octets rhs, int pos, int size)
        {
            replace(rhs, pos, size);
        }

        public Octets resize(int size)
        {
            reserve(size);
            count = size;
            return this;
        }

        public int size() { return count; }
        public int capacity() { return _buffer.Length; }
        public Octets clear() { count = 0; return this; }

        public Octets swap(Octets rhs)
        {
            int size = count; 
            count = rhs.count; 
            rhs.count = size;

            byte[] tmp = rhs._buffer; 
            rhs._buffer = _buffer; 
            _buffer = tmp;

            return this;
        }

        public Octets push_back(byte data)
        {
            reserve(count + 1);
            _buffer[count++] = data;
            return this;
        }		

        public Octets erase(int from, int to)
        {
            Array.Copy(_buffer, to, _buffer, from, count - to);
            count -= (to - from);
            return this;
        }

        public Octets insert(int from, byte[] data, int pos, int size)
        {
            reserve(count + size);
            Array.Copy(_buffer, from, _buffer, from + size, count - from);
            Array.Copy(data, pos, _buffer, from, size);
            count += size;
            return this;
        }

        public Octets insert(int from, Octets data, int pos, int size)
        {
            return insert(from, data._buffer, pos, size);
        }

        public Octets insert(int from, byte[] data)
        {
            return insert(from, data, 0, data.Length);
        }

        public Octets insert(int from, Octets data)
        {
            return insert(from, data._buffer, 0, data.size());
        }

		public Object Clone()
        {
            return new Octets(this);
        }

        public int CompareTo(Octets rhs)
        {
            // compare count first
            int c = count - rhs.count;
            if (c != 0) return c;

            byte[] v1 = _buffer;
            byte[] v2 = rhs._buffer;
            for (int i = 0; i < count; i++)
            {
                int v = v1[i] - v2[i];
                if (v != 0)
                    return v;
            }
            return 0;
        }

        //public int CompareTo(Object o)
        //{
        //    return CompareTo((Octets)o);
        //}

        public override bool Equals(Object o)
        {
            Octets octectO = o as Octets;
            if (octectO == null)
                return false;
            if (this == octectO)
                return true;
            return CompareTo(octectO) == 0;
        }

        public override int GetHashCode()
        {
            // same as java.util.Arrays.java
            if (_buffer == null)
                return 0;

            int result = 1;
            for (int i = 0; i < count; i++)
                result = 31 * result + _buffer[i];

            return result;
        }

        public override string ToString()
        {
            return getString();
        }

        public byte[] getBytes()
        {
            byte[] tmp = new byte[count];
            Array.Copy(_buffer, 0, tmp, 0, count);
            return tmp;
        }

		public byte[] Buffer()
		{
			return _buffer;
		}

        public Octets SetBuffer(byte[] buffer)
        {
            int size = buffer.Length;
            Array.Resize(ref _buffer, size);
            Array.Copy(buffer, 0, _buffer, 0, size);

            return this;
        }

        public byte[] array()
        {
            return array(0);
        }
        public byte[] array(int offset, int len = -1)
        {
            if (len < 0)
                len = size() - offset;
            byte[] ret = new byte[len];
            Array.Copy(_buffer, offset, ret, 0, len);
            return ret;
        }

        public byte getByte(int pos)
        {
            return _buffer[pos];
        }

        public void setByte(int pos, byte b)
        {
            _buffer[pos] = b;
        }

        public String getString(int pos, int len)
        {
            return DEFAULT_CHARSET.GetString(_buffer, pos, len);
        }

        public String getString()
        {
            return getString(0, count);
        }

        public String getStringUnicode()
        {
            return getString(4, count - 4, Encoding.Unicode);
        }
        public String getStringUnicodeFromPage936()
        {
            Encoding fromEnc = Encoding.GetEncoding(936);
            byte[] pUni = Encoding.Convert(fromEnc, Encoding.Unicode, _buffer);
            return Encoding.Unicode.GetString(pUni);
        }

        public String getStringUTF8()
        {
            return getString(0, count, Encoding.UTF8);
        }

        public String getString(int pos, int len, String encoding)
        {
            try
            {
                return Encoding.GetEncoding(encoding).GetString(_buffer, pos, len);
            }
            catch (SystemException x)
            {
                throw new SystemException(x.Message);
            }
        }

        public String getString(int pos, int len, Encoding encoding)
        {
            try
            {
                return encoding.GetString(_buffer, pos, len);
            }
            catch (SystemException x)
            {
                throw new SystemException(x.Message);
            }
        }

        public String getString(String encoding)
        {
            return getString(0, count, encoding);
        }

        public String getString(Encoding encoding)
        {
            return getString(0, count, encoding);
        }

        public void setString(String str)
        {
            _buffer = DEFAULT_CHARSET.GetBytes(str);
            count = _buffer.Length;
        }

		public void setStringUnicode(String str)
        {
            _buffer = Encoding.Unicode.GetBytes(str);
            count = _buffer.Length;			
        }

        public void Dump()
        {
            for (int i = 0; i < size(); ++i)
                Console.Write(_buffer[i] + " ");

            Console.WriteLine("");
        }

        public string DumpHex
        {
            get
            {
                StringBuilder strBuider = new StringBuilder();
                for (int index = 0; index < size(); index++)
                    strBuider.Append(((int)_buffer[index]).ToString("X2"));

                return strBuider.ToString();
            }
        }

        static public void setDefaultCharset(String name)
        {
            DEFAULT_CHARSET = Encoding.GetEncoding(name);
        }
/*
        static public void main(String[] arg)
        {
            Octets x = new Octets(Encoding.Default.GetBytes("ddd"));
            x.replace(Encoding.Default.GetBytes("abc"));
            x.replace(Encoding.Default.GetBytes("defghijklmn"));
            try
            {
                x.replace(Encoding.UTF8.GetBytes("0123456789"));
            }
            catch (Exception e)
            {
                throw e;
            }

            x.insert(x.size(), Encoding.Default.GetBytes("abc"));
            x.insert(x.size(), Encoding.Default.GetBytes("def"));
            Console.WriteLine(x.getString());
            Console.WriteLine("size = " + x.size());
            Octets y = new Octets(Encoding.Default.GetBytes("ABC"));
            x.insert(x.size(), y);
            Console.WriteLine(Encoding.Default.GetString(x.getBytes()));
            Octets z = (Octets)x.Clone();
            Console.WriteLine(Encoding.Default.GetString(x.getBytes()));
        }
*/
    }
}
