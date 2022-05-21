using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ConclusionEditor
{
    public partial class EndingControl : UserControl
    {
        public EndingControl()
        {
            InitializeComponent();
        }
        public int Number { get; set; }
        /// <summary>
        /// 结局父ID
        /// </summary>
        public Guid PGuid { get; set; }
        private int vintage;
        /// <summary>
        /// 事件年份时长
        /// </summary>
        public int Vintage
        {
            get
            {
                return vintage;
            }
            set
            {
                vintage = value;
                for (int i = 0; i < vintage; i++)
                {
                    DataRow dataRow = dataTable2.NewRow();
                    dataRow["nianfen"] = "第" + (i + 1) + "年";
                    dataTable2.Rows.Add(dataRow);
                }
                dataTable2.AcceptChanges();
            }
        }

        private List<Ending> endings;
        /// <summary>
        /// 结局集
        /// </summary>
        public List<Ending> Endings
        {
            get 
            {
                endings = new List<Ending>();
                if (dataTable1.Rows.Count>0)
                {
                    for (int i = 0; i < dataTable1.Rows.Count; i++)
                    {
                        Ending ending = new Ending();
                        ending.Stars = int.TryParse(dataTable1.Rows[i]["xingli"].ToString(),out int Stars)? Stars:0;
                        ending.Stellar = int.TryParse(dataTable1.Rows[i]["xingbi"].ToString(), out int Stellar) ? Stellar : 0;
                        ending.Productivity = int.TryParse(dataTable1.Rows[i]["shengchan"].ToString(), out int Productivity) ? Productivity : 0;
                        ending.Result = dataTable1.Rows[i]["jiejujieshao"].ToString();
                        ending.PGuid = new Guid(dataTable1.Rows[i]["PGuid"].ToString());
                        ending.CGuid = new Guid(dataTable1.Rows[i]["CGuid"].ToString());
                        string vintage = dataTable1.Rows[i]["nianfen"].ToString();
                        if (vintage.Length>2)
                        {
                            vintage = vintage.Remove(0, 1);
                            vintage = vintage.Remove(vintage.Length-1,1);
                        }
                        if (int.TryParse(vintage, out int vin))
                            ending.Vintage = vin;
                        else
                            ending.Vintage = 0;
                        endings.Add(ending);
                    }
                }
                return endings;
            }
            set
            {
                endings = value;
                for (int i = 0; i < endings.Count; i++)
                {
                    DataRow data = dataTable1.NewRow();
                    data["CGuid"] = endings[i].CGuid;
                    data["PGuid"] = endings[i].PGuid;
                    data["nianfen"] = "第"+endings[i].Vintage+"年";
                    data["jiejujieshao"] = endings[i].Result;
                    data["shengchan"] = endings[i].Productivity;
                    data["xingbi"] = endings[i].Stellar;
                    data["xingli"] = endings[i].Stars;
                    dataTable1.Rows.Add(data);
                }
                dataTable1.AcceptChanges();
            }
        }


        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView view = sender as DataGridView;
            DataGridViewColumn columns = view.Columns[e.ColumnIndex];
            if (columns.HeaderText == "删")
            {
                if (e.RowIndex == -1 || view.RowCount - 1 == e.RowIndex || e.RowIndex+1>dataTable1.Rows.Count) return;
                dataTable1.Rows.Remove(dataTable1.Rows[e.RowIndex]);
                dataTable1.AcceptChanges();
            }
        }

        private void dataTable1_RowChanged(object sender, DataRowChangeEventArgs e)
        {
            if (dataTable1.Rows.Count == 0) return;
            for (int i = 0; i < dataTable1.Rows.Count; i++)
            {
                if (dataTable1.Rows[i]["CGuid"] == null || dataTable1.Rows[i]["CGuid"].ToString() == "")
                {
                    dataTable1.Rows[i]["CGuid"] = Guid.NewGuid();
                    dataTable1.Rows[i]["PGuid"] = PGuid;
                }
            }
        }
    }
}
