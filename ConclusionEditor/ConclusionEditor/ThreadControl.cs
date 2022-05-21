using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ConclusionEditor
{
    public partial class ThreadControl : UserControl
    {
        public Guid CGuid { get; set; }
        public List<Fileid> fileids = new List<Fileid>();
        private Dictionary<Guid, Dictionary<Guid, string>> duihua = new Dictionary<Guid, Dictionary<Guid, string>>();
        public Dictionary<Guid, Dictionary<Guid, string>> duihuaDic 
        {
            get
            {
                duihua.Remove(CGuid);
                Dictionary<Guid, string> dic = new Dictionary<Guid, string>();
                for (int i = 0; i < duihuatable.Rows.Count; i++)
                {
                    if (duihuatable.Rows[i]["Guid"].ToString() == "")
                        duihuatable.Rows[i]["Guid"] = Guid.NewGuid();
                    dic.Add(new Guid(duihuatable.Rows[i]["Guid"].ToString()), duihuatable.Rows[i]["juese"].ToString() + "|" + duihuatable.Rows[i]["duihua"].ToString());
                }
                duihua.Add(CGuid, dic);
                return duihua;
            }
            set
            {
                duihua = value;
                Dictionary<Guid, string> valuePairs = new Dictionary<Guid, string>();
                if (duihua.TryGetValue(CGuid,out valuePairs))
                {
                    foreach (var item in valuePairs)
                    {
                        bool a = true;
                        for (int i = 0; i < duihuatable.Rows.Count; i++)
                        {
                            if (duihuatable.Rows[i]["Guid"].ToString() == item.Key.ToString())
                                a = false;
                        }
                        if (a)
                        {
                            DataRow dataRow = duihuatable.NewRow();
                            dataRow["Guid"] = item.Key;
                            string[] strinigs = item.Value.Split('|');
                            dataRow["juese"] = strinigs[0];
                            dataRow["duihua"] = strinigs[1];
                            duihuatable.Rows.Add(dataRow);
                        }
                    }
                }
            }
        }
        public ThreadControl()
        {
            InitializeComponent();
        }

        private void ThreadControl_Load(object sender, EventArgs e)
        {

        }
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView view = sender as DataGridView;
            DataGridViewColumn columns = view.Columns[e.ColumnIndex];
            if (columns.HeaderText == "删")
            {
                if (view.RowCount - 1 == e.RowIndex)
                    return;
                view.Rows.Remove(view.Rows[e.RowIndex]);
            }
        }

        public DataTable RoleTable 
        {
            get
            {
                return jueseTable;
            }
            set
            {
                this.jueseTable.Rows.Clear();
                this.jueseTable.Merge(RoleTable);
            }
        }

        public int Number { get; set; }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            DataGridView view = sender as DataGridView;
            DataGridViewColumn columns = view.Columns[e.ColumnIndex];
            if (view.RowCount - 1 == e.RowIndex || duihuatable.Rows.Count == 0)
                return;
            if (columns.HeaderText == "删")
            {
                view.Rows.Remove(view.Rows[e.RowIndex]);
            }
            else if (columns.HeaderText == "加")
            {
                if (duihuatable.Rows[e.RowIndex]["Guid"].ToString() == "")
                    duihuatable.Rows[e.RowIndex]["Guid"] = Guid.NewGuid();
                Guid guid = new Guid(duihuatable.Rows[e.RowIndex]["Guid"].ToString());
                AddbranchForm addbranchForm = new AddbranchForm(guid, duihuatable.Rows[e.RowIndex]["duihua"].ToString(), jueseTable);
                addbranchForm.fileid = fileids;
                addbranchForm.duihuaDic = duihuaDic;
                addbranchForm.ShowDialog();
                if (addbranchForm.DialogResult == DialogResult.OK)
                {
                    fileids = addbranchForm.fileid;
                    duihuaDic = addbranchForm.duihuaDic;
                }
            }
            else if (columns.HeaderText == "支")
            {
            }
        }

        private void duihuatable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (duihuatable.Rows.Count == 0) return;
            for (int i = 0; i < duihuatable.Rows.Count; i++)
            {
                if (duihuatable.Rows[i]["Guid"] == null || duihuatable.Rows[i]["Guid"].ToString() == "")
                {
                    duihuatable.Rows[i]["Guid"] = Guid.NewGuid();
                }
            }
        }
    }
}
