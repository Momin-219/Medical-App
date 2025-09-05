using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

public class AddPatientControl : UserControl
{
    private TextBox fullNameTextBox, ageTextBox, patientIdTextBox, symptomsTextBox;
    private Button proceedButton;
    private Dictionary<string, ComboBox> dropdowns = new Dictionary<string, ComboBox>();
    private readonly int currentDoctorId;

    public AddPatientControl(int doctorId)
    {
        this.currentDoctorId = doctorId;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Dock = DockStyle.Fill;
        this.BackColor = Color.FromArgb(245, 249, 252);
        this.Font = new Font("Segoe UI", 10F);

        var mainFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            AutoScroll = true,
            WrapContents = false,
            Padding = new Padding(30)
        };

        // --- Patient Info Section ---
        var patientInfoBox = CreateSectionBox("Patient Information");
        var patientInfoLayout = CreateSectionLayout();
        AddTextField(patientInfoLayout, "Full Name:", out fullNameTextBox);
        AddTextField(patientInfoLayout, "Patient ID (Unique):", out patientIdTextBox);
        AddTextField(patientInfoLayout, "Age:", out ageTextBox);
        AddMultilineField(patientInfoLayout, "Symptoms:", out symptomsTextBox);
        patientInfoBox.Controls.Add(patientInfoLayout);

        // --- Evaluation Sections ---
        var clinicalBox = CreateSectionBox("🧠 Clinical Evaluation");
        AddSectionFields(clinicalBox, new Dictionary<string, string[]>
        {
            { "Age Group", new[] { "> 19 years", "< 19 years" } },
            { "Testes Size", new[] { "< 4 ml", "4–12 ml", "> 12 ml" } },
            { "Epididymis", new[] { "Thin", "Normal", "Dilated" } },
            { "Vas Deferens", new[] { "Palpable", "Not Palpable" } },
            { "SSC", new[] { "Normal", "Absent" } }
        });

        var hormonalBox = CreateSectionBox("🧪 Hormonal Evaluation");
        AddSectionFields(hormonalBox, new Dictionary<string, string[]>
        {
            { "FSH", new[] { "Low", "Normal", "High" } },
            { "LH", new[] { "Low", "Normal", "High" } },
            { "Testosterone", new[] { "Low", "Normal", "High" } },
            { "Ejaculation", new[] { "Present", "Anejaculation" } },
            { "Prolactin", new[] { "Low", "Normal", "High" } },
            { "TSH", new[] { "Low", "Normal", "High" } }
        });

        var semenBox = CreateSectionBox("💠 Semen Analysis");
        AddSectionFields(semenBox, new Dictionary<string, string[]>
        {
            { "Azoospermia", new[] { "Present", "Absent" } },
            { "Semen pH", new[] { "> 7", "< 7" } },
            { "Semen Volume", new[] { "> 1 ml", "< 1 ml" } },
            { "Fructose", new[] { "Positive", "Negative" } },
            { "Semen Color", new[] { "Normal", "Yellowish", "Red" } }
        });

        var imagingBox = CreateSectionBox("🧲 Imaging");
        AddSectionFields(imagingBox, new Dictionary<string, string[]>
        {
            { "Seminal Vesicles", new[] { "Normal", "Enlarged", "Atrophied" } },
            { "Vas/Ejaculatory Ducts", new[] { "Normal", "Dilated", "Obstructed" } }
        });

        // --- Proceed Button ---
        proceedButton = new Button
        {
            Text = "Proceed to Preview",
            BackColor = Color.FromArgb(66, 153, 225),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Height = 45,
            Width = 200,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            Margin = new Padding(0, 20, 0, 0)
        };
        proceedButton.FlatAppearance.BorderSize = 0;
        proceedButton.Click += ProceedButton_Click;
        var buttonPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
        buttonPanel.Controls.Add(proceedButton);

        mainFlow.Controls.Add(patientInfoBox);
        mainFlow.Controls.Add(clinicalBox);
        mainFlow.Controls.Add(hormonalBox);
        mainFlow.Controls.Add(semenBox);
        mainFlow.Controls.Add(imagingBox);
        mainFlow.Controls.Add(buttonPanel);

        // --- LAYOUT FIX ---
        var bottomSpacer = new Panel { Height = 40 };
        mainFlow.Controls.Add(bottomSpacer);
        // ------------------

        this.Controls.Add(mainFlow);
    }

    private GroupBox CreateSectionBox(string title) => new GroupBox
    {
        Text = title,
        Font = new Font("Segoe UI", 12F, FontStyle.Bold),
        ForeColor = Color.FromArgb(45, 55, 72),
        AutoSize = true,
        MinimumSize = new Size(800, 0),
        Padding = new Padding(20, 25, 20, 20),
        Margin = new Padding(0, 0, 0, 15)
    };

    private TableLayoutPanel CreateSectionLayout()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        return layout;
    }

    private void AddTextField(TableLayoutPanel parent, string labelText, out TextBox textBox)
    {
        var label = new Label { Text = labelText, Font = new Font(this.Font, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        textBox = new TextBox { Dock = DockStyle.Fill, Font = new Font(this.Font, FontStyle.Regular), Height = 30 };
        parent.RowCount++;
        parent.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        parent.Controls.Add(label, 0, parent.RowCount - 1);
        parent.Controls.Add(textBox, 1, parent.RowCount - 1);
    }

    private void AddMultilineField(TableLayoutPanel parent, string labelText, out TextBox textBox)
    {
        var label = new Label { Text = labelText, Font = new Font(this.Font, FontStyle.Bold), Dock = DockStyle.Top, TextAlign = ContentAlignment.TopLeft, Padding = new Padding(0, 5, 0, 0) };
        textBox = new TextBox { Dock = DockStyle.Fill, Font = new Font(this.Font, FontStyle.Regular), Multiline = true, Height = 80, ScrollBars = ScrollBars.Vertical };
        parent.RowCount++;
        parent.RowStyles.Add(new RowStyle(SizeType.Absolute, 90F));
        parent.Controls.Add(label, 0, parent.RowCount - 1);
        parent.Controls.Add(textBox, 1, parent.RowCount - 1);
    }

    private void AddSectionFields(GroupBox parent, Dictionary<string, string[]> fields)
    {
        var layout = CreateSectionLayout();
        foreach (var field in fields)
        {
            var label = new Label { Text = field.Key, Font = new Font(this.Font, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            var comboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font(this.Font, FontStyle.Regular), Height = 30 };
            comboBox.Items.Add("Please select an option");
            comboBox.Items.AddRange(field.Value);
            comboBox.SelectedIndex = 0;
            dropdowns[field.Key] = comboBox;

            layout.RowCount++;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layout.Controls.Add(label, 0, layout.RowCount - 1);
            layout.Controls.Add(comboBox, 1, layout.RowCount - 1);
        }
        parent.Controls.Add(layout);
    }

    private void ProceedButton_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(fullNameTextBox.Text) || string.IsNullOrWhiteSpace(patientIdTextBox.Text))
        {
            MessageBox.Show("Patient Name and Patient ID are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (DatabaseHelper.PatientIdExists(patientIdTextBox.Text))
        {
            MessageBox.Show("This Patient ID already exists. Please enter a unique one.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (dropdowns.Values.Any(cb => cb.SelectedIndex <= 0))
        {
            MessageBox.Show("Please make a valid selection for all dropdown fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var patientData = new PatientData
        {
            PatientName = fullNameTextBox.Text,
            PatientId = patientIdTextBox.Text,
            Age = ageTextBox.Text,
            Symptoms = symptomsTextBox.Text
        };

        foreach (var kvp in dropdowns)
        {
            patientData.Selections.Add(kvp.Key, kvp.Value.SelectedItem.ToString());
        }

        using (var preview = new PreviewForm(patientData, this.currentDoctorId))
        {
            if (preview.ShowDialog() == DialogResult.OK)
            {
                fullNameTextBox.Clear();
                patientIdTextBox.Clear();
                ageTextBox.Clear();
                symptomsTextBox.Clear();
                foreach (var cb in dropdowns.Values) { cb.SelectedIndex = 0; }
            }
        }
    }
}