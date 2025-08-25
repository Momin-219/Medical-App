using System;
using System.Collections.Generic; // Added for List<T> support
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
            string createTablesQuery = @"
            CREATE TABLE IF NOT EXISTS Doctors (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL,
                FullName TEXT,
                Specialty TEXT,
                PhoneNumber TEXT,
                ProfilePicture BLOB
            );

            CREATE TABLE IF NOT EXISTS Patients (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                DoctorId INTEGER NOT NULL,
                PatientUniqueID TEXT NOT NULL UNIQUE,
                PatientName TEXT NOT NULL,
                FOREIGN KEY (DoctorId) REFERENCES Doctors (Id)
            );

            CREATE TABLE IF NOT EXISTS Diagnoses (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                PatientId INTEGER NOT NULL,
                DiagnosisDate TEXT NOT NULL,
                PdfPath TEXT NOT NULL,
                FOREIGN KEY (PatientId) REFERENCES Patients (Id)
            );";
            var command = new SQLiteCommand(createTablesQuery, connection);
            command.ExecuteNonQuery();
        }
    }

    // --- Patient and Diagnosis Methods ---

    public static bool PatientIdExists(string patientId)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT 1 FROM Patients WHERE PatientUniqueID = @PatientUniqueID";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@PatientUniqueID", patientId);
                return command.ExecuteScalar() != null;
            }
        }
    }

    public static int AddOrGetPatient(int doctorId, string patientUniqueId, string patientName)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string selectQuery = "SELECT Id FROM Patients WHERE PatientUniqueID = @PatientUniqueID";
            using (var command = new SQLiteCommand(selectQuery, connection))
            {
                command.Parameters.AddWithValue("@PatientUniqueID", patientUniqueId);
                var existingId = command.ExecuteScalar();
                if (existingId != null)
                {
                    throw new InvalidOperationException("A patient with this ID already exists.");
                }
            }

            string insertQuery = "INSERT INTO Patients (DoctorId, PatientUniqueID, PatientName) VALUES (@DoctorId, @PatientUniqueID, @PatientName); SELECT last_insert_rowid();";
            using (var command = new SQLiteCommand(insertQuery, connection))
            {
                command.Parameters.AddWithValue("@DoctorId", doctorId);
                command.Parameters.AddWithValue("@PatientUniqueID", patientUniqueId);
                command.Parameters.AddWithValue("@PatientName", patientName);
                return Convert.ToInt32(command.ExecuteScalar());
            }
        }
    }

    public static void AddDiagnosis(int patientId, string pdfPath)
    {
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Diagnoses (PatientId, DiagnosisDate, PdfPath) VALUES (@PatientId, @DiagnosisDate, @PdfPath)";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@PatientId", patientId);
                command.Parameters.AddWithValue("@DiagnosisDate", DateTime.Now.ToString("o"));
                command.Parameters.AddWithValue("@PdfPath", pdfPath);
                command.ExecuteNonQuery();
            }
        }
    }

    /// <summary>
    /// Searches for a patient by their unique ID and returns a list of paths to their diagnosis PDFs.
    /// </summary>
    public static List<string> GetDiagnosesForPatient(string patientUniqueId)
    {
        var pdfPaths = new List<string>();
        using (var connection = new SQLiteConnection(connectionString))
        {
            connection.Open();
            string query = @"
                SELECT d.PdfPath 
                FROM Diagnoses d
                JOIN Patients p ON d.PatientId = p.Id
                WHERE p.PatientUniqueID = @PatientUniqueID
                ORDER BY d.DiagnosisDate DESC";

            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@PatientUniqueID", patientUniqueId);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pdfPaths.Add(reader["PdfPath"].ToString());
                    }
                }
            }
        }
        return pdfPaths;
    }


    // --- Existing Doctor/Auth Methods ---

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