using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace databasepratice1
{
    public partial class LoginForm : Form
    {
        string cs = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;

        public LoginForm()
        {
            InitializeComponent();
        }

        // ===== FORM LOAD =====
        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Set placeholder text
            txtUsername.Text = "Enter your username";
            txtUsername.ForeColor = Color.Gray;
            txtPassword.Text = "Enter your password";
            txtPassword.ForeColor = Color.Gray;
            txtPassword.UseSystemPasswordChar = false;

            // Add focus events for placeholder effect
            txtUsername.Enter += RemoveUsernamePlaceholder;
            txtUsername.Leave += SetUsernamePlaceholder;
            txtPassword.Enter += RemovePasswordPlaceholder;
            txtPassword.Leave += SetPasswordPlaceholder;

            // Set Enter key to trigger login
            this.AcceptButton = btnLogin;

            // Focus on username
            txtUsername.Focus();
        }

        // ===== PLACEHOLDER HANDLERS FOR USERNAME =====
        private void RemoveUsernamePlaceholder(object sender, EventArgs e)
        {
            if (txtUsername.Text == "Enter your username")
            {
                txtUsername.Text = "";
                txtUsername.ForeColor = Color.FromArgb(52, 73, 94);
            }
        }

        private void SetUsernamePlaceholder(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                txtUsername.Text = "Enter your username";
                txtUsername.ForeColor = Color.Gray;
            }
        }

        // ===== PLACEHOLDER HANDLERS FOR PASSWORD =====
        private void RemovePasswordPlaceholder(object sender, EventArgs e)
        {
            if (txtPassword.Text == "Enter your password")
            {
                txtPassword.Text = "";
                txtPassword.ForeColor = Color.FromArgb(52, 73, 94);
                txtPassword.UseSystemPasswordChar = true;
            }
        }

        private void SetPasswordPlaceholder(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                txtPassword.Text = "Enter your password";
                txtPassword.ForeColor = Color.Gray;
                txtPassword.UseSystemPasswordChar = false;
            }
        }

        // ===== USERNAME TEXTBOX TEXT CHANGED =====
        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            // You can add real-time validation here if needed
        }

        // ===== PASSWORD TEXTBOX TEXT CHANGED =====
        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            // You can add real-time validation here if needed
        }

        // ===== LOGIN BUTTON =====
        private void btnLogin_Click(object sender, EventArgs e)
        {
            // Check if fields are empty or have placeholder text
            if (txtUsername.Text == "Enter your username" || string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter your username!", "⚠️ Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (txtPassword.Text == "Enter your password" || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter your password!", "⚠️ Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    // Query to check user credentials and get user details
                    SqlCommand cmd = new SqlCommand(
                        "SELECT Username, FullName, Role FROM dbo.Users WHERE Username=@username AND Password=@password AND IsActive=1", con);

                    cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@password", txtPassword.Text.Trim());

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        // Get user details
                        string username = reader["Username"].ToString();
                        string fullName = reader["FullName"].ToString();
                        string role = reader["Role"].ToString();
                        reader.Close();
                        con.Close();

                        // Success message with user info
                        MessageBox.Show($"✅ Login Successful!\n\nWelcome {fullName}!\nRole: {role}",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Open main form and pass username
                        Form1 mainForm = new Form1();

                        // Handle Form1 closing event
                        mainForm.FormClosed += (s, args) =>
                        {
                            // When Form1 closes, show login form again
                            this.Show();
                            ClearFields();
                        };

                        this.Hide();
                        mainForm.Show();
                    }
                    else
                    {
                        reader.Close();
                        con.Close();

                        MessageBox.Show("❌ Invalid username or password!\nPlease try again.",
                            "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        // Clear password field
                        txtPassword.Text = "Enter your password";
                        txtPassword.ForeColor = Color.Gray;
                        txtPassword.UseSystemPasswordChar = false;
                        txtPassword.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n\nPlease check your database connection.",
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== CLEAR BUTTON =====
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        // ===== CLEAR FIELDS METHOD =====
        private void ClearFields()
        {
            // Reset to placeholder text
            txtUsername.Text = "Enter your username";
            txtUsername.ForeColor = Color.Gray;
            txtPassword.Text = "Enter your password";
            txtPassword.ForeColor = Color.Gray;
            txtPassword.UseSystemPasswordChar = false;
            chkShowPassword.Checked = false;
            txtUsername.Focus();
        }

        // ===== SHOW PASSWORD CHECKBOX =====
        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (txtPassword.Text != "Enter your password" && !string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
            }
        }

        // ===== FORGOT PASSWORD LINK =====
        private void lblForgotPassword_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Please contact your administrator to reset your password.\n\n" +
                "📧 Email: admin@sms.com\n📞 Phone: +92-XXX-XXXXXXX\n\n" +
                "Administrator will verify your identity and reset your password.",
                "Password Recovery", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ===== WELCOME LABEL CLICK (OPTIONAL) =====
        private void lblWelcome_Click(object sender, EventArgs e)
        {
            // Optional: Easter egg
            MessageBox.Show("🎓 Student Management System\nVersion 1.0\nPremium Edition",
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ===== SUBTITLE LABEL CLICK (OPTIONAL) =====
        private void lblSubtitle_Click(object sender, EventArgs e)
        {
            // Optional: Version info
            MessageBox.Show("Developed by: Your Name\n© 2025 All Rights Reserved",
                "System Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ===== LOGIN LABEL CLICK (OPTIONAL) =====
        private void lblLogin_Click(object sender, EventArgs e)
        {
            // Optional: Help info
            MessageBox.Show("Need help?\n\n" +
                "1. Enter your username\n" +
                "2. Enter your password\n" +
                "3. Click Login button\n\n" +
                "Default credentials:\nUsername: admin\nPassword: admin123",
                "Login Help", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        // ===== FORM CLOSING EVENT =====
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Confirm exit
            DialogResult result = MessageBox.Show(
                "Are you sure you want to exit?",
                "Confirm Exit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true; // Cancel the close operation
            }
            else
            {
                Application.Exit(); // Exit entire application
            }
        }
    }
}