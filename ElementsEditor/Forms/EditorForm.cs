using ElementsEditor.Template;
using ElementsEditor.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ElementsEditor.Forms
{
    public partial class EditorForm : Form
    {
        public EditorForm()
        {
            InitializeComponent();

            SaveButton.Click += SaveButtonClick;
            AddtoButton.Click += AddtoButtonClick;
            comboBoxGames.SelectedIndexChanged += ComboBoxGamesSelectedIndexChanged;
            comboBoxConfigures.SelectedIndexChanged += ComboBoxConfiguresSelectedIndexChanged;
            checkedListBox.SelectedIndexChanged += CheckedListBoxSelectedIndexChanged;
            dataGridView.CellValueChanged += DataGridViewCellValueChanged;

            AddtoList.CheckedChanged += (object sender, EventArgs eventArgs) =>
            {
                RadioButton radioButton = sender as RadioButton;
                if (radioButton != null && radioButton.Checked)
                {
                    AddtoPropertyType.Enabled =
                    AddtoPropertyNotes.Enabled = false;
                    AddtoNew.Checked = true;
                }
            };

            AddtoProperty.CheckedChanged += (object sender, EventArgs eventArgs) =>
            {
                RadioButton radioButton = sender as RadioButton;
                if (radioButton != null && radioButton.Checked)
                {
                    AddtoPropertyType.Enabled =
                    AddtoPropertyNotes.Enabled = true;
                    AddtoNew.Checked = true;
                }
            };

            AddtoNew.CheckedChanged += (object sender, EventArgs eventArgs) =>
            {
                RadioButton radioButton = sender as RadioButton;
                if (radioButton != null && radioButton.Checked)
                {
                    AddtoInsertList.Enabled = false;
                }
            };

            AddtoInsert.CheckedChanged += (object sender, EventArgs eventArgs) =>
            {
                RadioButton radioButton = sender as RadioButton;
                if (radioButton != null && radioButton.Checked)
                {
                    AddtoInsertList.Items.Clear();
                    AddtoInsertList.Enabled = true;

                    if (AddtoList.Checked)
                    {
                        foreach (var item in checkedListBox.Items)
                            AddtoInsertList.Items.Add(item);
                    }
                    else if (AddtoProperty.Checked && checkedListBox.SelectedIndex >= 0)
                    {
                        Element element = __collection.Elements[checkedListBox.SelectedIndex];
                        foreach (var item in element.Fields)
                            AddtoInsertList.Items.Add(item);
                    }
                    else
                    {
                        AddtoNew.Checked = true;
                    }
                }
            };

            var names=new List<string>()
            {
                "武林外传",
                "完美国际",
                "诛仙",
                "赤壁",
                "热舞派对",
                "口袋西游",
                "梦幻诛仙",
                "神鬼传奇",
                "神魔大陆",
                "降龙之剑",
                "笑傲江湖",
                "神鬼世界",
                "神雕侠侣",
                "圣斗士星矢",
                "降龙之剑2",
                "射雕英雄传",
                "美食猎人",
                "Touch",
                "战三国"
            };
            EnumNumber = Enum.GetNames(typeof(Utility.GameFun));
            comboBoxGames.DataSource = names;

            var types = Enum.GetNames(typeof(Utility.TypeItem));
            AddtoPropertyType.DataSource = types;

            DataGridViewTextBoxColumn columnName = new DataGridViewTextBoxColumn();
            columnName.Name = "ColumnName";
            columnName.DataPropertyName = "name";//对应数据源的字段
            columnName.HeaderText = "名称";
            columnName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView.Columns.Add(columnName);

            DataGridViewComboBoxColumn columnType = new DataGridViewComboBoxColumn();
            columnType.Name = "ColumnType";
            columnType.DataPropertyName = "type";//对应数据源的字段
            columnType.HeaderText = "类型";
            this.dataGridView.Columns.Add(columnType);
            columnType.DataSource = Enum.GetNames(typeof(Utility.TypeItem)); //这里需要设置一下combox的itemsource,以便combox根据数据库中对应的值自动显示信息

            DataGridViewTextBoxColumn columnNum = new DataGridViewTextBoxColumn();
            columnNum.Name = "ColumnNum";
            columnNum.DataPropertyName = "num";//对应数据源的字段
            columnNum.HeaderText = "长度";
            this.dataGridView.Columns.Add(columnNum);

            DataGridViewTextBoxColumn columnNotes = new DataGridViewTextBoxColumn();
            columnNotes.Name = "columnNotes";
            columnNotes.DataPropertyName = "notes";//对应数据源的字段
            columnNotes.HeaderText = "注释";
            this.dataGridView.Columns.Add(columnNotes);
        }

        int function;
        bool changed = false;
        int currentSelectedIndex = -1;

        private string _configure;
        private List<string> _configures;
        private Collection __collection;

        string[] EnumNumber;

        private void ComboBoxGamesSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null && comboBox.SelectedIndex < 0) return;

            //function = (int)Enum.Parse(typeof(GameFun), comboBox.SelectedItem.ToString());
            function = (int)Enum.Parse(typeof(GameFun), EnumNumber[comboBox.SelectedIndex]);
            _configures = Directory.GetFiles(Application.StartupPath + "\\configure", string.Format("confure_{0}_*_*.cfg", function)).ToList();

            comboBoxConfigures.Items.Clear();
            foreach (var item in _configures)
                comboBoxConfigures.Items.Add(item.Replace(Application.StartupPath + "\\configure\\", ""));

            SaveButton.Enabled = false;
            AddtoGroupBox.Enabled = false;
        }

        private void ComboBoxConfiguresSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null || comboBox.SelectedIndex < 0) return;

            checkedListBox.Items.Clear();
            __collection = new Collection() { Editor = true };
            _configure = _configures[comboBox.SelectedIndex];
            __collection.LoadConfiguration(_configure);

            foreach (var item in __collection.Elements)
                checkedListBox.Items.Add(item.Name);

            SaveButton.Enabled = true;
            AddtoGroupBox.Enabled = true;
        }

        private void CheckedListBoxSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            CheckedListBox checkedListBox = sender as CheckedListBox;
            if (checkedListBox == null || checkedListBox.SelectedIndex < 0) return;

            // 值改变后保存
            if (changed)
            {
                Element el = __collection.Elements[currentSelectedIndex];

                el.Types.Clear();
                el.Fields.Clear();

                for (int i = 0; i < dataGridView.Rows.Count - 1; i++)
                {
                    var objs = dataGridView.Rows[i];
                    el.Fields.Add(objs.Cells[0].Value.ToString());
                    el.Types.Add(objs.Cells[1].Value.ToString());
                    //el.FieldSize.Add(objs.Cells[2].Value.ToString());
                }
                __collection.Elements[currentSelectedIndex] = el;

                changed = false;
            }

            dataGridView.Rows.Clear();

            // 针对完美跳过编辑
            if (checkedListBox.SelectedItem.ToString() == "SkinTag")
                return;

            // 针对对话路过编辑
            if (checkedListBox.SelectedItem.ToString() == "TalkProc")
                return;

            currentSelectedIndex = checkedListBox.SelectedIndex;
            Element element = __collection.Elements[currentSelectedIndex];

            for (int i = 0; i < element.Types.Count; i++)
            {
                int size;
                //if (checkBoxInitial.Checked)
                size = Extensions.GetTypeSize(element.Types[i]);
                //else size = int.Parse(element.FieldSize[i]);

                dataGridView.Rows.Add(
                    new object[]
                    {
                        element.Fields[i],
                        Extensions.GetTypeName(element.Types[i]).ToString(),
                        size
                    });
            }
        }

        private void DataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            changed = true;
        }

        private void SaveButtonClick(object sender, EventArgs eventArgs)
        {
            var newfile = string.Empty;
            for (int i = 1; i < 100; i++)
            {
                newfile = string.Format("{0}.{1}", _configure, i.ToString().PadLeft(4, '0'));
                if (!File.Exists(newfile))
                {
                    File.Move(_configure, newfile);
                    break;
                }
            }

            using (FileStream fileStream = new FileStream(_configure, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.WriteLine(__collection.Elements.Count);
                    streamWriter.WriteLine(__collection.TalkColumn);
                    streamWriter.WriteLine();

                    for (int i = 0; i < __collection.Elements.Count; i++)
                    {
                        string name = string.Format("{0} - {1}", i.ToString().PadLeft(3, '0'), __collection.Elements[i].Name);
                        streamWriter.WriteLine(name);

                        if (__collection.Elements[i].Name == "SkinTag")
                        {
                            streamWriter.WriteLine();
                            continue;
                        }

                        if (__collection.Elements[i].Name == "TalkProc")
                        {
                            streamWriter.WriteLine();
                            continue;
                        }

                        string fields = string.Empty;
                        for (int j = 0; j < __collection.Elements[i].Fields.Count; j++)
                        {
                            if (j == __collection.Elements[i].Fields.Count - 1)
                                fields += __collection.Elements[i].Fields[j];
                            else fields += string.Format("{0};", __collection.Elements[i].Fields[j]);
                        }

                        int size = 0;
                        string types = string.Empty;
                        //string fieldSize = string.Empty;

                        for (int j = 0; j < __collection.Elements[i].Types.Count; j++)
                        {
                            //if (j == __collection.Elements[i].Types.Count - 1)
                            //    types += Extensions.GetTypeName(__collection.Elements[i].Types[j]).ToString();
                            //else types += string.Format("{0};", Extensions.GetTypeName(__collection.Elements[i].Types[j]).ToString());

                            types += Extensions.GetTypeName(__collection.Elements[i].Types[j]).ToString();

                            if (types.EndsWith("string"))
                                types += string.Format(":{0}", Extensions.GetTypeSize(__collection.Elements[i].Types[j]));

                            if (j != __collection.Elements[i].Types.Count - 1)
                                types += string.Format(";");

                            //if (types.EndsWith("string;"))
                            //    types.Insert(types.Length - 1, string.Format(":{0}", Extensions.GetTypeSize(__collection.Elements[i].Types[j])));

                            size += Extensions.GetTypeSize(__collection.Elements[i].Types[j]);

                            //if (j == __collection.Elements[i].FieldSize.Count - 1)
                            //    fieldSize += Extensions.GetTypeSize(__collection.Elements[i].Types[j]);
                            //else fieldSize += string.Format("{0};", Extensions.GetTypeSize(__collection.Elements[i].Types[j]));
                        }

                        streamWriter.WriteLine(size);
                        //streamWriter.WriteLine(sizes[i]);
                        streamWriter.WriteLine(fields);
                        streamWriter.WriteLine(types);
                        //streamWriter.WriteLine(fieldSize);

                        if (i != __collection.Elements.Count - 1)
                            streamWriter.WriteLine();
                    }
                }
            }
        }

        private void AddtoButtonClick(object sender, EventArgs eventArgs)
        {
            if (AddtoList.Checked)
            {
                if (AddtoNew.Checked && AddtoName.Text.Trim() != string.Empty)
                {
                    __collection.Elements.Add(new Element()
                    {
                        Name = AddtoName.Text.Trim(),
                        Types = new List<string>(),
                        Fields = new List<string>(),
                        Values = new List<byte[]>()
                    });

                    checkedListBox.BeginUpdate();
                    int index = checkedListBox.Items.Add(AddtoName.Text.Trim());
                    checkedListBox.SelectedIndex = index;
                    checkedListBox.EndUpdate();
                }
                else
                {

                }
            }
        }

        /// <summary>
        /// 216
        /// </summary>
        int[] sizes = new int[] {
            68,
            68,
            68,
            68,
            68,
            68,
            68,
            68,
            100,
            772,
            1216,
            148,
            208,
            148,
            352,
            396,
            0,
            180,
            232,
            144,
            144,
            148,
            264,
            148,
            68,
            2120,
            68,
            1768,
            72,
            3280,
            1104,
            1092,
            1104,
            644,
            72,
            648,
            72,
            68,
            68,
            2080,
            4,
            248,
            328,
            200,
            200,
            1876,
            1420,
            228,
            4340,
            548,
            868,
            68,
            2048,
            0,
            0,
            196,
            148,
            1092,
            116,
            196,
            284,
            160,
            144,
            520,
            156,
            140,
            140,
            140,
            76,
            76,
            76,
            212,
            2752,
            152,
            1656,
            140,
            92,
            2600,
            664,
            0,
            0,
            0,
            0,
            144,
            200,
            536,
            236,
            388,
            136,
            68,
            1132,
            960,
            1036,
            164,
            168,
            776,
            740,
            216,
            4168,
            4172,
            148,
            876,
            172,
            164,
            168,
            156,
            160,
            192,
            232,
            160,
            204,
            192,
            208,
            324,
            752,
            1020,
            144,
            2068,
            156,
            184,
            836,
            236,
            252,
            244,
            836,
            264,
            524,
            152,
            448,
            628,
            1060,
            152,
            3400,
            360,
            612,
            3140,
            188,
            228,
            92,
            4804,
            1116,
            1564,
            172,
            148,
            544,
            1448,
            240,
            236,
            1992,
            768,
            1232,
            76,
            316,
            1040,
            292,
            1668,
            132,
            408,
            604,
            604,
            476,
            312,
            356,
            184,
            88,
            124,
            176,
            5316,
            672,
            176,
            204,
            140,
            152,
            152,
            152,
            276,
            132,
            236,
            208,
            160,
            240,
            292,
            144,
            92,
            2056,
            1264,
            860,
            144,
            188,
            104,
            152,
            140,
            372,
            144,
            152,
            148,
            648,
            1028,
            1380,
            156,
            104,
            148,
            816,
            180,
            408,
            88,
            2472,
            120,
            248,
            120,
            144,
            148,
            220,
            80,
            176,
            868,
            504,
            432,
            80,
            244,
            1888,
            136,
            148,
            868,
            472,
            868,
            124,
            148,
            144,
            2628,
            176,
            152,
            276,
            108,
            1072,
            176,
            144,
            164,
            3252,
            292,
            260,
            72,
            2068,
            148,
            176,
            280,
            120,
            116,
            96,

        };
    }
}
