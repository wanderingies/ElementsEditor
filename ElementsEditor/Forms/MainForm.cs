using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ElementsEditor.Template;
using ElementsEditor.Utility;

namespace ElementsEditor.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            ToolStripMenuItem menuItem;
            var encodings = new List<string>() { "GBK", "Unicode" };
            foreach (var item in encodings)
            {
                menuItem = new ToolStripMenuItem();
                menuItem.Name = item;
                menuItem.Text = item;
                menuItem.Click += ToolStripMenuItemEncodingClick;
                ToolStripMenuItemEncoding.DropDownItems.Add(menuItem);
            }

            Element.Encoding = Encoding.Unicode;
            (ToolStripMenuItemEncoding.DropDownItems[1] as ToolStripMenuItem).Checked = true;

            toolStripButtonOpenFile.Click += ToolStripMenuItemOpenFileClick;
            ToolStripMenuItemOpenFile.Click += ToolStripMenuItemOpenFileClick;

            toolStripButtonConfigureEditor.Click += ToolStripMenuItemConfigureEditorClick;
            ToolStripMenuItemConfigureEditor.Click += ToolStripMenuItemConfigureEditorClick;
            ToolStripMenuItemGenerateConfigure.Click += ToolStripMenuItemGenerateConfigureClick;

            comboBox.SelectedIndexChanged += ComboBoxSelectedIndexChanged;
            checkedListBox.SelectedIndexChanged += CheckedListBoxSelectedIndexChanged;

            ToolStripMenuItemViewVertical.Click += ToolStripMenuItemViewClick;
            ToolStripMenuItemViewHorizontal.Click += ToolStripMenuItemViewClick;

            toolStripButtonExport.Click += toolStripButtonExportClick;
        }

        private Collection __collection;
        private Element __CurrentElement;
        public int _ComboBoxSelectedIndex = -1;

        #region Events        

        private void ComboBoxSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox == null || comboBox.SelectedIndex < 0) return;

            __CurrentElement = __collection.Elements[comboBox.SelectedIndex];

            dataGridView.Rows.Clear();
            checkedListBox.Items.Clear();
            
            for (int i = 0; i < __CurrentElement.Values.Count; i++)
            {
                if (__CurrentElement.Name == "SkinTag")
                    continue;
                else if (__CurrentElement.Name == "SkinHash")
                    continue;
                else if (__CurrentElement.Name == "SkinMeta")
                    continue;
                else checkedListBox.Items.Add(Encoding.Unicode.GetString(__CurrentElement.Values[i].Skip(__CurrentElement.SkinPostion).ToArray()));
                //else checkedListBox.Items.Add(Extensions.GetValues(__CurrentElement.Values[i], __CurrentElement.Types[__CurrentElement.ShowPostion], ref __CurrentElement.SkinPostion));                
            }
        }

        private void CheckedListBoxSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            CheckedListBox checkedListBox = sender as CheckedListBox;
            if (checkedListBox == null || comboBox.SelectedIndex < 0 || checkedListBox.SelectedIndex < 0) return;

            // 横板和竖板切换开关
            if (ToolStripMenuItemViewVertical.Checked)
            {
                dataGridView.Rows.Clear();
                if (comboBox.SelectedIndex != _ComboBoxSelectedIndex || dataGridView.Columns.Count <= 0)
                {
                    _ComboBoxSelectedIndex = comboBox.SelectedIndex;
                    dataGridView.Columns.Clear();
                    dataGridView.Columns.Add("ColumnField", "Field");
                    dataGridView.Columns.Add("ColumnType", "Type");
                    //dataGridView.Columns.Add("ColumnValue", "Value");

                    DataGridViewTextBoxColumn ColumnValue = new DataGridViewTextBoxColumn();
                    ColumnValue.Name = "ColumnValue";
                    //columnNotes.DataPropertyName = "notes";//对应数据源的字段
                    ColumnValue.HeaderText = "Value";
                    ColumnValue.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    dataGridView.Columns.Add(ColumnValue);
                }

                int skin = 0;
                List<object> values = new List<object>();
                for (int i = 0; i < __CurrentElement.Types.Count; i++)
                {
                    values.Add(__CurrentElement.Fields[i]);
                    values.Add(__CurrentElement.Types[i]);
                    values.Add(__CurrentElement.Values[checkedListBox.SelectedIndex].GetValues(__CurrentElement.Types[i], ref skin));
                    dataGridView.Rows.Add(values.ToArray());
                    values.Clear();
                }

                dataGridView.Refresh();
            }
            else
            {
                if (comboBox.SelectedIndex != _ComboBoxSelectedIndex || dataGridView.Columns.Count <= 0)
                {
                    _ComboBoxSelectedIndex = comboBox.SelectedIndex;
                    dataGridView.Columns.Clear();
                    foreach (var item in __CurrentElement.Fields)
                        dataGridView.Columns.Add(item, item);
                }                

                dataGridView.Rows.Clear();
                var value = __CurrentElement.Values[checkedListBox.SelectedIndex];
                {
                    int skin = 0;
                    List<object> values = new List<object>();
                    foreach (var item in __CurrentElement.Types)
                        values.Add(value.GetValues(item, ref skin));

                    dataGridView.Rows.Add(values.ToArray());
                    dataGridView.Refresh();
                }
            }                      
        }
        #endregion

        #region Menu Events

        private void ToolStripMenuItemViewClick(object sender, EventArgs eventArgs)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem.Text == "横板")
            {
                ToolStripMenuItemViewVertical.Checked = false;
                ToolStripMenuItemViewHorizontal.Checked = true;
            }
            else if (menuItem.Text == "竖板")
            {
                ToolStripMenuItemViewVertical.Checked = true;
                ToolStripMenuItemViewHorizontal.Checked = false;
            }

            _ComboBoxSelectedIndex = -1;
        }

        private void ToolStripMenuItemOpenFileClick(object sender, EventArgs eventArgs)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Elements File (elements.data)|*.data|All Files (*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK && File.Exists(dialog.FileName))
            {
                progressBar.Style = ProgressBarStyle.Continuous;
                __collection = new Collection(dialog.FileName, ref progressBar);

                foreach (var item in __collection.Elements)
                    comboBox.Items.Add(item.Name);
            }
        }

        private void ToolStripMenuItemEncodingClick(object sender, EventArgs eventArgs)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

            foreach (ToolStripMenuItem item in ToolStripMenuItemEncoding.DropDownItems)
                item.Checked = false;

            menuItem.Checked = true;
        }

        private void ToolStripMenuItemConfigureEditorClick(object sender, EventArgs eventArgs)
        {
            EditorForm editor = new EditorForm();
            editor.Show();
        }
        private void ToolStripMenuItemGenerateConfigureClick(object sender, EventArgs eventArgs)
        {
            /*OpenFileDialog dialog = new OpenFileDialog();
            dialog.FileName = "elements.data";

            var names = Enum.GetNames(typeof(Utility.Function));
            foreach (var item in names)
                dialog.Filter += string.Format("{0}|*.data|", item);

            if (dialog.ShowDialog() == DialogResult.OK)
            {

            }*/
        }

        private void toolStripButtonExportClick(object sender, EventArgs eventArgs)
        {
            if (__CurrentElement.Name == "SkinTag")
                return;
            else if (__CurrentElement.Name == "SkinHash")
                return;
            else if (__CurrentElement.Name == "SkinMeta")
                return;

            bool saveAll = false;
            if (comboBox.SelectedIndex < 0)
            {
                string message = "当前未选需要导出的列，是否导出全部列?\n\n是: 导出全部列\n否: 返回选择列后再导出";
                string caption = "警告";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(this, message, caption, buttons,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign);

                if (result == DialogResult.No)
                    return;
                else saveAll = true;
            }

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() != DialogResult.OK) return;

            if (saveAll)
            {


                // 输出列

                // 输出类型

                // 输出大小

                // 输出数据

                return;
            }

            progressBar.Value = 0;
            progressBar.Maximum = __CurrentElement.Values.Count;
            string name = __CurrentElement.Name;
            using (StreamWriter writer = new StreamWriter(Path.Combine(dialog.SelectedPath, name)))
            {
                string types = string.Empty;
                for (int i = 0; i < __CurrentElement.Types.Count; i++)
                {
                    if (i == __CurrentElement.Types.Count - 1)
                        types += __CurrentElement.Types[i];
                    else types += string.Format("{0};", __CurrentElement.Types[i]);
                }

                string fields = string.Empty;
                for (int i = 0; i < __CurrentElement.Fields.Count; i++)
                {
                    if (i == __CurrentElement.Fields.Count - 1)
                        fields += __CurrentElement.Fields[i];
                    else fields += string.Format("{0};", __CurrentElement.Fields[i]);
                }

                writer.WriteLine(fields);
                writer.WriteLine(types);

                string values = string.Empty;
                for (int i = 0; i < __CurrentElement.Values.Count; i++)
                {
                    int skin = 0;
                    var value = __CurrentElement.Values[i];
                    for (int j = 0; j < __CurrentElement.Types.Count; j++)
                    {
                        var item = __CurrentElement.Types[j];
                        if (j == __CurrentElement.Types.Count - 1)
                            values += value.GetValues(item, ref skin);
                        else values += string.Format("{0};", value.GetValues(item, ref skin));
                    }

                    writer.WriteLine(values);
                    values = string.Empty;

                    progressBar.Value++;
                }
            }
        }
        #endregion
    }
}
