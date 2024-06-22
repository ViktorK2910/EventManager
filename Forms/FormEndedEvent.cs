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
    public partial class FormEndedEvent : Form
    {
        SqlConnection sqlConnection;
        SqlDataAdapter adapter = null;
        DataTable table = null;
        string conn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\EventManager\EventManagerKubSU\EventDB.mdf;Integrated Security=True;Connect Timeout=30";
        //string conn = @"Data Source=.\SQLEXPRESS; AttachDbFilename=|DataDirectory|\EventManagerKubSU\EventDB.mdf; Integrated Security=True;User Instance=True";
        public FormEndedEvent()
        {
            InitializeComponent();
        }

        private async void FormEndedEvent_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(conn);
            await sqlConnection.OpenAsync();
            table = new DataTable();
            try
            {
                table.Clear();

                adapter = new SqlDataAdapter("SELECT name_event AS 'Название', date AS 'Дата', venue AS 'Место', responsible AS 'Ответственный', required_amount AS 'Необходимое кол-во', comment AS 'Коммент' FROM [Event] WHERE completed = 1", sqlConnection);
                adapter.Fill(table);
                dataGridView1.DataSource = table;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection.Close();
            }

            /*SqlCommand sqlCmd = new SqlCommand("SELECT name_event FROM [Event] WHERE completed = 1", sqlConnection);
            SqlDataAdapter adap = new SqlDataAdapter(sqlCmd);
            adap.Fill(table);
            foreach (DataRow row in table.Rows)
            {
                string name = row["name_event"].ToString();
                Console.WriteLine(name);
                if (row["name_event"].ToString() == textBox1.Text.ToString())
                {
                    flag1 = true;
                    MessageBox.Show("Не заполнено обязательное поле 'Название мероприятия'! Или Такое мероприятие уже существует!");
                }

            }*/ 
            //поиск завершенных для последующего удаления групп из участников
        }
    }
}
