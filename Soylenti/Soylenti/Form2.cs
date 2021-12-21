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
using System.Collections;
using System.Threading;
using System.Diagnostics;

namespace Soylenti
{
    public partial class Form2 : Form
    {
        int table_id;
        OleDbConnection con;
        //OleDbDataAdapter da;
        OleDbCommand cmd;
        OleDbDataReader reader;
        DataSet ds;


        ListBox listBox2 = new ListBox();
        public Form2(int table_id)
        {
            WindowState = FormWindowState.Maximized;
            InitializeComponent();
            this.table_id = table_id;

            getOrders();
            cmd = new OleDbCommand("SELECT * FROM categories WHERE is_active = '1' ORDER BY name", con);
            con.Open();
            reader = cmd.ExecuteReader();
            int y2 = 50;
            int x2 = 350;
            while (reader.Read())
            {
                if (x2 > 1100)
                {
                    y2 += 140;
                    x2 = 350;
                }
                Button btn = new Button();
                btn.Text = reader[1].ToString();
                btn.Name = reader[0].ToString();
                btn.Click += new EventHandler(getCategoryProducts);
                btn.Size = new Size(120, 120);
                btn.Location = new Point(x2, y2);
                Controls.Add(btn);
                x2 += 140;
            }
            listBox2.Size = new System.Drawing.Size(550, 300);
            listBox2.Location = new System.Drawing.Point(350, 350);
            listBox2.Font = new Font("Microsof Sans Serif", 12);

            listBox2.Name = "listBox2";
            listBox2.DoubleClick += new EventHandler(listBox2_DoubleClick);

            listBox2.DisplayMember = "Text";
            listBox2.ValueMember = "Value";
            this.Controls.Add(listBox2);



        }
        private void Form2_Load(object sender, EventArgs e)
        {
            




        }

        private void getCategoryProducts(object sender, EventArgs e)
        {
            var button = (Button)sender;


            con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=soylenti_db.accdb");
            cmd = new OleDbCommand("SELECT * FROM products WHERE is_active = '1' AND category_id="+ Convert.ToInt32(button.Name), con);
            con.Open();
            reader = cmd.ExecuteReader();

            ArrayList product = new ArrayList();
            while (reader.Read())
            {
                product.Add(new product(reader[2].ToString() + " - " + money_format(Convert.ToDouble(reader[3].ToString())) + " ", reader[0].ToString()));

            }
            if (product.Count > 0)
            {
                listBox2.DataSource = product;
            }
            else
            {
                listBox2.DataSource = new List<product>();
            }
            con.Close();



        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {

            if (listBox2.SelectedItem != null)
            {
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=soylenti_db.accdb");
                con.Open();
                DateTime now = DateTime.Now;
                cmd = new OleDbCommand("INSERT INTO orders(table_id,product_name,product_price,is_paid,is_active,created_date) SELECT '"+this.table_id+"' AS table_id,name AS product_name,price AS product_price,'0' AS is_paid,'1' AS is_active,'"+ now + "' AS created_date FROM products WHERE id="+ Convert.ToInt32(listBox2.SelectedValue.ToString()), con);
                cmd.ExecuteNonQuery();

                //listBox1.Items.Add(listBox2.SelectedValue.ToString());
                Thread.Sleep(750);
                getOrders();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=soylenti_db.accdb");
                con.Open();

                foreach (var item in listBox1.SelectedItems)
                {
                    cmd = new OleDbCommand("UPDATE orders SET payment_method = 'Nakit',is_paid = '1' WHERE id="+ item.GetType().GetProperty("Value").GetValue(item, null), con);
                    cmd.ExecuteNonQuery();

                }
                Thread.Sleep(750);
                getOrders();
            }
            else
            {
                MessageBox.Show("Ürün Seçin");
            }
        }

        private void getOrders()
        {
            con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=soylenti_db.accdb");
            cmd = new OleDbCommand("SELECT id,product_name,product_price,payment_method,is_paid FROM orders WHERE table_id= " + this.table_id + " AND is_active = '1'", con);
            con.Open();
            double paid = 0;
            double remaining = 0;
            double total = 0;
            listBox1.DisplayMember = "Text";
            listBox1.ValueMember = "Value";
            reader = cmd.ExecuteReader();

            ArrayList order = new ArrayList();
            while (reader.Read())
            {
                string is_paid = "";
                if (reader[4].ToString() == "1")
                {
                    is_paid = " - "+reader[3].ToString()+" ile Ödendi";
                    paid += Convert.ToDouble(reader[2].ToString());
                }

                total += Convert.ToDouble(reader[2].ToString());
                order.Add(new order(reader[1].ToString() + " - " + money_format(Convert.ToDouble(reader[2].ToString())) + " "+ is_paid, reader[0].ToString()));
            }
            
            label4.Text = money_format(paid);
            label5.Text = money_format(total - paid);
            label6.Text = money_format(total);
            if(order.Count > 0)
            {
                listBox1.DataSource = order;
            }
            else
            {
                listBox1.DataSource = new List<order>();
            }
            con.Close();

        }

        private string money_format(double number)
        {
            string format = "";
            format = String.Format("{0:C2}", number);
            return format;
        }

        public class product
        {
            private string value;
            private string text;

            public product(string strText, string strValue)
            {

                this.value = strValue;
                this.text = strText;
            }

            public string Value
            {
                get
                {
                    return value;
                }
            }

            public string Text
            {

                get
                {
                    return text;
                }
            }
        }

        public class order
        {
            private string value;
            private string text;

            public order(string strText, string strValue)
            {

                this.value = strValue;
                this.text = strText;
            }

            public string Value
            {
                get
                {
                    return value;
                }
            }

            public string Text
            {

                get
                {
                    return text;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=soylenti_db.accdb");
            con.Open();

            cmd = new OleDbCommand("UPDATE orders SET is_active = '0' WHERE table_id="+this.table_id, con);
            cmd.ExecuteNonQuery();
            Thread.Sleep(750);
            getOrders();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=soylenti_db.accdb");
                con.Open();

                foreach (var item in listBox1.SelectedItems)
                {
                    cmd = new OleDbCommand("UPDATE orders SET payment_method = 'Kredi Kartı',is_paid = '1' WHERE id=" + item.GetType().GetProperty("Value").GetValue(item, null), con);
                    cmd.ExecuteNonQuery();

                }
                Thread.Sleep(750);
                getOrders();
            }
            else
            {
                MessageBox.Show("Ürün Seçin");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedItems.Count > 0)
            {
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=soylenti_db.accdb");
                con.Open();

                foreach (var item in listBox1.SelectedItems)
                {
                    cmd = new OleDbCommand("UPDATE orders SET payment_method = '',is_paid = '0' WHERE id=" + item.GetType().GetProperty("Value").GetValue(item, null), con);
                    cmd.ExecuteNonQuery();

                }
                Thread.Sleep(750);
                getOrders();
            }
            else
            {
                MessageBox.Show("Ürün Seçin");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count > 0)
            {
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=soylenti_db.accdb");
                con.Open();

                foreach (var item in listBox1.SelectedItems)
                {
                    cmd = new OleDbCommand("DELETE FROM orders WHERE id=" + item.GetType().GetProperty("Value").GetValue(item, null), con);
                    cmd.ExecuteNonQuery();

                }
                Thread.Sleep(500);
                getOrders();
            }
            else
            {
                MessageBox.Show("Ürün Seçin");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                listBox1.SetSelected(i, true);
            }
        }
    }
}
