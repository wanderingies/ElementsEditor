using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ElementsEditor.Utility;
using GNET.Common;
using GNET.Common.Security;

namespace ElementsEditor.Template
{
    internal class Collection
    {
        public bool Editor { get; set; } = false;

        public Collection()
        {
            Elements = new List<Element>();
        }

        public Collection(string filename, ref ProgressBar progressBar)
        {
            Elements = new List<Element>();
            Load(filename, ref progressBar);
        }

        public UInt32 Version;
        public UInt32 Signature;

        public Int32 TotalCount;
        public Int32 TalkColumn;        

        // 完美
        public int w2i_SkinTag = -1;
        public int w2i_SkinHash = -1;
        public int w2i_SkinMeta = -1;
        public List<W2iTalkProc> w2iTalkProc = new List<W2iTalkProc>();

        public List<Element> Elements = null;
        public List<Function> Functions = null;

        public void Load(string filename, ref ProgressBar progressBar)
        {
            using(FileStream fileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using(BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    Version = binaryReader.ReadUInt32();
                    Signature = binaryReader.ReadUInt32();

                    int gfunction = -1;
                    string version = Convert.ToInt64(Version).ToString("X8");
                    string[] configures = Directory.GetFiles(Application.StartupPath + "\\configure", string.Format("confure_*_*_{0}.cfg", version));
                    
                    if (configures.Length > 0)
                    {
                        // 设置窗口名称
                        gfunction = int.Parse(configures[0].Split('_')[1]);
                        //Forms.MainForm.GetInstance().NickName = Enum.GetName(typeof(Utility.Function), gfunction);
                    }

                    if (configures.Length > 0)
                    {
                        // 加载配置文件
                        LoadConfiguration(configures[0]);

                        // 判断游戏类型
                        switch (gfunction)
                        {
                            case 10:
                                {
                                    for (int i = 0; i < Elements.Count; i++)
                                    {
                                        if (TalkColumn == i)
                                        {
                                            int size = binaryReader.ReadInt32();
                                            w2iTalkProc = new List<W2iTalkProc>();

                                            for (int j = 0; j < size; j++)
                                                w2iTalkProc.Add(new W2iTalkProc().Load(binaryReader));

                                            continue;
                                        }



                                        if (w2i_SkinTag == i)
                                        {
                                            //uint tag = binaryReader.ReadUInt32();
                                            //uint len = binaryReader.ReadUInt32();
                                            //byte[] buffer = binaryReader.ReadBytes(len.ToInt32());
                                            //DateTime dateTime = binaryReader.ReadInt32().ToDateTime();
                                            //Elements[i].Values.Add(binaryReader.ReadBytes(Elements[i].Size));
                                            binaryReader.BaseStream.Seek(4, SeekOrigin.Current);
                                            int len = binaryReader.ReadInt32();
                                            binaryReader.BaseStream.Seek(-8, SeekOrigin.Current);
                                            Elements[i].Size = len + 12;
                                            Elements[i].Values.Add(binaryReader.ReadBytes(Elements[i].Size));
                                            continue;
                                        }

                                        if (w2i_SkinHash == i)
                                        {
                                            binaryReader.BaseStream.Seek(4, SeekOrigin.Current);
                                            int len = binaryReader.ReadInt32();
                                            binaryReader.BaseStream.Seek(-8, SeekOrigin.Current);
                                            Elements[i].Size = len + 8;
                                            Elements[i].Values.Add(binaryReader.ReadBytes(Elements[i].Size));
                                            //Elements[i].Values.Add(binaryReader.ReadBytes(Elements[i].Size));
                                            continue;
                                        }

                                        if (w2i_SkinMeta == i)
                                        {
                                            int len = binaryReader.ReadInt32();
                                            binaryReader.BaseStream.Seek(-4, SeekOrigin.Current);
                                            Elements[i].Size = len + 4;
                                            Elements[i].Values.Add(binaryReader.ReadBytes(Elements[i].Size));
                                            continue;
                                        }

                                        {                                            
                                            uint total;
                                            int num, size = -1;
                                            if (Version == 0x30000154)
                                            {
                                                num = binaryReader.ReadInt32();
                                                total = binaryReader.ReadUInt32();
                                                size = binaryReader.ReadInt32();
                                            }
                                            else
                                            {
                                                total = binaryReader.ReadUInt32();
                                            }

                                            for (int j = 0; j < total; j++)
                                            {
                                                Elements[i].Values.Add(binaryReader.ReadBytes(Elements[i].Size));
                                            }
                                        }
                                    }

                                    break;
                                }
                            case 23:
                                {
                                    int ii = 0;
                                    foreach (var item in Elements)
                                    {
                                        if (++ii > 88) break;
                                        uint total = binaryReader.ReadUInt32();
                                        List<EXP_STRUCT_ID> exp_strict_id_array = new List<EXP_STRUCT_ID>();
                                        for (int i = 0; i < total; i++)
                                            exp_strict_id_array.Add(new EXP_STRUCT_ID() { Id = binaryReader.ReadUInt32(), Size = binaryReader.ReadUInt16() });

                                        uint size = binaryReader.ReadUInt32();
                                        List<byte[]> bytes = new List<byte[]>();
                                        for (int i = 0; i < total; i++)
                                            bytes.Add(binaryReader.ReadBytes(exp_strict_id_array[i].Size));

                                        int cSize = item.Size;
                                        for (int i = 0; i < total; i++)
                                        {
                                            Octets octets = new Octets(bytes[i]);
                                            octets.resize(cSize);
                                            Security security = Security.Create("DECOMPRESSARCFOURSECURITY");
                                            security.Update(octets);

                                            item.Values.Add(octets.getBytes());
                                            //OctetsStream octetsStream = new OctetsStream(octets);
                                            //item.Values.Add(string.Format("{0}:{1}", octetsStream.unmarshalInt(), octetsStream.getStringUnicode()));

                                            //byte[] b2 = octets.getBytes();
                                            //listBox.Items.Add(BitConverter.ToInt32(b2, 0) + ": " + Encoding.Unicode.GetString(b2.Skip(4).ToArray()));
                                        }
                                    }

                                    break;
                                }
                        }
                    }
                }
            }
        }

        public void LoadConfiguration(string configure)
        {            
            using(StreamReader streamReader = new StreamReader(configure))
            {
                TotalCount = int.Parse(streamReader.ReadLine());
                TalkColumn = int.Parse(streamReader.ReadLine());

                while (!streamReader.EndOfStream)
                {
                    string line;
                    while ((line = streamReader.ReadLine()) == string.Empty) { }

                    Element element = new Element();                    

                    if (line.Contains("TalkProc"))
                    {
                        element.Name = "TalkProc";
                        element.Values = new List<byte[]>();
                        Elements.Add(element);
                        TalkColumn = int.Parse(line.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                        continue;
                    }

                    if (line.Contains("SkinTag"))
                    {
                        element.Name = "SkinTag";
                        element.Values = new List<byte[]>();
                        Elements.Add(element);
                        w2i_SkinTag = int.Parse(line.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                        continue;
                    }

                    if (line.Contains("SkinHash"))
                    {
                        element.Name = "SkinHash";
                        element.Values = new List<byte[]>();
                        Elements.Add(element);
                        w2i_SkinHash = int.Parse(line.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                        continue;
                    }

                    if (line.Contains("SkinMeta"))
                    {
                        element.Name = "SkinMeta";
                        element.Values = new List<byte[]>();
                        Elements.Add(element);
                        w2i_SkinMeta = int.Parse(line.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                        continue;
                    }

                    {
                        if (line.Contains('-'))
                            element.Name = line.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)[1].Trim();
                        else element.Name = line.Trim();
                        element.Size = int.Parse(streamReader.ReadLine());
                        element.ShowPostion = -1;

                        element.Fields = streamReader.ReadLine().Split(new char[] { ';' }).ToList();
                        element.Types = streamReader.ReadLine().Split(new char[] { ';' }).ToList();
                        //element.FieldSize = streamReader.ReadLine().Split(new char[] { ';' }).ToList();
                        element.Values = new List<byte[]>();
                        element.Notes = new List<string>();
                    }

                    for (int i = 0; i < element.Fields.Count; i++)
                    {
                        if (element.Fields[i].ToLower() == "name")
                        {
                            element.ShowPostion = i;
                            for (int j = 0; j < i; j++)
                                element.SkinPostion += Extensions.GetTypeSize(element.Types[j]);
                            continue;
                        }
                    }

                    if(element.ShowPostion == -1)
                    {
                        for (int i = 0; i < element.Fields.Count; i++)
                        {
                            if (element.Fields[i].ToLower() == "id")
                            {
                                element.ShowPostion = i;
                                for (int j = 0; j < i; j++)
                                    element.SkinPostion += Extensions.GetTypeSize(element.Types[j]);
                                continue;
                            }
                        }
                    }

                    Elements.Add(element);
                }
            }
        }
    }
}
