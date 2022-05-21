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
        public Dictionary<Guid,Dictionary<Guid, string>> duihuaDic = new Dictionary<Guid,Dictionary<Guid,string>>();
        public AddbranchForm(Guid guid, string duihua, DataTable jueseTable)
        {
            InitializeComponent();
            Duihua = duihua;
            Guid = guid;
            fileid = new List<Fileid>();
            JueseTable.Merge(jueseTable);
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
            SelectForm selectForm = new SelectForm(Guid, JueseTable, duihuaDic, fileid);
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
            FleidInputForm selectForm = new FleidInputForm();
            selectForm.ShowDialog();
            if (selectForm.DialogResult == DialogResult.OK)
            {
                Closes();
            }
        }
        /// <summary>
        /// 添加BGM
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            BGMForm selectForm = new BGMForm();
            selectForm.ShowDialog();
            if (selectForm.DialogResult == DialogResult.OK)
            {
                Closes();
            }
        }
        /// <summary>
        /// 添加动画
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            AnimationForm selectForm = new AnimationForm();
            selectForm.ShowDialog();
            if (selectForm.DialogResult == DialogResult.OK)
            {
                Closes();
            }
        }
        /// <summary>
        /// 添加结局分支
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            EndingForm selectForm = new EndingForm();
            selectForm.ShowDialog();
            if (selectForm.DialogResult == DialogResult.OK)
            {
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
