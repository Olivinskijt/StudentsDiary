using System;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Drawing;

namespace StudentsDiary
{
    public partial class Form1 : Form
    {
        private string dbPath = "Data Source=diary.db;Version=3;";
        private string currentUser = "";
        private string role = "";

        public Form1()
        {
            this.Size = new Size(650, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeDB();
            ShowLogin();
        }

        private void InitializeDB()
        {
            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY, 
                        Login TEXT UNIQUE, 
                        Pass TEXT, 
                        FullName TEXT, 
                        Role TEXT);
                    CREATE TABLE IF NOT EXISTS Grades (
                        Id INTEGER PRIMARY KEY, 
                        StudentLogin TEXT, 
                        Subject TEXT, 
                        Value INTEGER, 
                        Date TEXT);
                    INSERT OR IGNORE INTO Users (Login, Pass, FullName, Role) VALUES ('admin', 'admin', 'Адміністратор', 'admin');";
                new SQLiteCommand(sql, conn).ExecuteNonQuery();
            }
        }

        private void ShowLogin()
        {
            this.Controls.Clear();
            this.Text = "Авторизація | StudentsDiary";

            Panel p = new Panel { Dock = DockStyle.Fill };
            Label l1 = new Label { Text = "Логін:", Top = 100, Left = 150, AutoSize = true };
            TextBox t1 = new TextBox { Top = 100, Left = 220, Width = 150 };
            Label l2 = new Label { Text = "Пароль:", Top = 135, Left = 150, AutoSize = true };
            TextBox t2 = new TextBox { Top = 135, Left = 220, Width = 150, PasswordChar = '*' };
            Button b1 = new Button { Text = "Увійти в систему", Top = 180, Left = 220, Width = 150, Height = 30 };

            b1.Click += (s, e) => {
                using (var conn = new SQLiteConnection(dbPath))
                {
                    conn.Open();
                    var cmd = new SQLiteCommand("SELECT Role FROM Users WHERE Login=@l AND Pass=@p", conn);
                    cmd.Parameters.AddWithValue("@l", t1.Text);
                    cmd.Parameters.AddWithValue("@p", t2.Text);
                    var res = cmd.ExecuteScalar();
                    if (res != null)
                    {
                        currentUser = t1.Text;
                        role = res.ToString();
                        if (role == "admin") ShowAdmin(); else ShowStudent();
                    }
                    else MessageBox.Show("Невірний логін або пароль!");
                }
            };
            this.Controls.AddRange(new Control[] { l1, t1, l2, t2, b1 });
        }

        private void ShowAdmin()
        {
            this.Controls.Clear();
            this.Text = "Панель адміністратора | StudentsDiary";

            DataGridView dgv = new DataGridView
            {
                Top = 210,
                Left = 20,
                Width = 600,
                Height = 200,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            GroupBox gbUser = new GroupBox { Text = "Реєстрація студента", Top = 10, Left = 20, Width = 280, Height = 140 };
            TextBox tLog = new TextBox { Top = 25, Left = 10, Width = 120, PlaceholderText = "Логін" };
            TextBox tPas = new TextBox { Top = 25, Left = 140, Width = 120, PlaceholderText = "Пароль" };
            TextBox tName = new TextBox { Top = 60, Left = 10, Width = 250, PlaceholderText = "ПІБ Студента" };
            Button bAddU = new Button { Text = "Створити аккаунт", Top = 95, Left = 10, Width = 250 };

            GroupBox gbGrade = new GroupBox { Text = "Успішність", Top = 10, Left = 320, Width = 300, Height = 140 };
            TextBox tSubj = new TextBox { Top = 25, Left = 10, Width = 150, PlaceholderText = "Предмет" };
            NumericUpDown nVal = new NumericUpDown { Top = 25, Left = 170, Width = 60, Minimum = 1, Maximum = 100 };
            Button bAddG = new Button { Text = "Записати оцінку", Top = 60, Left = 10, Width = 220 };

            Button bLoad = new Button { Text = "Список студентів", Top = 165, Left = 20, Width = 150 };
            Button bDel = new Button { Text = "Видалити", Top = 165, Left = 180, Width = 100, BackColor = Color.MistyRose };
            Button bExit = new Button { Text = "Вихід", Top = 165, Left = 520, Width = 100 };

            bAddU.Click += (s, e) => {
                if (string.IsNullOrWhiteSpace(tLog.Text) || string.IsNullOrWhiteSpace(tName.Text)) return;
                using (var conn = new SQLiteConnection(dbPath))
                {
                    conn.Open();
                    var cmd = new SQLiteCommand("INSERT OR IGNORE INTO Users (Login, Pass, FullName, Role) VALUES (@l, @p, @f, 'student')", conn);
                    cmd.Parameters.AddWithValue("@l", tLog.Text);
                    cmd.Parameters.AddWithValue("@p", tPas.Text);
                    cmd.Parameters.AddWithValue("@f", tName.Text);
                    cmd.ExecuteNonQuery();
                }
                bLoad.PerformClick();
                tLog.Clear(); tPas.Clear(); tName.Clear();
            };

            bAddG.Click += (s, e) => {
                if (dgv.CurrentRow == null) { MessageBox.Show("Виберіть студента!"); return; }
                string selectedLogin = dgv.CurrentRow.Cells["Логін"].Value.ToString();
                RunSql($"INSERT INTO Grades (StudentLogin, Subject, Value, Date) VALUES ('{selectedLogin}', '{tSubj.Text}', {nVal.Value}, '{DateTime.Now:dd.MM.yyyy}')");
            };

            bLoad.Click += (s, e) => {
                dgv.DataSource = GetTable("SELECT Login as 'Логін', FullName as 'ПІБ Студента', Pass as 'Пароль' FROM Users WHERE Role='student'");
            };

            bDel.Click += (s, e) => {
                if (dgv.CurrentRow == null) return;
                string login = dgv.CurrentRow.Cells["Логін"].Value.ToString();
                RunSql($"DELETE FROM Users WHERE Login='{login}'");
                RunSql($"DELETE FROM Grades WHERE StudentLogin='{login}'");
                bLoad.PerformClick();
            };

            bExit.Click += (s, e) => ShowLogin();

            gbUser.Controls.AddRange(new Control[] { tLog, tPas, tName, bAddU });
            gbGrade.Controls.AddRange(new Control[] { tSubj, nVal, bAddG });
            this.Controls.AddRange(new Control[] { gbUser, gbGrade, bLoad, bDel, bExit, dgv });
        }

        private void ShowStudent()
        {
            this.Controls.Clear();

            string fullName = "";
            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                var cmd = new SQLiteCommand("SELECT FullName FROM Users WHERE Login=@l", conn);
                cmd.Parameters.AddWithValue("@l", currentUser);
                fullName = cmd.ExecuteScalar()?.ToString() ?? currentUser;
            }

            this.Text = $"Кабінет студента: {currentUser}";

            Label lHeader = new Label { Text = $"Академічна інформація студента: {fullName}", Top = 15, Left = 20, AutoSize = true, Font = new Font(this.Font, FontStyle.Bold) };
            DataGridView dgv = new DataGridView { Top = 50, Left = 20, Width = 545, Height = 250, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true };

            DataTable dt = GetTable($"SELECT Subject as 'Предмет', Value as 'Оцінка', Date as 'Дата' FROM Grades WHERE StudentLogin='{currentUser}'");
            dgv.DataSource = dt;

            double avg = 0;
            if (dt.Rows.Count > 0)
            {
                double sum = 0;
                foreach (DataRow row in dt.Rows) sum += Convert.ToDouble(row["Оцінка"]);
                avg = Math.Round(sum / dt.Rows.Count, 2);
            }

            Label lAvg = new Label { Text = $"Ваш середній бал: {avg}", Top = 310, Left = 20, AutoSize = true, ForeColor = Color.Blue };
            Button bExit = new Button { Text = "Вихід з аккаунту", Top = 340, Left = 20, Width = 150 };

            bExit.Click += (s, e) => ShowLogin();
            this.Controls.AddRange(new Control[] { lHeader, dgv, lAvg, bExit });
        }

        private void RunSql(string sql)
        {
            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(sql, conn)) cmd.ExecuteNonQuery();
            }
        }

        private DataTable GetTable(string sql)
        {
            DataTable dt = new DataTable();
            using (var conn = new SQLiteConnection(dbPath))
            {
                conn.Open();
                using (var adp = new SQLiteDataAdapter(sql, conn)) adp.Fill(dt);
            }
            return dt;
        }
    }
}