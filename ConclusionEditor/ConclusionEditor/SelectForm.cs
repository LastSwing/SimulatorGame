using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ConclusionEditor
{
    public partial class SelectForm : Form
    {
        Guid Guid = Guid.Empty;
        DataTable JueseTable = new DataTable();
        private Dictionary<Guid, Dictionary<Guid, string>> duihuadic = new Dictionary<Guid, Dictionary<Guid, string>>();
        public Dictionary<Guid, Dictionary<Guid, string>> duihuaDic { get { return duihuadic; } set{ duihuadic = value; } }
        private List<Fileid> fileids1 = new List<Fileid>();
        private DataTable JieJuTable = new DataTable();
        public List<Fileid> fileids { get { return fileids1; } set { fileids1 = value; } }
        private bool state = false;//判断是选择输入还是字段输入 false选择输入
        public SelectForm(Guid guid,DataTable juesetable, Dictionary<Guid, Dictionary<Guid, string>> keyValues, List<Fileid> fileids2,DataTable jiejuTable)
        {
            InitializeComponent();
            Guid = guid;
            
            JueseTable.Merge(juesetable);
            duihuadic = keyValues;
            fileids1 = fileids2;
            JieJuTable.Merge(jiejuTable);
        }
        private void ThreadTable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (dataTable1.Rows.Count == 0 || dataTable1.Rows.Count == tabControl1.TabPages.Count) return;
            if (dataTable1.Rows.Count == 0) return;
            if (dataTable1.Rows.Count == tabControl1.TabCount) return;
            DataRow dataRow = e.Row;
            if(dataRow["Guid"].ToString() != "")
            {
                ThreadControl thread = new ThreadControl();
                thread.CGuid = new Guid(dataRow["Guid"].ToString());
                thread.Number = dataTable1.Rows.Count;
                thread.RoleTable.Rows.Clear();
                thread.RoleTable.Merge(JueseTable);
                thread.JiejuTable.Merge(JieJuTable);
                thread.duihuaDic = duihuadic;
                thread.fileids = fileids1;
                TabPage tabPage = new TabPage();
                tabPage.Controls.Add(thread);
                tabPage.Text = "选择" + dataTable1.Rows.Count;
                tabControl1.TabPages.Add(tabPage);
            }
            else
            {
                for (int i = 0; i < dataTable1.Rows.Count; i++)
                {
                    Guid guid = Guid.NewGuid();
                    if (dataTable1.Rows[i]["Guid"] == null || dataTable1.Rows[i]["Guid"].ToString() == "")
                    {
                        TabPage tabPage = new TabPage();
                        tabPage.Text = "选择" + dataTable1.Rows.Count;
                        tabControl1.TabPages.Add(tabPage);
                        dataTable1.Rows[i]["Guid"] = guid;
                        ThreadControl thread = new ThreadControl();
                        thread.CGuid = guid;
                        thread.Number = dataTable1.Rows.Count;
                        thread.RoleTable.Rows.Clear();
                        thread.RoleTable.Merge(JueseTable);
                        thread.JiejuTable.Merge(JieJuTable);
                        thread.duihuaDic = duihuadic;
                        thread.fileids = fileids1;
                        tabPage.Controls.Add(thread);
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (TabPage page in tabControl1.TabPages)
            {
                ThreadControl thread = (ThreadControl)page.Controls[0];
                if (thread != null)
                {
                    duihuadic = thread.duihuaDic;
                    fileids1 = thread.fileids;
                }
            }
            for (int i = 0; i < dataTable1.Rows.Count; i++)
            {
                Fileid fileid = new Fileid();
                if (radioButton1.Checked)
                    fileid.InsertByte = 0;
                else
                    fileid.InsertByte = 1;
                if(state)
                    fileid.Fileidtype = FileidType.判断对话;
                else
                    fileid.Fileidtype = FileidType.选择;
                fileid.ParentId = Guid;
                fileid.PathName = dataTable1.Rows[i]["xuanze"].ToString();
                if (dataTable1.Rows[i]["Guid"].ToString() == "")
                    dataTable1.Rows[i]["Guid"] = Guid.NewGuid();
                fileid.Id = new Guid(dataTable1.Rows[i]["Guid"].ToString());
                for (int j = 0; j < fileids1.Count; j++)
                {
                    if (fileids1[j].Id == fileid.Id)
                        fileids1.Remove(fileids1[j]);
                }
                fileids1.Add(fileid);
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            DataGridView view = sender as DataGridView;
            DataGridViewColumn columns = view.Columns[e.ColumnIndex];
            if (view.RowCount - 1 == e.RowIndex)
                return;
            if (columns.HeaderText == "删")
            {
                bool ischange = false;
                TabPage tabPage = new TabPage();
                foreach (TabPage item in tabControl1.TabPages)
                {
                    if (!ischange)
                    {
                        string str1 = "选择" + (e.RowIndex + 1);
                        if (item.Text.Equals(str1))
                        {
                            tabPage = item;
                            ischange = true;
                        }
                    }
                    else
                    {
                        item.Text = "选择" + (Convert.ToInt32(item.Text.Replace("选择", "")) - 1);
                        ThreadControl thread = item.Controls[0] as ThreadControl;
                        thread.Number--;
                    }
                }
                tabControl1.TabPages.Remove(tabPage);
                view.Rows.Remove(view.Rows[e.RowIndex]);
                Guid guid = Guid.NewGuid();
            }
        }

        private void SelectForm_Load(object sender, EventArgs e)
        {
            bool aa = false;
            foreach (var item in fileids1)
            {
                if (item.ParentId == Guid && item.Fileidtype == FileidType.判断对话)
                {
                    state = true;
                    if (item.InsertByte == 1)
                        radioButton1.Checked = false;
                    DataRow dataRow = dataTable1.NewRow();
                    dataRow["Guid"] = item.Id;
                    dataRow["xuanze"] = item.PathName;
                    dataTable1.Rows.Add(dataRow);
                    aa = true;
                    continue;
                }
                if (item.ParentId == Guid && item.Fileidtype == FileidType.选择)
                {
                    if (item.InsertByte == 1)
                        radioButton1.Checked = false;
                    DataRow dataRow = dataTable1.NewRow();
                    dataRow["Guid"] = item.Id;
                    dataRow["xuanze"] = item.PathName;
                    dataTable1.Rows.Add(dataRow);
                }
            }
            if (!aa)
            {
                foreach (var item in fileids1)
                {
                    if (item.Id == Guid && item.Fileidtype == FileidType.字段)
                    {
                        state = true;
                        foreach (var items in item.Fileids)
                        {
                            DataRow dataRow = dataTable1.NewRow();
                            dataRow["Guid"] = Guid.NewGuid();
                            dataRow["xuanze"] = items;
                            dataTable1.Rows.Add(dataRow);
                        }
                    }
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                radioButton2.Checked = false;
            else
                radioButton2.Checked = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                radioButton1.Checked = false;
            else
                radioButton1.Checked = true;
        }
    }
}
