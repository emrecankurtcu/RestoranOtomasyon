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
    public partial class Form5 : Form
    {
        OleDbConnection con;
        OleDbCommand cmd;
        OleDbCommand cmd2;
        OleDbDataReader reader;
        OleDbDataReader reader2;
        DataSet ds;
        public Form5()
        {
            WindowState = FormWindowState.Maximized;
            InitializeComponent();
            con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=soylenti_db.accdb");
            comboBox1.DisplayMember = "Text";
            comboBox1.ValueMember = "Value";
            panel1.Location = new System.Drawing.Point(400, 40);
            panel1.Size = new System.Drawing.Size(1000, 800);
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            getProducts();
        }

        private void getProducts()
        {
            cmd = new OleDbCommand("SELECT * FROM categories WHERE is_active = '1' ORDER BY name", con);
            con.Open();
            reader = cmd.ExecuteReader();
            int y2 = 50;
            int x2 = 15;
            ArrayList category = new ArrayList();
            while (reader.Read())
            {
                if (x2 > 914)
                {
                    y2 += 350;
                    x2 = 15;
                }

                ListBox listbox = new ListBox();
                listbox.Size = new System.Drawing.Size(400, 300);
                listbox.Location = new System.Drawing.Point(0, 15);
                listbox.Font = new Font("Microsof Sans Serif", 12);
                listbox.Click += new EventHandler(listBox_Click);

                listbox.Name = reader[0].ToString();
                listbox.DoubleClick += new EventHandler(listBox_DoubleClick);

                listbox.DisplayMember = "Text";
                listbox.ValueMember = "Value";
                GroupBox groupBox = new GroupBox();
                groupBox.Text = reader[1].ToString();
                groupBox.Location = new System.Drawing.Point(x2, y2);
                groupBox.Size = new System.Drawing.Size(400, 300);
                groupBox.Controls.Add(listbox);
                panel1.Controls.Add(groupBox);
                category.Add(new category(reader[1].ToString(), reader[0].ToString()));

                x2 += 450;
                cmd2 = new OleDbCommand("SELECT * FROM products WHERE is_active = '1' AND category_id = "+ reader[0].ToString() + " ORDER BY name", con);
                reader2 = cmd2.ExecuteReader();

                ArrayList product = new ArrayList();
                while (reader2.Read())
                {
                    product.Add(new product(reader2[2].ToString() + " - " + money_format(Convert.ToDouble(reader2[3].ToString())), reader2[0].ToString()));
                }
                if (product.Count > 0)
                {
                    listbox.DataSource = product;
                }
                else
                {
                    listbox.DataSource = new List<product>();
                }


            }
            if (category.Count > 0)
            {
                comboBox1.DataSource = category;
            }
            else
            {
                comboBox1.DataSource = new List<category>();
            }

            con.Close();

        }

        private void listBox_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            comboBox1.SelectedIndex = 0;
            button1.Visible = true;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
        }

        private void listBox_DoubleClick(object sender, EventArgs e)
        {
            var listbox = (ListBox)sender;
            string product_id = listbox.SelectedValue.ToString();
            cmd = new OleDbCommand("SELECT products.id AS product_id,products.name AS product_name,products.price AS product_price,categories.id AS category_id,categories.name AS category_name FROM products,categories WHERE categories.id = products.category_id AND products.id = " + product_id+"", con);
            con.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                textBox1.Text = reader[1].ToString();
                comboBox1.SelectedValue = reader[3].ToString();
                textBox2.Text = reader[2].ToString();
            }



            button1.Visible = false;
            button2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            button2.Name = product_id;
            button3.Name = product_id;
            con.Close();
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

        public class category
        {
            private string value;
            private string text;

            public category(string strText, string strValue)
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

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            comboBox1.SelectedIndex = 0;
            button1.Visible = true;
            button2.Visible = false;
            button3.Visible = false;
            button4.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string product_name = textBox1.Text;
            string product_category = comboBox1.SelectedValue.ToString();
            string product_price = textBox2.Text;
            if (product_name.Length > 0 || product_price.Length > 0)
            {
                con.Open();

                cmd = new OleDbCommand("INSERT INTO products(name,category_id,price,is_active) VALUES('"+ product_name + "',"+ product_category + ","+product_price+",'1')", con);
                cmd.ExecuteNonQuery();

                textBox1.Text = "";
                textBox2.Text = "";
                button1.Visible = true;
                button2.Visible = false;
                button3.Visible = false;
                button4.Visible = false;
                con.Close();
                panel1.Controls.Clear();
                Thread.Sleep(750);
                getProducts();
            }
            else
            {
                MessageBox.Show("Tüm alanları doldurun.");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string message = "Ürünü silmek istiyor musunuz?";
            string title = "Ürün Silinecek";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                con.Open();

                cmd = new OleDbCommand("UPDATE products SET is_active='0' WHERE id=" + button3.Name, con);
                cmd.ExecuteNonQuery();

                textBox1.Text = "";
                textBox2.Text = "";
                button1.Visible = true;
                button2.Visible = false;
                button3.Visible = false;
                button4.Visible = false;
                con.Close();
                panel1.Controls.Clear();
                Thread.Sleep(750);
                getProducts();

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string product_name = textBox1.Text;
            string product_category = comboBox1.SelectedValue.ToString();
            string product_price = textBox2.Text;
            if (product_name.Length > 0 || product_price.Length > 0)
            {
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=soylenti_db.accdb");
                con.Open();

                cmd = new OleDbCommand("UPDATE products SET name='"+product_name+"',price="+product_price+",category_id="+product_category+" WHERE id="+button2.Name, con);
                cmd.ExecuteNonQuery();

                textBox1.Text = "";
                textBox2.Text = "";
                button1.Visible = true;
                button2.Visible = false;
                button3.Visible = false;
                button4.Visible = false;
                con.Close();
                panel1.Controls.Clear();
                Thread.Sleep(750);
                getProducts();
            }
            else
            {
                MessageBox.Show("Tüm alanları doldurun.");
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
        (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
        private string money_format(double number)
        {
            string format = "";
            format = String.Format("{0:C2}", number);
            return format;
        }
    }
}
