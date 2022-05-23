using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ConclusionEditor
{
    public partial class FleidInputForm : Form
    {
        public List<Fileid> fleids = new List<Fileid>();
        private Guid Guid = Guid.Empty;
        public FleidInputForm(string duihua,Guid guid, List<Fileid> fileids)
        {
            InitializeComponent();
            textBox1.Text = duihua;
            Guid = guid;
            fleids = fileids;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0 && int.TryParse(textBox2.Text, out int InsertType))
            {
                string StrType = "";
                for (int i = 0; i < dataTable1.Rows.Count; i++)
                {
                    StrType += dataTable1.Rows[i][0].ToString() + "|";
                }
                StrType = StrType.Trim().Remove(StrType.Length-1,1);
                Fileid fileid = new Fileid();
                fileid.Fileids = StrType.Split('|');
                fileid.Id = Guid.NewGuid();
                fileid.ParentId = Guid;
                fileid.PathName = textBox1.Text;
                fileid.Fileidtype = FileidType.字段;
                fileid.InsertByte = InsertType;
                for (int i = 0; i < fleids.Count; i++)
                {
                    if (fleids[i].ParentId == Guid && fleids[i].Fileidtype == FileidType.字段)
                    {
                        fleids.Remove(fleids[i]);
                    }
                }
                fleids.Add(fileid);
            }
            MessageBox.Show("保存成功！");
            this.DialogResult = DialogResult.OK;    
            this.Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            int InsertType = 0;
            string Str = "";
            if (dataTable1.Rows.Count >0 && int.TryParse(textBox2.Text, out InsertType))
            {
                Str = dataTable1.Rows[0][0].ToString();
                if (InsertType > textBox1.Text.Length)
                {
                    MessageBox.Show("超了超了");
                    textBox2.Text = "";
                    return;
                }
                textBox4.Text = textBox1.Text.Insert(InsertType, Str);
            }
        }

        private void FleidInputForm_Load(object sender, EventArgs e)
        {
            foreach (var item in fleids)
            {
                if (item.ParentId == Guid && item.Fileidtype == FileidType.字段)
                {
                    foreach (var str in item.Fileids)
                    {
                        DataRow dr = dataTable1.NewRow();
                        dr["xuanzexiang"] = str;
                        dataTable1.Rows.Add(dr);
                    }
                    textBox2.Text = item.InsertByte.ToString();
                }
            }
        }
    }
}
