using System;
using System.ComponentModel;
using System.Linq;

namespace ElementsEditor.Utility
{
    [Serializable]
    public enum TypeItem
    {
        @bool,
        @char,
        @byte,
        @sbyte,
        @int,
        @uint,
        @short,
        @ushort,
        @long,
        @ulong,
        @float,
        @double,
        @decimal,
        @string,
        @wstring,
        @array,
        @struct,
    }

    public class TypeItemConverter: TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            if (context.Instance is TypeItem item)
                return true;
            else
                return GetStandardValuesSupported();
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            string[] names = Enum.GetNames(typeof(TypeItem))
                               .Where(x => !x.StartsWith("bool")).ToArray();

            return new StandardValuesCollection(names);
        }
    }
}
