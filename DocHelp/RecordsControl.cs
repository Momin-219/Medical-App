using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using PdfiumViewer;

public class RecordsControl : UserControl
{
    private TextBox searchTextBox;
    private Button searchButton;

    // --- KEY CHANGE: This is now a TableLayoutPanel ---
    private TableLayoutPanel resultsPanel;

    private PdfViewer pdfViewer;
    private Panel viewerPlaceholderPanel;
    private Panel activeCard = null;

    private string selectedPdfPath;


    public RecordsControl()
    {
        InitializeComponent();
        ShowPlaceholder();
    }

    private void InitializeComponent()
    {
        this.Dock = DockStyle.Fill;
        this.BackColor = Color.FromArgb(245, 249, 252);
        this.Font = new Font("Segoe UI", 10F);
        this.Padding = new Padding(30);

        var searchPanel = new Panel { Dock = DockStyle.Top, Height = 100, Padding = new Padding(0, 0, 0, 20) };
        var searchBox = new GroupBox
        {
            Text = "Search Patient Records",
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(45, 55, 72),
            Dock = DockStyle.Fill
        };
        searchTextBox = new TextBox { Bounds = new Rectangle(15, 35, 300, 30), Font = new Font("Segoe UI", 11F), Anchor = AnchorStyles.Top | AnchorStyles.Left };
        searchButton = new Button { Text = "Search", Bounds = new Rectangle(320, 34, 100, 32), Font = new Font("Segoe UI", 10F, FontStyle.Bold), BackColor = Color.FromArgb(66, 153, 225), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        searchButton.FlatAppearance.BorderSize = 0;
        searchButton.Click += SearchButton_Click;
        searchTextBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) searchButton.PerformClick(); };
        searchBox.Controls.Add(searchTextBox);
        searchBox.Controls.Add(searchButton);
        searchPanel.Controls.Add(searchBox);

        var splitContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.FixedSingle,
            SplitterDistance = 350,
            FixedPanel = FixedPanel.Panel1
        };


        //Chat logic starts here
        // Chatbot Panel
        var chatbotPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 220,
            Padding = new Padding(10),
            BackColor = Color.WhiteSmoke
        };

        // Chat Display
        var chatDisplay = new RichTextBox
        {
            ReadOnly = true,
            Dock = DockStyle.Top,
            Height = 140,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        // Input
        var chatInput = new TextBox
        {
            Dock = DockStyle.Bottom,
            Height = 30,
            Font = new Font("Segoe UI", 10F)
        };

        // Button
        var sendButton = new Button
        {
            Text = "Ask",
            Dock = DockStyle.Bottom,
            Height = 30,
            BackColor = Color.FromArgb(66, 153, 225),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        sendButton.FlatAppearance.BorderSize = 0;

        // Send Event
        /*sendButton.Click += async (s, e) =>
        {
            var question = chatInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(question)) return;

            chatDisplay.AppendText($"> {question}\n");

            // CALL your backend here
            string response = await AskChatbotAsync(selectedPdfPath, question);  // You'll define this function

            chatDisplay.AppendText($"{response}\n\n");
            chatInput.Clear();
        };*/

        // Add to chatbotPanel
        chatbotPanel.Controls.Add(sendButton);
        chatbotPanel.Controls.Add(chatInput);
        chatbotPanel.Controls.Add(chatDisplay);

        this.Controls.Add(chatbotPanel);




        var resultsContainer = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        var resultsHeader = new Label { Text = "Diagnosis Reports", Dock = DockStyle.Top, Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.FromArgb(45, 55, 72), Padding = new Padding(10), Height = 40, TextAlign = ContentAlignment.MiddleLeft };

        // --- KEY CHANGE: Initialize as a TableLayoutPanel ---
        resultsPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            ColumnCount = 1, // Only one column
            RowCount = 0,
        };
        resultsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Make the column fill the width

        resultsContainer.Controls.Add(resultsPanel);
        resultsContainer.Controls.Add(resultsHeader);
        splitContainer.Panel1.Controls.Add(resultsContainer);

        pdfViewer = new PdfViewer { Dock = DockStyle.Fill, ShowToolbar = false, ZoomMode = PdfViewerZoomMode.FitWidth };
        viewerPlaceholderPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        var placeholderLabel = new Label { Text = "Select a report from the left to view it here.", Font = new Font("Segoe UI", 12F, FontStyle.Italic), ForeColor = Color.Gray, AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };
        viewerPlaceholderPanel.Controls.Add(placeholderLabel);
        splitContainer.Panel2.Controls.Add(viewerPlaceholderPanel);
        splitContainer.Panel2.Controls.Add(pdfViewer);

        this.Controls.Add(splitContainer);
        this.Controls.Add(searchPanel);
        this.Disposed += (s, e) => pdfViewer.Document?.Dispose();
    }

    private void ClearResults()
    {
        resultsPanel.Controls.Clear();
        resultsPanel.RowStyles.Clear();
        resultsPanel.RowCount = 0;
    }

    private void SearchButton_Click(object sender, EventArgs e)
    {
        ClearResults();
        ShowPlaceholder();

        string patientId = searchTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(patientId))
        {
            DisplayMessage("Please enter a Patient ID to search.");
            return;
        }

        List<string> pdfPaths = DatabaseHelper.GetDiagnosesForPatient(patientId);

        if (pdfPaths.Count == 0)
        {
            DisplayMessage($"No records found for Patient ID: {patientId}");
        }
        else
        {
            ClearResults();
            foreach (string path in pdfPaths)
            {
                // --- KEY CHANGE: Add the card to the table ---
                var card = CreateResultCard(path);
                resultsPanel.RowCount++;
                resultsPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 65F)); // Set the row height
                resultsPanel.Controls.Add(card, 0, resultsPanel.RowCount - 1); // Add card to the new row
            }
        }
    }

    private Panel CreateResultCard(string filePath)
    {
        var borderPanel = new Panel
        {
            // --- KEY CHANGE: Use Dock.Fill to stretch horizontally ---
            Dock = DockStyle.Fill,
            Margin = new Padding(5),
            Padding = new Padding(1),
            BackColor = Color.LightGray
        };

        var contentPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(8, 4, 8, 4)
        };
        table.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
        table.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));

        var link = new LinkLabel
        {
            Text = Path.GetFileName(filePath),
            Tag = filePath,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            LinkColor = Color.FromArgb(0, 120, 215),
            ActiveLinkColor = Color.Red,
            AutoEllipsis = true,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        link.LinkClicked += PdfLink_LinkClicked;

        var dateLabel = new Label
        {
            Text = $"Created: {File.GetCreationTime(filePath):yyyy-MM-dd HH:mm}",
            Font = new Font("Segoe UI", 8F, FontStyle.Italic),
            ForeColor = Color.Gray,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft
        };

        table.Controls.Add(link, 0, 0);
        table.Controls.Add(dateLabel, 0, 1);
        contentPanel.Controls.Add(table);
        borderPanel.Controls.Add(contentPanel);
        borderPanel.Tag = contentPanel;

        return borderPanel;
    }

    private void PdfLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        var clickedLink = sender as LinkLabel;
        if (clickedLink?.Tag == null) return;

        var card = clickedLink.Parent?.Parent?.Parent as Panel;
        if (card == null) return;

        if (activeCard != null && activeCard.Tag is Panel oldContent)
        {
            oldContent.BackColor = Color.White;
        }
        activeCard = card;
        if (activeCard.Tag is Panel newContent)
        {
            newContent.BackColor = Color.AliceBlue;
        }

        string filePath = clickedLink.Tag.ToString();

        if (File.Exists(filePath))
        {
            ShowPdfViewer();
            pdfViewer.Document?.Dispose();
            pdfViewer.Document = PdfDocument.Load(filePath);
            selectedPdfPath = filePath;
        }
        else
        {
            MessageBox.Show($"The file could not be found at the specified path:\n{filePath}\n\nIt may have been moved or deleted.", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DisplayMessage(string text)
    {
        ClearResults();
        var label = new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 11F, FontStyle.Regular),
            ForeColor = Color.DimGray,
            AutoSize = true,
            Margin = new Padding(10)
        };
        resultsPanel.RowCount = 1;
        resultsPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        resultsPanel.Controls.Add(label, 0, 0);
    }

    private void ShowPlaceholder()
    {
        pdfViewer.Visible = false;
        viewerPlaceholderPanel.Visible = true;
        pdfViewer.Document?.Dispose();
        pdfViewer.Document = null;
    }

    private void ShowPdfViewer()
    {
        viewerPlaceholderPanel.Visible = false;
        pdfViewer.Visible = true;
    }
}