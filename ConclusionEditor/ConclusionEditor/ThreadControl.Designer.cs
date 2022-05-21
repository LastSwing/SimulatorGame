namespace ConclusionEditor
{
    partial class ThreadControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataSet1 = new System.Data.DataSet();
            this.jueseTable = new System.Data.DataTable();
            this.dataColumn5 = new System.Data.DataColumn();
            this.jueseid = new System.Data.DataColumn();
            this.duihuatable = new System.Data.DataTable();
            this.dataColumn1 = new System.Data.DataColumn();
            this.dataColumn2 = new System.Data.DataColumn();
            this.dataColumn3 = new System.Data.DataColumn();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Column7 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Add = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Column10 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.zhi = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.jueseTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.duihuatable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataSet1
            // 
            this.dataSet1.DataSetName = "NewDataSet";
            this.dataSet1.Tables.AddRange(new System.Data.DataTable[] {
            this.jueseTable,
            this.duihuatable});
            // 
            // jueseTable
            // 
            this.jueseTable.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn5,
            this.jueseid});
            this.jueseTable.TableName = "jueseTable";
            // 
            // dataColumn5
            // 
            this.dataColumn5.Caption = "角色";
            this.dataColumn5.ColumnName = "juese";
            // 
            // jueseid
            // 
            this.jueseid.ColumnName = "jueseid";
            // 
            // duihuatable
            // 
            this.duihuatable.Columns.AddRange(new System.Data.DataColumn[] {
            this.dataColumn1,
            this.dataColumn2,
            this.dataColumn3});
            this.duihuatable.TableName = "duihuatable";
            this.duihuatable.RowChanged += new System.Data.DataRowChangeEventHandler(this.duihuatable_RowChanged);
            // 
            // dataColumn1
            // 
            this.dataColumn1.Caption = "juese";
            this.dataColumn1.ColumnName = "juese";
            // 
            // dataColumn2
            // 
            this.dataColumn2.ColumnName = "duihua";
            // 
            // dataColumn3
            // 
            this.dataColumn3.Caption = "Guid";
            this.dataColumn3.ColumnName = "Guid";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column7,
            this.Column8,
            this.Add,
            this.Column10,
            this.zhi});
            this.dataGridView1.DataMember = "duihuatable";
            this.dataGridView1.DataSource = this.dataSet1;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(680, 405);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick_1);
            // 
            // Column7
            // 
            this.Column7.DataPropertyName = "juese";
            this.Column7.DataSource = this.dataSet1;
            this.Column7.DisplayMember = "juesetable.juese";
            this.Column7.DisplayStyle = System.Windows.Forms.DataGridViewComboBoxDisplayStyle.ComboBox;
            this.Column7.HeaderText = "角色";
            this.Column7.MaxDropDownItems = 10;
            this.Column7.Name = "Column7";
            this.Column7.ValueMember = "juesetable.juese";
            // 
            // Column8
            // 
            this.Column8.DataPropertyName = "duihua";
            this.Column8.HeaderText = "对话";
            this.Column8.Name = "Column8";
            this.Column8.Width = 457;
            // 
            // Add
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.NullValue = "+";
            this.Add.DefaultCellStyle = dataGridViewCellStyle7;
            this.Add.HeaderText = "加";
            this.Add.Name = "Add";
            this.Add.Text = "+";
            this.Add.Width = 35;
            // 
            // Column10
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.NullValue = "-";
            this.Column10.DefaultCellStyle = dataGridViewCellStyle8;
            this.Column10.HeaderText = "删";
            this.Column10.Name = "Column10";
            this.Column10.Width = 35;
            // 
            // zhi
            // 
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.NullValue = "分支";
            this.zhi.DefaultCellStyle = dataGridViewCellStyle9;
            this.zhi.HeaderText = "支";
            this.zhi.Name = "zhi";
            this.zhi.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.zhi.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.zhi.Text = "";
            this.zhi.Width = 50;
            // 
            // ThreadControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.Name = "ThreadControl";
            this.Size = new System.Drawing.Size(680, 405);
            this.Load += new System.EventHandler(this.ThreadControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataSet1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.jueseTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.duihuatable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Data.DataSet dataSet1;
        private System.Data.DataTable jueseTable;
        private System.Data.DataColumn dataColumn5;
        private System.Data.DataColumn jueseid;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Data.DataTable duihuatable;
        private System.Data.DataColumn dataColumn1;
        private System.Data.DataColumn dataColumn2;
        private System.Data.DataColumn dataColumn3;
        private System.Windows.Forms.DataGridViewComboBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewButtonColumn Add;
        private System.Windows.Forms.DataGridViewButtonColumn Column10;
        private System.Windows.Forms.DataGridViewButtonColumn zhi;
    }
}
