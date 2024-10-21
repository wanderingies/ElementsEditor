
namespace ElementsEditor.Forms
{
    partial class EditorForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBoxGames = new System.Windows.Forms.ComboBox();
            this.comboBoxConfigures = new System.Windows.Forms.ComboBox();
            this.checkedListBox = new System.Windows.Forms.CheckedListBox();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.AddtoGroupBox = new System.Windows.Forms.GroupBox();
            this.AddtoButton = new System.Windows.Forms.Button();
            this.AddtoPropertyType = new System.Windows.Forms.ComboBox();
            this.AddtoPropertyNotes = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.AddtoName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.AddtoInsertList = new System.Windows.Forms.ComboBox();
            this.AddtoNew = new System.Windows.Forms.RadioButton();
            this.AddtoInsert = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AddtoList = new System.Windows.Forms.RadioButton();
            this.AddtoProperty = new System.Windows.Forms.RadioButton();
            this.SaveButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.AddtoGroupBox.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxGames
            // 
            this.comboBoxGames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxGames.FormattingEnabled = true;
            this.comboBoxGames.Location = new System.Drawing.Point(12, 12);
            this.comboBoxGames.Name = "comboBoxGames";
            this.comboBoxGames.Size = new System.Drawing.Size(200, 20);
            this.comboBoxGames.TabIndex = 0;
            // 
            // comboBoxConfigures
            // 
            this.comboBoxConfigures.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxConfigures.FormattingEnabled = true;
            this.comboBoxConfigures.Location = new System.Drawing.Point(12, 38);
            this.comboBoxConfigures.Name = "comboBoxConfigures";
            this.comboBoxConfigures.Size = new System.Drawing.Size(154, 20);
            this.comboBoxConfigures.TabIndex = 1;
            // 
            // checkedListBox
            // 
            this.checkedListBox.FormattingEnabled = true;
            this.checkedListBox.Location = new System.Drawing.Point(12, 64);
            this.checkedListBox.Name = "checkedListBox";
            this.checkedListBox.Size = new System.Drawing.Size(200, 372);
            this.checkedListBox.TabIndex = 2;
            // 
            // dataGridView
            // 
            this.dataGridView.BackgroundColor = System.Drawing.Color.White;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Location = new System.Drawing.Point(218, 12);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowTemplate.Height = 23;
            this.dataGridView.Size = new System.Drawing.Size(570, 332);
            this.dataGridView.TabIndex = 3;
            // 
            // AddtoGroupBox
            // 
            this.AddtoGroupBox.Controls.Add(this.AddtoButton);
            this.AddtoGroupBox.Controls.Add(this.AddtoPropertyType);
            this.AddtoGroupBox.Controls.Add(this.AddtoPropertyNotes);
            this.AddtoGroupBox.Controls.Add(this.label3);
            this.AddtoGroupBox.Controls.Add(this.label2);
            this.AddtoGroupBox.Controls.Add(this.AddtoName);
            this.AddtoGroupBox.Controls.Add(this.label1);
            this.AddtoGroupBox.Controls.Add(this.panel2);
            this.AddtoGroupBox.Controls.Add(this.panel1);
            this.AddtoGroupBox.Enabled = false;
            this.AddtoGroupBox.Location = new System.Drawing.Point(218, 350);
            this.AddtoGroupBox.Name = "AddtoGroupBox";
            this.AddtoGroupBox.Size = new System.Drawing.Size(570, 86);
            this.AddtoGroupBox.TabIndex = 4;
            this.AddtoGroupBox.TabStop = false;
            this.AddtoGroupBox.Text = "增加项";
            // 
            // AddtoButton
            // 
            this.AddtoButton.Location = new System.Drawing.Point(480, 54);
            this.AddtoButton.Name = "AddtoButton";
            this.AddtoButton.Size = new System.Drawing.Size(75, 23);
            this.AddtoButton.TabIndex = 11;
            this.AddtoButton.Text = "提 交";
            this.AddtoButton.UseVisualStyleBackColor = true;
            // 
            // AddtoPropertyType
            // 
            this.AddtoPropertyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AddtoPropertyType.Enabled = false;
            this.AddtoPropertyType.FormattingEnabled = true;
            this.AddtoPropertyType.Location = new System.Drawing.Point(404, 22);
            this.AddtoPropertyType.Name = "AddtoPropertyType";
            this.AddtoPropertyType.Size = new System.Drawing.Size(151, 20);
            this.AddtoPropertyType.TabIndex = 10;
            // 
            // AddtoPropertyNotes
            // 
            this.AddtoPropertyNotes.Enabled = false;
            this.AddtoPropertyNotes.Location = new System.Drawing.Point(233, 55);
            this.AddtoPropertyNotes.Name = "AddtoPropertyNotes";
            this.AddtoPropertyNotes.Size = new System.Drawing.Size(241, 21);
            this.AddtoPropertyNotes.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(198, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "注释";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(369, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "类型";
            // 
            // AddtoName
            // 
            this.AddtoName.Location = new System.Drawing.Point(51, 55);
            this.AddtoName.Name = "AddtoName";
            this.AddtoName.Size = new System.Drawing.Size(141, 21);
            this.AddtoName.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "名称";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.AddtoInsertList);
            this.panel2.Controls.Add(this.AddtoNew);
            this.panel2.Controls.Add(this.AddtoInsert);
            this.panel2.Location = new System.Drawing.Point(128, 20);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(233, 24);
            this.panel2.TabIndex = 3;
            // 
            // AddtoInsertList
            // 
            this.AddtoInsertList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AddtoInsertList.Enabled = false;
            this.AddtoInsertList.FormattingEnabled = true;
            this.AddtoInsertList.Location = new System.Drawing.Point(109, 1);
            this.AddtoInsertList.Name = "AddtoInsertList";
            this.AddtoInsertList.Size = new System.Drawing.Size(121, 20);
            this.AddtoInsertList.TabIndex = 4;
            // 
            // AddtoNew
            // 
            this.AddtoNew.AutoSize = true;
            this.AddtoNew.Checked = true;
            this.AddtoNew.Location = new System.Drawing.Point(3, 3);
            this.AddtoNew.Name = "AddtoNew";
            this.AddtoNew.Size = new System.Drawing.Size(47, 16);
            this.AddtoNew.TabIndex = 2;
            this.AddtoNew.TabStop = true;
            this.AddtoNew.Text = "新增";
            this.AddtoNew.UseVisualStyleBackColor = true;
            // 
            // AddtoInsert
            // 
            this.AddtoInsert.AutoSize = true;
            this.AddtoInsert.Location = new System.Drawing.Point(56, 3);
            this.AddtoInsert.Name = "AddtoInsert";
            this.AddtoInsert.Size = new System.Drawing.Size(47, 16);
            this.AddtoInsert.TabIndex = 3;
            this.AddtoInsert.Text = "插入";
            this.AddtoInsert.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.AddtoList);
            this.panel1.Controls.Add(this.AddtoProperty);
            this.panel1.Location = new System.Drawing.Point(14, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(108, 24);
            this.panel1.TabIndex = 2;
            // 
            // AddtoList
            // 
            this.AddtoList.AutoSize = true;
            this.AddtoList.Checked = true;
            this.AddtoList.Location = new System.Drawing.Point(3, 3);
            this.AddtoList.Name = "AddtoList";
            this.AddtoList.Size = new System.Drawing.Size(47, 16);
            this.AddtoList.TabIndex = 0;
            this.AddtoList.TabStop = true;
            this.AddtoList.Text = "列表";
            this.AddtoList.UseVisualStyleBackColor = true;
            // 
            // AddtoProperty
            // 
            this.AddtoProperty.AutoSize = true;
            this.AddtoProperty.Location = new System.Drawing.Point(56, 3);
            this.AddtoProperty.Name = "AddtoProperty";
            this.AddtoProperty.Size = new System.Drawing.Size(47, 16);
            this.AddtoProperty.TabIndex = 1;
            this.AddtoProperty.Text = "属性";
            this.AddtoProperty.UseVisualStyleBackColor = true;
            // 
            // SaveButton
            // 
            this.SaveButton.Enabled = false;
            this.SaveButton.Location = new System.Drawing.Point(172, 38);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(40, 20);
            this.SaveButton.TabIndex = 5;
            this.SaveButton.Text = "保存";
            this.SaveButton.UseVisualStyleBackColor = true;
            // 
            // EditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.AddtoGroupBox);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.checkedListBox);
            this.Controls.Add(this.comboBoxConfigures);
            this.Controls.Add(this.comboBoxGames);
            this.Name = "EditorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EditorForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.AddtoGroupBox.ResumeLayout(false);
            this.AddtoGroupBox.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxGames;
        private System.Windows.Forms.ComboBox comboBoxConfigures;
        private System.Windows.Forms.CheckedListBox checkedListBox;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.GroupBox AddtoGroupBox;
        private System.Windows.Forms.ComboBox AddtoPropertyType;
        private System.Windows.Forms.TextBox AddtoPropertyNotes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox AddtoName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox AddtoInsertList;
        private System.Windows.Forms.RadioButton AddtoNew;
        private System.Windows.Forms.RadioButton AddtoInsert;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton AddtoList;
        private System.Windows.Forms.RadioButton AddtoProperty;
        private System.Windows.Forms.Button AddtoButton;
        private System.Windows.Forms.Button SaveButton;
    }
}