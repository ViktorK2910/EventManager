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

namespace EventManagerKubSU
{
    public partial class Form1 : Form
    {
        //SqlConnection sqlConnection;
        DataTable table = null;
        private Button currenButton;
        private Random random;
        private int tempIndex;
        private Form activeForm;
        private string text;
        private int len = 0;
        SqlConnection sqlConnection;
        string conn = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\EventManager\EventManagerKubSU\EventDB.mdf;Integrated Security=True;Connect Timeout=30";
        //string conn = @"Data Source=.\SQLEXPRESS; AttachDbFilename=|DataDirectory|\EventManagerKubSU\EventDB.mdf; Integrated Security=True;User Instance=True";

        public Form1()
        {
            InitializeComponent();
            this.CenterToScreen();
            random = new Random();
            text = label2.Text;
            label2.Text = "";
            timer1.Start();
        }

        private Color SelectThemeColor()
        {
            int index = random.Next(ThemeColor.ColorList.Count);
            while (tempIndex == index)
            {
                index = random.Next(ThemeColor.ColorList.Count);
            }
            tempIndex = index;
            string color = ThemeColor.ColorList[index];
            return ColorTranslator.FromHtml(color);
        }

        private void ActivateButton(object btnSender)
        {
            if (btnSender != null)
            {
                if (currenButton != (Button)btnSender)
                {
                    DisableButton();
                    Color color = SelectThemeColor();
                    currenButton = (Button)btnSender;
                    currenButton.BackColor = color;
                    currenButton.ForeColor = Color.White;
                    currenButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 12.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                    TitleBar.BackColor = color;
                    Logo.BackColor = ThemeColor.ChangeColorBrightness(color, -0.3);
                    ThemeColor.PrimaryColor = color;
                    ThemeColor.SecondaryColor = ThemeColor.ChangeColorBrightness(color, -0.3);
                }
            }
        }

        private void DisableButton()
        {
            foreach (Control previousBtn in Menu.Controls)
            {
                if (previousBtn.GetType() == typeof(Button))
                {
                    previousBtn.BackColor = Color.FromArgb(51, 51, 76);
                    previousBtn.ForeColor = Color.Gainsboro;
                    previousBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                }
            }
        }

        public void OpenChildForm(Form childForm, object btnSender, string a = null)
        {
            if (activeForm != null)
            {
                activeForm.Close();
            }
            ActivateButton(btnSender);
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            this.DesktopPanel.Controls.Add(childForm);
            this.DesktopPanel.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
            LabelTitle.Text = childForm.Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.FormAddPartic(), sender);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.FormListEvent(), sender);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.FormEndedEvent(), sender);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.FormNotificationSetting(), sender);
        }

        private void AddEvent_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.FormAddEvent(), sender);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (len < text.Length)
            {
                label2.Text = label2.Text + text.ElementAt(len);
                len++;
            }
            else timer1.Stop();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(conn);
            await sqlConnection.OpenAsync();
            table = new DataTable();

            string sql1 = "UPDATE Event SET completed = 1 WHERE date = @date OR date < @date";
            SqlCommand command1 = new SqlCommand(sql1, sqlConnection);
            command1.Parameters.AddWithValue("date", DateTime.Now);
            command1.ExecuteNonQuery();

            await Task.Delay(4800);
            button3.BackColor = SelectThemeColor();
            button3.Focus();
            //MessageBox.Show("Возможно у вас есть завершённые мероприятия, перейдите во вкладку Завершённые для просмотра!");
            button3.BackColor = Color.Transparent;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Forms.FormGroupList(), sender);
        }

    }
}
