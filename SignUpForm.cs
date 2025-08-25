using System;
using System.Drawing;
using System.Windows.Forms;

public class SignUpForm : Form
{
    private TextBox usernameTextBox;
    private TextBox passwordTextBox;
    private Button signupButton;
    private Button backButton;
    private Label statusLabel;
    private ProgressBar passwordStrengthBar;
    private Label passwordStrengthLabel;
    private Form welcomeForm;

    public SignUpForm(Form welcome)
    {
        this.welcomeForm = welcome;
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Text = "Create Account";
        this.FormBorderStyle = FormBorderStyle.None;
        this.WindowState = FormWindowState.Maximized;
        this.BackColor = Color.FromArgb(245, 249, 252);
        this.FormClosed += (s, e) => this.welcomeForm.Show();

        // Controls
        usernameTextBox = new TextBox { Font = new Font("Segoe UI", 12), Size = new Size(350, 30), PlaceholderText = "Choose a username" };
        passwordTextBox = new TextBox { Font = new Font("Segoe UI", 12), Size = new Size(350, 30), PlaceholderText = "Create a strong password", UseSystemPasswordChar = true };
        passwordTextBox.TextChanged += PasswordTextBox_TextChanged;

        passwordStrengthBar = new ProgressBar { Minimum = 0, Maximum = 5, Size = new Size(350, 10), Style = ProgressBarStyle.Continuous };
        passwordStrengthLabel = new Label { Font = new Font("Segoe UI", 9), ForeColor = Color.Gray, AutoSize = true, Text = "Password Strength" };

        signupButton = new Button { Text = "Create Account", Font = new Font("Segoe UI", 12, FontStyle.Bold), BackColor = Color.FromArgb(66, 153, 225), ForeColor = Color.White, Size = new Size(350, 45), FlatStyle = FlatStyle.Flat };
        signupButton.FlatAppearance.BorderSize = 0;
        signupButton.Click += SignupButton_Click;

        backButton = new Button { Text = "<- Back to Welcome", Font = new Font("Segoe UI", 10), BackColor = Color.Transparent, ForeColor = Color.FromArgb(45, 55, 72), FlatStyle = FlatStyle.Flat };
        backButton.FlatAppearance.BorderSize = 0;
        backButton.AutoSize = true;
        backButton.Click += (s, e) => this.Close();

        statusLabel = new Label { Font = new Font("Segoe UI", 9), ForeColor = Color.Red, AutoSize = true, Visible = false, MaximumSize = new Size(350, 0) };

        // Layout
        TableLayoutPanel mainPanel = new TableLayoutPanel { Dock = DockStyle.Fill };
        FlowLayoutPanel centerFlow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Anchor = AnchorStyles.None, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, WrapContents = false };

        centerFlow.Controls.Add(new Label { Text = "Sign Up", Font = new Font("Segoe UI", 28, FontStyle.Bold), ForeColor = Color.FromArgb(45, 55, 72), AutoSize = true, Margin = new Padding(0, 0, 0, 20) });
        centerFlow.Controls.Add(statusLabel);
        centerFlow.Controls.Add(usernameTextBox);
        centerFlow.Controls.Add(passwordTextBox);
        centerFlow.Controls.Add(passwordStrengthBar);
        centerFlow.Controls.Add(passwordStrengthLabel);
        centerFlow.Controls.Add(signupButton);
        centerFlow.Controls.Add(backButton);

        // Margins
        statusLabel.Margin = new Padding(0, 0, 0, 10);
        usernameTextBox.Margin = new Padding(0, 10, 0, 10);
        passwordTextBox.Margin = new Padding(0, 0, 0, 5);
        passwordStrengthBar.Margin = new Padding(0, 0, 0, 0);
        passwordStrengthLabel.Margin = new Padding(0, 0, 0, 20);
        signupButton.Margin = new Padding(0, 0, 0, 10);

        mainPanel.Controls.Add(centerFlow);
        this.Controls.Add(mainPanel);
    }

    private void PasswordTextBox_TextChanged(object sender, EventArgs e)
    {
        var strength = DatabaseHelper.CheckPasswordStrength(passwordTextBox.Text);
        passwordStrengthBar.Value = (int)strength;
        passwordStrengthLabel.Text = $"Strength: {strength}";

        switch (strength)
        {
            case DatabaseHelper.PasswordStrength.VeryWeak: passwordStrengthBar.ForeColor = Color.Red; break;
            case DatabaseHelper.PasswordStrength.Weak: passwordStrengthBar.ForeColor = Color.OrangeRed; break;
            case DatabaseHelper.PasswordStrength.Medium: passwordStrengthBar.ForeColor = Color.Orange; break;
            case DatabaseHelper.PasswordStrength.Strong: passwordStrengthBar.ForeColor = Color.YellowGreen; break;
            case DatabaseHelper.PasswordStrength.VeryStrong: passwordStrengthBar.ForeColor = Color.Green; break;
            default: passwordStrengthBar.ForeColor = Color.Gray; break;
        }
    }

    private void SignupButton_Click(object sender, EventArgs e)
    {
        if (DatabaseHelper.CheckPasswordStrength(passwordTextBox.Text) < DatabaseHelper.PasswordStrength.Medium)
        {
            statusLabel.Text = "Password is too weak. Please choose a stronger one.";
            statusLabel.Visible = true;
            return;
        }

        if (DatabaseHelper.SignUp(usernameTextBox.Text, passwordTextBox.Text))
        {
            MessageBox.Show("Account created successfully! You can now log in.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close(); // Closes sign up form, shows welcome form
        }
        else
        {
            statusLabel.Text = "This username is already taken. Please choose another.";
            statusLabel.Visible = true;
        }
    }
}