using System.Data;
using System.Data.SQLite;

namespace StudentsDiary
{
    public static class Db
    {
        public static string ConnectionString = "Data Source=diary.db;Version=3;";

        public static void Initialize()
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();

                string sql = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY,
                    Login TEXT UNIQUE,
                    Pass TEXT,
                    FullName TEXT,
                    Course INTEGER,
                    GroupName TEXT,
                    Role TEXT
                );

                CREATE TABLE IF NOT EXISTS Grades (
                    Id INTEGER PRIMARY KEY,
                    StudentLogin TEXT,
                    Subject TEXT,
                    Value INTEGER,
                    Date TEXT
                );

                INSERT OR IGNORE INTO Users (Login, Pass, FullName, Role)
                VALUES ('admin', 'admin', 'Адміністратор', 'admin');
                ";

                new SQLiteCommand(sql, conn).ExecuteNonQuery();
            }
        }

        public static void Execute(string sql)
        {
            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                new SQLiteCommand(sql, conn).ExecuteNonQuery();
            }
        }

        public static DataTable GetTable(string sql)
        {
            DataTable dt = new DataTable();

            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                using (var adp = new SQLiteDataAdapter(sql, conn))
                    adp.Fill(dt);
            }

            return dt;
        }
    }
}