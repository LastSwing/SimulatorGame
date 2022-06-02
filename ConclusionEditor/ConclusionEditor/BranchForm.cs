using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ConclusionEditor
{
    public partial class BranchForm : Form
    {
        private List<Fileid> Fileids = new List<Fileid>();
        Guid Guid = Guid.Empty;
        public BranchForm(List<Fileid> fileids,Guid guid)
        {
            InitializeComponent();
            Fileids = fileids;
            Guid = guid;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BranchForm_Load(object sender, EventArgs e)
        {
            foreach (var item in Fileids)
            {
                if (item.ParentId == Guid)
                {
                    if (item.Fileidtype.ToString() == "")
                        continue;
                    switch (item.Fileidtype)
                    {
                        case FileidType.选择:
                            label6.Text = label6.Text == "无"?"1":(Convert.ToInt32(label6.Text)+1).ToString();
                            break;
                        case FileidType.字段:
                            label7.Text = label7.Text == "无" ? "1" : (Convert.ToInt32(label7.Text) + 1).ToString();
                            break;
                        case FileidType.结局:
                            label8.Text = label8.Text == "无" ? "1" : (Convert.ToInt32(label8.Text) + 1).ToString();
                            break;
                        case FileidType.背景音乐:
                            label9.Text = label9.Text == "无" ? "1" : (Convert.ToInt32(label9.Text) + 1).ToString();
                            break;
                        case FileidType.动画:
                            label10.Text = label10.Text == "无" ? "1" : (Convert.ToInt32(label10.Text) + 1).ToString();
                            break;
                        case FileidType.判断对话:
                            label12.Text = label12.Text == "无" ? "1" : (Convert.ToInt32(label12.Text) + 1).ToString();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
