using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudentsDiary
{
    public class AdminForm : Form
    {
        string currentUser;
        DataGridView dgv = new DataGridView();

        TextBox tLog = new TextBox();
        TextBox tPass = new TextBox();
        TextBox tName = new TextBox();
        NumericUpDown nCourse = new NumericUpDown();
        TextBox tGroup = new TextBox();

        TextBox tSubj = new TextBox();
        NumericUpDown nVal = new NumericUpDown();

        public AdminForm(string user)
        {
            currentUser = user;
            this.Text = "Адміністративна панель — " + user;
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);

            BuildUI();
            LoadData();
        }

        void BuildUI()
        {
            GroupBox gbStudent = new GroupBox { Text = "Новий студент", Left = 20, Top = 20, Width = 550, Height = 100 };

            AddLabel(gbStudent, "Логін:", 20, 20);
            tLog.SetBounds(20, 40, 90, 25);

            AddLabel(gbStudent, "Пароль:", 120, 20);
            tPass.SetBounds(120, 40, 90, 25);

            AddLabel(gbStudent, "ПІБ:", 220, 20);
            tName.SetBounds(220, 40, 130, 25);

            AddLabel(gbStudent, "Курс:", 360, 20);
            nCourse.SetBounds(360, 40, 50, 25);
            nCourse.Minimum = 1; nCourse.Maximum = 6;

            AddLabel(gbStudent, "Група:", 420, 20);
            tGroup.SetBounds(420, 40, 110, 25);

            Button btnAddUser = new Button { Text = "Додати студента", Top = 70, Left = 20, Width = 510, BackColor = Color.LightBlue };
            btnAddUser.Click += AddUser;
            gbStudent.Controls.AddRange(new Control[] { tLog, tPass, tName, nCourse, tGroup, btnAddUser });

            GroupBox gbActions = new GroupBox { Text = "Оцінювання та дії", Left = 580, Top = 20, Width = 280, Height = 150 };

            AddLabel(gbActions, "Предмет:", 20, 25);
            tSubj.SetBounds(100, 22, 160, 25);

            AddLabel(gbActions, "Оцінка:", 20, 55);
            nVal.SetBounds(100, 52, 160, 25);
            nVal.Maximum = 100;

            Button btnAddGrade = new Button { Text = "Поставити оцінку", Top = 85, Left = 20, Width = 240, BackColor = Color.LightGreen };
            btnAddGrade.Click += AddGrade;

            Button btnDel = new Button { Text = "Видалити обраного студента", Top = 115, Left = 20, Width = 240, ForeColor = Color.White, BackColor = Color.IndianRed };
            btnDel.Click += Delete;

            gbActions.Controls.AddRange(new Control[] { tSubj, nVal, btnAddGrade, btnDel });

            dgv.SetBounds(20, 180, 840, 320);
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ReadOnly = true;

            Button btnExit = new Button { Text = "Вийти з аккаунту", Top = 510, Left = 710, Width = 150, Height = 35 };
            btnExit.Click += (s, e) => {
                new LoginForm().Show();
                this.Close();
            };

            this.Controls.AddRange(new Control[] { gbStudent, gbActions, dgv, btnExit });
        }

        void AddLabel(Control parent, string text, int x, int y)
        {
            parent.Controls.Add(new Label { Text = text, Left = x, Top = y, AutoSize = true, Font = new Font("Segoe UI", 8) });
        }

        void LoadData()
        {
            dgv.DataSource = Db.GetTable("SELECT Login, FullName, Course, GroupName FROM Users WHERE Role='student'");
        }

        void AddUser(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tLog.Text) || string.IsNullOrWhiteSpace(tName.Text)) return;

            Db.Execute($"INSERT INTO Users (Login, Pass, FullName, Course, GroupName, Role) VALUES " +
                       $"('{tLog.Text.Replace("'", "''")}', '{tPass.Text.Replace("'", "''")}', '{tName.Text.Replace("'", "''")}', {nCourse.Value}, '{tGroup.Text.Replace("'", "''")}', 'student')");

            LoadData();
            ClearStudentFields();
        }

        void AddGrade(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null || string.IsNullOrWhiteSpace(tSubj.Text))
            {
                MessageBox.Show("Оберіть студента в таблиці та введіть предмет");
                return;
            }

            string login = dgv.CurrentRow.Cells["Login"].Value.ToString();
            Db.Execute($"INSERT INTO Grades (StudentLogin, Subject, Value, Date) VALUES " +
                       $"('{login}', '{tSubj.Text.Replace("'", "''")}', {nVal.Value}, '{DateTime.Now:yyyy-MM-dd}')");

            MessageBox.Show("Оцінку додано!");
        }

        void Delete(object sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;

            var res = MessageBox.Show("Видалити цього студента?", "Підтвердження", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                string login = dgv.CurrentRow.Cells["Login"].Value.ToString();
                Db.Execute($"DELETE FROM Users WHERE Login='{login}'");
                LoadData();
            }
        }

        void ClearStudentFields()
        {
            tLog.Clear(); tPass.Clear(); tName.Clear(); tGroup.Clear();
        }
    }
}