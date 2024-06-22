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
using Excel = Microsoft.Office.Interop.Excel;


namespace EventManagerKubSU.Forms
{
    public partial class FormListEvent : Form
    {
        public string a;
        SqlConnection sqlConnection;
        SqlDataAdapter adapter = null;
        SqlDataAdapter adapter2 = null;
        DataTable table = null;
        DataTable table2 = null;

        public FormListEvent()
        {
            InitializeComponent();

        }

        private void FormListEvent_Load(object sender, EventArgs e)
        {
            string conn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\EventManager\EventManagerKubSU\EventDB.mdf;Integrated Security=True;Connect Timeout=30";
            //string conn = @"Data Source=.\SQLEXPRESS; AttachDbFilename=|DataDirectory|\EventManagerKubSU\EventDB.mdf; Integrated Security=True;User Instance=True";
            sqlConnection = new SqlConnection(conn);
            sqlConnection.Open();
            table = new DataTable();
            string sql2 = "UPDATE Event SET completed = 1 WHERE date = @date OR date < @date";
            SqlCommand command2 = new SqlCommand(sql2, sqlConnection);
            command2.Parameters.AddWithValue("date", DateTime.Now);
            command2.ExecuteNonQuery();
            try
            {
                table.Clear();

                adapter = new SqlDataAdapter("SELECT name_event AS 'Название', date AS 'Дата', venue AS 'Место', responsible AS 'Ответственный', required_amount AS 'Необходимое кол-во', comment AS 'Коммент' FROM [Event] WHERE completed = 0", sqlConnection);
                adapter.Fill(table);
                dataGridView1.DataSource = table;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), ex.Source.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
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
            //Form1 temp = new Form1();
            //temp.OpenChildForm(new Forms.FormListEvent(), sender,a);

        }

        private void Export_Click(object sender, EventArgs e)
        {
            Excel.Application exApp = new Excel.Application();

            exApp.Workbooks.Add();
            Excel.Worksheet worksheet = (Excel.Worksheet)exApp.ActiveSheet;
            int i, j;
            for (i = 0; i < dataGridView1.RowCount - 2; i++)
            {
                for (j = 0; j< dataGridView1.ColumnCount; j++)
                {
                    worksheet.Cells[i + 1, j + 1] = dataGridView1[i, j].Value.ToString();
                }
            }

            exApp.Visible = true;
        }
    }
}
