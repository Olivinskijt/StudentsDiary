using System;
using System.Drawing;
using System.Windows.Forms;

namespace StudentsDiary
{
    public class LoginForm : Form
    {
        TextBox tLogin = new TextBox();
        TextBox tPass = new TextBox();

        public LoginForm()
        {
            this.Text = "Авторизація";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            Label l1 = new Label { Text = "Логін", Top = 40, Left = 40, AutoSize = true };
            Label l2 = new Label { Text = "Пароль", Top = 80, Left = 40, AutoSize = true };

            tLogin.Location = new Point(120, 40);
            tLogin.Width = 180;

            tPass.Location = new Point(120, 80);
            tPass.Width = 180;
            tPass.PasswordChar = '*';

            Button b = new Button
            {
                Text = "Увійти",
                Top = 130,
                Left = 120,
                Width = 180,
                Height = 30
            };

            b.Click += Login;

            this.Controls.Add(l1);
            this.Controls.Add(l2);
            this.Controls.Add(tLogin);
            this.Controls.Add(tPass);
            this.Controls.Add(b);
        }

        private void Login(object sender, EventArgs e)
        {
            string loginText = tLogin.Text.Replace("'", "''");
            string passText = tPass.Text.Replace("'", "''");

            var dt = Db.GetTable($"SELECT Role FROM Users WHERE Login='{loginText}' AND Pass='{passText}'");

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("Невірний логін або пароль", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string role = dt.Rows[0]["Role"].ToString();

            if (role == "admin")
                new AdminForm(tLogin.Text).Show();
            else
                new StudentForm(tLogin.Text).Show();

            this.Hide();
        }
    }
}