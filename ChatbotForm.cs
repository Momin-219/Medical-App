using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DocHelp
{
    public partial class ChatbotForm : Form
    {
        private readonly HttpClient httpClient;
        private TextBox txtQuestion;
        private RichTextBox txtChat;
        private Button btnUploadPdf;
        private Button btnSend;

        public ChatbotForm()
        {
            InitializeCustomComponents();
            httpClient = new HttpClient();
        }

        private void InitializeCustomComponents()
        {
            this.Text = "Medical Chatbot";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Chat display
            txtChat = new RichTextBox
            {
                Location = new Point(20, 20),
                Size = new Size(540, 300),
                ReadOnly = true,
                BackColor = Color.WhiteSmoke
            };

            // Question input
            txtQuestion = new TextBox
            {
                Location = new Point(20, 340),
                Size = new Size(400, 30)
            };

            txtQuestion.KeyDown += TxtQuestion_KeyDown;

            // Upload PDF button
            btnUploadPdf = new Button
            {
                Text = "Upload PDF",
                Location = new Point(20, 390),
                Size = new Size(120, 35),
                BackColor = Color.LightSteelBlue
            };
            btnUploadPdf.Click += BtnUploadPdf_Click;

            // Send button
            btnSend = new Button
            {
                Text = "Send",
                Location = new Point(440, 340),
                Size = new Size(120, 35),
                BackColor = Color.LightGreen
            };
            btnSend.Click += BtnSend_Click;

            // Add controls
            this.Controls.Add(txtChat);
            this.Controls.Add(txtQuestion);
            this.Controls.Add(btnUploadPdf);
            this.Controls.Add(btnSend);
        }

        // ChatbotForm.cs
        // Handle PDF Upload
        private async void BtnUploadPdf_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                txtChat.AppendText($"[System] Uploading {Path.GetFileName(filePath)}...\r\n");

                try
                {
                    using var form = new MultipartFormDataContent();
                    using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    form.Add(new StreamContent(fileStream), "file", Path.GetFileName(filePath));

                    var response = await httpClient.PostAsync("http://127.0.0.1:5000/upload", form);
                    response.EnsureSuccessStatusCode();

                    string result = await response.Content.ReadAsStringAsync();
                    txtChat.AppendText($"[System] {result}\r\n");
                }
                catch (Exception ex)
                {
                    txtChat.AppendText($"[Error] {ex.Message}\r\n");
                }
            }
        }

        // Handle Ask Bot
        private async void BtnSend_Click(object sender, EventArgs e)
        {
            string question = txtQuestion.Text.Trim();
            if (string.IsNullOrEmpty(question)) return;

            txtChat.AppendText($"[You] {question}\r\n");
            txtQuestion.Clear();

            string answer = await AskBotAsync(question);
            txtChat.AppendText($"[Bot] {answer}\r\n");
        }


        private async Task<string> AskBotAsync(string question)
        {
            try
            {
                var json = $"{{\"query\":\"{question}\"}}";
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("http://127.0.0.1:5000/ask", content);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private void TxtQuestion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Prevents the "ding" sound
                BtnSend_Click(sender, e);  // Trigger send button click
            }
        }

    }
}
