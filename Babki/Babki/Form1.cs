using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Xml.Linq;
using System.Linq;
using System.Drawing;
using System.Xml;
using System.Net.Sockets;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace Babki
{
    public partial class Form1 : Form
    {
        private SQLiteConnection SQLiteConn;
        private DataTable dTable, dT;
        double kontrol = 0;
        Form3 form3;
        Form4 form4;
        public Form1()
        {
            InitializeComponent();
            comboBox1.KeyPress += (sender, e) => e.Handled = true;
            comboBox2.KeyPress += (sender, e) => e.Handled = true;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = "1";
            SQLiteConn = new SQLiteConnection();
            dTable = new DataTable();
            dT = new DataTable();
            ConnectDB(); // Подключение к БД
            ShowTable(SQL_AllTable()); // Чтение БД и Заполнение табл.
            SQLiteCommand SQLT = SQLiteConn.CreateCommand();
            SQLT.CommandText = "SELECT data FROM ДатаПоследногоОбновления WHERE id=1";
            SQLiteDataReader reader1 = SQLT.ExecuteReader();
            while (reader1.Read())
            {
                label6.Text = reader1[0].ToString();// Дата последного обновления валют
            }
            Obnov();// Обновление курса валют 
        }
        private void ConnectDB()
        {
            string path = string.Concat(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"\Курс.sqlite");

            SQLiteConn = new SQLiteConnection("Data Source=" +path+"; Version=3;");
            SQLiteConn.Open();
            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = SQLiteConn;
        }
        private string SQL_AllTable()
        {
            return "SELECT * FROM [Курс];";
        }
        private void ShowTable(string SQLQuery)
        {
            dTable.Clear();
            SQLiteDataAdapter piu = new SQLiteDataAdapter(SQLQuery, SQLiteConn);
            piu.Fill(dTable);
            DataBase_View.Columns.Clear();
            DataBase_View.Rows.Clear();
            for (int i = 0; i < dTable.Columns.Count; i++)
            {
                string iName = dTable.Columns[i].ColumnName;
                DataBase_View.Columns.Add(iName, iName);
                DataBase_View.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            }
            for (int k = 0; k < dTable.Rows.Count; k++)
            {
                DataBase_View.Rows.Add(dTable.Rows[k].ItemArray);

            }
            comboBox1.Items.Add("Российский рубль");
            comboBox2.Items.Add("Российский рубль");
            for (int j = 0; j < dTable.Rows.Count; j++)
            {
                comboBox1.Items.Add(Convert.ToString(dTable.Rows[j][3]));
                comboBox2.Items.Add(Convert.ToString(dTable.Rows[j][3]));
            }

        }
        private string SQL_AllTable2()
        {
            return "SELECT * FROM [Статистика];";
        }
        private void ShowTable2(string SQLQuery)
        {
            dT.Clear();
            SQLiteDataAdapter piu = new SQLiteDataAdapter(SQLQuery, SQLiteConn);
            piu.Fill(dT);
        }
        private void ShowTable(DataTable DTable, DataGridView Table)//заполнение таблицы
        {
            Table.Columns.Clear();
            Table.Rows.Clear();
            for (int colum = 0; colum < DTable.Columns.Count; colum++)
            {
                string ColumName = DTable.Columns[colum].ColumnName;
                Table.Columns.Add(ColumName, ColumName);
                Table.Columns[colum].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            for (int row = 0; row < DTable.Rows.Count; row++)
            {
                Table.Rows.Add(DTable.Rows[row].ItemArray);
            }
            foreach (DataGridViewColumn column in Table.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        private void global_FormClosed(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string a, b;
            a = comboBox1.Text;
            b = comboBox2.Text;
            comboBox1.Text = b;
            comboBox2.Text = a;
            if (kontrol == 1)
            {
                Perevod();
            }
        }

        private void Perevod()
        {
            string cb1, cb2, zn;
            if (comboBox1.Text == "" || comboBox2.Text == "")
            {
                MessageBox.Show("Перевод невозможен!Укажите валюту.", "Внимание!");
                return;
            }
            else
            {
                cb1 = comboBox1.SelectedItem.ToString();
                cb2 = comboBox2.SelectedItem.ToString();
            }
            if (textBox1.Text == "")
            {
                MessageBox.Show("Введите количество валюты для перевода!", "Внимание!");
            }
            else { zn = textBox1.SelectedText.ToString(); }

            if (comboBox1.Text == comboBox2.Text)
            {
                label7.Text = textBox1.Text + " " + cb1 + " = " + textBox1.Text + " " + cb2;
            }
            else
            {
                var rez = Raschot(cb1, cb2);
                label7.Text = textBox1.Text + " " + cb1 + " = " + rez + " " + cb2;
            }

            //var parsY = Parsing(Url: "https://cash.rbc.ru/converter.html?from=" + from + "&to=" + to + "&sum=" + textBox1.Text + "&date=&rate=cbrf");
            //label7.Text = Convert.ToString(parsY);
        }
        private object Raschot(string cb1, string cb2)
        {
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                if (cb1 == "Российский рубль")
                {
                    if (cb2 == Convert.ToString(dTable.Rows[i][3]))
                    {
                        double rez = Math.Round(Convert.ToDouble(textBox1.Text) / (Convert.ToDouble(dTable.Rows[i][4]) / Convert.ToDouble(dTable.Rows[i][2])), 4);
                        return rez;
                    }
                }
                if (cb2 == "Российский рубль")
                {
                    if (cb1 == Convert.ToString(dTable.Rows[i][3]))
                    {
                        double rez = Math.Round(Convert.ToDouble(textBox1.Text) * (Convert.ToDouble(dTable.Rows[i][4]) / Convert.ToDouble(dTable.Rows[i][2])), 4);
                        return rez;
                    }
                }
                if (cb1 != "Российские рубль" && cb2 != "Российские рубль")
                {
                    if (cb1 == Convert.ToString(dTable.Rows[i][3]))
                    {
                        double Vrub = Convert.ToDouble(textBox1.Text) * (Convert.ToDouble(dTable.Rows[i][4]) / Convert.ToDouble(dTable.Rows[i][2]));
                        if (Vrub != 0)
                        {
                            for (int j = 0; j < dTable.Rows.Count; j++)
                            {
                                if (cb2 == Convert.ToString(dTable.Rows[j][3]))
                                {
                                    double IZrub = Math.Round(Vrub / (Convert.ToDouble(dTable.Rows[j][4]) / Convert.ToDouble(dTable.Rows[j][2])), 4);
                                    return IZrub;
                                }
                            }
                        }
                        else { return "off"; }
                    }
                }
            }
            return "Ошибка перевода";
        }

        //private Object Parsing(string Url)
        //{
        //    try
        //    {
        //        using (HttpClientHandler Http = new HttpClientHandler() { AllowAutoRedirect = false, AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.None })
        //        {
        //            using (var Client = new HttpClient(Http))
        //            {
        //                using (HttpResponseMessage response = Client.GetAsync(Url).Result)
        //                {
        //                    if (response.IsSuccessStatusCode)
        //                    {
        //                        Console.WriteLine("Первый вызовУ: " + response);
        //                        var html = response.Content.ReadAsStringAsync().Result;
        //                        if (!string.IsNullOrEmpty(html))
        //                        {
        //                            HtmlAgilityPack.HtmlDocument htmlDocument = new HtmlAgilityPack.HtmlDocument();
        //                            htmlDocument.LoadHtml(html);
        //                            Console.WriteLine("2: " + htmlDocument);
        //                            var znach = htmlDocument.DocumentNode.SelectNodes(".//div[@class='calc']//div[@class='calc__box']//div[@class='calc__side']//div[@class='calc__input_box']");                                   
        //                            if (znach != null)
        //                            {
        //                                Console.WriteLine("3: " + znach);
        //                                foreach (var zn in znach)
        //                                {
        //                                    var vovo = zn.SelectNodes(".//div[@class='calc__input_box__rate js-converter-rate-from']");
        //                                    if (vovo != null)
        //                                    {
        //                                        Console.WriteLine("4: "+ vovo.First().InnerText);
        //                                        return vovo.First().InnerText; 
        //                                    }
        //                                }
        //                            }
        //                            else
        //                            {
        //                                MessageBox.Show("ШО-ТО ПОШЛО НЕ ТАК!!!", "Внимание!");
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex) { Console.WriteLine(ex.Message); }
        //    return "BOOM";
        //}

        private void button2_Click(object sender, EventArgs e)
        {
            kontrol = 1;
            Perevod();
            Console.WriteLine();
        }
        private void XmlLinq()
        {
            double[] stkurs = new double[dTable.Rows.Count];
            double[] kode = new double[dTable.Rows.Count];
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                kode[i] = Convert.ToDouble(dTable.Rows[i][0]);
                stkurs[i] = Convert.ToDouble(dTable.Rows[i][4]);
            }
            XDocument xdoc = XDocument.Load("https://www.cbr-xml-daily.ru/daily.xml");
            for (int i = 0; dTable.Rows.Count > i; i++)
            {
                var Val = xdoc.Element("ValCurs").Elements("Valute").Where(p => p.Element("CharCode").Value == "" + dTable.Rows[i][1] + "").Select(
                    p => new { value = p.Element("Value").Value });
                if (Val != null)
                {
                    foreach (var Valute in Val)
                    {
                        double V = Convert.ToDouble(Valute.value);
                        dTable.Rows[i][4] = V;
                    }
                }
            }
            ShowTable(dTable, DataBase_View);
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                if (kode[i] == Convert.ToDouble(dTable.Rows[i][0]))
                {
                    if (stkurs[i] < Convert.ToDouble(dTable.Rows[i][4]))
                    {
                        DataBase_View.Rows[i].Cells[4].Style.ForeColor = Color.Green;
                    }
                    else
                    {
                        DataBase_View.Rows[i].Cells[4].Style.ForeColor = Color.Red;
                    }
                }
            }

        }
        private void Obnov()
        {
            try
            {
                XmlDocument xDoc = new XmlDocument();
                xDoc.Load("https://www.cbr-xml-daily.ru/daily.xml");
                XmlElement xRoot = xDoc.DocumentElement;
                if (xRoot != null)
                {
                    var data = xRoot.Attributes.GetNamedItem("Date");
                    if (Convert.ToString(data.Value) != label6.Text)
                    {
                        label6.Text = Convert.ToString(data.Value);
                        SQLiteCommand _cmd = new SQLiteCommand();
                        _cmd.CommandText = @"UPDATE ДатаПоследногоОбновления SET data=@A WHERE id=1";
                        _cmd.Connection = SQLiteConn;
                        _cmd.Parameters.Add(new SQLiteParameter("@A", label6.Text.ToString().Replace(',', '.')));
                        _cmd.ExecuteNonQuery();
                        XmlLinq();
                        coxpaH();
                        Statice();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Отсутствует подключение к интернету.\nПроверти подключение к интернету и перезапустите программу, чтобы получить последние обновления по курсам валют.","Внимание");
            }
        }
        private void coxpaH()
        {
            SQLiteCommand _cmd = new SQLiteCommand();
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                _cmd.CommandText = @"UPDATE Курс SET Курс=@A WHERE ЦифрКод='" + dTable.Rows[i][0].ToString() + "'";
                _cmd.Connection = SQLiteConn;
                _cmd.Parameters.Add(new SQLiteParameter("@A", dTable.Rows[i][4].ToString().Replace(',', '.')));
                _cmd.ExecuteNonQuery();
            }
        }

        private void Statice()
        {
            SQLiteCommand _cmd = new SQLiteCommand();
            _cmd.Connection = SQLiteConn;
            _cmd.CommandText = "INSERT INTO Статистика (Дата) VALUES ('" + label6.Text.ToString() + "')";
            int number = _cmd.ExecuteNonQuery();
            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                _cmd.CommandText = @"UPDATE Статистика SET '" + dTable.Rows[i][1] + "'=@A WHERE Дата='" + label6.Text.ToString() + "'";
                _cmd.Connection = SQLiteConn;
                _cmd.Parameters.Add(new SQLiteParameter("@A", dTable.Rows[i][4].ToString().Replace(',', '.')));
                _cmd.ExecuteNonQuery();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShowTable2(SQL_AllTable2());
            if (form3 == null || form3.IsDisposed)
            {
                form3 = new Form3(dT);
                form3.Show();
            }
            else
            {
                form3.Show();
                form3.Focus();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            textBox1.MaxLength = 9;
            char n = e.KeyChar;
            if((n<'0'||n>'9')&&n!='\b'&&(n!=','|| textBox1.Text.IndexOf(",")!=-1))
            {
                e.Handled = true;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (form4 == null || form4.IsDisposed)
            {
                form4 = new Form4();
                form4.Show();
            }
            else
            {
                form4.Show();
                form4.Focus();
            }
        }
    }
}

