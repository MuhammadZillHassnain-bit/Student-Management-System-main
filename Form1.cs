using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;

namespace databasepratice1
{
    public partial class Form1 : Form
    {
        string cs = ConfigurationManager.ConnectionStrings["dbcs"].ConnectionString;
        int selectedStudentId = 0;

        public Form1()
        {
            InitializeComponent();
        }

        // ===== FORM LOAD =====
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadData();
            dgvStudents.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        // ===== LOAD DATA IN DATAGRIDVIEW =====
        void LoadData()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT ID, FullName, Email, Contact, Course, DOB FROM dbo.Students ORDER BY ID DESC", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvStudents.DataSource = dt;

                    // Hide ID column but keep it for operations
                    if (dgvStudents.Columns["ID"] != null)
                    {
                        dgvStudents.Columns["ID"].Visible = false;
                    }

                    // Format column headers
                    dgvStudents.Columns["FullName"].HeaderText = "Full Name";
                    dgvStudents.Columns["Email"].HeaderText = "Email Address";
                    dgvStudents.Columns["Contact"].HeaderText = "Contact Number";
                    dgvStudents.Columns["Course"].HeaderText = "Course";
                    dgvStudents.Columns["DOB"].HeaderText = "Date of Birth";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Loading Data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== SAVE BUTTON =====
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(cmbCourse.Text))
            {
                MessageBox.Show("Please fill all required fields!", "⚠️ Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Email validation
            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
            {
                MessageBox.Show("Please enter a valid email address!", "⚠️ Invalid Email",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO dbo.Students (FullName, Email, Contact, Course, DOB) " +
                        "VALUES (@name, @email, @contact, @course, @dob)", con);

                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@contact", string.IsNullOrWhiteSpace(txtContact.Text) ? "" : txtContact.Text.Trim());
                    cmd.Parameters.AddWithValue("@course", cmbCourse.Text);
                    cmd.Parameters.AddWithValue("@dob", dtpDOB.Value.Date);

                    con.Open();
                    int result = cmd.ExecuteNonQuery();
                    con.Close();

                    if (result > 0)
                    {
                        MessageBox.Show("✅ Student saved successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadData();
                        ClearFields();
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                if (sqlEx.Number == 2627) // Unique constraint violation
                {
                    MessageBox.Show("This email already exists in the database!", "Duplicate Email",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Database Error: " + sqlEx.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Saving Data: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== UPDATE BUTTON =====
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedStudentId == 0)
            {
                MessageBox.Show("Please select a student from the grid to update!", "⚠️ No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(cmbCourse.Text))
            {
                MessageBox.Show("Please fill all required fields!", "⚠️ Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Email validation
            if (!txtEmail.Text.Contains("@") || !txtEmail.Text.Contains("."))
            {
                MessageBox.Show("Please enter a valid email address!", "⚠️ Invalid Email",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to update this student's information?",
                "Confirm Update",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        SqlCommand cmd = new SqlCommand(
                            "UPDATE dbo.Students SET FullName=@name, Email=@email, Contact=@contact, " +
                            "Course=@course, DOB=@dob WHERE ID=@id", con);

                        cmd.Parameters.AddWithValue("@id", selectedStudentId);
                        cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                        cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@contact", string.IsNullOrWhiteSpace(txtContact.Text) ? "" : txtContact.Text.Trim());
                        cmd.Parameters.AddWithValue("@course", cmbCourse.Text);
                        cmd.Parameters.AddWithValue("@dob", dtpDOB.Value.Date);

                        con.Open();
                        int updateResult = cmd.ExecuteNonQuery();
                        con.Close();

                        if (updateResult > 0)
                        {
                            MessageBox.Show("✅ Student information updated successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadData();
                            ClearFields();
                            selectedStudentId = 0;
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 2627)
                    {
                        MessageBox.Show("This email already exists in the database!", "Duplicate Email",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show("Database Error: " + sqlEx.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Updating Data: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ===== DELETE BUTTON =====
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedStudentId == 0)
            {
                MessageBox.Show("Please select a student from the grid to delete!", "⚠️ No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this student?\nThis action cannot be undone!",
                "⚠️ Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(cs))
                    {
                        SqlCommand cmd = new SqlCommand("DELETE FROM dbo.Students WHERE ID=@id", con);
                        cmd.Parameters.AddWithValue("@id", selectedStudentId);

                        con.Open();
                        int deleteResult = cmd.ExecuteNonQuery();
                        con.Close();

                        if (deleteResult > 0)
                        {
                            MessageBox.Show("✅ Student deleted successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadData();
                            ClearFields();
                            selectedStudentId = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error Deleting Data: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ===== CLEAR BUTTON =====
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
            selectedStudentId = 0;
        }

        // ===== CLEAR FUNCTION =====
        void ClearFields()
        {
            txtName.Clear();
            txtEmail.Clear();
            txtContact.Clear();
            cmbCourse.SelectedIndex = -1;
            dtpDOB.Value = DateTime.Now;
            txtName.Focus();
        }

        // ===== SEARCH FUNCTIONALITY =====
        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    string searchQuery = @"SELECT ID, FullName, Email, Contact, Course, DOB 
                                          FROM dbo.Students 
                                          WHERE FullName LIKE @search 
                                          OR Email LIKE @search 
                                          OR Contact LIKE @search 
                                          OR Course LIKE @search
                                          ORDER BY ID DESC";

                    SqlDataAdapter da = new SqlDataAdapter(searchQuery, con);
                    da.SelectCommand.Parameters.AddWithValue("@search", "%" + txtSearch.Text.Trim() + "%");

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvStudents.DataSource = dt;

                    // Hide ID column
                    if (dgvStudents.Columns["ID"] != null)
                    {
                        dgvStudents.Columns["ID"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Searching: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ===== DATAGRIDVIEW CELL CLICK - LOAD DATA TO FORM =====
        private void dgvStudents_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow row = dgvStudents.Rows[e.RowIndex];

                    selectedStudentId = Convert.ToInt32(row.Cells["ID"].Value);
                    txtName.Text = row.Cells["FullName"].Value.ToString();
                    txtEmail.Text = row.Cells["Email"].Value.ToString();
                    txtContact.Text = row.Cells["Contact"].Value.ToString();
                    cmbCourse.Text = row.Cells["Course"].Value.ToString();
                    dtpDOB.Value = Convert.ToDateTime(row.Cells["DOB"].Value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading student data: " + ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ===== EMPTY EVENT HANDLERS =====
        private void txtName_TextChanged(object sender, EventArgs e) { }
        private void txtEmail_TextChanged(object sender, EventArgs e) { }
        private void txtContact_TextChanged(object sender, EventArgs e) { }
        private void cmbCourse_SelectedIndexChanged(object sender, EventArgs e) { }
        private void dtpDOB_ValueChanged(object sender, EventArgs e) { }
        private void lblName_Click(object sender, EventArgs e) { }
        private void lblEmail_Click(object sender, EventArgs e) { }
        private void lblContact_Click(object sender, EventArgs e) { }
        private void lblCourse_Click(object sender, EventArgs e) { }
        private void lblDOB_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
    }
}