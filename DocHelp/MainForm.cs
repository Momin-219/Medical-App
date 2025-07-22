using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class MainForm : Form
{
    private readonly int currentDoctorId;
    private readonly Form welcomeForm;

    private Panel sidebarPanel;
    private Panel contentPanel;
    private Panel separatorPanel;
    private Panel navButtonsPanel;

    // --- Button Declarations ---
    private Button menuToggleButton;
    private Button dashboardButton;
    private Button profileButton;
    private Button addPatientButton;
    private Button recordsButton;
    private Button signoutButton;
    private Label menuLabel;

    // --- Aesthetic Fields ---
    private Button activeButton;
    private readonly Color sidebarColor = Color.FromArgb(26, 32, 44);
    private readonly Color hoverColor = Color.FromArgb(45, 55, 72);
    private readonly Color activeColor = Color.FromArgb(66, 153, 225);

    // --- State and Animation Fields ---
    private System.Windows.Forms.Timer animationTimer;
    private bool isSidebarExpanded = false;
    private const int sidebarExpandedWidth = 260;
    private const int sidebarCollapsedWidth = 75;

    // --- Icon Strings ---
    private const string openMenuIcon = "\u25B6";
    private const string closeMenuIcon = "\u25C0";
    private const string homeIcon = "\uE10F";
    private const string profileIcon = "\uE13D";
    private const string addPatientIcon = "\uE109";
    private const string recordsIcon = "\uE160";
    private const string signOutIcon = "\uE182";

    public MainForm(int doctorId, Form welcome)
    {
        this.currentDoctorId = doctorId;
        this.welcomeForm = welcome;
        InitializeComponents();

        // --- Set the initial startup page ---
        ActivateButton(dashboardButton);
        NavigateTo(new DashboardHomeControl());
    }

    private void InitializeComponents()
    {
        this.Text = "Doctor Dashboard";
        this.FormBorderStyle = FormBorderStyle.None;
        this.WindowState = FormWindowState.Maximized;
        this.BackColor = Color.White;
        this.Font = new Font("Segoe UI", 10F);

        animationTimer = new System.Windows.Forms.Timer { Interval = 15 };
        animationTimer.Tick += AnimationTimer_Tick;

        sidebarPanel = new Panel
        {
            Dock = DockStyle.Left,
            Width = sidebarCollapsedWidth,
            BackColor = sidebarColor,
        };

        menuToggleButton = new Button
        {
            Text = openMenuIcon,
            Font = new Font("Segoe UI Symbol", 16F),
            ForeColor = Color.White,
            BackColor = sidebarColor,
            Dock = DockStyle.Top,
            Height = 60,
            FlatStyle = FlatStyle.Flat,
            TextAlign = ContentAlignment.MiddleCenter,
        };
        menuToggleButton.FlatAppearance.BorderSize = 0;
        menuToggleButton.Click += (s, e) => { isSidebarExpanded = !isSidebarExpanded; animationTimer.Start(); };

        menuLabel = new Label
        {
            Text = "MENU",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Top,
            Height = 40,
            TextAlign = ContentAlignment.MiddleCenter,
            Visible = false,
        };

        signoutButton = CreateNavButton(signOutIcon + "  Sign Out", -2);
        signoutButton.Dock = DockStyle.Bottom;
        signoutButton.BackColor = Color.FromArgb(40, 40, 40);
        signoutButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 30, 30);

        navButtonsPanel = new Panel { Dock = DockStyle.Fill };
        navButtonsPanel.Resize += (s, e) => PositionNavButtons();

        // Create all navigation buttons here for consistent handling.
        dashboardButton = CreateNavButton(homeIcon + "  Dashboard", 3);
        profileButton = CreateNavButton(profileIcon + "  Profile", 0);
        addPatientButton = CreateNavButton(addPatientIcon + "  Add Patient", 1);
        recordsButton = CreateNavButton(recordsIcon + "  Records", 2);

        // Add buttons to the panel in the desired visual order.
        navButtonsPanel.Controls.Add(dashboardButton);
        navButtonsPanel.Controls.Add(profileButton);
        navButtonsPanel.Controls.Add(addPatientButton);
        navButtonsPanel.Controls.Add(recordsButton);

        sidebarPanel.Controls.Add(navButtonsPanel);
        sidebarPanel.Controls.Add(signoutButton);
        sidebarPanel.Controls.Add(menuLabel);
        sidebarPanel.Controls.Add(menuToggleButton);

        contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(245, 249, 252),
            Padding = new Padding(20)
        };

        separatorPanel = new Panel
        {
            Width = 1,
            Dock = DockStyle.Left,
            BackColor = Color.LightGray,
        };

        this.Controls.Add(contentPanel);
        this.Controls.Add(separatorPanel);
        this.Controls.Add(sidebarPanel);

        // Position and set the initial UI state for all buttons.
        PositionNavButtons();
        UpdateButtonsUI();
    }

    private void PositionNavButtons()
    {
        int buttonCount = navButtonsPanel.Controls.Count;
        int buttonHeight = 55;
        int totalHeight = buttonCount * buttonHeight;
        int startY = (navButtonsPanel.Height - totalHeight) / 2;

        for (int i = 0; i < buttonCount; i++)
        {
            if (navButtonsPanel.Controls[i] is Button button)
            {
                button.Location = new Point(0, startY + (i * buttonHeight));
                button.Width = sidebarPanel.Width;
            }
        }
    }

    private Button CreateNavButton(string text, int index)
    {
        var button = new Button
        {
            Text = text,
            Tag = index,
            Font = new Font("Segoe UI Symbol", 12F),
            ForeColor = Color.FromArgb(226, 232, 240),
            BackColor = sidebarColor,
            Size = new Size(sidebarExpandedWidth, 55),
            FlatStyle = FlatStyle.Flat,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(22, 0, 0, 0),
        };
        button.FlatAppearance.BorderSize = 0;
        button.Click += NavButton_Click;
        button.MouseEnter += (s, e) => { if (s != activeButton) ((Button)s).BackColor = hoverColor; };
        button.MouseLeave += (s, e) => { if (s != activeButton) ((Button)s).BackColor = sidebarColor; };
        return button;
    }

    private void ActivateButton(Button button)
    {
        if (activeButton != null)
        {
            activeButton.BackColor = sidebarColor;
        }
        activeButton = button;
        activeButton.BackColor = activeColor;
    }

    private void AnimationTimer_Tick(object sender, EventArgs e)
    {
        int step = 25;
        if (isSidebarExpanded)
        {
            sidebarPanel.Width += step;
            if (sidebarPanel.Width >= sidebarExpandedWidth)
            {
                sidebarPanel.Width = sidebarExpandedWidth;
                animationTimer.Stop();
                UpdateButtonsUI();
            }
        }
        else
        {
            sidebarPanel.Width -= step;
            if (sidebarPanel.Width <= sidebarCollapsedWidth)
            {
                sidebarPanel.Width = sidebarCollapsedWidth;
                animationTimer.Stop();
                UpdateButtonsUI();
            }
        }
        PositionNavButtons();
    }

    private void UpdateButtonsUI()
    {
        menuToggleButton.Text = isSidebarExpanded ? closeMenuIcon : openMenuIcon;
        menuLabel.Visible = isSidebarExpanded;

        // Update all button texts based on the sidebar state.
        dashboardButton.Text = isSidebarExpanded ? homeIcon + "  Dashboard" : homeIcon;
        profileButton.Text = isSidebarExpanded ? profileIcon + "  Profile" : profileIcon;
        addPatientButton.Text = isSidebarExpanded ? addPatientIcon + "  Add Patient" : addPatientIcon;
        recordsButton.Text = isSidebarExpanded ? recordsIcon + "  Records" : recordsIcon;
        signoutButton.Text = isSidebarExpanded ? signOutIcon + "  Sign Out" : signOutIcon;

        ContentAlignment alignment = isSidebarExpanded ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
        foreach (Control c in sidebarPanel.Controls) if (c is Button b) b.TextAlign = alignment;
    }

    private void NavButton_Click(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            // Don't mark the signout button as "active" in the same way.
            if (Convert.ToInt32(button.Tag) != -2)
            {
                ActivateButton(button);
            }

            int index = Convert.ToInt32(button.Tag);

            switch (index)
            {
                case 0: NavigateTo(new ProfileControl(currentDoctorId)); break;
                // --- THIS IS THE FIXED LINE ---
                case 1: NavigateTo(new AddPatientControl()); break;
                // -----------------------------
                case 2: MessageBox.Show("Records functionality will be implemented later."); break;
                case 3: NavigateTo(new DashboardHomeControl()); break;
                case -2: SignoutButton_Click(); break;
            }
        }
    }

    private void SignoutButton_Click() { welcomeForm.Show(); this.Close(); }

    public void NavigateTo(UserControl page)
    {
        ShowPage(page);
    }

    private void ShowPage(UserControl page)
    {
        contentPanel.Controls.Clear();
        page.Dock = DockStyle.Fill;
        contentPanel.Controls.Add(page);
    }
}