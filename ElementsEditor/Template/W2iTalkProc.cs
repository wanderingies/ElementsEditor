using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementsEditor.Template
{
    internal struct W2iTalkProc
    {
        public struct Option
        {
            public uint id;                 // 选项链接的子窗口或功能ID, 如果最高位为1表示是一个预定义的功能, 为0表示一个子窗口
            public byte[] text;             // 选项链接的提示文字，最多63个汉字
            public uint param;              // 选项相关的参数

            public Option Load(BinaryReader binaryReader)
            {
                id = binaryReader.ReadUInt32();
                text = binaryReader.ReadBytes(128);
                param = binaryReader.ReadUInt32();

                return this;
            }
        }

        public struct Window
        {
            public uint id;                 // 窗口ID, 最高位不能为1
            public uint id_parent;          // 父窗口ID, 为-1表示根窗口

            public int talk_text_len;       // 对话文字的长度
            public byte[] talk_text;        // 对话文字

            public int num_option;          // 选项数目
            public List<Option> options;    // 选项列表

            public Window Load(BinaryReader binaryReader)
            {
                id = binaryReader.ReadUInt32();
                id_parent = binaryReader.ReadUInt32();

                talk_text_len = binaryReader.ReadInt32();
                talk_text = binaryReader.ReadBytes(talk_text_len * 2);

                num_option = binaryReader.ReadInt32();
                options = new List<Option>();
                for (int i = 0; i < num_option; i++)
                    options.Add(new Option().Load(binaryReader));

                return this;
            }
        }

        public uint id_talk;               // 对话对象的ID
        public byte[] text;                 // 对话的第一个窗口的提示文字，最多63个汉字

        public int num_window;             // 带对话文字的窗口个数
        public List<Window> windows;				// 带对话文字的窗口

        public W2iTalkProc Load(BinaryReader binaryReader)
        {
            id_talk = binaryReader.ReadUInt32();
            text = binaryReader.ReadBytes(128);

            num_window = binaryReader.ReadInt32();
            windows = new List<Window>();
            for (int i = 0; i < num_window; i++)
                windows.Add(new Window().Load(binaryReader));

            return this;
        }
    }
}
