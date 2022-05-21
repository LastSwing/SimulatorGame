using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ConclusionEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Livelibrary Livelibrary = new Livelibrary();
        private bool State = false;
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件夹";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    MessageBox.Show("请选择文件夹！");
                    return;
                }
                textBox1.Text = dialog.SelectedPath;
                if (File.Exists(Application.StartupPath + @"\h.txt"))
                    File.Delete(Application.StartupPath + @"\h.txt");
                File.WriteAllText(Application.StartupPath + @"\h.txt", textBox1.Text);
                DataLoad();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            initialization();
            DataLoad();
        }

        private void initialization()
        {
            try
            {
                if (!File.Exists(Application.StartupPath + @"\h.txt"))
                    File.WriteAllText(Application.StartupPath + @"\h.txt", "");
                else
                    textBox1.Text = File.ReadAllText(Application.StartupPath + @"\h.txt");
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void DataLoad()
        {
            comboBox1.Items.Add("不衔接事件");
            comboBox1.SelectedIndex = 0;
            if (textBox1.Text == "") return;
            Dictionary<string,string> keyValues =  JsonHelper.DataRead("sortjson.txt",textBox1.Text);
            foreach (var item in keyValues)
            {
                DataRow dr = dataSet1.Tables["shijiantable"].NewRow();
                dr[0] = item.Key;
                dr[1] = item.Value;
                dataSet1.Tables["shijiantable"].Rows.Add(dr);
                comboBox1.Items.Add(item.Key);
            }
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;
            DataGridView view = sender as DataGridView;
            DataGridViewColumn columns = view.Columns[e.ColumnIndex];
            if (view.Name == "dataGridView4")
            {
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
                            string str1 = "结局" + (e.RowIndex + 1);
                            if (item.Text.Equals(str1))
                            {
                                tabPage = item;
                                ischange = true;
                            }
                        }
                        else
                        {
                            item.Text = "结局" + (Convert.ToInt32(item.Text.Replace("结局", "")) - 1);
                            EndingControl End = item.Controls[0] as EndingControl;
                            End.Number--;
                        }
                    }
                    tabControl1.TabPages.Remove(tabPage);
                    view.Rows.Remove(view.Rows[e.RowIndex]);
                }
            }
            else if (view.Name == "dataGridView2")//角色名称
            {
                if (view.RowCount - 1 == e.RowIndex)
                    return;
                if (columns.HeaderText == "删")
                {
                    bool isuse = true;
                    if (!isuse)
                    {
                        MessageBox.Show("角色名称正在使用！请取消使用后再删除。");
                        return;
                    }
                }
            }
            else if (view.Name == "ViewKu")//事件库
            {
                if (columns.HeaderText == "删")
                {
                    if (MessageBox.Show("确定要删除事件吗，不可恢复呦~", "提示", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        DataGridViewRow gridView = view.Rows[e.RowIndex];
                        string name = gridView.Cells[0].Value.ToString();
                        string filepath = gridView.Cells[1].Value.ToString();
                        Dictionary<string, string> keyValues = JsonHelper.DataRead("sortjson.txt", textBox1.Text);
                        keyValues.Remove(gridView.Cells[0].Value.ToString());
                        File.Delete(textBox1.Text + "/" + filepath + ".txt");
                        JsonHelper.DataExport(keyValues, "sortjson", textBox1.Text);
                        view.Rows.Remove(view.Rows[e.RowIndex]);
                        shijiantable.AcceptChanges();
                    }
                }
            }
            else if (view.Name == "dataGridView3")
            {
                if (view.RowCount - 1 == e.RowIndex || duihuatable.Rows.Count == 0 || e.RowIndex+1>duihuatable.Rows.Count)
                    return;
                if (columns.HeaderText == "删")
                {
                    view.Rows.Remove(view.Rows[e.RowIndex]);
                }
                else if (columns.HeaderText == "加")
                {
                    Guid guid = new Guid(duihuatable.Rows[e.RowIndex]["Guid"].ToString());
                    AddbranchForm addbranchForm = new AddbranchForm(guid, duihuatable.Rows[e.RowIndex]["duihua"].ToString(),juesetable);
                    addbranchForm.duihuaDic = Livelibrary.Dialogue;
                    addbranchForm.fileid = Livelibrary.Fileid;
                    addbranchForm.ShowDialog();
                    if (addbranchForm.DialogResult == DialogResult.OK)
                    {
                        if (addbranchForm.fileid != null && addbranchForm.fileid.Count != 0)
                        {
                            if (Livelibrary.Fileid == null)
                                Livelibrary.Fileid = new List<Fileid>();
                            Livelibrary.Fileid = addbranchForm.fileid;
                        }
                        if (Livelibrary.Dialogue == null)
                            Livelibrary.Dialogue = new Dictionary<Guid, Dictionary<Guid, string>>();
                        Livelibrary.Dialogue = addbranchForm.duihuaDic;
                    }
                }
                else if (columns.HeaderText == "支")
                {
                }
            }
        }

        //保存事件
        private void button2_Click(object sender, EventArgs e)
        {
            if (!Judge())
                return;
            Dictionary<string, string> keyValues = JsonHelper.DataRead("sortjson.txt", textBox1.Text);
            if (keyValues.ContainsKey(textBox2.Text))//修改
            {
                string FileName = "";
                if (keyValues.TryGetValue(textBox2.Text, out FileName))
                {
                    Save(FileName);
                }
                MessageBox.Show("修改事件成功");
            }
            else
            {
                ViewKu.CurrentCellChanged -= ViewKu_CurrentCellChanged;
                string fileName = "";
                fileName = "case_" + (shijiantable.Rows.Count + 1);
                keyValues.Add(textBox2.Text, fileName);
                JsonHelper.DataExport(keyValues, "sortjson", textBox1.Text);
                Save(fileName);
                DataRow dataRow = shijiantable.NewRow();
                dataRow["ThreadName"] = textBox2.Text;
                dataRow["lujin"] = fileName;
                shijiantable.Rows.Add(dataRow);
                shijiantable.AcceptChanges();
                ViewKu.CurrentCellChanged += ViewKu_CurrentCellChanged;
                MessageBox.Show("新增事件成功");
            }
            State = false;
        }
        /// <summary>
        /// 保存当前页数据
        /// </summary>
        /// <returns></returns>
        private void Save(string fileName)
        {
            Dictionary<string, string> valuePairs = new Dictionary<string, string>();
            Livelibrary.Name = textBox2.Text.Trim();
            Livelibrary.Lifetime = textBox3.Text.Replace(@"\n", "");
            string role = "";
            for (int i = 0; i < juesetable.Rows.Count; i++)
                role += juesetable.Rows[i][0].ToString() + "|";
            Livelibrary.Role = role.Length == 0 ? "" : role.Remove(role.Length - 1, 1);
            Livelibrary.YearDuration = Convert.ToInt32(textBox4.Text.Trim());
            Livelibrary.YearJoin = Convert.ToInt32(textBox5.Text.Trim());
            Livelibrary.Year = Convert.ToInt32(textBox6.Text.Trim());
            Livelibrary.JoinName = comboBox1.SelectedItem.ToString() == "不衔接事件" ? "" : comboBox1.SelectedItem.ToString();
            Dictionary<Guid, string> keyValues = new Dictionary<Guid, string>();
            for (int i = 0; i < duihuatable.Rows.Count; i++)
            {
                keyValues.Add(new Guid(duihuatable.Rows[i]["Guid"].ToString()), duihuatable.Rows[i]["juese"] + "|" + duihuatable.Rows[i]["duihua"]);
            }
            if (Livelibrary.Dialogue == null)
                Livelibrary.Dialogue = new Dictionary<Guid, Dictionary<Guid, string>>();
            Livelibrary.Dialogue.Remove(Guid.Empty);
            Livelibrary.Dialogue.Add(Guid.Empty, keyValues);
            if (Livelibrary.Ending == null)
                Livelibrary.Ending = new Dictionary<string, List<Ending>>();
            for (int i = 0; i < jiejutable.Rows.Count; i++)
            {
                TabPage tabPage = tabControl1.TabPages[i];
                EndingControl endingControl = (EndingControl)tabPage.Controls[0];
                Livelibrary.Ending.Remove(jiejutable.Rows[i]["jieju"].ToString() + "|" + jiejutable.Rows[i]["Guid"].ToString());
                Livelibrary.Ending.Add(jiejutable.Rows[i]["jieju"].ToString()+"|"+ jiejutable.Rows[i]["Guid"].ToString(), endingControl.Endings);
            }
            JsonHelper.LivelibraryTojson(Livelibrary, textBox1.Text, fileName);
        }
        private bool Judge()
        {
            if (textBox2.Text.Trim().Equals(String.Empty))
            {
                MessageBox.Show("请输入事件名称！");
                return false;
            }
            if (!int.TryParse(textBox4.Text.Trim(),out int i))
            {
                MessageBox.Show("请输入年份时长！");
                return false;
            }
            if (!int.TryParse(textBox5.Text.Trim(), out int j) || !int.TryParse(textBox5.Text.Trim(), out int o))
            {
                MessageBox.Show("衔接年份时长和开始时间至少要输入一个！");
                return false;
            }
            return true;
        }
        private void ViewKu_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGridViewRow dr = ViewKu.CurrentRow;
            if (dr == null || dr.Cells[0].Value == null || State) return;
            textBox2.Text = dr.Cells[0].Value.ToString();
            if (textBox2.Text == "") return;
            Clear();
            string lujin = "";
            for (int i = 0; i < shijiantable.Rows.Count; i++)
            {
                if (shijiantable.Rows[i]["ThreadName"].ToString() == textBox2.Text)
                {
                    lujin = shijiantable.Rows[i]["lujin"].ToString();
                }
            }
            Livelibrary = new Livelibrary();
            Livelibrary = JsonHelper.JsonToLivelibrary(lujin+ ".txt",textBox1.Text);
            if (Livelibrary.Name == null)return;
            textBox2.Text = Livelibrary.Name;
            textBox3.Text = Livelibrary.Lifetime;
            textBox4.Text = Livelibrary.YearDuration.ToString();
            textBox5.Text = Livelibrary.YearJoin.ToString();
            textBox6.Text = Livelibrary.Year.ToString();
            foreach (var item in comboBox1.Items)
            {
                if (item.ToString() == Livelibrary.JoinName)
                {
                    comboBox1.SelectedItem = item;
                }
            }
            foreach (var item in Livelibrary.Dialogue)
            {
                if (item.Key == Guid.Empty)
                {
                    foreach (var items in item.Value)
                    {
                        DataRow datarow = duihuatable.NewRow();
                        datarow["Guid"] = items.Key;
                        string[] str = items.Value.Split('|');
                        datarow["juese"] = str[0];
                        datarow["duihua"] = str[1];
                        duihuatable.Rows.Add(datarow);
                    }
                    duihuatable.AcceptChanges();
                }
            }
            string[] role = Livelibrary.Role.Split('|');
            for (int i = 0; i < role.Length; i++)
            {
                DataRow tdr = juesetable.NewRow();
                tdr[0] = role[i];
                juesetable.Rows.Add(tdr);
            }
            juesetable.AcceptChanges();
            foreach (var item in Livelibrary.Ending)
            {
                string[] str = item.Key.Split('|');
                DataRow dataRow =  jiejutable.NewRow();
                dataRow["jieju"] = str[0];
                dataRow["Guid"] = new Guid(str[1]);
                jiejutable.Rows.Add(dataRow);
                jiejutable.AcceptChanges();
            }
        }

        private void juesetable_RowChanged(object sender, DataRowChangeEventArgs e)
        {


        }
        private void Clear()
        {
            textBox3.Text = "";
            juesetable.Rows.Clear();
            duihuatable.Rows.Clear();
            tabControl1.TabPages.Clear();
        }

        private void dataGridView3_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void duihuatable_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (duihuatable.Rows.Count == 0) return;
            for (int i = 0; i < duihuatable.Rows.Count; i++)
            {
                if (duihuatable.Rows[i]["Guid"] == null || duihuatable.Rows[i]["Guid"].ToString() == "")
                {
                    duihuatable.Rows[i]["Guid"] = Guid.NewGuid();
                    if (Livelibrary.Dialogue == null)
                        Livelibrary.Dialogue = new Dictionary<Guid, Dictionary<Guid, string>>();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            State = true;
            textBox2.ReadOnly = false;
            Livelibrary = new Livelibrary();
            Livelibrary.Dialogue = new Dictionary<Guid, Dictionary<Guid, string>>();
            Livelibrary.Fileid = new List<Fileid>();
            Livelibrary.Ending = new Dictionary<string, List<Ending>>();
        }

        private void jiejutable_RowChanged(object sender, DataRowChangeEventArgs e)
        {

            if (jiejutable.Rows.Count == 0 || jiejutable.Rows.Count == tabControl1.TabCount) return;
            if (textBox4.Text.Equals(String.Empty))
            {
                MessageBox.Show("请输入年份时长！");
                jiejutable.Rows.Clear();
                return;
            }
            if (!int.TryParse(textBox4.Text, out int Vintage))
            {
                MessageBox.Show("小老弟年份时长不能是数字。");
                jiejutable.Rows.Clear();
                return;
            }
            for (int i = 0; i < jiejutable.Rows.Count; i++)
            {
                if (jiejutable.Rows[i]["Guid"] == null || jiejutable.Rows[i]["Guid"].ToString() == "")
                {
                    TabPage tabPage = new TabPage();
                    tabPage.Text = "结局" + jiejutable.Rows.Count;
                    tabControl1.Controls.Add(tabPage);
                    jiejutable.Rows[i]["Guid"] = Guid.NewGuid();
                    EndingControl endingControl = new EndingControl();
                    endingControl.PGuid = new Guid(jiejutable.Rows[i]["Guid"].ToString());
                    endingControl.Vintage = Vintage;
                    endingControl.Number = jiejutable.Rows.Count;
                    tabPage.Controls.Add(endingControl);
                }
            }
            DataRow row = e.Row;
            if (row["Guid"].ToString() != "" && Livelibrary.Ending != null && tabControl1.TabPages.Count != jiejutable.Rows.Count)
            {
                TabPage tabPage = new TabPage();
                tabPage.Text = "结局" + jiejutable.Rows.Count;
                tabControl1.Controls.Add(tabPage);
                EndingControl endingControl = new EndingControl();
                endingControl.PGuid = new Guid(row["Guid"].ToString());
                endingControl.Vintage = Vintage;
                endingControl.Number = jiejutable.Rows.Count;
                foreach (var item in Livelibrary.Ending)
                {
                    if (item.Key.Contains(row["Guid"].ToString()))
                    {
                        endingControl.Endings = item.Value;
                    }
                }
                tabPage.Controls.Add(endingControl);
            }
        }
    }
}
