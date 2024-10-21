using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ElementsEditor.Utility
{
    internal static class Extensions
    {
        public static int GetTypeSize(string type)
        {
            switch (type.ToLower())
            {
                case "bool":
                case "char":
                case "byte":
                case "sbyte":
                    return 1;
                case "short":
                case "ushort":
                case "int16":
                    return 2;
                case "int":
                case "uint":
                case "int32":
                case "float":
                    return 4;
                case "long":
                case "ulong":
                case "int64":
                case "double":
                    return 8;
                case "decimal":
                    return 16;
                default:
                    if (type.StartsWith("string:"))
                        return int.Parse(type.Split(':')[1]);

                    if (type.StartsWith("wstring:"))
                        return int.Parse(type.Split(':')[1]);

                    return 0;
            }
        }

        public static TypeItem GetTypeName(string type)
        {
            switch (type.ToLower())
            {
                case "short":                
                case "int16":
                    return TypeItem.@short;
                case "int":                
                case "int32":
                    return TypeItem.@int;
                case "long":
                case "int64":
                    return TypeItem.@long;
                case "ushort":
                    return TypeItem.@ushort;
                case "uint":
                    return TypeItem.@uint;
                case "ulong":
                    return TypeItem.@ulong;
                case "float":
                    return TypeItem.@float;
                case "double":
                    return TypeItem.@double;
                case "decimal":
                    return TypeItem.@decimal;
                case "array":
                    return TypeItem.array;
                default:
                    if (type.StartsWith("string:"))
                        return TypeItem.@string;

                    if (type.StartsWith("wstring:"))
                        return TypeItem.wstring;

                    return 0;
            }
        }

        public static object GetValues(this byte[] value, string type, ref int skin)
        {
            int num = skin;
            skin += GetTypeSize(type);

            switch (type.ToLower())
            {
                case "short":
                case "int16":
                    return BitConverter.ToInt16(value, num);
                case "ushort":
                    return BitConverter.ToUInt16(value, num);
                case "int":
                case "int32":
                    return BitConverter.ToInt32(value, num);
                case "uint":
                    return BitConverter.ToUInt32(value, num);
                case "long":
                case "int64":
                    return BitConverter.ToInt64(value, num);
                case "ulong":
                    return BitConverter.ToUInt64(value, num);
                case "float":
                    return BitConverter.ToSingle(value, num);
                case "double":
                    return BitConverter.ToDouble(value, num);                    
                default:
                    if (type.StartsWith("array"))
                        return BitConverter.ToString(value, num).Replace("-", "");

                    Encoding encoding;
                    if (type.StartsWith("string"))
                        encoding = Encoding.GetEncoding("GBK");
                    else if (type.StartsWith("wstring"))
                        encoding = Encoding.GetEncoding("Unicode");
                    else encoding = Encoding.Default;
                    
                    int size = int.Parse(type.Split(':')[1]);
                    return encoding.GetString(value, num, size).Replace("\0", "");
            }
        }

        public static void SetValues(this byte[] value, object stuff, string type, ref int skin)
        {
            byte[] buffer;
            int num = skin;
            skin += GetTypeSize(type);

            switch (type.ToLower())
            {
                case "int16":
                    buffer = BitConverter.GetBytes((Int16)stuff);
                    Array.Copy(value, num, buffer, 0, buffer.Length);
                    break;
                case "int32":
                    buffer = BitConverter.GetBytes((Int32)stuff);
                    Array.Copy(value, num, buffer, 0, buffer.Length);
                    break;
                case "int64":
                    buffer = BitConverter.GetBytes((Int64)stuff);
                    Array.Copy(value, num, buffer, 0, buffer.Length);
                    break;
                case "float":
                    buffer = BitConverter.GetBytes((Single)stuff);
                    Array.Copy(value, num, buffer, 0, buffer.Length);
                    break;
                case "double":
                    buffer = BitConverter.GetBytes((Double)stuff);
                    Array.Copy(value, num, buffer, 0, buffer.Length);
                    break;
                default:
                    Encoding encoding;
                    if (type.Contains("string:"))
                        encoding = Encoding.GetEncoding("GBK");
                    else if (type.Contains("wstring:"))
                        encoding = Encoding.GetEncoding("Unicode");
                    else encoding = Encoding.Default;

                    int size = int.Parse(type.Split(':')[1]);
                    buffer = encoding.GetBytes(stuff.ToString());

                    Array.Resize(ref buffer, size);
                    Array.Copy(value, num, buffer, 0, buffer.Length);
                    break;
            }
        }

        public static Int32 ToInt32(this UInt32 value)
        {
            return (Int32)value;
        }

        public static DateTime ToDateTime(this int value)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0);
            startTime = startTime.AddSeconds(value).ToLocalTime();
            return startTime;
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 6, CharSet = CharSet.Unicode)]
    public struct EXP_STRUCT_ID
    {
        [FieldOffset(0)] public UInt32 Id;
        [FieldOffset(4)] public UInt16 Size;
    }

    internal class MarshalExtensions
    {
        public static byte[] Serialize<T>(T obj)
        {
            int rawsize = Marshal.SizeOf(obj);
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.StructureToPtr(obj, buffer, false);
            byte[] rawdatas = new byte[rawsize];
            Marshal.Copy(buffer, rawdatas, 0, rawsize);
            Marshal.FreeHGlobal(buffer);
            return rawdatas;
        }

        public static T Deserialize<T>(byte[] rawdatas)
        {
            Type anytype = typeof(T);
            int rawsize = Marshal.SizeOf(anytype);
            if (rawsize > rawdatas.Length) return default(T);

            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.Copy(rawdatas, 0, buffer, rawsize);
            object retobj = Marshal.PtrToStructure(buffer, anytype);
            Marshal.FreeHGlobal(buffer);

            return (T)retobj;
        }
    }
}
