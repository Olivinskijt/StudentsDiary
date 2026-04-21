using System;
using System.Windows.Forms;

namespace StudentsDiary
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Db.Initialize();

            Application.Run(new LoginForm());
        }
    }
}