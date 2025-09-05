using System;
using System.Drawing;
using System.Windows.Forms;


namespace DocHelp{
    public class LoginForm : Form
    {
        private TextBox usernameTextBox;
        private TextBox passwordTextBox;
        private Button loginButton;
        private Button backButton;
        private Label statusLabel;
        private Form welcomeForm; // Reference to the welcome screen
        private bool loginSuccessful = false; // A flag to track success

        public LoginForm(Form welcome)
        {
            this.welcomeForm = welcome;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Login";
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 249, 252);

            // This event now correctly handles closing
            this.FormClosing += LoginForm_FormClosing;

            // Controls
            usernameTextBox = new TextBox { Font = new Font("Segoe UI", 12), Size = new Size(350, 30), PlaceholderText = "Username" };
            passwordTextBox = new TextBox { Font = new Font("Segoe UI", 12), Size = new Size(350, 30), PlaceholderText = "Password", UseSystemPasswordChar = true };

            loginButton = new Button { Text = "Login", Font = new Font("Segoe UI", 12, FontStyle.Bold), BackColor = Color.FromArgb(66, 153, 225), ForeColor = Color.White, Size = new Size(350, 45), FlatStyle = FlatStyle.Flat };
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Click += LoginButton_Click;

            backButton = new Button { Text = "<- Back to Welcome", Font = new Font("Segoe UI", 10), BackColor = Color.Transparent, ForeColor = Color.FromArgb(45, 55, 72), FlatStyle = FlatStyle.Flat };
            backButton.FlatAppearance.BorderSize = 0;
            backButton.AutoSize = true;
            backButton.Click += (s, e) => this.Close(); // Back button simply closes this form

            statusLabel = new Label { Font = new Font("Segoe UI", 9), ForeColor = Color.Red, AutoSize = true, Visible = false, MaximumSize = new Size(350, 0) };

            // Layout (This is unchanged)
            TableLayoutPanel mainPanel = new TableLayoutPanel { Dock = DockStyle.Fill };
            FlowLayoutPanel centerFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Anchor = AnchorStyles.None, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, WrapContents = false };

            centerFlow.Controls.Add(new Label { Text = "Login to Portal", Font = new Font("Segoe UI", 28, FontStyle.Bold), ForeColor = Color.FromArgb(45, 55, 72), AutoSize = true, Margin = new Padding(0, 0, 0, 20) });
            centerFlow.Controls.Add(statusLabel);
            centerFlow.Controls.Add(usernameTextBox);
            centerFlow.Controls.Add(passwordTextBox);
            centerFlow.Controls.Add(loginButton);
            centerFlow.Controls.Add(backButton);

            statusLabel.Margin = new Padding(0, 0, 0, 10);
            usernameTextBox.Margin = new Padding(0, 10, 0, 10);
            passwordTextBox.Margin = new Padding(0, 0, 0, 20);
            loginButton.Margin = new Padding(0, 0, 0, 10);

            mainPanel.Controls.Add(centerFlow);
            this.Controls.Add(mainPanel);
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            var doctorId = DatabaseHelper.Login(usernameTextBox.Text, passwordTextBox.Text);
            if (doctorId.HasValue)
            {
                // 1. Set the success flag to true
                this.loginSuccessful = true;

                // 2. Create and show the main application form
                var mainApp = new MainForm(doctorId.Value, welcomeForm);
                mainApp.Show();

                // 3. Close the login form
                this.Close();
            }
            else
            {
                statusLabel.Text = "Invalid username or password.";
                statusLabel.Visible = true;
            }
        }

        // This new method is the key to the solution
        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if the form is closing WITHOUT a successful login
            if (!this.loginSuccessful)
            {
                // If so, show the welcome form again
                this.welcomeForm.Show();
            }
            // If login WAS successful, this code does nothing, and the
            // welcome form remains hidden, as it should.
        }
    }
}