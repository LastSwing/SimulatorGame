namespace ConclusionEditor
{
    partial class EndingControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridView3 = new System.Windows.Forms.DataGridView();
            this.dataSet1 = new System.Data.DataSet();
            this.dataTable1 = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataColumn4 = new System.Data.DataColumn();
            this.dataColumn5 = new System.Data.DataColumn();
            this.dataColumn6 = new System.Data.DataColumn();
            this.dataColumn7 = new System.Data.DataColumn();
            this.dataTable2 = new System.Data.DataTable();
            this.dataColumn8 = new System.Data.DataColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.jiejujieshaoDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.xingbiDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.xingliDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.shengchanDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable2)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView3
            // 
            this.dataGridView3.AutoGenerateColumns = false;
            this.dataGridView3.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView3.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column7,
            this.jiejujieshaoDataGridViewTextBoxColumn,
            this.xingbiDataGridViewTextBoxColumn,
            this.xingliDataGridViewTextBoxColumn,
            this.shengchanDataGridViewTextBoxColumn,
            this.Column10});
            this.dataGridView3.DataMember = "Table1";
            this.dataGridView3.DataSource = this.dataSet1;
            this.dataGridView3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView3.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView3.Location = new System.Drawing.Point(0, 0);
            this.dataGridView3.Name = "dataGridView3";
            this.dataGridView3.RowHeadersVisible = false;
            this.dataGridView3.RowTemplate.Height = 23;
            this.dataGridView3.Size = new System.Drawing.Size(751, 287);
            this.dataGridView3.TabIndex = 1;
            this.dataGridView3.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView3_CellContentClick);
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.dataTable1,
            this.dataTable2});
            // 
            // dataTable1
            // 
            this.dataTable1.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3,
            this.dataColumn4,
            this.dataColumn5,
            this.dataColumn6,
            this.dataColumn7});
            this.dataTable1.TableName = "Table1";
            this.dataTable1.RowChanged += new System.Data.DataRowChangeEventHandler(this.dataTable1_RowChanged);
            // 
            // dataColumn1
            // 
            this.dataColumn1.Caption = "发生年份";
            this.dataColumn1.ColumnName = "nianfen";
            // 
            // dataColumn2
            // 
            this.dataColumn2.Caption = "结局介绍";
            this.dataColumn2.ColumnName = "jiejujieshao";
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "星币";
            this.dataColumn3.ColumnName = "xingbi";
            // 
            // dataColumn4
            // 
            this.dataColumn4.Caption = "星力";
            this.dataColumn4.ColumnName = "xingli";
            // 
            // dataColumn5
            // 
            this.dataColumn5.Caption = "生产";
            this.dataColumn5.ColumnName = "shengchan";
            // 
            // dataColumn6
            // 
            this.dataColumn6.ColumnName = "CGuid";
            // 
            // dataColumn7
            // 
            this.dataColumn7.ColumnName = "PGuid";
            // 
            // dataTable2
            // 
            this.dataTable2.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn8});
            this.dataTable2.TableName = "Table2";
            // 
            // dataColumn8
            // 
            this.dataColumn8.Caption = "nianfen";
            this.dataColumn8.ColumnName = "nianfen";
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "nianfen";
            this.Column7.DataSource = this.dataSet1;
            this.Column7.DisplayMember = "Table2.nianfen";
            this.Column7.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Column7.HeaderText = "发生年份";
            this.Column7.MaxDropDownItems = 10;
            this.Column7.Name = "Column7";
            // 
            // jiejujieshaoDataGridViewTextBoxColumn
            // 
            this.jiejujieshaoDataGridViewTextBoxColumn.DataPropertyName = "jiejujieshao";
            this.jiejujieshaoDataGridViewTextBoxColumn.HeaderText = "结局介绍";
            this.jiejujieshaoDataGridViewTextBoxColumn.Name = "jiejujieshaoDataGridViewTextBoxColumn";
            this.jiejujieshaoDataGridViewTextBoxColumn.Width = 403;
            // 
            // xingbiDataGridViewTextBoxColumn
            // 
            this.xingbiDataGridViewTextBoxColumn.DataPropertyName = "xingbi";
            this.xingbiDataGridViewTextBoxColumn.HeaderText = "星币";
            this.xingbiDataGridViewTextBoxColumn.Name = "xingbiDataGridViewTextBoxColumn";
            this.xingbiDataGridViewTextBoxColumn.Width = 70;
            // 
            // xingliDataGridViewTextBoxColumn
            // 
            this.xingliDataGridViewTextBoxColumn.DataPropertyName = "xingli";
            this.xingliDataGridViewTextBoxColumn.HeaderText = "星力";
            this.xingliDataGridViewTextBoxColumn.Name = "xingliDataGridViewTextBoxColumn";
            this.xingliDataGridViewTextBoxColumn.Width = 70;
            // 
            // shengchanDataGridViewTextBoxColumn
            // 
            this.shengchanDataGridViewTextBoxColumn.DataPropertyName = "shengchan";
            this.shengchanDataGridViewTextBoxColumn.HeaderText = "生产";
            this.shengchanDataGridViewTextBoxColumn.Name = "shengchanDataGridViewTextBoxColumn";
            this.shengchanDataGridViewTextBoxColumn.Width = 70;
            // 
            // Column10
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = "-";
            this.Column10.DefaultCellStyle = dataGridViewCellStyle1;
            this.Column10.HeaderText = "删";
            this.Column10.Name = "Column10";
            this.Column10.Width = 35;
            // 
            // EndingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView3);
            this.Name = "EndingControl";
            this.Size = new System.Drawing.Size(751, 287);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataTable2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView3;
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable dataTable1;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Data.DataColumn dataColumn4;
        private System.Data.DataColumn dataColumn5;
        private System.Data.DataColumn dataColumn6;
        private System.Data.DataColumn dataColumn7;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column7;
        private System.Data.DataTable dataTable2;
        private System.Data.DataColumn dataColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn jiejujieshaoDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn xingbiDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn xingliDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn shengchanDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewButtonColumn Column10;
    }
}
