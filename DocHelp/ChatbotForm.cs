using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;       // For TTS
using NAudio.Wave;                          // Mic recording
using Vosk;                                 // Offline speech recognition
using Newtonsoft.Json.Linq;                 // JSON handling
/*using System.Speech.Recognition;   // For Speech-to-Text
using System.Speech.Synthesis;    // For Text-to-Speech
using System.Drawing;*/             // For UI elements

namespace DocHelp
{
    public partial class ChatbotForm : Form
    {
        private readonly HttpClient httpClient;
        private TextBox txtQuestion;
        private RichTextBox txtChat;
        private Button btnUploadPdf;
        private Button btnSend;

        private Button btnVoice;                    // 🎤 New button for voice input
        
        // For Voice
        private WaveInEvent waveIn;
        private VoskRecognizer recognizer;

        public ChatbotForm()
        {
            InitializeCustomComponents();
            httpClient = new HttpClient();
            //synthesizer = new SpeechSynthesizer();

            // Init Vosk STT model
            Vosk.Vosk.SetLogLevel(0);
            var model = new Vosk.Model("models/vosk-model-small-en-us-0.15");       // put model folder inside /models
            recognizer = new VoskRecognizer(model, 16000f);
        }

        private void InitializeCustomComponents()
        {
            /*this.Text = "Medical Chatbot";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;*/

            this.Text = "🩺 DocHelp Assistant";
            this.Size = new Size(650, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // Chat display
            /*txtChat = new RichTextBox
            {
                Location = new Point(20, 20),
                Size = new Size(540, 300),
                ReadOnly = true,
                BackColor = Color.WhiteSmoke
            };*/

            txtChat = new RichTextBox
            {
                Location = new Point(20, 60),
                Size = new Size(590, 350),
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Question input
            /*txtQuestion = new TextBox
            {
                Location = new Point(20, 340),
                Size = new Size(400, 30)
            };*/

            txtQuestion = new TextBox
            {
                Location = new Point(20, 430),
                Size = new Size(400, 35),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            txtQuestion.KeyDown += TxtQuestion_KeyDown;

            // Upload PDF button
            /*btnUploadPdf = new Button
            {
                Text = "Upload PDF",
                Location = new Point(20, 390),
                Size = new Size(120, 35),
                BackColor = Color.LightSteelBlue
            };*/

            btnUploadPdf = new Button
            {
                Text = "📎 Upload PDF",
                Location = new Point(20, 480),
                Size = new Size(150, 35),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnUploadPdf.FlatAppearance.BorderSize = 0;
            btnUploadPdf.Click += BtnUploadPdf_Click;

            // Send button
            /*btnSend = new Button
            {
                Text = "Send",
                Location = new Point(440, 340),
                Size = new Size(120, 35),
                BackColor = Color.LightGreen
            };*/

            btnSend = new Button
            {
                Text = "Send 📨",
                Location = new Point(440, 430),
                Size = new Size(80, 35),
                BackColor = Color.MediumSeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Click += BtnSend_Click;

            // 🎤 Voice Input button
            /*btnVoice = new Button
            {
                Text = "🎤 Voice",
                Location = new Point(370, 370),
                Size = new Size(80, 35),
                BackColor = Color.LightPink
            };*/

            btnVoice = new Button
            {
                Text = "🎤 Voice",
                Location = new Point(540, 430),
                Size = new Size(70, 35),
                BackColor = Color.IndianRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnVoice.FlatAppearance.BorderSize = 0;
            btnVoice.Click += BtnVoice_Click;

            // Header label
            Label lblTitle = new Label
            {
                Text = "🤖 Medical Chat Assistant",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 60, 60)
            };

            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(txtChat);
            this.Controls.Add(txtQuestion);
            this.Controls.Add(btnUploadPdf);
            this.Controls.Add(btnSend);
            this.Controls.Add(btnVoice);
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
                    txtChat.AppendText($"[System] {result}\r");
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

            txtChat.AppendText($"[You] {question}\r");
            txtQuestion.Clear();

            string answer = await AskBotAsync(question);
            txtChat.AppendText($"[Bot] {answer}\r");

            // 🗣️ Speak the bot's answer
            //synthesizer.SpeakAsync(answer);
            // 🔊 Speak answer
            SpeakText(answer);
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
                result = result.Replace("\\n", " ").Replace("\n", " ").Trim();
                result = result.Replace("answer", "").Trim();
                result = result.Replace(":", "").Trim();
                return result;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        // --------------------------
        // 🎤 Handle Voice Input
        private void BtnVoice_Click(object sender, EventArgs e)
        {
            txtChat.AppendText("[System] Listening for 5 seconds...\r\n");

            waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(16000, 1)
            };

            waveIn.DataAvailable += (s, a) =>
            {
                recognizer.AcceptWaveform(a.Buffer, a.BytesRecorded);
            };

            waveIn.RecordingStopped += async (s, a) =>
            {
                var result = recognizer.FinalResult();
                var text = JObject.Parse(result)["text"]?.ToString();

                if (!string.IsNullOrEmpty(text))
                {
                    txtChat.AppendText($"[You - Voice] {text}\r");
                    string answer = await AskBotAsync(text);
                    txtChat.AppendText($"[Bot] {answer}\r");
                    SpeakText(answer);
                }

                waveIn.Dispose();
            };

            waveIn.StartRecording();

            // Auto-stop after 6 sec
            Task.Delay(6000).ContinueWith(_ => waveIn.StopRecording());
        }

        // --------------------------
        // 🔊 Text-to-Speech (Windows SAPI via COM)
        private void SpeakText(string text)
        {
            try
            {
                Type sapiType = Type.GetTypeFromProgID("SAPI.SpVoice");
                dynamic sapi = Activator.CreateInstance(sapiType);
                sapi.Speak(text);
            }
            catch (Exception ex)
            {
                txtChat.AppendText($"[Error - TTS] {ex.Message}\r\n");
            }
        }

        private void TxtQuestion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;                      // Prevents the "ding" sound
                BtnSend_Click(sender, e);                       // Trigger send button click
            }
        }

    }
}
