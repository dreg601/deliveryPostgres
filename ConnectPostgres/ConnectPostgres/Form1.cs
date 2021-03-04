using Npgsql;
using Dapper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace ConnectPostgres
{
    public partial class Form1 : Form
    {
        private NpgsqlConnection connection;
        private string connectionstring = ("Server =localhost; Port =5432; Database =delivery; User ID =postgres; Password =jup4uv9k;");
        private string sql;
        private NpgsqlCommand cmd;
        private DataTable dt;
        private int rowIndex = 1;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            connection = new NpgsqlConnection(connectionstring);
            Select();
        }

        private void Select()
        {
            try
            {
                connection.Open();
                var sql = @"SELECT * FROM worker";
                cmd = new NpgsqlCommand(sql, connection);
                dt = new DataTable();
                //var dtt = connection.Query<worker>(sql).First();
                dt.Load(cmd.ExecuteReader());
                connection.Close();
                dgvData.DataSource = null;//reset datagridview
                dgvData.DataSource = dt;
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public class worker
        {
            public int id_worker { get; set; }
            public string worker_name { get; set; }
            public string worker_surname { get; set; }
            public string worker_position { get; set; }

            public string gender { get; set; }
            public string phone_number { get; set; }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (rowIndex < 0)
            {
                MessageBox.Show("Пожалуйста, выберете работника для обновления");
                return;
            }
            textBox2.Enabled = textBox3.Enabled = textBox4.Enabled = textBox6.Enabled = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            connection.Open();
            sql = @"select * from w_delete(:_id_worker)";
            cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("_id_worker", int.Parse(dgvData.Rows[rowIndex].Cells["id_worker"].Value.ToString()));
            var value = cmd.ExecuteNonQuery();
            if (value==-1)
            {
                connection.Close();
                MessageBox.Show("Удаление успешно");
                rowIndex = -1;
                Select();
            }
            
        }

        private void dgvData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex>=0)
            {
                rowIndex = e.RowIndex;
                textBox2.Text = dgvData.Rows[e.RowIndex].Cells["worker_name"].Value.ToString();
                textBox3.Text = dgvData.Rows[e.RowIndex].Cells["worker_surname"].Value.ToString();
                textBox4.Text = dgvData.Rows[e.RowIndex].Cells["worker_position"].Value.ToString();
                textBox6.Text = dgvData.Rows[e.RowIndex].Cells["phone_number"].Value.ToString();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int result = 0;
            long worker_id = 0;
            for (int i = 0; i < dgvData.Rows.Count; i++)
            {
                var data = dgvData.Rows[i];
                if ((string)data.Cells[data.Cells.Count - 1].Value == textBox6.Text)
                {
                    worker_id = (long)data.Cells[0].Value;
                }
            }

            if (worker_id==0) //insert
            {
                try
                {
                    connection.Open();
                    sql = $@"INSERT INTO public.worker(
	worker_name, worker_surname, worker_position, gender, phone_number)
	VALUES ( '{textBox2.Text}', '{textBox3.Text}', '{textBox4.Text}', {(checkBox1.Checked ? "TRUE" : "FALSE")},'{textBox6.Text}');";
                    cmd = new NpgsqlCommand(sql, connection);
                    result = cmd.ExecuteNonQuery();
                    connection.Close();
                    if(result ==1)
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
            else//updaate
            {
                try
                {
                    connection.Open();        
                    sql = $@"UPDATE public.worker
	SET  worker_name='{textBox2.Text}', worker_surname='{textBox3.Text}', worker_position='{textBox4.Text}', gender={(checkBox1.Checked?"TRUE":"FALSE")}, phone_number='{textBox6.Text}'
	WHERE id_worker= {worker_id};";
                    cmd = new NpgsqlCommand(sql, connection);
                    result = cmd.ExecuteNonQuery();
                    connection.Close();
                    if (result ==1)
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            Form2 form2 = new Form2();
            form2.Show();
        }
    }
}
