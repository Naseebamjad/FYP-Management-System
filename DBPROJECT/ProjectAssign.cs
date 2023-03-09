using DBPROJECT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using MaterialSkin.Controls;
using MaterialSkin;

using System.Threading.Tasks;
using System.Windows.Forms;

namespace Modern_Dashboard_Design
{
    public partial class ProjectAssign : Form
    {
        public ProjectAssign()
        {
            InitializeComponent();
        }

        private void RegNoTextField_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a search option.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string searchText = RegNoTextField.Text;
            string searchBy = comboBox1.SelectedItem.ToString();

            // Check if search text is empty
            if (searchText.Length == 0)
            {
                dataGridView1.DataSource = null;
                return;
            }

            // Construct the SQL query based on the selected search option
            string query = "";
            if (searchBy == "ID")
            {
                query = @"
        SELECT 
            
            g.Created_On, 
            (SELECT value From Lookup Where Category = 'Status' and id = gs.Status) AS Status, 
            gs.AssignmentDate, 
            
            s.RegistrationNo, 
            p.FirstName, 
            p.LastName,
            gp.ProjectId,
            pr.Title AS ProjectTitle
        FROM 
            [Group] g
            LEFT JOIN GroupStudent gs ON g.ID = gs.GroupID
            LEFT JOIN Student s ON gs.StudentID = s.ID
            LEFT JOIN Person p ON s.ID = p.ID
            LEFT JOIN GroupProject gp ON g.ID = gp.GroupId
            LEFT JOIN Project pr ON gp.ProjectId = pr.Id
        WHERE 
            g.ID = @SearchText;";
            }
            else if (searchBy == "REG NO")
            {
                // Show group student data for the specified student registration number
                query = @"
        SELECT 
             
            g.Created_On, 
               (SELECT value From Lookup Where Category = 'Status' and id = gs.Status) AS Status,
            gs.AssignmentDate, 
            
            s.RegistrationNo, 
            p.FirstName, 
            p.LastName,
            gp.ProjectId,
            pr.Title AS ProjectTitle
        FROM 
            [Group] g
            LEFT JOIN GroupStudent gs ON g.ID = gs.GroupID
            LEFT JOIN Student s ON gs.StudentID = s.ID
            LEFT JOIN Person p ON s.ID = p.ID
            LEFT JOIN GroupProject gp ON g.ID = gp.GroupId
            LEFT JOIN Project pr ON gp.ProjectId = pr.Id
        WHERE 
            s.RegistrationNo LIKE @SearchTextFilter
        ORDER BY 
            g.ID ASC, 
            s.RegistrationNo ASC;";
                searchText = searchText + "%"; // Add the wildcard character to search for partial matches
            }

            // Execute the query and populate the data grid view with the results
            try
            {
                SqlConnection con = Configuration.getInstance().getConnection();
                SqlCommand command = new SqlCommand(query, con);
                if (searchBy == "ID")
                {
                    command.Parameters.AddWithValue("@SearchText", searchText);
                }
                else if (searchBy == "REG NO")
                {
                    command.Parameters.AddWithValue("@SearchTextFilter", searchText);
                }
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                // Clear the data grid view
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.DataSource = dataTable;


              //  dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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


            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error executing query: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ProjectAssign_Load(object sender, EventArgs e)
        {
          
        }

        private void ProjectTexTField_Click(object sender, EventArgs e)
        {
            
        }

        private void ProjectTexTField_TextChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please select a search option.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string searchText = ProjectTexTField.Text;
            string searchBy = comboBox2.SelectedItem.ToString();
            

           

            // Check if search text is empty
            if (searchText.Length == 0)
            {
                dataGridView1.DataSource = null;
                return;
            }

            // Construct the SQL query based on the selected search option
            string query = "";
            if (searchBy == "ID")
            {
                query = @"
            SELECT 
               
                p.Title AS ProjectTitle,
                p.Description,
                ga.AssignmentDate AS ProjectCreation,
               
                per.FirstName + ' ' + per.LastName AS AdvisorName
            FROM 
                Project p
                LEFT JOIN GroupProject gp ON p.Id = gp.ProjectID
                LEFT JOIN [Group] g ON gp.GroupID = g.ID
                LEFT JOIN ProjectAdvisor ga ON p.ID = ga.ProjectId
                LEFT JOIN Advisor a ON ga.AdvisorId = a.Id
                LEFT JOIN Person per ON a.Id = per.Id
            WHERE 
                p.Id = @SearchText;";
            }
            else if (searchBy == "NAME")
            {
                query = @"
            SELECT 
               
                p.Title AS ProjectTitle,
                p.Description,
               
                ga.AssignmentDate AS ProjectCreation,
                per.FirstName + ' ' + per.LastName AS AdvisorName
            FROM 
                Project p
                LEFT JOIN GroupProject gp ON p.Id = gp.ProjectID
                LEFT JOIN [Group] g ON gp.GroupID = g.ID
                LEFT JOIN  ProjectAdvisor ga ON p.ID = ga.ProjectId
                LEFT JOIN Advisor a ON ga.AdvisorId = a.Id
                LEFT JOIN Person per ON a.Id = per.Id
            WHERE 
                p.Title LIKE @SearchTextFilter
            ORDER BY 
                p.Title ASC;";
                searchText = "%" + searchText + "%"; // Add the wildcard character to search for partial matches
            }
            try
            {
                SqlConnection con = Configuration.getInstance().getConnection();
                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@SearchText", searchText);
                if (searchBy == "NAME")
                {
                    command.Parameters.AddWithValue("@SearchTextFilter", searchText);
                }
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Clear the data grid view

                // Bind the data table to the data grid view
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


               

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Assigned_Click(object sender, EventArgs e)
        {
            string registrationNo = RegNoTextField.Text;
            string projectTitle = ProjectTexTField.Text;

            // Check if the registration number and project title are not empty
            if (registrationNo.Length == 0 || projectTitle.Length == 0)
            {
                MessageBox.Show("Please enter a valid registration number and project title.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                SqlConnection con = Configuration.getInstance().getConnection();
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand command = new SqlCommand();
                command.Connection = con;

                // Check if the registration number exists in the Student table
                command.CommandText = @"
            SELECT COUNT(*) 
            FROM Student 
            WHERE RegistrationNo = @RegistrationNo;
        ";
                command.Parameters.AddWithValue("@RegistrationNo", registrationNo);
                int registrationCount = Convert.ToInt32(command.ExecuteScalar());

                if (registrationCount == 0)
                {
                    MessageBox.Show("The specified registration number does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get the student ID from the Student table
                command.CommandText = @"
            SELECT ID 
            FROM Student 
            WHERE RegistrationNo = @RegistrationNo;
        ";
                int studentId = Convert.ToInt32(command.ExecuteScalar());

                // Check if the student is part of a group in the GroupStudent table
                command.CommandText = @"
            SELECT COUNT(*) 
            FROM GroupStudent 
            WHERE StudentID = @StudentId;
        ";
                command.Parameters.AddWithValue("@StudentId", studentId);
                int groupStudentCount = Convert.ToInt32(command.ExecuteScalar());

                if (groupStudentCount == 0)
                {
                    MessageBox.Show("The specified student is not part of any group.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get the group ID from the GroupStudent table
                command.CommandText = @"
            SELECT GroupID 
            FROM GroupStudent 
            WHERE StudentID = @StudentId;
        ";
                int groupId = Convert.ToInt32(command.ExecuteScalar());

                // Check if the project title exists in the Project table
                command.CommandText = @"
            SELECT COUNT(*) 
            FROM Project 
            WHERE Title = @ProjectTitle;
        ";
                command.Parameters.AddWithValue("@ProjectTitle", projectTitle);
                int projectCount = Convert.ToInt32(command.ExecuteScalar());

                if (projectCount == 0)
                {
                    MessageBox.Show("The specified project title does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get the project ID from the Project table
                command.CommandText = @"
SELECT ID
FROM Project
WHERE Title = @ProjectTitle;
";
                int projectId = Convert.ToInt32(command.ExecuteScalar());
                // Check if the project is already assigned to another group
                command.CommandText = @"
        SELECT COUNT(*) 
        FROM GroupProject 
        WHERE ProjectID = @ProjectId;
    ";
                command.Parameters.AddWithValue("@ProjectId", projectId);
                int projectAssignedCount = Convert.ToInt32(command.ExecuteScalar());

                if (projectAssignedCount > 0)
                {
                    MessageBox.Show("The specified project is already assigned to another group.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Insert the new record into the GroupProject table
                command.CommandText = @"
        INSERT INTO GroupProject (GroupID, ProjectID, AssignmentDate)
        VALUES (@GroupId, @ProjectIdd, @AssignmentDate);
    ";
                command.Parameters.AddWithValue("@GroupId", groupId);
                command.Parameters.AddWithValue("@ProjectIdd", projectId);
                command.Parameters.AddWithValue("@AssignmentDate", DateTime.Now);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Project assigned to group successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error assigning project to group.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error executing SQL query: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


            private void RegNoTextField_Click(object sender, EventArgs e)
        {

        }

        private void Delete_Click(object sender, EventArgs e)
        {
            string deleteGroupId = RegNoTextField.Text;
            string deleteProjectId = ProjectTexTField.Text;


           
            // Check if group ID and project ID are not empty
            if (deleteGroupId.Length == 0 || deleteProjectId.Length == 0)
            {
                MessageBox.Show("Please enter a valid Group ID and Project ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if group ID and project ID are integers
            int deleteGroupInt, deleteProjectInt;
            if (!int.TryParse(deleteGroupId, out deleteGroupInt) || !int.TryParse(deleteProjectId, out deleteProjectInt))
            {
                MessageBox.Show("Please enter valid integer values for Group ID and Project ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                SqlConnection con = Configuration.getInstance().getConnection();
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand command = new SqlCommand();
                command.Connection = con;

                // Check if group ID and project ID exist in their respective tables
                command.CommandText = @"
            SELECT COUNT(*) 
            FROM [Group] 
            WHERE ID = @GroupId;
        ";
                command.Parameters.AddWithValue("@GroupId", deleteGroupInt);
                int groupCount = Convert.ToInt32(command.ExecuteScalar());

                command.CommandText = @"
            SELECT COUNT(*) 
            FROM Project 
            WHERE ID = @ProjectId;
        ";
                command.Parameters.AddWithValue("@ProjectId", deleteProjectInt);
                int projectCount = Convert.ToInt32(command.ExecuteScalar());

                if (groupCount == 0 || projectCount == 0)
                {
                    MessageBox.Show("Group or project does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check if group is assigned to the specified project
                command.CommandText = @"
            SELECT COUNT(*) 
            FROM GroupProject 
            WHERE GroupID = @GroupId AND ProjectID = @ProjectId;
        ";
                int groupProjectCount = Convert.ToInt32(command.ExecuteScalar());

                if (groupProjectCount == 0)
                {
                    MessageBox.Show("Group is not assigned to the specified project.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Delete the record from the GroupProject table
                command.CommandText = @"
            DELETE FROM GroupProject 
            WHERE GroupID = @GroupId AND ProjectID = @ProjectId;
        ";
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Project deleted from group successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error deleting project from group.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error executing SQL query: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void button4_Click(object sender, EventArgs e)
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

        private void button2_Click(object sender, EventArgs e)
        {
            Form Manage_Projects = new EvaluationMarking();
            this.Hide();
            Manage_Projects.Show();
        }
    }
}
