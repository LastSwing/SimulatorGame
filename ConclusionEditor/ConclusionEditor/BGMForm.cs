using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ConclusionEditor
{
    public partial class BGMForm : Form
    {
        public List<Fileid> fileids = new List<Fileid>();
        private  Guid Guid = Guid.Empty;
        string Duihua = "";
        public BGMForm(Guid guid,string duihua,List<Fileid> Fileids)
        {
            InitializeComponent();
            fileids = Fileids;
            Guid = guid;
            Duihua = duihua;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择音乐文件";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.FileName))
                {
                    MessageBox.Show("请选择音乐文件！");
                    return;
                }
                textBox3.Text = dialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Fileid fileid = new Fileid();
            if (int.TryParse(textBox1.Text, out int o) && int.TryParse(textBox2.Text, out int j))
            {
                MessageBox.Show("请输入开始和结束字符任意一项");
                return;
            }
            if (textBox3.Text == "")
            {
                MessageBox.Show("没有路径！");
                return;
            }
            if (int.TryParse(textBox1.Text,out int insert))
            {
                fileid.InsertByte = insert;
            }
            if (int.TryParse(textBox2.Text, out int end))
            {
                fileid.EndByte = end;
            }
            fileid.PathName = textBox3.Text.Trim();
            fileid.Id = Guid.NewGuid();
            fileid.ParentId = Guid;
            for (int i = 0; i < fileids.Count; i++)
            {
                if (fileids[i].ParentId == Guid && fileids[i].Fileidtype == FileidType.背景音乐)
                {
                    fileids.Remove(fileids[i]);
                }
            }
            fileid.Fileidtype = FileidType.背景音乐;
            fileids.Add(fileid);
            MessageBox.Show("保存成功！");
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BGMForm_Load(object sender, EventArgs e)
        {
            foreach (var item in fileids)
            {
                if (item.ParentId == Guid && item.Fileidtype == FileidType.背景音乐)
                {
                    textBox1.Text = item.InsertByte.ToString();
                    textBox2.Text = item.EndByte.ToString();
                    textBox3.Text=item.PathName.ToString();
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text,out int insert))
            {
                if (insert > Duihua.Length)
                {
                    MessageBox.Show("超过当前对话语句的"+Duihua.Length+"最大字节");
                    textBox1.Text = "";
                }
            }
            else
            {
                textBox1.Text = "";
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(textBox2.Text, out int insert))
            {
                if (insert > Duihua.Length)
                {
                    MessageBox.Show("超过当前对话语句的" + Duihua.Length + "最大字节");
                    textBox2.Text = "";
                }
            }
            else
            {
                textBox2.Text = "";
            }
        }
    }
}
