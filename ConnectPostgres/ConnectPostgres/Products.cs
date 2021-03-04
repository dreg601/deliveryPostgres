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
    public partial class Products : Form
    {
        private NpgsqlConnection connection;
        private string connectionstring = ("Server =localhost; Port =5432; Database =delivery; User ID =postgres; Password =jup4uv9k;");
        private string sql;
        private NpgsqlCommand cmd;
        private DataTable dt;
        private int rowIndex = 1;
        public Products()
        {
            InitializeComponent();
        }

        private void Products_Load(object sender, EventArgs e)
        {
            connection = new NpgsqlConnection(connectionstring);
            Select();
        }

        private void Select()
        {
            try
            {
                connection.Open();
                var sql = @"SELECT * FROM product";
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

        private void Products_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (rowIndex < 0)
            {
                MessageBox.Show("Пожалуйста, выберете продукт для обновления");
                return;
            }
            textBox1.Enabled = textBox2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            connection.Open();
            sql = @"select * from www_delete(:_id_product)";
            cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("_id_product", int.Parse(dataGridView1.Rows[rowIndex].Cells["id_product"].Value.ToString()));
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
            rowIndex = e.RowIndex;
            textBox1.Text = dataGridView1.Rows[e.RowIndex].Cells["cost_product"].Value.ToString();
            textBox2.Text = dataGridView1.Rows[e.RowIndex].Cells["product_name"].Value.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int result = 0;
            long product_id = 0;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                var data = dataGridView1.Rows[i];
                if ((string)data.Cells[data.Cells.Count - 1].Value == textBox2.Text)
                {
                    product_id = (long)data.Cells[0].Value;
                }
            }

            if (product_id == 0) //insert
            {
                try
                {
                    connection.Open();
                    sql = $@"INSERT INTO public.product(
	cost_product, product_name)
	VALUES ( '{textBox1.Text}', '{textBox2.Text}');";
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
                    sql = $@"UPDATE public.product
	SET  cost_product='{textBox1.Text}', product_name='{textBox2.Text}'
	WHERE id_product= {product_id};";
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
    }
}
