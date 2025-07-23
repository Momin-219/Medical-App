using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class AddPatientControl : UserControl
{
    private TableLayoutPanel formLayout;
    private TextBox fullNameTextBox, ageTextBox, symptomsTextBox;
    private Label resultLabel;
    private Button diagnoseButton;

    private Dictionary<string, ComboBox> dropdowns = new Dictionary<string, ComboBox>();

    public AddPatientControl()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Dock = DockStyle.Fill;
        this.BackColor = Color.White;

        // --- THIS IS THE ONLY CHANGE ---
        // This line enables scrolling for the entire control.
        this.AutoScroll = true;
        // -------------------------------

        diagnoseButton = new Button
        {
            Text = "Diagnose",
            BackColor = Color.Teal,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Height = 40,
            Width = 120,
            Margin = new Padding(30, 20, 0, 10),
            Anchor = AnchorStyles.Left
        };
        diagnoseButton.FlatAppearance.BorderSize = 0;
        diagnoseButton.Click += DiagnoseButton_Click;

        formLayout = new TableLayoutPanel
        {
            ColumnCount = 2,
            RowCount = 0,
            Dock = DockStyle.Top,
            Padding = new Padding(30),
            AutoSize = true
        };
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        AddTextField("Full Name", out fullNameTextBox);
        AddTextField("Age", out ageTextBox);
        AddMultilineField("Symptoms", out symptomsTextBox);

        AddSection("🧠 Clinical Evaluation", new Dictionary<string, string[]>
                {
                    { "Age Group", new[] { "> 19 years", "< 19 years" } },
                    { "Testes Size", new[] { "< 4 ml", "4–12 ml", "> 12 ml" } },
                    { "Epididymis", new[] { "Thin", "Normal", "Dilated" } },
                    { "Vas Deferens", new[] { "Palpable", "Not Palpable" } },
                    { "SSC", new[] { "Normal", "Absent" } }
                });

        AddSection("🧪 Hormonal Evaluation", new Dictionary<string, string[]>
                {
                    { "FSH", new[] { "Low", "Normal", "High" } },
                    { "LH", new[] { "Low", "Normal", "High" } },
                    { "Testosterone", new[] { "Low", "Normal", "High" } },
                    { "Ejaculation", new[] { "Present", "Anejaculation" } },
                    { "Prolactin", new[] { "Low", "Normal", "High" } },
                    { "TSH", new[] { "Low", "Normal", "High" } }
                });

        AddSection("💠 Semen Analysis", new Dictionary<string, string[]>
                {
                    { "Azoospermia", new[] { "Present", "Absent" } },
                    { "Semen pH", new[] { "> 7", "< 7" } },
                    { "Semen Volume", new[] { "> 1 ml", "< 1 ml" } },
                    { "Fructose", new[] { "Positive", "Negative" } },
                    { "Semen Color", new[] { "Normal", "Yellowish", "Red" } }
                });

        AddSection("🧲 Imaging", new Dictionary<string, string[]>
                {
                    { "Seminal Vesicles", new[] { "Normal", "Enlarged", "Atrophied" } },
                    { "Vas/Ejaculatory Ducts", new[] { "Normal", "Dilated", "Obstructed" } }
                });

        resultLabel = new Label
        {
            Text = "",
            AutoSize = true,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.DarkGreen,
            Margin = new Padding(30, 10, 0, 0)
        };

        var panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(30),
            AutoSize = true
        };

        panel.Controls.Add(diagnoseButton);
        panel.Controls.Add(resultLabel);

        // The order here matters. The control added last with DockStyle.Top will be at the very top.
        // This means the formLayout will appear first, and the panel with the button will appear after it.
        this.Controls.Add(panel);
        this.Controls.Add(formLayout);
    }

    private void AddTextField(string labelText, out TextBox textBox)
    {
        Label label = new Label { Text = labelText, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        textBox = new TextBox { Dock = DockStyle.Fill };
        formLayout.RowCount++;
        formLayout.Controls.Add(label, 0, formLayout.RowCount - 1); // Corrected to use RowCount-1
        formLayout.Controls.Add(textBox, 1, formLayout.RowCount - 1);
    }

    private void AddMultilineField(string labelText, out TextBox textBox)
    {
        Label label = new Label { Text = labelText, Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopLeft };
        textBox = new TextBox { Dock = DockStyle.Fill, Multiline = true, Height = 60, ScrollBars = ScrollBars.Vertical };
        formLayout.RowCount++;
        formLayout.Controls.Add(label, 0, formLayout.RowCount - 1); // Corrected to use RowCount-1
        formLayout.Controls.Add(textBox, 1, formLayout.RowCount - 1);
    }

    private void AddSection(string header, Dictionary<string, string[]> fields)
    {
        Label sectionHeader = new Label
        {
            Text = header,
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            ForeColor = Color.DarkSlateGray,
            AutoSize = true,
            Padding = new Padding(0, 10, 0, 5)
        };

        formLayout.RowCount++;
        formLayout.Controls.Add(sectionHeader, 0, formLayout.RowCount - 1); // Corrected to use RowCount-1
        formLayout.SetColumnSpan(sectionHeader, 2);

        foreach (var field in fields)
        {
            Label label = new Label { Text = field.Key, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
            ComboBox comboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            comboBox.Items.AddRange(field.Value);
            if (comboBox.Items.Count > 0) comboBox.SelectedIndex = 0;

            dropdowns[field.Key] = comboBox;

            formLayout.RowCount++;
            formLayout.Controls.Add(label, 0, formLayout.RowCount - 1); // Corrected to use RowCount-1
            formLayout.Controls.Add(comboBox, 1, formLayout.RowCount - 1);
        }
    }

    private void DiagnoseButton_Click(object sender, EventArgs e)
    {
        string name = fullNameTextBox.Text;
        string age = ageTextBox.Text;
        string symptoms = symptomsTextBox.Text;

        string azoospermia = dropdowns["Azoospermia"].SelectedItem?.ToString();
        string fsh = dropdowns["FSH"].SelectedItem?.ToString();
        string vas = dropdowns["Vas Deferens"].SelectedItem?.ToString();

        string diagnosis = "❌ Diagnosis not conclusive.";

        if (azoospermia == "Present")
        {
            if (vas == "Not Palpable" && dropdowns["Fructose"].SelectedItem?.ToString() == "Negative")
            {
                diagnosis = "🧬 Likely Obstructive Azoospermia (Congenital absence of Vas Deferens)";
            }
            else if (fsh == "High" && dropdowns["Testes Size"].SelectedItem?.ToString() == "< 4 ml")
            {
                diagnosis = "🧬 Likely Non-Obstructive Azoospermia (Testicular Failure)";
            }
        }

        resultLabel.Text = $"Result for {name}: {diagnosis}";
    }
}