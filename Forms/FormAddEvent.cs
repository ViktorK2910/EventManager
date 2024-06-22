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
    public partial class FormAddEvent : Form
    {
        //DataTable table = null;
        //SqlDataAdapter adapter = null;
        SqlConnection sqlConnection;
        string conn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\EventManager\EventManagerKubSU\EventDB.mdf;Integrated Security=True;Connect Timeout=30";
        //string conn = @"Data Source=.\SQLEXPRESS; AttachDbFilename=|DataDirectory|\EventManagerKubSU\EventDB.mdf; Integrated Security=True;User Instance=True";
        List<string> nameEvent = new List<string>();
        DataTable table = new DataTable();
        bool flag1 = false;
        bool flag2 = false;

        public FormAddEvent()
        {
            InitializeComponent();
            sqlConnection = new SqlConnection(conn);
            sqlConnection.Open();           
        }


        private void button1_Click(object sender, EventArgs e)
        {
            SqlCommand sqlCmd = new SqlCommand("SELECT name_event FROM [Event] WHERE completed = 0", sqlConnection);
            SqlDataAdapter adap = new SqlDataAdapter(sqlCmd);
            adap.Fill(table);
          
            if (textBox1.Text != "")
            {
                foreach (DataRow row in table.Rows)
                {
                    string name = row["name_event"].ToString();
                    Console.WriteLine(name);
                    if (row["name_event"].ToString() == textBox1.Text.ToString())
                    {
                        flag1 = true;
                        MessageBox.Show("Такое мероприятие уже существует!");
                    }
                }

                if (DateTime.Now >= dateTimePicker1.Value)
                {
                    flag2 = true;
                    MessageBox.Show("Указанная дата прошла. Введите другую!");
                }
            }
            else MessageBox.Show("Введите название мероприятия!");

            if (textBox1.Text != "" && flag1 == false && flag2 == false)
            {
                Console.WriteLine(textBox1.Text);
                string sql = "INSERT INTO [Event] (name_event,date,venue,responsible,required_amount,comment) VALUES (@name_event,@date,@venue,@responsible,@required_amount,@comment)";
                SqlCommand command = new SqlCommand(sql,sqlConnection);
                command.Parameters.AddWithValue("name_event", this.textBox1.Text);
                command.Parameters.AddWithValue("date", this.dateTimePicker1.Value.Date);
                command.Parameters.AddWithValue("venue", this.textBox3.Text);
                command.Parameters.AddWithValue("responsible", this.textBox4.Text);
                command.Parameters.AddWithValue("required_amount", Convert.ToInt32(this.numericUpDown1.Text));  
                command.Parameters.AddWithValue("comment", this.richTextBox1.Text);
                command.ExecuteNonQuery();
                

                sql = "INSERT INTO [Participants] (name_event) VALUES (@name_event)";
                command = new SqlCommand(sql, sqlConnection);
                command.Parameters.AddWithValue("name_event", this.textBox1.Text);
                command.ExecuteNonQuery();

                this.textBox1.Text = "";
                this.textBox3.Text = "";
                this.textBox4.Text = "";
                this.numericUpDown1.Value = 0;
                this.richTextBox1.Text = "";
                MessageBox.Show("Мероприятие добавлено!");
            }
            flag1 = false;
            flag2 = false;
        }

        private void FormAddEvent_Load(object sender, EventArgs e)
        {
            //sqlConnection = new SqlConnection(conn);
            //await sqlConnection.OpenAsync();
            table = new DataTable();
        }
    }
}
