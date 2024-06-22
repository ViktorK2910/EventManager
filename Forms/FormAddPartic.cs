using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace EventManagerKubSU.Forms
{
    public partial class FormAddPartic : Form
    {
        DataTable table = null;
        DataTable table2 = null;
        DataTable table3 = null;
        DataTable table4 = null;
        SqlDataAdapter adapter = null;
        SqlConnection sqlConnection;
        bool flag = false;
        string conn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\EventManager\EventManagerKubSU\EventDB.mdf;Integrated Security=True;Connect Timeout=30";
        //string conn = @"Data Source=.\SQLEXPRESS; AttachDbFilename=|DataDirectory|\EventManagerKubSU\EventDB.mdf; Integrated Security=True;User Instance=True";

        public FormAddPartic()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(conn);
            sqlConnection.Open();
            table = new DataTable();
            table2 = new DataTable();
            table3 = new DataTable();
            SqlCommand sqlCmd = new SqlCommand("SELECT name_event FROM [Event] WHERE completed = 0", sqlConnection);
            SqlCommand sqlCmd2 = new SqlCommand("SELECT number_group FROM [Group]", sqlConnection);
            SqlDataAdapter adap = new SqlDataAdapter(sqlCmd);
            SqlDataAdapter adap2 = new SqlDataAdapter(sqlCmd2);
            adap.Fill(table);
            adap2.Fill(table2);
            comboBox1.DisplayMember = "name_event";
            comboBox2.DisplayMember = "number_group";
            comboBox1.ValueMember = "name_event";
            comboBox2.ValueMember = "number_group";
            comboBox1.DataSource = table;
            comboBox2.DataSource = table2;
            sqlConnection.Close();
            AddAmountStudent.Text = "";
            
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(conn);
            await sqlConnection.OpenAsync();
            string num_gr = comboBox2.SelectedValue.ToString();
            string name_event = comboBox1.SelectedValue.ToString();

            if (AddAmountStudent.Text != "" && comboBox1.SelectedIndex > -1 && comboBox2.SelectedIndex > -1)
            {

                SqlCommand com = new SqlCommand("SELECT count(*) FROM ListEventGroup WHERE name_event = @name_event AND number_group = @num_gr", sqlConnection);
                com.Parameters.AddWithValue("name_event", name_event);
                com.Parameters.AddWithValue("@num_gr", num_gr);


                int i = Convert.ToInt32(com.ExecuteScalar());

                Console.WriteLine(i);


                string sql;
                SqlCommand command;
                if (AddAmountStudent.Text != "")
                {
                    if (i == 1)
                    {
                        MessageBox.Show("Данная группа уже участвует в мероприятии, добавили участников!");
                        sql = "UPDATE ListEventGroup SET registered_amount = registered_amount + @registered_amount WHERE name_event = @name_event AND number_group = @number_group";
                        command = new SqlCommand(sql, sqlConnection);
                        command.Parameters.AddWithValue("number_group", this.comboBox2.SelectedValue.ToString());
                        command.Parameters.AddWithValue("registered_amount", Convert.ToInt32(this.AddAmountStudent.Text));
                        command.Parameters.AddWithValue("name_event", this.comboBox1.SelectedValue.ToString());
                        command.ExecuteNonQuery();
                        CheckCollect();
                    }
                    else
                    {
                        sql = "INSERT INTO [ListEventGroup] (number_group, name_event, registered_amount) VALUES (@number_group,@name_event,@registered_amount)";
                        command = new SqlCommand(sql, sqlConnection);
                        command.Parameters.AddWithValue("number_group", this.comboBox2.SelectedValue.ToString());
                        command.Parameters.AddWithValue("name_event", this.comboBox1.SelectedValue.ToString());
                        command.Parameters.AddWithValue("registered_amount", Convert.ToInt32(this.AddAmountStudent.Text));
                        command.ExecuteNonQuery();
                        CheckCollect();
                    }
                }
            }

            else
            {
                MessageBox.Show("Заполните все поля");
            }
        }

        private void CheckCollect()
        {
            string sql1 = "UPDATE Participants SET registered_amount = registered_amount + @registered_amount WHERE name_event = @name_event";
            SqlCommand command1 = new SqlCommand(sql1, sqlConnection);
            command1.Parameters.AddWithValue("registered_amount", Convert.ToInt32(this.AddAmountStudent.Text));
            command1.Parameters.AddWithValue("name_event", this.comboBox1.SelectedValue.ToString());
            command1.ExecuteNonQuery();

            string sql2 = "SELECT registered_amount FROM Participants WHERE name_event = @name_event";
            SqlCommand command2 = new SqlCommand(sql2, sqlConnection);
            command2.Parameters.AddWithValue("name_event", this.comboBox1.SelectedValue.ToString());
            int a = Convert.ToInt32(command2.ExecuteScalar().ToString());


            string sql3 = "SELECT required_amount FROM Event WHERE name_event = @name_event";
            command2 = new SqlCommand(sql3, sqlConnection);
            command2.Parameters.AddWithValue("name_event", this.comboBox1.SelectedValue.ToString());
            int b = Convert.ToInt32(command2.ExecuteScalar().ToString());

            if (a >= b)
            {
                sql2 = "UPDATE Participants SET collected = 1 WHERE name_event = @name_event";
                command2 = new SqlCommand(sql2, sqlConnection);
                command2.Parameters.AddWithValue("name_event", this.comboBox1.SelectedValue.ToString());
                command2.ExecuteNonQuery();
                MessageBox.Show("Собрано достаточно участников на это мероприятие: " + this.comboBox1.SelectedValue.ToString());
            }

            this.AddAmountStudent.Text = "";
            sqlConnection.Close();

            ShowPartic();

            sqlConnection.Close();
        }

        private async void ShowPartic()
        {
            await Task.Delay(500);
            sqlConnection = new SqlConnection(conn);
            await sqlConnection.OpenAsync();

            try
            {
                table3.Clear();

                adapter = new SqlDataAdapter("SELECT name_event AS 'Назавние мероприятия', registered_amount AS 'Зарегистрировано', CASE WHEN collected = 0 THEN N'Не собрано' ELSE N'Достаточно' END AS 'Участники' FROM [Participants]  WHERE name_event IN (SELECT name_event FROM [Event] WHERE completed = 0)", sqlConnection);
                adapter.Fill(table3);
                dataGridView1.DataSource = table3;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            } 
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Hi!");
            }
        }

        private void FormAddPartic_Load(object sender, EventArgs e)
        {

        }
    }
}
