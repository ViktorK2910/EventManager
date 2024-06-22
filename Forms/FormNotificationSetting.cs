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
using ExcelDataReader;
using System.IO;
using System.Configuration;
using System.Globalization;
using System.Net.Mail;

namespace EventManagerKubSU.Forms
{
    public partial class FormNotificationSetting : Form
    {
        SqlConnection sqlConnection;
        SqlDataAdapter adapter = null;
        DataTable table = null;
        string conn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\EventManager\EventManagerKubSU\EventDB.mdf;Integrated Security=True;Connect Timeout=30";
        //string conn = @"Data Source=(LocalDB)\MSSQLLocalDB; AttachDbFilename=|DataDirectory|\EventManagerKubSU\EventDB.mdf; Integrated Security=True;User Instance=True;
        
        DataTableCollection tableCollection;
        public string a;
        SqlDataAdapter adapter2 = null;
        DataTable table2 = null;

        public FormNotificationSetting()
        {
            InitializeComponent();
            table = new DataTable();
            table2 = new DataTable();
            label3.Visible = false;
            comboBox3.Visible = false;
            button3.Visible = false;
        }

        private async void FormNotificationSetting_Load(object sender, EventArgs e)
        { 
            sqlConnection = new SqlConnection(conn);
            await sqlConnection.OpenAsync();
            SqlCommand sqlCmd = new SqlCommand("SELECT name_event FROM [Event] WHERE completed = 0", sqlConnection);
            SqlDataAdapter adap = new SqlDataAdapter(sqlCmd);
            adap.Fill(table);
            comboBox2.DisplayMember = "name_event";
            comboBox2.ValueMember = "name_event";
            comboBox2.DataSource = table;

            try
            {
                string sql2 = "SELECT number_group FROM Participants WHERE name_event = @name_event";
                SqlCommand command2 = new SqlCommand(sql2, sqlConnection);
                command2.Parameters.AddWithValue("name_event", this.comboBox2.SelectedValue.ToString());
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }




            sqlConnection.Close();
            /*try
            {
                table.Clear();

                adapter = new SqlDataAdapter("SELECT name_event AS 'Название', date AS 'Дата', venue AS 'Место', responsible AS 'Ответственный' FROM [Event] WHERE completed = 0", sqlConnection);
                adapter.Fill(table);
                dataGridView1.DataSource = table;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }*/
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                button1.Text = "Выполнить";
                label3.Visible = false;
                comboBox3.Visible = false;
                button3.Visible = false;
            }
            else button1.Text = "Выбрать группу";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                
            }
            else
            {
                table2.Clear();
                label3.Visible = true;
                comboBox3.Visible = true;
                button3.Visible = true;
                string name_event = comboBox2.SelectedValue.ToString();
                SqlCommand sqlCmd2 = new SqlCommand("SELECT number_group FROM [ListEventGroup] WHERE name_event = @name_event", sqlConnection);
                SqlDataAdapter adap2 = new SqlDataAdapter(sqlCmd2);
                sqlCmd2.Parameters.AddWithValue("name_event", name_event);
                adap2.Fill(table2);
                comboBox3.DisplayMember = "number_group";
                comboBox3.ValueMember = "number_group";
                comboBox3.DataSource = table2;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.mail.ru");

            smtpClient.UseDefaultCredentials = true;
            smtpClient.EnableSsl = true;

            System.Net.NetworkCredential basicAuthentificationInfo = new
                System.Net.NetworkCredential("kubsu.event@mail.ru","2UMnNz1zZTQ9yLBycAJS");
            smtpClient.Credentials = basicAuthentificationInfo;

            MailAddress from = new MailAddress("kubsu.event@mail.ru", "Мероприятия КубГУ");
            MailAddress to = new MailAddress("boss.kolcov@mail.ru", "Студент КубГУ");
            MailMessage myMail = new System.Net.Mail.MailMessage(from, to);

            MailAddress replyTo = new MailAddress("kubsu.event@mail.ru");
            myMail.ReplyToList.Add(replyTo);

            myMail.Subject = "Test";
            myMail.SubjectEncoding = System.Text.Encoding.UTF8;

            myMail.Body = "<b>Test Mail</b><br>using <b>HTML</b>.";
            myMail.BodyEncoding = System.Text.Encoding.UTF8;

            myMail.IsBodyHtml = true;

            smtpClient.Send(myMail);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFile = new OpenFileDialog() { Filter = "Excel Workbook|*.xlsx|Excel 97-2003 Workbook|*.xls" })
            {
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    using (var stream = File.Open(openFile.FileName, FileMode.Open, FileAccess.Read))
                    {
                        using (IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                            });

                            tableCollection = result.Tables;
                            foreach (DataTable table in tableCollection)
                            {
                                comboBox1.Items.Add(table.TableName);
                            }
                        }
                    }
                }
            }
        }

        private async void Import_Click(object sender, EventArgs e)
        {
            using (sqlConnection = new SqlConnection(conn))
            {
                await sqlConnection.OpenAsync();
                string sql = "DELETE FROM [ListMailStudents]";
                SqlCommand command1 = new SqlCommand(sql, sqlConnection);
                command1.ExecuteNonQuery();
            }
            table = tableCollection[comboBox1.SelectedItem.ToString()];

            try
            {
                using (SqlConnection connection = new SqlConnection(conn))
                {
                    await connection.OpenAsync();

                    foreach (DataRow r in table.Rows)
                    {
                        string sql1 = "INSERT INTO [ListMailStudents] (FIO, email, number_group) VALUES (N'" + r["ФИО"] + "', '" + r["Почта"] + "', N'" + r["Номер группы"] + "')";
                        SqlCommand cmd = connection.CreateCommand();
                        cmd.CommandText = sql1;
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            using (sqlConnection = new SqlConnection(conn))
            {
                await sqlConnection.OpenAsync();
                string sql = "DELETE FROM [ListMailStudents]";
                SqlCommand command1 = new SqlCommand(sql, sqlConnection);
                command1.ExecuteNonQuery();
            }
        }

        /*private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            table2 = new DataTable();
            table2.Clear();
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                a = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            }
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.AutoGenerateColumns = true;
            //dataGridView1.Rows.Clear();


            adapter2 = new SqlDataAdapter("SELECT number_group AS 'Номер группы', registered_amount AS 'Зарегистрировано' FROM [ListEventGroup] WHERE name_event = N'" + a + "'", sqlConnection);
            adapter2.Fill(table2);
            dataGridView1.DataSource = table2;
        }*/
    }
}
