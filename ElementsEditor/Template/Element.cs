using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementsEditor.Template
{
    internal class Element
    {
        public static Encoding Encoding { get; set; }
        
        public string Name;
        public int Size;

        public int ShowPostion;
        public int SkinPostion;

        public List<string> Types;
        public List<string> Fields;
        //public List<string> FieldSize;
        public List<byte[]> Values;
        public List<string> Notes;
    }
}
