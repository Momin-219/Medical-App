using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class PreviewForm : Form
{
    private readonly PatientData patientData;
    private readonly int doctorId;

    public PreviewForm(PatientData data, int docId)
    {
        this.patientData = data;
        this.doctorId = docId;
        this.patientData.DiagnosisResult = DiagnosisLogic.GetDiagnosis(this.patientData);

        InitializeComponent();
        PopulatePreview();
    }

    private void InitializeComponent()
    {
        this.Text = "Preview Diagnosis";
        this.Size = new Size(750, 800);
        this.StartPosition = FormStartPosition.CenterParent;
        this.Font = new Font("Segoe UI", 10F);
        this.BackColor = Color.FromArgb(245, 249, 252);

        var mainFlowPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(30)
        };

        var titleLabel = new Label
        {
            Text = "Confirm Diagnosis Details",
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            ForeColor = Color.FromArgb(45, 55, 72),
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 20)
        };
        mainFlowPanel.Controls.Add(titleLabel);

        var buttonPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Dock = DockStyle.Bottom,
            BackColor = Color.White,
            Padding = new Padding(15),
            Height = 70
        };

        var saveButton = new Button { Text = "Save PDF", BackColor = Color.FromArgb(56, 161, 105), ForeColor = Color.White, Size = new Size(150, 40), Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
        var backButton = new Button { Text = "Back to Edit", Size = new Size(150, 40) };

        saveButton.Click += SaveButton_Click;
        backButton.Click += (s, e) => this.Close();

        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(backButton);

        this.Controls.Add(mainFlowPanel);
        this.Controls.Add(buttonPanel);
    }

    private void PopulatePreview()
    {
        var mainFlowPanel = this.Controls[0] as FlowLayoutPanel;
        if (mainFlowPanel == null) return;

        var dataTable = new TableLayoutPanel { ColumnCount = 2, AutoSize = true, Dock = DockStyle.Top, Margin = new Padding(0, 0, 0, 20) };
        dataTable.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
        dataTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainFlowPanel.Controls.Add(dataTable);

        AddDataRow(dataTable, "Patient Name:", patientData.PatientName);
        AddDataRow(dataTable, "Patient ID:", patientData.PatientId);
        AddDataRow(dataTable, "Age:", patientData.Age);
        AddDataRow(dataTable, "Symptoms:", patientData.Symptoms);

        foreach (var selection in patientData.Selections)
        {
            AddDataRow(dataTable, $"{selection.Key}:", selection.Value);
        }

        var diagnosisBox = new GroupBox
        {
            Text = "Final Diagnosis",
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(45, 55, 72),
            AutoSize = true,
            Dock = DockStyle.Top,
            Padding = new Padding(15),
            Margin = new Padding(0, 10, 0, 0)
        };

        var diagnosisLabel = new Label
        {
            Text = patientData.DiagnosisResult,
            Font = new Font("Segoe UI", 10F, FontStyle.Regular),
            AutoSize = true,
            MaximumSize = new Size(650, 0),
            ForeColor = Color.DarkSlateBlue,
            Padding = new Padding(5)
        };

        diagnosisBox.Controls.Add(diagnosisLabel);
        mainFlowPanel.Controls.Add(diagnosisBox);

        var bottomSpacer = new Panel { Height = 40 };
        mainFlowPanel.Controls.Add(bottomSpacer);
    }

    private void AddDataRow(TableLayoutPanel parent, string label, string value)
    {
        var labelControl = new Label { Text = label, Font = new Font(this.Font, FontStyle.Bold), AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft };
        var valueControl = new Label { Text = string.IsNullOrEmpty(value) ? "N/A" : value, AutoSize = true, Anchor = AnchorStyles.Left, TextAlign = ContentAlignment.MiddleLeft };

        parent.RowCount++;
        parent.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
        parent.Controls.Add(labelControl, 0, parent.RowCount - 1);
        parent.Controls.Add(valueControl, 1, parent.RowCount - 1);
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        try
        {
            string rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "DocHelpApp");
            string doctorPath = Path.Combine(rootPath, $"Doc_{doctorId}");

            string safePatientName = SanitizeFolderName(patientData.PatientName);
            string patientFolderName = $"{safePatientName}_{patientData.PatientId}";
            string patientPath = Path.Combine(doctorPath, patientFolderName);

            Directory.CreateDirectory(patientPath);

            string fileName = $"Diagnosis_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string fullPath = Path.Combine(patientPath, fileName);

            PdfGenerator.Generate(patientData, fullPath);

            int patientDbId = DatabaseHelper.AddOrGetPatient(doctorId, patientData.PatientId, patientData.PatientName);
            DatabaseHelper.AddDiagnosis(patientDbId, fullPath);

            MessageBox.Show($"Successfully saved PDF to:\n{fullPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred while saving:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Removes characters that are illegal in folder names to prevent errors.
    /// </summary>
    private string SanitizeFolderName(string name)
    {
        if (string.IsNullOrEmpty(name)) return "Unnamed";

        foreach (char c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        foreach (char c in Path.GetInvalidPathChars())
        {
            name = name.Replace(c, '_');
        }
        return name;
    }
}