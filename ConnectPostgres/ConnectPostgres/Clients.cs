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
    public partial class Clients : Form
    {
        private NpgsqlConnection connection;
        private string connectionstring = ("Server =localhost; Port =5432; Database =delivery; User ID =postgres; Password =jup4uv9k;");
        private string sql;
        private NpgsqlCommand cmd;
        private DataTable dt;
        private int rowIndex = 1;

        public Clients()
        {
            InitializeComponent();
        }

        private void Clients_Load(object sender, EventArgs e)
        {
            connection = new NpgsqlConnection(connectionstring);
            Select();
        }

        private void Select()
        {
            try
            {
                connection.Open();
                var sql = @"SELECT * FROM clients";
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

        public class clients
        {
            public int id_client { get; set; }
            public string client_name { get; set; }
            public string client_surname { get; set; }
            public string adress_order { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (rowIndex < 0)
            {
                MessageBox.Show("Пожалуйста, выберете клиента для обновления");
                return;
            }
            textBox1.Enabled = textBox2.Enabled = textBox3.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            connection.Open();
            sql = @"select * from ww_delete(:_id_client)";
            cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("_id_client", int.Parse(dataGridView1.Rows[rowIndex].Cells["id_client"].Value.ToString()));
            var value = cmd.ExecuteNonQuery();
            if (value == -1)
            {
                connection.Close();
                MessageBox.Show("Удаление успешно");
                rowIndex = -1;
                Select();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                rowIndex = e.RowIndex;
                textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["client_name"].Value.ToString();
                textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells["client_surname"].Value.ToString();
                textBox3.Text = dataGridView1.Rows[e.RowIndex].Cells["adress_order"].Value.ToString();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int result = 0;
            long client_id = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                var data = dataGridView1.Rows[i];
                if ((string)data.Cells[data.Cells.Count - 1].Value == textBox3.Text)
                {
                    client_id = (long)data.Cells[0].Value;
                }
            }

            if (client_id == 0) //insert
            {
                try
                {
                    connection.Open();
                    sql = $@"INSERT INTO public.clients(
	client_name, client_surname, adress_order)
	VALUES ( '{textBox1.Text}', '{textBox2.Text}', '{textBox3.Text}');";
                    cmd = new NpgsqlCommand(sql, connection);
                    result = cmd.ExecuteNonQuery();
                    connection.Close();
                    if (result == 1)
                    {
                        MessageBox.Show("Успешное добавление");
                        Select();
                    }
                }
                catch (Exception ex)
                {
                    connection.Close();
                    MessageBox.Show("Ошибка при добавлении" + ex.Message);
                }

            }
            else//update
            {
                try
                {
                    connection.Open();
                    sql = $@"UPDATE public.clients
	SET  client_name='{textBox1.Text}', client_surname='{textBox2.Text}', adress_order='{textBox3.Text}'
	WHERE id_client= {client_id};";
                    cmd = new NpgsqlCommand(sql, connection);
                    result = cmd.ExecuteNonQuery();
                    connection.Close();
                    if (result == 1)
                    {
                        MessageBox.Show("Успешное обновление");
                        Select();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении");
                    }
                }
                catch (Exception ex)
                {
                    connection.Close();
                    MessageBox.Show("Ошибка при удалении" + ex.Message);
                }
            }
            result = 0;
        }

        private void Clients_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }
    }
}
