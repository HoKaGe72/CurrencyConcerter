using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Babki
{
    public partial class Form3 : Form
    {
        private DataTable dTable;
        public Form3(DataTable dT)
        {
            InitializeComponent();
            dTable = dT;
            comboBox1.KeyPress += (sender, e) => e.Handled = true;
        }
        private void Form3_Load(object sender, EventArgs e)
        {
            ShowTable(dTable, dataGridView1);
        }
        private void ShowTable(DataTable dTable, DataGridView dataGridView1)
        {
            for (int i = 0; i < dTable.Columns.Count; i++)
            {
                string iName = dTable.Columns[i].ColumnName;
                dataGridView1.Columns.Add(iName, iName);
                dataGridView1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            for (int k = 0; k < dTable.Rows.Count; k++)
            {
                dataGridView1.Rows.Add(dTable.Rows[k].ItemArray);
            }
            for (int i = 1; i < dTable.Columns.Count; i++)
            {
                comboBox1.Items.Add(Convert.ToString(dTable.Columns[i]));
            }
            for (int i = 1; i < dTable.Columns.Count; i++)
            {
                for (int j = 0; j < dTable.Rows.Count; j++)
                {
                    if (j >= 1)
                    {
                        if (Convert.ToDouble(dTable.Rows[j][i]) > Convert.ToDouble(dTable.Rows[j - 1][i]))
                        {
                            dataGridView1.Rows[j].Cells[i].Style.ForeColor = Color.Green;
                        }
                        else
                        {
                            dataGridView1.Rows[j].Cells[i].Style.ForeColor = Color.Red;
                        }
                    }
                }
            }
        }
        public void Graf(Chart chart,  string k)
        {
            chart.ChartAreas.Add("s1");
            chart.ChartAreas[0].AxisX.Title = "Дата";
            chart.ChartAreas[0].AxisY.Title = "Рубль";
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "{0:###.##}";
            chart.ChartAreas[0].AxisY.IsStartedFromZero = false;
            chart.ChartAreas[0].AxisX.IsStartedFromZero = true;
            Series s1 = new Series($"{k}"); 
            for(int i = 1; i < dTable.Columns.Count;i++)
            {
                if (k==Convert.ToString(dTable.Columns[i].ColumnName))
                {
                    for (int j = 0; j < dTable.Rows.Count; j++)
                    {
                        s1.Points.AddXY($"{Convert.ToString(dTable.Rows[j][0])}", Convert.ToDouble(dTable.Rows[j][i]));
                        s1.Points[j].Label = $"{Math.Round(Convert.ToDouble(dTable.Rows[j][i]),2)}";
                        s1.Points[j].Font = new Font("Microsoft Sans Serif", 10f, FontStyle.Bold);
                    }
                }
            }
            s1.ChartType = SeriesChartType.Spline;
            s1.MarkerBorderColor = Color.Black;
            s1.MarkerStyle = MarkerStyle.Circle;
            s1.MarkerSize = 6;
            chart.Series.Add(s1);
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Chart chart = chart1;
            chart.ChartAreas.Clear();
            chart.Series.Clear();
            string k = Convert.ToString(comboBox1.Text);
            Graf(chart, k);

        }
    }
}
