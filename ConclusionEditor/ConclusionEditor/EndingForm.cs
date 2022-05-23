using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ConclusionEditor
{
    public partial class EndingForm : Form
    {
        DataTable DataTable = null;
        public List<Fileid> Fileids = new List<Fileid>();
        private Guid CGuid = Guid.Empty;
        public EndingForm(string duihua,DataTable dataTable, List<Fileid> fileids,Guid cGuid)
        {
            InitializeComponent();
            DataTable = dataTable;
            textBox1.Text = duihua;
            Fileids = fileids;
            CGuid = cGuid;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.OK;
            foreach (DataRow item in dataTable1.Rows)
            {
                string jieju = this.comboBox1.SelectedValue.ToString();
                if (item["jieju"].ToString() == jieju)
                {
                    Fileid fileid = new Fileid();
                    fileid.Fileidtype = FileidType.结局;
                    fileid.ParentId = CGuid;
                    fileid.Id = new Guid(item["Guid"].ToString());
                    fileid.PathName = item["jieju"].ToString();
                    for (int i = 0; i < Fileids.Count; i++)
                    {
                        if (Fileids[i].ParentId == CGuid && Fileids[i].Fileidtype == FileidType.结局)
                        {
                            Fileids.Remove(Fileids[i]);
                        }
                    }
                    Fileids.Add(fileid);
                }
            }
            
        }

        private void EndingForm_Load(object sender, EventArgs e)
        {
            dataTable1.Merge(DataTable);
            foreach (var item in Fileids)
            {
                if (item.ParentId == CGuid && item.Fileidtype == FileidType.结局)
                { 
                    comboBox1.SelectedValue = item.PathName;
                }
            }
        }
    }
}
