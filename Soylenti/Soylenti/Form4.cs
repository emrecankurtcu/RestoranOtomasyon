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
    public partial class Form4 : Form
    {
        int table_id;
        OleDbConnection con;
        //OleDbDataAdapter da;
        OleDbCommand cmd;
        OleDbDataReader reader;
        DataSet ds;
        public Form4()
        {
            WindowState = FormWindowState.Maximized;
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            getCategories();
        }

        private void getCategories()
        {
            con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=soylenti_db.accdb");
            cmd = new OleDbCommand("SELECT * FROM categories WHERE is_active='1' ORDER BY name", con);
            con.Open();
            reader = cmd.ExecuteReader();

            listBox1.DisplayMember = "Text";
            listBox1.ValueMember = "Value";
            ArrayList category = new ArrayList();
            while (reader.Read())
            {
                Debug.WriteLine(reader[1].ToString());
                category.Add(new category(reader[1].ToString(), reader[0].ToString()));

            }
            if (category.Count > 0)
            {
                listBox1.DataSource = category;
            }
            else
            {
                listBox1.DataSource = new List<category>();
            }
            con.Close();
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

        private void button1_Click(object sender, EventArgs e)
        {
            string category_name = textBox1.Text;
            if (category_name.Length > 0)
            {
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=soylenti_db.accdb");
                con.Open();

                cmd = new OleDbCommand("INSERT INTO categories(name,is_active) VALUES ('" + category_name + "','1')", con);
                cmd.ExecuteNonQuery();

                Thread.Sleep(750);
                getCategories();
                textBox1.Text = "";
            }
            else
            {
                MessageBox.Show("Kategori adı yazın.");
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            string category_id = listBox1.SelectedValue.ToString();
            string category_name = listBox1.GetItemText(listBox1.SelectedItem);
            textBox1.Text = category_name;
            button1.Visible = false;
            button2.Visible = true;
            button3.Visible = true;
            button5.Visible = true;
            button2.Name = category_id;
            button3.Name = category_id;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string category_name = textBox1.Text;
            if (category_name.Length > 0)
            {
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=soylenti_db.accdb");
                con.Open();

                cmd = new OleDbCommand("UPDATE categories SET name='" + category_name + "' WHERE id=" + button2.Name, con);
                cmd.ExecuteNonQuery();

                Thread.Sleep(750);
                getCategories();
                textBox1.Text = "";
                button1.Visible = true;
                button2.Visible = false;
                button3.Visible = false;
                button5.Visible = false;
            }
            else
            {
                MessageBox.Show("Kategori adı yazın.");
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string message = "Kategoriyi silmek istiyor musunuz?";
            string title = "Kategori Silinecek";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, title, buttons);
            if (result == DialogResult.Yes)
            {
                con = new OleDbConnection("Provider=Microsoft.ACE.Oledb.12.0;Data Source=soylenti_db.accdb");
                con.Open();

                cmd = new OleDbCommand("UPDATE categories SET is_active='0' WHERE id=" + button3.Name, con);
                cmd.ExecuteNonQuery();

                Thread.Sleep(750);
                getCategories();
                textBox1.Text = "";
                button1.Visible = true;
                button2.Visible = false;
                button3.Visible = false;
                button5.Visible = false;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            button1.Visible = true;
            button2.Visible = false;
            button3.Visible = false;
            button5.Visible = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            button1.Visible = true;
            button2.Visible = false;
            button3.Visible = false;
            button5.Visible = false;
        }
    }
}
