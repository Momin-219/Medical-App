using System;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class ProfileControl : UserControl
{
    private readonly int doctorId;
    private PictureBox profilePictureBox;
    private TextBox nameTextBox;
    private TextBox specialtyTextBox;
    private TextBox phoneTextBox;
    private Button saveButton;
    private Button uploadButton;

    public ProfileControl(int currentDoctorId)
    {
        this.doctorId = currentDoctorId;
        InitializeComponents();
        LoadDoctorProfile();
    }

    private void InitializeComponents()
    {
        this.Dock = DockStyle.Fill;
        this.BackColor = Color.FromArgb(245, 249, 252);
        this.Font = new Font("Segoe UI", 10F);

        // Main Title
        var titleLabel = new Label
        {
            Text = "Your Profile",
            Font = new Font("Segoe UI", 26F, FontStyle.Bold),
            ForeColor = Color.FromArgb(45, 55, 72),
            Location = new Point(40, 30),
            AutoSize = true
        };

        // --- Details Card ---
        var detailsPanel = new Panel
        {
            Location = new Point(40, 100),
            Size = new Size(800, 400),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
        };

        // Profile PictureBox
        profilePictureBox = new PictureBox
        {
            Size = new Size(150, 150),
            Location = new Point(40, 40),
            BorderStyle = BorderStyle.FixedSingle,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.FromArgb(226, 232, 240)
        };

        uploadButton = new Button
        {
            Text = "Upload Picture",
            Location = new Point(40, 200),
            Size = new Size(150, 40),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            BackColor = Color.FromArgb(237, 242, 247),
            ForeColor = Color.FromArgb(45, 55, 72),
            FlatStyle = FlatStyle.Flat
        };
        uploadButton.FlatAppearance.BorderSize = 0;
        uploadButton.Click += UploadButton_Click;

        // Input Fields
        nameTextBox = new TextBox { Font = new Font("Segoe UI", 12F), Location = new Point(240, 65), Width = 400 };
        specialtyTextBox = new TextBox { Font = new Font("Segoe UI", 12F), Location = new Point(240, 145), Width = 400 };
        phoneTextBox = new TextBox { Font = new Font("Segoe UI", 12F), Location = new Point(240, 225), Width = 400 };

        // Save Button
        saveButton = new Button
        {
            Text = "Save Changes",
            Location = new Point(240, 300),
            Size = new Size(400, 45),
            Font = new Font("Segoe UI", 12F, FontStyle.Bold),
            BackColor = Color.FromArgb(56, 161, 105),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
        };
        saveButton.FlatAppearance.BorderSize = 0;
        saveButton.Click += SaveButton_Click;

        detailsPanel.Controls.Add(profilePictureBox);
        detailsPanel.Controls.Add(uploadButton);
        detailsPanel.Controls.Add(CreateLabel("Full Name", new Point(240, 40)));
        detailsPanel.Controls.Add(nameTextBox);
        detailsPanel.Controls.Add(CreateLabel("Medical Specialty", new Point(240, 120)));
        detailsPanel.Controls.Add(specialtyTextBox);
        detailsPanel.Controls.Add(CreateLabel("Phone Number", new Point(240, 200)));
        detailsPanel.Controls.Add(phoneTextBox);
        detailsPanel.Controls.Add(saveButton);

        this.Controls.Add(titleLabel);
        this.Controls.Add(detailsPanel);
    }

    private Label CreateLabel(string text, Point location) => new Label
    {
        Text = text,
        Location = location,
        AutoSize = true,
        Font = new Font("Segoe UI", 10F, FontStyle.Bold),
        ForeColor = Color.FromArgb(113, 128, 150)
    };

    private void LoadDoctorProfile()
    {
        using (var connection = new SQLiteConnection("Data Source=doctors_app.sqlite;Version=3;"))
        {
            connection.Open();
            string query = "SELECT FullName, Specialty, PhoneNumber, ProfilePicture FROM Doctors WHERE Id = @Id";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Id", doctorId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        nameTextBox.Text = reader["FullName"]?.ToString();
                        specialtyTextBox.Text = reader["Specialty"]?.ToString();
                        phoneTextBox.Text = reader["PhoneNumber"]?.ToString();
                        if (reader["ProfilePicture"] != DBNull.Value)
                        {
                            byte[] imageData = (byte[])reader["ProfilePicture"];
                            using (var ms = new MemoryStream(imageData)) { profilePictureBox.Image = Image.FromStream(ms); }
                        }
                    }
                }
            }
        }
    }

    private void UploadButton_Click(object sender, EventArgs e)
    {
        using (var ofd = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif" })
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                profilePictureBox.Image = new Bitmap(ofd.FileName);
            }
        }
    }

    private void SaveButton_Click(object sender, EventArgs e)
    {
        using (var connection = new SQLiteConnection("Data Source=doctors_app.sqlite;Version=3;"))
        {
            connection.Open();
            string query = "UPDATE Doctors SET FullName = @FullName, Specialty = @Specialty, PhoneNumber = @PhoneNumber, ProfilePicture = @ProfilePicture WHERE Id = @Id";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FullName", nameTextBox.Text);
                command.Parameters.AddWithValue("@Specialty", specialtyTextBox.Text);
                command.Parameters.AddWithValue("@PhoneNumber", phoneTextBox.Text);
                command.Parameters.AddWithValue("@Id", doctorId);

                if (profilePictureBox.Image != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        profilePictureBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        command.Parameters.AddWithValue("@ProfilePicture", ms.ToArray());
                    }
                }
                else
                {
                    command.Parameters.AddWithValue("@ProfilePicture", DBNull.Value);
                }

                command.ExecuteNonQuery();
                MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}