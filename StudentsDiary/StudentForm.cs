using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace StudentsDiary
{
    public class StudentForm : Form
    {
        string user;
        DataGridView dgv = new DataGridView();
        Panel infoPanel = new Panel();
        Panel bottomPanel = new Panel();
        Label avgLabel = new Label();
        TextBox filterBox = new TextBox();
        Button filterBtn = new Button();

        public StudentForm(string login)
        {
            user = login;
            this.Text = "Кабінет студента";
            this.Size = new Size(700, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            BuildUI();
            LoadStudentInfo();
            LoadGrades();
        }

        void BuildUI()
        {
            infoPanel.Dock = DockStyle.Top;
            infoPanel.Height = 120;
            infoPanel.BackColor = Color.FromArgb(240, 240, 240);

            dgv.Dock = DockStyle.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 80;

            avgLabel.Location = new Point(20, 25);
            avgLabel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            avgLabel.AutoSize = true;

            filterBox.Location = new Point(20, 70);
            filterBox.Width = 200;
            filterBox.PlaceholderText = "Предмет";

            filterBtn.Text = "Фільтр";
            filterBtn.Location = new Point(230, 68);
            filterBtn.Click += FilterBtn_Click;

            Button exit = new Button
            {
                Text = "Вийти",
                Size = new Size(100, 35),
                Location = new Point(560, 20)
            };

            exit.Click += (s, e) => { new LoginForm().Show(); this.Close(); };

            bottomPanel.Controls.Add(avgLabel);
            bottomPanel.Controls.Add(exit);

            infoPanel.Controls.Add(filterBox);
            infoPanel.Controls.Add(filterBtn);

            this.Controls.Add(dgv);
            this.Controls.Add(infoPanel);
            this.Controls.Add(bottomPanel);
        }

        void LoadStudentInfo()
        {
            DataTable dt = Db.GetTable($"SELECT FullName, Course, GroupName FROM Users WHERE Login='{user}'");
            if (dt.Rows.Count > 0)
            {
                DataRow r = dt.Rows[0];
                Label lName = new Label
                {
                    Text = r["FullName"].ToString(),
                    Font = new Font("Segoe UI", 14, FontStyle.Bold),
                    Location = new Point(20, 10),
                    AutoSize = true
                };

                Label lDet = new Label
                {
                    Text = $"Курс: {r["Course"]} | Група: {r["GroupName"]}",
                    Location = new Point(22, 40),
                    AutoSize = true
                };

                infoPanel.Controls.Add(lName);
                infoPanel.Controls.Add(lDet);
            }
        }

        void LoadGrades(string subjectFilter = "")
        {
            string query = $@"
                SELECT Subject as 'Предмет', Value as 'Оцінка', Date as 'Дата'
                FROM Grades
                WHERE StudentLogin='{user}'";

            if (!string.IsNullOrEmpty(subjectFilter))
                query += $" AND Subject LIKE '%{subjectFilter}%'";

            DataTable dt = Db.GetTable(query);
            dgv.DataSource = dt;

            double sum = 0;
            foreach (DataRow r in dt.Rows)
                sum += Convert.ToDouble(r["Оцінка"]);

            double avg = dt.Rows.Count > 0 ? sum / dt.Rows.Count : 0;

            avgLabel.Text = "Середній бал: " + Math.Round(avg, 2);
            avgLabel.ForeColor = avg >= 75 ? Color.Green : Color.Red;
        }

        void FilterBtn_Click(object sender, EventArgs e)
        {
            LoadGrades(filterBox.Text.Trim());
        }
    }
}