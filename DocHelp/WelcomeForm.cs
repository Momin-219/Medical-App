using System;
using System.Drawing;
using System.Windows.Forms;

namespace DocHelp{
    public class WelcomeForm : Form
    {
        private Button loginButton;
        private Button signupButton;
        private Label titleLabel;
        private Label subtitleLabel;

        public WelcomeForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Doctor Portal";
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 249, 252);

            // Title Label
            titleLabel = new Label
            {
                Text = "Welcome to the Doctor Portal",
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                ForeColor = Color.FromArgb(26, 32, 44),
                AutoSize = true,
            };

            // Subtitle Label
            subtitleLabel = new Label
            {
                Text = "Your dedicated application for patient management.",
                Font = new Font("Segoe UI", 16),
                ForeColor = Color.FromArgb(113, 128, 150),
                AutoSize = true,
            };

            // Login Button
            loginButton = new Button
            {
                Text = "Login",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                BackColor = Color.FromArgb(66, 153, 225),
                ForeColor = Color.White,
                Size = new Size(200, 60),
                FlatStyle = FlatStyle.Flat,
            };
            loginButton.FlatAppearance.BorderSize = 0;
            loginButton.Click += (s, e) => ShowForm(new LoginForm(this));

            // Signup Button
            signupButton = new Button
            {
                Text = "Sign Up",
                Font = new Font("Segoe UI", 14, FontStyle.Regular),
                BackColor = Color.FromArgb(237, 242, 247),
                ForeColor = Color.FromArgb(45, 55, 72),
                Size = new Size(200, 60),
                FlatStyle = FlatStyle.Flat,
            };
            signupButton.FlatAppearance.BorderSize = 0;
            signupButton.Click += (s, e) => ShowForm(new SignUpForm(this));

            // Layout Panel
            TableLayoutPanel mainPanel = new TableLayoutPanel { Dock = DockStyle.Fill };
            FlowLayoutPanel centerFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Anchor = AnchorStyles.None,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false,
                Padding = new Padding(20)
            };

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
            };

            buttonPanel.Controls.Add(loginButton);
            buttonPanel.Controls.Add(signupButton);
            loginButton.Margin = new Padding(10);
            signupButton.Margin = new Padding(10);

            centerFlow.Controls.Add(titleLabel);
            centerFlow.Controls.Add(subtitleLabel);
            centerFlow.Controls.Add(buttonPanel);

            titleLabel.Margin = new Padding(0, 0, 0, 10);
            subtitleLabel.Margin = new Padding(0, 0, 0, 30);

            mainPanel.Controls.Add(centerFlow);
            this.Controls.Add(mainPanel);
        }

        private void ShowForm(Form newForm)
        {
            this.Hide();
            newForm.Show();
        }
    }
}