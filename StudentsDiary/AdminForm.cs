using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudentsDiary
{
    public class AdminForm : Form
    {
        string currentUser;

        DataGridView dgvStudents = new DataGridView();
        DataGridView dgvGrades = new DataGridView();

        TextBox tLog = new TextBox();
        TextBox tPass = new TextBox();
        TextBox tName = new TextBox();
        NumericUpDown nCourse = new NumericUpDown();
        TextBox tGroup = new TextBox();

        TextBox filterBox = new TextBox();
        TextBox gradeFilterBox = new TextBox();

        Button btnAddGrade = new Button();

        public AdminForm(string user)
        {
            currentUser = user;
            this.Text = "Адміністративна панель — " + user;
            this.Size = new Size(950, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);

            BuildUI();
            LoadStudents();
        }

        void BuildUI()
        {
            GroupBox gbStudent = new GroupBox
            {
                Text = "Новий студент",
                Left = 20,
                Top = 20,
                Width = 550,
                Height = 120
            };

            AddLabel(gbStudent, "Логін:", 20, 20);
            tLog.SetBounds(20, 40, 90, 25);

            AddLabel(gbStudent, "Пароль:", 120, 20);
            tPass.SetBounds(120, 40, 90, 25);

            AddLabel(gbStudent, "ПІБ:", 220, 20);
            tName.SetBounds(220, 40, 130, 25);

            AddLabel(gbStudent, "Курс:", 360, 20);
            nCourse.SetBounds(360, 40, 50, 25);
            nCourse.Minimum = 1;
            nCourse.Maximum = 6;

            AddLabel(gbStudent, "Група:", 420, 20);
            tGroup.SetBounds(420, 40, 110, 25);

            Button btnAddUser = new Button
            {
                Text = "Додати студента",
                Top = 75,
                Left = 20,
                Width = 510,
                BackColor = Color.LightBlue
            };
            btnAddUser.Click += AddUser;

            gbStudent.Controls.AddRange(new Control[]
            {
                tLog, tPass, tName, nCourse, tGroup, btnAddUser
            });

            filterBox.SetBounds(20, 150, 300, 25);
            filterBox.PlaceholderText = "Фільтр студентів";
            filterBox.TextChanged += (s, e) => LoadStudents(filterBox.Text.Trim());

            dgvStudents.SetBounds(20, 180, 900, 180);
            dgvStudents.BackgroundColor = Color.White;
            dgvStudents.ReadOnly = true;
            dgvStudents.AllowUserToAddRows = false;
            dgvStudents.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvStudents.CellClick += (s, e) =>
            {
                if (dgvStudents.CurrentRow != null)
                {
                    string login = dgvStudents.CurrentRow.Cells["Login"].Value.ToString();
                    LoadGrades(login);

                    btnAddGrade.Enabled = true;
                }
            };

            gradeFilterBox.SetBounds(20, 370, 200, 25);
            gradeFilterBox.PlaceholderText = "Фільтр предмету";

            gradeFilterBox.TextChanged += (s, e) =>
            {
                if (dgvStudents.CurrentRow != null)
                {
                    LoadGrades(
                        dgvStudents.CurrentRow.Cells["Login"].Value.ToString(),
                        gradeFilterBox.Text.Trim()
                    );
                }
            };

            dgvGrades.SetBounds(20, 400, 900, 150);
            dgvGrades.BackgroundColor = Color.White;
            dgvGrades.ReadOnly = true;
            dgvGrades.AllowUserToAddRows = false;
            dgvGrades.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvGrades.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            Button btnDeleteGrade = new Button
            {
                Text = "Видалити оцінку",
                Top = 560,
                Left = 20,
                Width = 200,
                BackColor = Color.IndianRed,
                ForeColor = Color.White
            };
            btnDeleteGrade.Click += DeleteGrade;

            Button btnDeleteStudent = new Button
            {
                Text = "Видалити студента",
                Top = 560,
                Left = 230,
                Width = 200,
                BackColor = Color.DarkRed,
                ForeColor = Color.White
            };
            btnDeleteStudent.Click += DeleteStudent;

            btnAddGrade = new Button
            {
                Text = "Додати оцінку",
                Top = 560,
                Left = 440,
                Width = 200,
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                Enabled = false
            };
            btnAddGrade.Click += AddGrade;

            Button btnExit = new Button
            {
                Text = "Вихід",
                Top = 560,
                Left = 820,
                Width = 100
            };

            btnExit.Click += (s, e) =>
            {
                new LoginForm().Show();
                this.Close();
            };

            this.Controls.AddRange(new Control[]
            {
                gbStudent,
                filterBox,
                dgvStudents,
                gradeFilterBox,
                dgvGrades,
                btnDeleteGrade,
                btnDeleteStudent,
                btnAddGrade,
                btnExit
            });
        }

        void AddLabel(Control parent, string text, int x, int y)
        {
            parent.Controls.Add(new Label
            {
                Text = text,
                Left = x,
                Top = y,
                AutoSize = true
            });
        }

        void LoadStudents(string filter = "")
        {
            string q = "SELECT Login, FullName, Course, GroupName FROM Users WHERE Role='student'";

            if (!string.IsNullOrWhiteSpace(filter))
            {
                q += $@" AND (
                    Login LIKE '%{filter}%'
                    OR FullName LIKE '%{filter}%'
                    OR GroupName LIKE '%{filter}%'
                    OR CAST(Course AS NVARCHAR) LIKE '%{filter}%'
                )";
            }

            dgvStudents.DataSource = Db.GetTable(q);
        }

        void LoadGrades(string login, string subjectFilter = "")
        {
            string q = $"SELECT Subject, Value, Date FROM Grades WHERE StudentLogin='{login}'";

            if (!string.IsNullOrWhiteSpace(subjectFilter))
                q += $" AND Subject LIKE '%{subjectFilter}%'";

            dgvGrades.DataSource = Db.GetTable(q);
        }

        void AddUser(object sender, EventArgs e)
        {
            Db.Execute($@"
                INSERT INTO Users (Login, Pass, FullName, Course, GroupName, Role)
                VALUES (
                    '{tLog.Text.Replace("'", "''")}',
                    '{tPass.Text.Replace("'", "''")}',
                    '{tName.Text.Replace("'", "''")}',
                    {nCourse.Value},
                    '{tGroup.Text.Replace("'", "''")}',
                    'student'
                )");

            LoadStudents();

            tLog.Clear();
            tPass.Clear();
            tName.Clear();
            tGroup.Clear();
            nCourse.Value = 1;
        }

        void AddGrade(object sender, EventArgs e)
        {
            if (dgvStudents.CurrentRow == null) return;

            string login = dgvStudents.CurrentRow.Cells["Login"].Value.ToString();

            using (var form = new AddGradeForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    Db.Execute($@"
                        INSERT INTO Grades (StudentLogin, Subject, Value, Date)
                        VALUES (
                            '{login}',
                            '{form.Subject.Replace("'", "''")}',
                            {form.Value},
                            GETDATE()
                        )");

                    LoadGrades(login, gradeFilterBox.Text.Trim());
                }
            }
        }

        void DeleteGrade(object sender, EventArgs e)
        {
            if (dgvGrades.CurrentRow == null || dgvStudents.CurrentRow == null) return;

            string login = dgvStudents.CurrentRow.Cells["Login"].Value.ToString();
            string subject = dgvGrades.CurrentRow.Cells["Subject"].Value.ToString();
            string date = dgvGrades.CurrentRow.Cells["Date"].Value.ToString();

            Db.Execute($"DELETE FROM Grades WHERE StudentLogin='{login}' AND Subject='{subject}' AND Date='{date}'");

            LoadGrades(login, gradeFilterBox.Text.Trim());
        }

        void DeleteStudent(object sender, EventArgs e)
        {
            if (dgvStudents.CurrentRow == null) return;

            string login = dgvStudents.CurrentRow.Cells["Login"].Value.ToString();

            var res = MessageBox.Show("Видалити студента разом з оцінками?", "Підтвердження", MessageBoxButtons.YesNo);

            if (res == DialogResult.Yes)
            {
                Db.Execute($"DELETE FROM Grades WHERE StudentLogin='{login}'");
                Db.Execute($"DELETE FROM Users WHERE Login='{login}'");

                LoadStudents();
                dgvGrades.DataSource = null;
                btnAddGrade.Enabled = false;
            }
        }
    }
}