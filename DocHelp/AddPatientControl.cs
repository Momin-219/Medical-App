using System;
using System.Drawing;
using System.Windows.Forms;

public class AddPatientControl : UserControl
{
    private FlowLayoutPanel mainFlowPanel;
    private GroupBox patientIdSection, clinicalSection, hormonalSection, historySection, geneticSection, systemicHealthSection, imagingSection;
    private Panel semenAnalysisPanel, prostateAbnormalPanel;
    private Button proceedToClinicalButton, proceedToHormonalButton, proceedToHistoryButton, proceedToGeneticButton, proceedToSystemicButton, proceedToImagingButton, finishButton;

    public AddPatientControl()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        this.Dock = DockStyle.Fill;
        this.BackColor = Color.FromArgb(245, 249, 252);
        this.Font = new Font("Segoe UI", 10F);
        this.AutoScroll = true;

        mainFlowPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(30)
        };

        // Create all sections based on the diagnostic sheet
        CreatePatientIdSection();
        CreateClinicalSection1();
        CreateHormonalSection2();
        CreateHistorySection();
        CreateGeneticSection();
        CreateSystemicHealthSection();
        CreateImagingSection3(); // This is the final section

        mainFlowPanel.Controls.AddRange(new Control[] {
            patientIdSection,
            clinicalSection,
            hormonalSection,
            historySection,
            geneticSection,
            systemicHealthSection,
            imagingSection
        });
        this.Controls.Add(mainFlowPanel);

        // Set initial visibility
        clinicalSection.Visible = false;
        hormonalSection.Visible = false;
        historySection.Visible = false;
        geneticSection.Visible = false;
        systemicHealthSection.Visible = false;
        imagingSection.Visible = false;
    }

    // --- Helper Methods for UI Creation ---

    private GroupBox CreateSectionBox(string title)
    {
        return new GroupBox
        {
            Text = title,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(45, 55, 72),
            AutoSize = true,
            MinimumSize = new Size(850, 0),
            Padding = new Padding(20, 25, 20, 20)
        };
    }

    private TableLayoutPanel CreateSectionLayout()
    {
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 2 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300F)); // Increased width for longer labels
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        return layout;
    }

    private FlowLayoutPanel CreateOptionsPanel(params Control[] controls)
    {
        var panel = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, Margin = new Padding(0, 5, 0, 5) };
        panel.Controls.AddRange(controls);
        return panel;
    }

    private Label CreateFieldLabel(string text) => new Label { Text = text, Anchor = AnchorStyles.Left, Font = new Font("Segoe UI", 10F, FontStyle.Bold), AutoSize = true, TextAlign = ContentAlignment.MiddleLeft, Margin = new Padding(0, 8, 0, 8) };

    private Button CreateProceedButton(string text) => new Button { Text = text, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Size = new Size(200, 40), Anchor = AnchorStyles.Left };

    // --- Section Creation Methods ---

    private void CreatePatientIdSection()
    {
        patientIdSection = CreateSectionBox("Patient Identification");
        var layout = CreateSectionLayout();

        var patientNameTextBox = new TextBox { Width = 300, Font = new Font("Segoe UI", 10F) };
        var patientIdTextBox = new TextBox { Width = 300, Font = new Font("Segoe UI", 10F) };

        proceedToClinicalButton = CreateProceedButton("Begin Evaluation");
        proceedToClinicalButton.Click += (s, e) => { if (!string.IsNullOrWhiteSpace(patientNameTextBox.Text)) clinicalSection.Visible = true; else MessageBox.Show("Patient Name is required."); };

        layout.Controls.Add(CreateFieldLabel("Patient Name:"), 0, 0);
        layout.Controls.Add(patientNameTextBox, 1, 0);
        layout.Controls.Add(CreateFieldLabel("Patient ID:"), 0, 1);
        layout.Controls.Add(patientIdTextBox, 1, 1);
        layout.Controls.Add(new Panel(), 0, 2); // Spacer
        layout.Controls.Add(proceedToClinicalButton, 1, 3);

        patientIdSection.Controls.Add(layout);
    }

    private void CreateClinicalSection1()
    {
        clinicalSection = CreateSectionBox("Section 1: Clinical Evaluation");
        var layout = CreateSectionLayout();

        layout.Controls.Add(CreateFieldLabel("Age:"), 0, 0);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "> 19 years" }), 1, 0);

        layout.Controls.Add(CreateFieldLabel("Testes Size (ml):"), 0, 1);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "< 4 ml" }, new RadioButton { Text = "4-12 ml" }, new RadioButton { Text = "> 12 ml" }), 1, 1);

        layout.Controls.Add(CreateFieldLabel("Epididymis:"), 0, 2);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Thin" }, new RadioButton { Text = "Normal" }, new RadioButton { Text = "Dilated" }), 1, 2);

        layout.Controls.Add(CreateFieldLabel("Vas Deferens (Palpable):"), 0, 3);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Palpable" }, new RadioButton { Text = "Not Palpable" }), 1, 3);

        layout.Controls.Add(CreateFieldLabel("Secondary Sex Chars (SSC):"), 0, 4);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Normal" }, new RadioButton { Text = "Absent" }), 1, 4);

        proceedToHormonalButton = CreateProceedButton("Proceed to Section 2");
        proceedToHormonalButton.Click += (s, e) => { hormonalSection.Visible = true; };
        layout.Controls.Add(new Panel(), 0, 5); // Spacer
        layout.Controls.Add(proceedToHormonalButton, 1, 6);

        clinicalSection.Controls.Add(layout);
    }

    private void CreateHormonalSection2()
    {
        hormonalSection = CreateSectionBox("Section 2: Hormonal & Ejaculatory Evaluation");
        var layout = CreateSectionLayout();

        string[] tests = { "FSH", "LH", "Testosterone", "Prolactin", "Thyroid Hormones (TSH/T3/T4)", "Cortisol" };
        for (int i = 0; i < tests.Length; i++)
        {
            layout.Controls.Add(CreateFieldLabel(tests[i] + ":"), 0, i);
            layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Low" }, new RadioButton { Text = "Normal" }, new RadioButton { Text = "High" }), 1, i);
        }

        var ejaculationPresentRb = new RadioButton { Text = "Present" };
        var anejaculationRb = new RadioButton { Text = "Anejaculation" };
        ejaculationPresentRb.CheckedChanged += EjaculationStatus_Changed;
        anejaculationRb.CheckedChanged += EjaculationStatus_Changed;

        layout.Controls.Add(CreateFieldLabel("Ejaculation:"), 0, 6);
        layout.Controls.Add(CreateOptionsPanel(ejaculationPresentRb, anejaculationRb), 1, 6);

        CreateSemenAnalysisPanel();
        layout.Controls.Add(semenAnalysisPanel, 0, 7);
        layout.SetColumnSpan(semenAnalysisPanel, 2);

        proceedToHistoryButton = CreateProceedButton("Proceed to History");
        proceedToHistoryButton.Click += (s, e) => { historySection.Visible = true; };
        layout.Controls.Add(new Panel(), 0, 8); // Spacer
        layout.Controls.Add(proceedToHistoryButton, 1, 9);

        hormonalSection.Controls.Add(layout);
    }

    private void CreateSemenAnalysisPanel()
    {
        semenAnalysisPanel = new Panel { Dock = DockStyle.Fill, AutoSize = true, Visible = false, Padding = new Padding(0, 15, 0, 0) };
        var header = new Label { Text = "If Ejaculation is Present, perform Semen Analysis + Biochemistry:", Font = new Font("Segoe UI", 10F, FontStyle.Italic), AutoSize = true, Margin = new Padding(0, 0, 0, 10) };
        var semenLayout = CreateSectionLayout();

        // --- Basic Semen Analysis ---
        semenLayout.Controls.Add(CreateFieldLabel("Azoospermia:"), 0, 0);
        semenLayout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Present" }, new RadioButton { Text = "Absent" }), 1, 0);
        semenLayout.Controls.Add(CreateFieldLabel("Semen pH:"), 0, 1);
        semenLayout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "> 7" }, new RadioButton { Text = "< 7" }), 1, 1);
        semenLayout.Controls.Add(CreateFieldLabel("Semen Volume:"), 0, 2);
        semenLayout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "> 1 ml" }, new RadioButton { Text = "< 1 ml" }), 1, 2);
        semenLayout.Controls.Add(CreateFieldLabel("Fructose (Semen):"), 0, 3);
        semenLayout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Positive" }, new RadioButton { Text = "Negative" }), 1, 3);
        semenLayout.Controls.Add(CreateFieldLabel("Semen Color:"), 0, 4);
        semenLayout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Normal" }, new RadioButton { Text = "Yellowish" }, new RadioButton { Text = "Red" }), 1, 4);

        // --- Advanced Semen Analysis Considerations ---
        var advancedHeader = new Label { Text = "Advanced Semen Analysis:", Font = new Font("Segoe UI", 10F, FontStyle.Italic), AutoSize = true, Margin = new Padding(0, 15, 0, 5) };
        semenLayout.Controls.Add(advancedHeader, 0, 5);
        semenLayout.SetColumnSpan(advancedHeader, 2);

        semenLayout.Controls.Add(CreateFieldLabel("Sperm Motility:"), 0, 6);
        semenLayout.Controls.Add(CreateOptionsPanel(new CheckBox { Text = "Progressive" }, new CheckBox { Text = "Non-Progressive" }, new CheckBox { Text = "Immotile" }), 1, 6);
        semenLayout.Controls.Add(CreateFieldLabel("Sperm Morphology (Kruger):"), 0, 7);
        semenLayout.Controls.Add(new TextBox { Width = 200 }, 1, 7);
        semenLayout.Controls.Add(CreateFieldLabel("Sperm Concentration (millions/ml):"), 0, 8);
        semenLayout.Controls.Add(new TextBox { Width = 200 }, 1, 8);
        semenLayout.Controls.Add(CreateFieldLabel("Anti-sperm Antibodies:"), 0, 9);
        semenLayout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Present" }, new RadioButton { Text = "Absent" }), 1, 9);
        semenLayout.Controls.Add(CreateFieldLabel("Sperm DNA Fragmentation (DFI):"), 0, 10);
        semenLayout.Controls.Add(new TextBox { Width = 200 }, 1, 10);
        semenLayout.Controls.Add(CreateFieldLabel("Reactive Oxygen Species (ROS):"), 0, 11);
        semenLayout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Present" }, new RadioButton { Text = "Absent" }), 1, 11);


        semenAnalysisPanel.Controls.Add(header);
        semenAnalysisPanel.Controls.Add(semenLayout);
        semenLayout.Location = new Point(0, header.Height);
    }

    private void CreateHistorySection()
    {
        historySection = CreateSectionBox("Additional Considerations: Clinical History / Background");
        var layout = CreateSectionLayout();
        int row = 0;

        layout.Controls.Add(CreateFieldLabel("Duration of Infertility:"), 0, row);
        layout.Controls.Add(new TextBox { Width = 200, PlaceholderText = ">1 year of unprotected intercourse" }, 1, row++);

        layout.Controls.Add(CreateFieldLabel("History of Mumps Orchitis:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Previous Paternity:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Sexual Dysfunction:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new CheckBox { Text = "ED" }, new CheckBox { Text = "Premature Ejaculation" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Libido Level:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Low" }, new RadioButton { Text = "Normal" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Inguinal or Scrotal Surgeries:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Occupational Exposure (chemicals, etc.):"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Smoking/Alcohol/Drug Use:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Use of Anabolic Steroids:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Use of Androgenic Meds/5a-reductase inhibitors:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Chemotherapy/Radiation History:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("STDs / Genital Infections:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Exposure to environmental estrogens:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);


        proceedToGeneticButton = CreateProceedButton("Proceed to Genetic");
        proceedToGeneticButton.Click += (s, e) => { geneticSection.Visible = true; };
        layout.Controls.Add(new Panel(), 0, row); // Spacer
        layout.Controls.Add(proceedToGeneticButton, 1, ++row);

        historySection.Controls.Add(layout);
    }

    private void CreateGeneticSection()
    {
        geneticSection = CreateSectionBox("Additional Considerations: Genetic / Congenital");
        var layout = CreateSectionLayout();
        int row = 0;

        layout.Controls.Add(CreateFieldLabel("Karyotype / Chromosomal Abnormalities:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Present" }, new RadioButton { Text = "Absent" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Y-chromosome Microdeletions (AZFa, AZFb, AZFc):"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Present" }, new RadioButton { Text = "Absent" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("CFTR Gene Mutation (esp. with CBAVD):"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Present" }, new RadioButton { Text = "Absent" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Cryptorchidism History (even if corrected):"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);

        proceedToSystemicButton = CreateProceedButton("Proceed to Systemic Health");
        proceedToSystemicButton.Click += (s, e) => { systemicHealthSection.Visible = true; };
        layout.Controls.Add(new Panel(), 0, row); // Spacer
        layout.Controls.Add(proceedToSystemicButton, 1, ++row);

        geneticSection.Controls.Add(layout);
    }

    private void CreateSystemicHealthSection()
    {
        systemicHealthSection = CreateSectionBox("Additional Considerations: Systemic Health Factors");
        var layout = CreateSectionLayout();
        int row = 0;

        layout.Controls.Add(CreateFieldLabel("Diabetes Mellitus:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Thyroid Disorders (clinical status):"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Cushing's Syndrome or Addison's Disease:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Pituitary Disorders / Mass:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Liver Cirrhosis / Chronic Illness:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Renal Failure:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Yes" }, new RadioButton { Text = "No" }), 1, row++);


        proceedToImagingButton = CreateProceedButton("Proceed to Section 3");
        proceedToImagingButton.Click += (s, e) => { imagingSection.Visible = true; };
        layout.Controls.Add(new Panel(), 0, row); // Spacer
        layout.Controls.Add(proceedToImagingButton, 1, ++row);


        systemicHealthSection.Controls.Add(layout);
    }

    private void CreateImagingSection3()
    {
        imagingSection = CreateSectionBox("Section 3: Imaging & Functional Tests");
        var layout = CreateSectionLayout();
        int row = 0;

        // --- Standard Imaging ---
        layout.Controls.Add(CreateFieldLabel("TRUS/MRI - Seminal Vesicle (Right):"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Atrophic" }, new RadioButton { Text = "Normal" }, new RadioButton { Text = "Absent" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("TRUS/MRI - Seminal Vesicle (Left):"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Atrophic" }, new RadioButton { Text = "Normal" }, new RadioButton { Text = "Absent" }), 1, row++);

        var prostateNormalRb = new RadioButton { Text = "Normal" };
        var prostateAbnormalRb = new RadioButton { Text = "Abnormal" };
        prostateNormalRb.CheckedChanged += ProstateStatus_Changed;
        prostateAbnormalRb.CheckedChanged += ProstateStatus_Changed;
        layout.Controls.Add(CreateFieldLabel("TRUS/MRI - Prostate:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(prostateNormalRb, prostateAbnormalRb), 1, row++);

        CreateProstateAbnormalPanel();
        layout.Controls.Add(prostateAbnormalPanel, 1, row++);

        layout.Controls.Add(CreateFieldLabel("TRUS/MRI - Vas Deferens:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Normal" }, new RadioButton { Text = "Absent" }, new RadioButton { Text = "Hypoplastic" }), 1, row++);

        // --- Other Imaging or Functional Tests ---
        var otherHeader = new Label { Text = "Other Imaging or Functional Tests:", Font = new Font("Segoe UI", 10F, FontStyle.Italic), AutoSize = true, Margin = new Padding(0, 15, 0, 5) };
        layout.Controls.Add(otherHeader, 0, row);
        layout.SetColumnSpan(otherHeader, 2);
        row++;

        layout.Controls.Add(CreateFieldLabel("Scrotal Ultrasound:"), 0, row);
        layout.Controls.Add(new TextBox { Width = 300, PlaceholderText = "e.g., varicocele, hydrocele, testicular mass" }, 1, row++);

        layout.Controls.Add(CreateFieldLabel("Testicular Biopsy:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Performed" }, new RadioButton { Text = "Not Performed" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("Post-Ejaculatory Urinalysis:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Performed" }, new RadioButton { Text = "Not Performed" }), 1, row++);

        layout.Controls.Add(CreateFieldLabel("MRI Brain/Pituitary:"), 0, row);
        layout.Controls.Add(CreateOptionsPanel(new RadioButton { Text = "Performed" }, new RadioButton { Text = "Not Performed" }), 1, row++);


        // --- Final Button ---
        finishButton = new Button { Text = "Save Diagnosis", Font = new Font("Segoe UI", 12F, FontStyle.Bold), Size = new Size(200, 50), BackColor = Color.FromArgb(56, 161, 105), ForeColor = Color.White, Anchor = AnchorStyles.Left };
        finishButton.Click += (s, e) => { MessageBox.Show("Patient diagnostic data saved (simulation)."); };
        layout.Controls.Add(new Panel(), 0, row); // Spacer
        layout.Controls.Add(finishButton, 1, ++row);

        imagingSection.Controls.Add(layout);
    }

    private void CreateProstateAbnormalPanel()
    {
        prostateAbnormalPanel = new Panel { Dock = DockStyle.Fill, AutoSize = true, Visible = false, Padding = new Padding(0, 5, 0, 0) };
        var header = new Label { Text = "If Prostate = Abnormal, specify:", Font = new Font("Segoe UI", 10F, FontStyle.Italic), AutoSize = true, Margin = new Padding(0, 0, 0, 5) };
        var flow = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true };
        flow.Controls.Add(new CheckBox { Text = "Prostatic Cyst" });
        flow.Controls.Add(new CheckBox { Text = "Abnormal Mass" });
        var otherFlow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
        otherFlow.Controls.Add(new CheckBox { Text = "Other:", AutoSize = true });
        otherFlow.Controls.Add(new TextBox { Width = 200 });
        flow.Controls.Add(otherFlow);

        prostateAbnormalPanel.Controls.Add(header);
        prostateAbnormalPanel.Controls.Add(flow);
        flow.Location = new Point(20, header.Height); // Indent for clarity
    }

    // --- Event Handlers ---

    private void EjaculationStatus_Changed(object sender, EventArgs e)
    {
        if (sender is RadioButton rb && rb.Checked)
        {
            semenAnalysisPanel.Visible = (rb.Text == "Present");
        }
    }

    private void ProstateStatus_Changed(object sender, EventArgs e)
    {
        if (sender is RadioButton rb && rb.Checked)
        {
            prostateAbnormalPanel.Visible = (rb.Text == "Abnormal");
        }
    }
}