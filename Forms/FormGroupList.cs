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

namespace EventManagerKubSU.Forms
{
    public partial class FormGroupList : Form
    {
        SqlConnection sqlConnection;
        SqlDataAdapter adapter = null;
        SqlDataAdapter adapter2 = null;
        DataTable table = null;
        DataTable table2 = new DataTable("Group");
        string conn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\EventManager\EventManagerKubSU\EventDB.mdf;Integrated Security=True;Connect Timeout=30";
        //string conn = @"Data Source=.\SQLEXPRESS; AttachDbFilename=|DataDirectory|\EventManagerKubSU\EventDB.mdf; Integrated Security=True;User Instance=True";
        DataTableCollection tableCollection;
        DataSet result = new DataSet();
        DataSet update = new DataSet();
        
        public FormGroupList()
        {
            InitializeComponent();
        }

        private async void FormGroupList_Load(object sender, EventArgs e)
        { 
            sqlConnection = new SqlConnection(conn);
            await sqlConnection.OpenAsync();
            table = new DataTable();
            try
            {
                table.Clear();

                adapter = new SqlDataAdapter("SELECT number_group AS 'Номер группы', course AS 'Курс', amount_of_students AS 'Кол-во студентов' FROM [Group]", sqlConnection);
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
        }

        private void Import_Click(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            table = tableCollection[comboBox1.SelectedItem.ToString()];
            dataGridView1.DataSource = table;
        }

        private async void Import_Click_1(object sender, EventArgs e)
        {
            using (sqlConnection = new SqlConnection(conn))
            {
                await sqlConnection.OpenAsync();
                string sql = "DELETE FROM [Group]";
                SqlCommand command1 = new SqlCommand(sql, sqlConnection);
                command1.ExecuteNonQuery();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(conn))
                {
                    await connection.OpenAsync();

                    foreach (DataRow r in table.Rows)
                    {
                        string sql1 = "INSERT INTO [Group] (number_group, course, amount_of_students) VALUES (N'" + r["Номер группы"] + "', '" + r["Курс"] + "', '" + r["Кол-во студентов"] + "')";
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

            /*foreach (DataColumn column in table.Columns)
                Console.Write($"{column.ColumnName}\t");
            Console.WriteLine();
           
            foreach (DataRow row in table.Rows)
            {
                var cells = row.ItemArray;
                foreach (object cell in cells)
                    Console.Write($"{cell}\t");
                Console.WriteLine();
            }*/ 
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            using (sqlConnection = new SqlConnection(conn))
            {
                await sqlConnection.OpenAsync();
                string sql = "DELETE FROM [Group]";
                SqlCommand command1 = new SqlCommand(sql, sqlConnection);
                command1.ExecuteNonQuery();
                dataGridView1.DataSource = null;
            }
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
                            result = reader.AsDataSet(new ExcelDataSetConfiguration()
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
    }
}
