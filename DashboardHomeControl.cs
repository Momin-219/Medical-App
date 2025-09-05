using System.Drawing;
using System.Windows.Forms;


namespace DocHelp
{
    public class DashboardHomeControl : UserControl
    {
        public DashboardHomeControl()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(245, 249, 252);
            this.Font = new Font("Segoe UI", 10F);

            // Main Title
            var titleLabel = new Label
            {
                Text = "Doctor Dashboard",
                Font = new Font("Segoe UI", 26F, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 55, 72),
                Location = new Point(40, 30),
                AutoSize = true
            };

            // Welcome Subtitle
            var welcomeLabel = new Label
            {
                Text = "Welcome! This portal is designed for ease of use and professional patient management.",
                Font = new Font("Segoe UI", 14F),
                ForeColor = Color.FromArgb(113, 128, 150),
                Location = new Point(42, 85),
                AutoSize = true
            };

            // --- "How it Works" Card ---
            var howItWorksPanel = CreateInfoCard("How It Works", 40, 150, 400, 220);
            var howItWorksLabel = new Label
            {
                Text = "• Navigate using the menu on the left.\n\n" +
                       "• Use the arrow icon to expand or collapse the menu.\n\n" +
                       "• Your Profile: Update your personal details and picture.\n\n" +
                       "• Add Patient & Records: These sections will be enabled in a future update.",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(45, 55, 72),
                Location = new Point(20, 60),
                Size = new Size(360, 150),
            };
            howItWorksPanel.Controls.Add(howItWorksLabel);

            // --- "Guiding Principles" Card ---
            var principlesPanel = CreateInfoCard("Guiding Principles", 480, 150, 400, 220);
            var principlesLabel = new Label
            {
                Text = "• Security: All data is stored locally and securely.\n\n" +
                       "• Efficiency: The interface is designed for quick access to essential functions.\n\n" +
                       "• Clarity: A clean and uncluttered UI to reduce cognitive load and allow you to focus on what matters.",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.FromArgb(45, 55, 72),
                Location = new Point(20, 60),
                Size = new Size(360, 150),
            };
            principlesPanel.Controls.Add(principlesLabel);

            // Chatbot Button
            var chatbotButton = new Button
            {
                Text = "Open Chatbot",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                BackColor = Color.FromArgb(66, 153, 225),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(160, 45),
                Location = new Point(40, 400) // Adjust as needed
            };
            chatbotButton.FlatAppearance.BorderSize = 0;

            // Event Handler for Button Click
            chatbotButton.Click += ChatbotButton_Click;


            this.Controls.Add(chatbotButton);
            this.Controls.Add(titleLabel);
            this.Controls.Add(welcomeLabel);
            this.Controls.Add(howItWorksPanel);
            this.Controls.Add(principlesPanel);
        }

        private void ChatbotButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Launching Chatbot (you can connect your RAG system here)", "Chatbot", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // In real use case: Launch a chatbot form or start your backend interaction
            var chatbotForm = new ChatbotForm();
            chatbotForm.Show();
        }

        // Helper method to create a consistent card style
        private Panel CreateInfoCard(string title, int x, int y, int width, int height)
        {
            var panel = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
            };

            var titleLabel = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(66, 153, 225),
                Location = new Point(15, 15),
                AutoSize = true
            };

            panel.Controls.Add(titleLabel);
            return panel;
        }
    }
}