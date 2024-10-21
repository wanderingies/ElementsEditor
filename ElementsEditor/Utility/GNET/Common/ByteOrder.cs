using System;
using System.Collections.Generic;
using System.Text;

namespace GNET.Common
{
	public class ByteOrder : object
	{
		public static ushort byteorder_16 (ushort x)
		{
			if (BitConverter.IsLittleEndian) {
				byte[] buffer = BitConverter.GetBytes (x);
				Array.Reverse (buffer);
				return BitConverter.ToUInt16 (buffer, 0);
			}
			return x;
		}

		public static uint byteorder_32 (uint x)
		{
			if (BitConverter.IsLittleEndian) {
				byte[] buffer = BitConverter.GetBytes (x);
				Array.Reverse (buffer);
				return BitConverter.ToUInt32 (buffer, 0);
			}
			return x;
		}

		public static ulong byteorder_64 (ulong x)
		{
			if (BitConverter.IsLittleEndian) {
				byte[] buffer = BitConverter.GetBytes (x);
				Array.Reverse (buffer);
				return BitConverter.ToUInt64 (buffer, 0);
			}
			return x;
		}
	}
}
