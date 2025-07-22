using System;
using System.Windows.Forms;

static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // This crucial line makes sure the database file and tables exist before any other part
        // of the application tries to use them.
        DatabaseHelper.InitializeDatabase();

        // Starts the application by showing the WelcomeForm.
        Application.Run(new WelcomeForm());
    }
}