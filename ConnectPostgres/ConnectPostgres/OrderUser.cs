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
    public partial class OrderUser : Form
    {
        private NpgsqlConnection connection;
        private string connectionstring = ("Server =localhost; Port =5432; Database =delivery; User ID =postgres; Password =jup4uv9k;");
        private string sql;
        private NpgsqlCommand cmd;
        private DataTable dt;
        private int rowIndex = 1;
        public OrderUser()
        {
            InitializeComponent();
        }

        private void OrderUser_Load(object sender, EventArgs e)
        {
            connection = new NpgsqlConnection(connectionstring);
            Select();
        }

        private void Select()
        {
            try
            {
                connection.Open();
                var sql = @"SELECT * FROM orderr";
                cmd = new NpgsqlCommand(sql, connection);
                dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                connection.Close();
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = dt;
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void OrderUser_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
