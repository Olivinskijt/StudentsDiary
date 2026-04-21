using System;
using System.Drawing;
using System.Windows.Forms;

public class AddGradeForm : Form
{
    public string Subject => tSubject.Text;
    public int Value => (int)nValue.Value;

    TextBox tSubject = new TextBox();
    NumericUpDown nValue = new NumericUpDown();

    public AddGradeForm()
    {
        this.Text = "Додати оцінку";
        this.Size = new Size(300, 200);
        this.StartPosition = FormStartPosition.CenterParent;

        Label l1 = new Label { Text = "Предмет", Left = 20, Top = 20 };
        tSubject.SetBounds(20, 40, 240, 25);

        Label l2 = new Label { Text = "Оцінка", Left = 20, Top = 70 };
        nValue.SetBounds(20, 90, 240, 25);
        nValue.Minimum = 1;
        nValue.Maximum = 12;

        Button btnOk = new Button
        {
            Text = "Зберегти",
            Left = 20,
            Top = 130,
            Width = 110
        };

        btnOk.Click += (s, e) =>
        {
            if (string.IsNullOrWhiteSpace(tSubject.Text))
            {
                MessageBox.Show("Введіть назву предмету!", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        };

        Button btnCancel = new Button
        {
            Text = "Відміна",
            Left = 150,
            Top = 130,
            Width = 110,
            DialogResult = DialogResult.Cancel
        };

        this.Controls.AddRange(new Control[]
        {
            l1, tSubject,
            l2, nValue,
            btnOk, btnCancel
        });

        this.AcceptButton = btnOk;
        this.CancelButton = btnCancel;
    }
}