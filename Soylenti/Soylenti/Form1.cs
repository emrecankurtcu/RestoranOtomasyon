using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Threading;

namespace Soylenti
{
    public partial class Form1 : Form
    {
        OleDbConnection con;
        //OleDbDataAdapter da;
        OleDbCommand cmd;
        OleDbDataReader reader;
        DataSet ds;
        public Form1()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            panel1.Location = new System.Drawing.Point(180, 50);
            panel1.Size = new System.Drawing.Size(1000, 700);

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            getTables();
        }

        private void getForm2(object sender, EventArgs e)
        {
            var button = (Button)sender;
            
            Form2 form = new Form2(Convert.ToInt32(button.Name));
            form.Show();
        }

        private void getTables()
        {
            con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=soylenti_db.accdb");
            cmd = new OleDbCommand("SELECT *,(SELECT IIF(IsNull(SUM(orders.product_price)),0,SUM(orders.product_price)) FROM orders WHERE orders.table_id=tables.id AND is_active = '1') AS total FROM tables WHERE tables.is_active = '1' ORDER BY id", con);
            con.Open();
            reader = cmd.ExecuteReader();
            int y = 15;
            int x = 15;
            while (reader.Read())
            {
                if (x > 830)
                {
                    y += 140;
                    x = 15;
                }
                Button btn = new Button();
                btn.Text = reader[2].ToString()+" - "+ money_format(Convert.ToDouble(reader[0].ToString()));
                btn.Name = reader[1].ToString();
                btn.Click += new EventHandler(getForm2);
                btn.ForeColor = Color.White;
                btn.Font = new Font("Microsoft Sans Serif", 11);
                if (Convert.ToDouble(reader[0].ToString()) == 0)
                {
                    btn.BackColor = Color.Green;
                }
                else
                {
                    btn.BackColor = Color.Red;
                }
                btn.Size = new Size(120, 120);
                btn.Location = new Point(x, y);
                this.AutoScroll = true;
                panel1.Controls.Add(btn);
                x += 140;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Controls.Clear();
            this.InitializeComponent();
            panel1.Location = new System.Drawing.Point(180, 50);
            panel1.Size = new System.Drawing.Size(1000, 700);
            getTables();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.Show();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Form5 form5 = new Form5();
            form5.Show();
        }

        private string money_format(double number)
        {
            string format = "";
            format = String.Format("{0:C2}", number);
            return format;
        }
    }
}
