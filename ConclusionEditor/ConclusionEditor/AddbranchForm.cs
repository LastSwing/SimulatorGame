using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ConclusionEditor
{
    public partial class AddbranchForm : Form
    {
        private string Duihua;
        private Guid Guid;
        public List<Fileid> fileid;
        private DataTable JueseTable = new DataTable();
        private DataTable jiejuTable = new DataTable();
        public Dictionary<Guid,Dictionary<Guid, string>> duihuaDic = new Dictionary<Guid,Dictionary<Guid,string>>();
        public AddbranchForm(Guid guid, string duihua, DataTable jueseTable,DataTable JieJuTable)
        {
            InitializeComponent();
            Duihua = duihua;
            Guid = guid;
            fileid = new List<Fileid>();
            JueseTable.Merge(jueseTable);
            jiejuTable.Merge(JieJuTable);
        }
        /// <summary>
        /// 添加选择分支
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            if (duihuaDic == null)
            {
                duihuaDic = new Dictionary<Guid, Dictionary<Guid, string>>();
            }
            if (fileid == null)
            {
                fileid = new List<Fileid>();
            }
            SelectForm selectForm = new SelectForm(Guid, JueseTable, duihuaDic, fileid,jiejuTable);
            selectForm.ShowDialog();
            if (selectForm.DialogResult == DialogResult.OK)
            {
                duihuaDic = selectForm.duihuaDic;
                fileid = selectForm.fileids;
                Closes();
            }
        }
        /// <summary>
        /// 添加字段输入
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            FleidInputForm selectForm = new FleidInputForm(Duihua,Guid,fileid);
            selectForm.ShowDialog();
            if (selectForm.DialogResult == DialogResult.OK)
            {
                fileid = selectForm.fleids;
                Closes();
            }
        }
        /// <summary>
        /// 添加BGM
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            BGMForm selectForm = new BGMForm(Guid,Duihua,fileid);
            selectForm.ShowDialog();
            if (selectForm.DialogResult == DialogResult.OK)
            {
                fileid = selectForm.fileids;
                Closes();
            }
        }
        /// <summary>
        /// 添加动画
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            AnimationForm selectForm = new AnimationForm(Guid,fileid);
            selectForm.ShowDialog();
            if (selectForm.DialogResult == DialogResult.OK)
            {
                fileid = selectForm.fileds;
                Closes();
            }
        }
        /// <summary>
        /// 添加结局分支
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            if (jiejuTable.Rows.Count == 0)
            {
                MessageBox.Show("请先添加结局！");
                return;
            }
            EndingForm selectForm = new EndingForm(Duihua,jiejuTable,fileid,Guid);
            selectForm.ShowDialog();
            if (selectForm.DialogResult == DialogResult.OK)
            {
                fileid = selectForm.Fileids;
                Closes();
            }
        }

        void Closes()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
