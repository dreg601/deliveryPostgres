using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConnectPostgres
{
    public partial class Mainform : Form
    {
        private NpgsqlConnection connection;
        private string connectionstring = ("Server =localhost; Port =5432; Database =delivery; User ID =postgres; Password =jup4uv9k;");
        private string sql;
        private NpgsqlCommand cmd;
        private DataTable dt;
        private int rowIndex = 1;
        public Mainform()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            connection = new NpgsqlConnection(connectionstring);
            Select();
            string admin = "admin";
            string user = "user";
            if (textBox1.Text == admin)
            {
                if (textBox2.Text == admin)
                {
                    this.Hide();
                    Form2 form2 = new Form2();
                    form2.Show();
                }
            }
            else if (textBox1.Text == user)
            {
                if (textBox2.Text == user)
                {
                    this.Hide();
                    OrderUser orderUser = new OrderUser();
                    orderUser.Show();
                }
            }
        }

        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            textBox1.Text = "";
        }

        private void textBox2_MouseDown(object sender, MouseEventArgs e)
        {
            textBox2.Text = "";
        }
    }
}
