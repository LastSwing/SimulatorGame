using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ConclusionEditor
{
    public partial class AnimationForm : Form
    {
        private Guid Guid;
        public List<Fileid> fileds = new List<Fileid>();
        public AnimationForm(Guid guid,List<Fileid> Fileds)
        {
            InitializeComponent();
            fileds = Fileds;
            Guid = guid;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择动画文件";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.FileName))
                {
                    MessageBox.Show("请选择动画文件！");
                    return;
                }
                textBox3.Text = dialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Fileid fileid = new Fileid();
            fileid.PathName = textBox3.Text.Trim();
            fileid.Id = Guid.NewGuid();
            fileid.ParentId = Guid;
            for (int i = 0; i < fileds.Count; i++)
            {
                if (fileds[i].ParentId == Guid && fileds[i].Fileidtype == FileidType.动画)
                {
                    fileds.Remove(fileds[i]);
                }
            }
            fileid.Fileidtype = FileidType.动画;
            fileds.Add(fileid);
            MessageBox.Show("保存成功！");
            DialogResult = DialogResult.OK;
            Close();
        }

        private void AnimationForm_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < fileds.Count; i++)
            {
                if (fileds[i].ParentId == Guid && fileds[i].Fileidtype == FileidType.动画)
                {
                    textBox3.Text = fileds[i].PathName;
                }
            }
        }
    }
}
