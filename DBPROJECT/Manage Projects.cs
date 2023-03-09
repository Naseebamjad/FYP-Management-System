using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MaterialSkin.Controls;
using MaterialSkin;
using System.Windows.Forms;
using DBPROJECT;
using System.Text.RegularExpressions;
using System.Activities.Expressions;

namespace Modern_Dashboard_Design
{
    public partial class Manage_Projects : Form
    {
        private List<string> allNames = new List<string>();




        public Manage_Projects()
        {
            InitializeComponent();

        }


        private void AddProject_Click(object sender, EventArgs e)
        {
            if (TitleTextField.Text != "" && richTextBox.Text != "" && genderComboBox.SelectedItem != null && dateTimePicker1.Value != null && dateTimePicker1.Value != DateTime.MinValue && comboBox1.SelectedItem != null)
            {

                // Get advisor ID from selected name
                if (CheckProjectTitleExists(TitleTextField.Text))
                {
                    MessageBox.Show("Project Title already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string advisorName = genderComboBox.SelectedItem.ToString();
                int advisorId = -1;
                SqlConnection con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("SELECT p.Id FROM Person p INNER JOIN Advisor a ON p.Id = a.Id WHERE p.FirstName + ' ' + p.LastName = @AdvisorName", con);
                cmd.Parameters.AddWithValue("@AdvisorName", advisorName);


                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        advisorId = reader.GetInt32(0);
                        Console.WriteLine(advisorId);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    con.Close();
                    return;
                }
                finally
                {
                    con.Close();
                }

                // If advisor ID is still -1, no matching record was found
                if (advisorId == -1)
                {
                    MessageBox.Show("Selected advisor not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }


                int projectId = -1;
                cmd = new SqlCommand("INSERT INTO Project (Description, Title) OUTPUT INSERTED.Id VALUES (@Description, @Title)", con);
                cmd.Parameters.AddWithValue("@Description", richTextBox.Text);
                cmd.Parameters.AddWithValue("@Title", TitleTextField.Text);

                try
                {
                    con.Open();
                    projectId = (int)cmd.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    con.Close();
                    return;
                }
                finally
                {
                    con.Close();
                }

                // Get advisor role
                string advisorRole = comboBox1.SelectedItem.ToString();
                int advisorRoleId = -1;


                cmd = new SqlCommand("SELECT Id FROM Lookup WHERE Category = 'ADVISOR_ROLE' AND Value = @RoleValue", con);
                cmd.Parameters.AddWithValue("@RoleValue", advisorRole);

                try
                {
                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        advisorRoleId = reader.GetInt32(0);
                        Console.WriteLine("\n" + advisorRoleId);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    con.Close();
                    return;
                }
                finally
                {
                    con.Close();
                }

                // If advisor role ID is still -1, no matching record was found
                if (advisorRoleId == -1)
                {
                    MessageBox.Show("Selected advisor role not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Insert project advisor into database
                cmd = new SqlCommand("INSERT INTO ProjectAdvisor (AdvisorId, ProjectId, AdvisorRole, AssignmentDate) VALUES (@AdvisorId, @ProjectId, @AdvisorRole, @AssigmentDate)", con);
                cmd.Parameters.AddWithValue("@AdvisorId", advisorId);
                cmd.Parameters.AddWithValue("@ProjectId", projectId);
                cmd.Parameters.AddWithValue("@AdvisorRole", advisorRoleId);
                cmd.Parameters.AddWithValue("@AssigmentDate", dateTimePicker1.Value);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("New Project Has Been Added", "Manage Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TitleTextField.Text = "";
                    genderComboBox.DataSource = null;
                    richTextBox.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    con.Close();
                }


            }
            else
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetAdvisorRole(int designationId)
        {
            SqlConnection con = Configuration.getInstance().getConnection();
            string role = "";

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT Value FROM Lookup WHERE Category = 'Advisor Role' AND Id = (SELECT RoleId FROM Designation WHERE Id = @DesignationId)", con);
                cmd.Parameters.AddWithValue("@DesignationId", designationId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    role = reader.GetString(0);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }

            return role;
        }






        private void TitleTextField_Click(object sender, EventArgs e)
        {


        }


        private bool CheckProjectTitleExists(string title)
        {
            bool exists = false;
            SqlConnection con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Project WHERE Title = @Title", con);
            cmd.Parameters.AddWithValue("@Title", title);

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            try
            {
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    exists = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }

            return exists;
        }

        private void Manage_Projects_Load(object sender, EventArgs e)
        {
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            // Connect to database
            dateTimePicker1.ShowUpDown = true;


            SqlConnection con = Configuration.getInstance().getConnection();

            // Query for advisors
            string query = "SELECT Person.FirstName + ' ' + Person.LastName AS Name " +
                           "FROM Advisor " +
                           "INNER JOIN Person ON Advisor.Id = Person.Id";
            SqlCommand command = new SqlCommand(query, con);

            // Load names into a list
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            try
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    allNames.Add(reader["Name"].ToString());
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }

            // Set combobox datasource to empty list
            genderComboBox.DataSource = new List<string>();

            // Set Advisor Role ComboBox datasource
            List<string> roles = new List<string>();
            query = "SELECT Value FROM Lookup WHERE Category = 'Advisor_Role'";
            command = new SqlCommand(query, con);

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            try
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    roles.Add(reader["Value"].ToString());
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                con.Close();
            }

            comboBox1.DataSource = roles;

            // Set the format of the DateTimePicker
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";

            // Set the font of the DateTimePicker
            dateTimePicker1.Font = new Font("Arial", 12);

            // Set the foreground and background colors of the DateTimePicker
            dateTimePicker1.ForeColor = Color.FromArgb(46, 53, 73);
            dateTimePicker1.BackColor = Color.FromArgb(255, 255, 255);
        }

        private void Update_Click(object sender, EventArgs e)
        {

            if (TitleTextField.Text != "" && richTextBox.Text != "" && genderComboBox.SelectedItem != null && dateTimePicker1.Value != null && dateTimePicker1.Value != DateTime.MinValue && comboBox1.SelectedItem != null)
            {
                int projectId = -1;

                // Get project ID
                SqlConnection con = Configuration.getInstance().getConnection();

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("SELECT Id FROM Project WHERE Title = @Title", con);
                cmd.Parameters.AddWithValue("@Title", TitleTextField.Text);

                try
                {

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        projectId = reader.GetInt32(0);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }
                finally
                {
                   
                }

                // If project ID is still -1, no matching record was found
                if (projectId == -1)
                {
                    MessageBox.Show("Selected project not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get advisor ID from selected name
                string advisorName = genderComboBox.SelectedItem.ToString();
                int advisorId = -1;
                cmd = new SqlCommand("SELECT p.Id FROM Person p INNER JOIN Advisor a ON p.Id = a.Id WHERE p.FirstName + ' ' + p.LastName = @AdvisorName", con);
                cmd.Parameters.AddWithValue("@AdvisorName", advisorName);

                try
                {

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        advisorId = reader.GetInt32(0);
                        Console.WriteLine(advisorId);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                   
                    return;
                }
                finally
                {

                }

                // If advisor ID is still -1, no matching record was found
                if (advisorId == -1)
                {
                    MessageBox.Show("Selected advisor not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update project description and title
                cmd = new SqlCommand("UPDATE Project SET Description = @Description, Title = @Title WHERE Id = @Id", con);

                cmd.Parameters.AddWithValue("@Id", projectId);
                cmd.Parameters.AddWithValue("@Description", richTextBox.Text);
                cmd.Parameters.AddWithValue("@Title", TitleTextField.Text);
                try
                {

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    return;
                }
                finally
                {

                }
                // Get advisor role
                string advisorRole = comboBox1.SelectedItem.ToString();
                int advisorRoleId = -1;


                cmd = new SqlCommand("SELECT Id FROM Lookup WHERE Category = 'ADVISOR_ROLE' AND Value = @RoleValue", con);
                cmd.Parameters.AddWithValue("@RoleValue", advisorRole);

                try
                {
                    
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        advisorRoleId = reader.GetInt32(0);
                        Console.WriteLine("\n" + advisorRoleId);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    return;
                }
                finally
                {
                    
                }


                // Update project advisor information
                cmd = new SqlCommand("UPDATE ProjectAdvisor SET AdvisorId = @AdvisorId, AdvisorRole = @AdvisorRole, AssignmentDate = @AssigmentDate WHERE ProjectId = @ProjectId", con);
                cmd.Parameters.AddWithValue("@AdvisorId", advisorId);
                cmd.Parameters.AddWithValue("@ProjectId", projectId);
                cmd.Parameters.AddWithValue("@AdvisorRole", advisorRoleId);
                cmd.Parameters.AddWithValue("@AssigmentDate", dateTimePicker1.Value);

                try
                {

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Project Has Been Updated", "Manage Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    TitleTextField.Text = "";
                    genderComboBox.DataSource = null;
                    richTextBox.Text = "";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    
                }
            }
            else
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
            
          
        
                private void materialSingleLineTextField1_TextChanged(object sender, EventArgs e)
        {
            // Get search text
            string searchText = materialSingleLineTextField1.Text.Trim();

            // Filter names based on search text
            List<string> filteredNames = allNames.Where(name => name.StartsWith(searchText, StringComparison.OrdinalIgnoreCase)).ToList();

            // Display filtered names in the combo box
            genderComboBox.DataSource = filteredNames;

            // Connect to database
            SqlConnection con = Configuration.getInstance().getConnection();

            // Query for projects based on filtered title
            string query = "SELECT  Project.Title, Project.Description, Person.FirstName + ' ' + Person.LastName AS AdvisorName, Lookup.Value AS AdvisorRole, ProjectAdvisor.AssignmentDate " +
               "FROM Project " +
               "INNER JOIN ProjectAdvisor ON Project.Id = ProjectAdvisor.ProjectId " +
               "INNER JOIN Advisor ON ProjectAdvisor.AdvisorId = Advisor.Id " +
               "INNER JOIN Person ON Advisor.Id = Person.Id " +
               "INNER JOIN Lookup ON ProjectAdvisor.AdvisorRole = Lookup.Id " +
               "WHERE CONCAT(Person.FirstName, ' ', Person.LastName) LIKE @AdvisorName";


            SqlCommand command = new SqlCommand(query, con);
            command.Parameters.AddWithValue("@AdvisorName", "%" + searchText + "%");


            // Load projects into a data table
            DataTable dataTable = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable);


            // Clear the data grid view
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.DataSource = dataTable;
            if (dataTable.Rows.Count > 0)
            {
                dataGridView1.DataSource = dataTable;

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.AllowUserToDeleteRows = false;
                dataGridView1.AllowUserToOrderColumns = false;
                dataGridView1.AllowUserToResizeRows = false;
                dataGridView1.ReadOnly = true;
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.MultiSelect = false;
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 53, 73);
                dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
                dataGridView1.RowsDefaultCellStyle.BackColor = Color.White;
                dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(237, 243, 255);
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(46, 53, 73);
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9, FontStyle.Bold);

                DataGridViewButtonColumn editButton = new DataGridViewButtonColumn();
                editButton.HeaderText = "Edit";
                editButton.Text = "Edit";
                editButton.UseColumnTextForButtonValue = true;
                editButton.DefaultCellStyle.BackColor = Color.FromArgb(72, 123, 237);
                editButton.DefaultCellStyle.ForeColor = Color.White;
                editButton.DefaultCellStyle.Font = new Font("Tahoma", 9, FontStyle.Bold);
                dataGridView1.Columns.Add(editButton);
                dataGridView1.CellClick += dataGridView1_CellClick;
            }

        }


        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_Click(object sender, EventArgs e)
        {

        }

        private void materialFlatButton2_Click(object sender, EventArgs e)
        {
            TitleTextField.Text = "";
            TitleTextField.Enabled = true;
            genderComboBox.DataSource = null;
            richTextBox.Text = "";

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void TitleTextField_TextChanged(object sender, EventArgs e)
        {
            string title = TitleTextField.Text;

            SqlConnection con = Configuration.getInstance().getConnection();

            string query = "SELECT p.Title AS 'Project Title', p.Description,CONCAT(person.FirstName, ' ', person.LastName) AS 'Advisor Name', lookup.Value AS 'Advisor Role', pa.AssignmentDate " +
               "FROM Project p " +
               "INNER JOIN ProjectAdvisor pa ON p.Id = pa.ProjectId " +
               "INNER JOIN Advisor a ON pa.AdvisorId = a.Id " +
               "INNER JOIN Person person ON a.Id = person.Id " +
               "INNER JOIN Lookup lookup ON pa.AdvisorRole = lookup.Id " +
               "WHERE p.Title LIKE '%' + @Title + '%'";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Title", title);
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            try
            {

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                // Clear the data grid view
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.DataSource = dt;
                if (dt.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dt;

                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.AllowUserToDeleteRows = false;
                    dataGridView1.AllowUserToOrderColumns = false;
                    dataGridView1.AllowUserToResizeRows = false;
                    dataGridView1.ReadOnly = true;
                    dataGridView1.RowHeadersVisible = false;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.MultiSelect = false;
                    dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 53, 73);
                    dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
                    dataGridView1.RowsDefaultCellStyle.BackColor = Color.White;
                    dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(237, 243, 255);
                    dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(46, 53, 73);
                    dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9, FontStyle.Bold);

                    DataGridViewButtonColumn editButton = new DataGridViewButtonColumn();
                    editButton.HeaderText = "Edit";
                    editButton.Text = "Edit";
                    editButton.UseColumnTextForButtonValue = true;
                    editButton.DefaultCellStyle.BackColor = Color.FromArgb(72, 123, 237);
                    editButton.DefaultCellStyle.ForeColor = Color.White;
                    editButton.DefaultCellStyle.Font = new Font("Tahoma", 9, FontStyle.Bold);
                    dataGridView1.Columns.Add(editButton);
                    dataGridView1.CellClick += dataGridView1_CellClick;
                }
                else
                {
                   // MessageBox.Show("NOT FOUND ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

            }
        }

        private void materialSingleLineTextField1_Click(object sender, EventArgs e)
        {

        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            // Get the selected advisor role
            string advisorRole = comboBox1.SelectedItem.ToString();
            int advisorRoleId = -1;

            // Get the advisor role ID from the lookup table
            SqlConnection con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("SELECT Id FROM Lookup WHERE Category = 'ADVISOR_ROLE' AND Value = @RoleValue", con);
            cmd.Parameters.AddWithValue("@RoleValue", advisorRole);

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    advisorRoleId = reader.GetInt32(0);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                con.Close();
                return;
            }
            finally
            {
                con.Close();
            }

            // If advisor role ID is still -1, no matching record was found
            if (advisorRoleId == -1)
            {
                MessageBox.Show("Selected advisor role not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the projects with the selected advisor role
            cmd = new SqlCommand("SELECT Project.Title,Project.Description, CONCAT(Person.FirstName, ' ', Person.LastName) AS AdvisorName, Lookup.Value AS AdvisorRole, ProjectAdvisor.AssignmentDate " +
                       "FROM Project " +
                       "INNER JOIN ProjectAdvisor ON Project.Id = ProjectAdvisor.ProjectId " +
                       "INNER JOIN Advisor ON ProjectAdvisor.AdvisorId = Advisor.Id " +
                       "INNER JOIN Person ON Advisor.Id = Person.Id " +
                       "INNER JOIN Lookup ON ProjectAdvisor.AdvisorRole = Lookup.Id " +
                       "WHERE Lookup.Id = @AdvisorRoleId", con);





            cmd.Parameters.AddWithValue("@AdvisorRoleId", advisorRoleId);

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                // Clear the data grid view
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();

                if (dt.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dt;

                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dataGridView1.AllowUserToAddRows = false;
                    dataGridView1.AllowUserToDeleteRows = false;
                    dataGridView1.AllowUserToOrderColumns = false;
                    dataGridView1.AllowUserToResizeRows = false;
                    dataGridView1.ReadOnly = true;
                    dataGridView1.RowHeadersVisible = false;
                    dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dataGridView1.MultiSelect = false;
                    dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(46, 53, 73);
                    dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
                    dataGridView1.RowsDefaultCellStyle.BackColor = Color.White;
                    dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(237, 243, 255);
                    dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(46, 53, 73);
                    dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9, FontStyle.Bold);

                    DataGridViewButtonColumn editButton = new DataGridViewButtonColumn();
                    editButton.HeaderText = "Edit";
                    editButton.Text = "Edit";
                    editButton.UseColumnTextForButtonValue = true;
                    editButton.DefaultCellStyle.BackColor = Color.FromArgb(72, 123, 237);
                    editButton.DefaultCellStyle.ForeColor = Color.White;
                    editButton.DefaultCellStyle.Font = new Font("Tahoma", 9, FontStyle.Bold);
                    dataGridView1.Columns.Add(editButton);
                    dataGridView1.CellClick += dataGridView1_CellClick;
                }
                else
                {
                    MessageBox.Show("NOT FOUND ", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

            }
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Check if the click is on the 5th column
                if (e.ColumnIndex == 5)
                {
                    // Get the selected row
                    DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                    // Disable the title text field
                    TitleTextField.Enabled = false;

                    // Set the text fields with the values from the row
                    TitleTextField.Text = row.Cells[0].Value.ToString();
                    // richTextBox.Text = row.Cells[1].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void materialSingleLineTextField1_Enter(object sender, EventArgs e)
        {
            // Get advisor ID from selected name
            string advisorName = "";
            if (genderComboBox.SelectedItem != null)
            {
                 advisorName = genderComboBox.SelectedItem.ToString();
                
                // continue with your code
            }
            else
            {
                // handle the case where no item is selected in the combobox
               // MessageBox.Show("Please select a ADVISOR");
                return;
            }

            int advisorId = -1;
            SqlConnection con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("SELECT p.Id FROM Person p INNER JOIN Advisor a ON p.Id = a.Id WHERE p.FirstName + ' ' + p.LastName = @AdvisorName", con);
            cmd.Parameters.AddWithValue("@AdvisorName", advisorName);

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    advisorId = reader.GetInt32(0);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                con.Close();
                return;
            }
            finally
            {

            }

            // If advisor ID is still -1, no matching record was found
            if (advisorId == -1)
            {
                MessageBox.Show("Selected advisor not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get advisor role
            string advisorRole = comboBox1.SelectedItem.ToString();
            int advisorRoleId = -1;

            cmd = new SqlCommand("SELECT Id FROM Lookup WHERE Category = 'ADVISOR_ROLE' AND Value = @RoleValue", con);
            cmd.Parameters.AddWithValue("@RoleValue", advisorRole);

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    advisorRoleId = reader.GetInt32(0);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            finally
            {

            }

            // If advisor role ID is still -1, no matching record was found
            if (advisorRoleId == -1)
            {
                MessageBox.Show("Selected advisor role not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update project in database
            cmd = new SqlCommand("UPDATE Project SET Description = @Description WHERE Title = @Title", con);
            cmd.Parameters.AddWithValue("@Description", richTextBox.Text);
            cmd.Parameters.AddWithValue("@Title", TitleTextField.Text);

            SqlCommand cmd2 = new SqlCommand("SELECT p.Id FROM Person p INNER JOIN Advisor a ON p.Id = a.Id WHERE p.FirstName + ' ' + p.LastName = @AdvisorName", con);
            cmd2.Parameters.AddWithValue("@AdvisorName", advisorName);
            SqlDataReader reader2 = cmd2.ExecuteReader();
            if (reader2.Read())
            {
                advisorId = reader2.GetInt32(0);
            }
            reader2.Close();

            // If advisor ID is still -1, no matching record was found
            if (advisorId == -1)
            {
                MessageBox.Show("Selected advisor not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get advisor role
            SqlCommand cmd3 = new SqlCommand("SELECT Id FROM Lookup WHERE Category = 'ADVISOR_ROLE' AND Value = @RoleValue", con);
            cmd3.Parameters.AddWithValue("@RoleValue", advisorRole);
            SqlDataReader reader3 = cmd3.ExecuteReader();
            if (reader3.Read())
            {
                advisorRoleId = reader3.GetInt32(0);
            }
            reader3.Close();

            // If advisor role ID is still -1, no matching record was found
            if (advisorRoleId == -1)
            {
                MessageBox.Show("Selected advisor role not found.", "Error", MessageBoxButtons.OK);


        }


        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            Form Manage_Projects = new Form1();
            this.Hide();
            Manage_Projects.Show();
        }

        private void btnAnalytics_Click(object sender, EventArgs e)
        {
            Form Manage_Projects = new Manage_Projects();
            this.Hide();
            Manage_Projects.Show();
        }

        private void btnCalander_Click(object sender, EventArgs e)
        {
            Form Manage_Projects = new Groups();
            this.Hide();
            Manage_Projects.Show();
        }

        private void btnContactUs_Click(object sender, EventArgs e)
        {
            Form Manage_Projects = new ProjectAssign();
            this.Hide();
            Manage_Projects.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form Manage_Projects = new ManageEvaluation();
            this.Hide();
            Manage_Projects.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form Manage_Projects = new Advisor();
            this.Hide();
            Manage_Projects.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form Manage_Projects = new EvaluationMarking();
            this.Hide();
            Manage_Projects.Show();
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
    }
}
