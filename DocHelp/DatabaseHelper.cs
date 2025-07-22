using System;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public static class DatabaseHelper
{
    private static readonly string dbFile = "doctors_app.sqlite";
    private static readonly string connectionString = $"Data Source={dbFile};Version=3;";

    public static void InitializeDatabase()
    {
        if (!File.Exists(dbFile))
        {
            SQLiteConnection.CreateFile(dbFile);
        }

        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string checkTableQuery = "SELECT name FROM sqlite_master WHERE type='table' AND name='Doctors';";
            var command = new SQLiteCommand(checkTableQuery, connection);
            var result = command.ExecuteScalar();

            if (result == null)
            {
                string createTableQuery = @"
                CREATE TABLE Doctors (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    FullName TEXT,
                    Specialty TEXT,
                    PhoneNumber TEXT,
                    ProfilePicture BLOB
                );";
                command.CommandText = createTableQuery;
                command.ExecuteNonQuery();
            }
        }
    }

    private static string HashPassword(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }
    }

    public static bool SignUp(string username, string password)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Doctors (Username, PasswordHash) VALUES (@Username, @PasswordHash)";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username.ToLower());
                command.Parameters.AddWithValue("@PasswordHash", HashPassword(password));
                try
                {
                    command.ExecuteNonQuery();
                    return true;
                }
                catch (SQLiteException)
                {
                    return false;
                }
            }
        }
    }

    public static int? Login(string username, string password)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT Id FROM Doctors WHERE Username = @Username AND PasswordHash = @PasswordHash";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Username", username.ToLower());
                command.Parameters.AddWithValue("@PasswordHash", HashPassword(password));
                var result = command.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                return null;
            }
        }
    }

    public enum PasswordStrength
    {
        Blank = 0,
        VeryWeak = 1,
        Weak = 2,
        Medium = 3,
        Strong = 4,
        VeryStrong = 5
    }

    public static PasswordStrength CheckPasswordStrength(string password)
    {
        if (string.IsNullOrEmpty(password)) return PasswordStrength.Blank;
        int score = 0;
        if (password.Length >= 8) score++;
        if (Regex.IsMatch(password, @"[a-z]")) score++;
        if (Regex.IsMatch(password, @"[A-Z]")) score++;
        if (Regex.IsMatch(password, @"[0-9]")) score++;
        if (Regex.IsMatch(password, @"[\W_]")) score++;

        return (PasswordStrength)score;
    }
}