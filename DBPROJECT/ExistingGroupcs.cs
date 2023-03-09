using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using DBPROJECT;
using System.Linq;
using System.Text;
using MaterialSkin.Controls;
using MaterialSkin;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Modern_Dashboard_Design
{
    public partial class ExistingGroupcs : Form
    {
        public ExistingGroupcs()
        {
            InitializeComponent();
        }

        private void ExistingGroupcs_Load(object sender, EventArgs e)
        {

        }

        private void StudentTextField_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Get the entered registration number
                string regNo = StudentTextField.Text.Trim();

                // Construct the query to fetch student and group information
                string query = @"
    DECLARE @regNoFilter varchar(50)
    SET @regNoFilter = @regNo + '%'

    SELECT 
        
        s.RegistrationNo, 
        p.FirstName, 
        p.LastName, 
        g.ID AS GroupID, 
        g.Created_On, 
        (SELECT value From Lookup Where Category = 'Status' and id = gs.Status) AS Status,
        gs.AssignmentDate
    FROM 
        Student s
        LEFT JOIN GroupStudent gs ON s.ID = gs.StudentID
        LEFT JOIN [Group] g ON g.ID = gs.GroupID
        LEFT JOIN Person p ON s.ID = p.ID
    WHERE 
        s.RegistrationNo LIKE @regNoFilter
    ORDER BY 
        s.RegistrationNo ASC,
        g.Created_On ASC;
";


                // Create a DataTable to hold the result set
                DataTable dt = new DataTable();
                SqlConnection con = Configuration.getInstance().getConnection();

                // Open the database connection
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                // Create a SqlCommand and set its parameters
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@regNo", regNo + "%");

                // Create a SqlDataAdapter to fill the DataTable
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

                // Bind the DataTable to the DataGridView
                // Clear the data grid view
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.DataSource = dt;


               
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


              



                // Close the database connection
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void StudentTextField_Click(object sender, EventArgs e)
        {

        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (!materialRadioButton1.Checked && !materialRadioButton2.Checked)
            {
                MessageBox.Show("Please select a group status.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (StudentTextField.Text == "")
            {
                MessageBox.Show("Please select a student.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (GroupID.Text == "")
            {
                MessageBox.Show("Please select a group.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the group exists
            int groupId = int.Parse(GroupID.Text);
            string checkGroupQuery = $"SELECT COUNT(*) FROM [Group] WHERE ID = {groupId};";
            int groupCount = 0;

            SqlConnection con = Configuration.getInstance().getConnection();

            // Open the database connection
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            SqlCommand command = new SqlCommand(checkGroupQuery, con);

            try
            {
                
                groupCount = (int)command.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error checking group: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                
            }

            if (groupCount == 0)
            {
                MessageBox.Show("Group does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the student exists
            string studentRegNo = StudentTextField.Text;
            string checkStudentQuery = $"SELECT COUNT(*) FROM Student WHERE RegistrationNo = '{studentRegNo}';";
            int studentCount = 0;

           
            command = new SqlCommand(checkStudentQuery, con);

            try
            {
               
                studentCount = (int)command.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error checking student: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                
            }

            if (studentCount == 0)
            {
                MessageBox.Show("Student does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Check if the student is already in a group
            string checkGroupStudentQuery = $"SELECT COUNT(*) FROM GroupStudent WHERE StudentID = (SELECT ID FROM Student WHERE RegistrationNo = '{studentRegNo}') AND GroupID <> {groupId} AND Status = '3';";
            int groupStudentCount = 0;

 
 command = new SqlCommand(checkGroupStudentQuery, con);
            
try
{
   
    groupStudentCount = (int)command.ExecuteScalar();
}
catch (SqlException ex)
{
    MessageBox.Show("Error checking group student: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    return;
}
finally
{
    
    
}

if (groupStudentCount > 0)
{
    MessageBox.Show("Student is already in another group.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    return;
}

// Add the student to the group
string addStudentToGroupQuery = $"INSERT INTO GroupStudent (StudentID, GroupID, Status, AssignmentDate) VALUES ((SELECT ID FROM Student WHERE RegistrationNo = '{studentRegNo}'), {groupId}, {(materialRadioButton1.Checked ? 3 : 4)}, GETDATE());";


command = new SqlCommand(addStudentToGroupQuery, con);
            
try
{
    command.ExecuteNonQuery();
}
catch (SqlException ex)
{
    MessageBox.Show("Error adding student to group: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    return;
}
finally
{
    
}

MessageBox.Show("Student added to the group.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

           
        
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            if (GroupID.Text == "")
            {
                MessageBox.Show("Please select a group.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Check if either MaterialRadioButton2 or MaterialRadioButton3 is checked
            if (!materialRadioButton1.Checked && !materialRadioButton2.Checked)
            {
                MessageBox.Show("Please select a group status.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the group exists
            int groupId = int.Parse(GroupID.Text);
            string checkGroupQuery = $"SELECT COUNT(*) FROM [Group] WHERE ID = {groupId};";
            int groupCount = 0;


            SqlConnection con = Configuration.getInstance().getConnection();

            // Open the database connection
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            SqlCommand command = new SqlCommand(checkGroupQuery, con);

            try
            {
                groupCount = (int)command.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error checking group: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (groupCount == 0)
            {
                MessageBox.Show("Group does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the student exists
            string studentRegNo = StudentTextField.Text;
            string checkStudentQuery = $"SELECT COUNT(*) FROM Student WHERE RegistrationNo = '{studentRegNo}';";
            int studentCount = 0;

            command = new SqlCommand(checkStudentQuery, con);

            try
            {
                studentCount = (int)command.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error checking student: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (studentCount == 0)
            {
                MessageBox.Show("Student does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the student is already in the group
            string checkGroupStudentQuery = $"SELECT COUNT(*) FROM GroupStudent WHERE StudentID = (SELECT ID FROM Student WHERE RegistrationNo = '{studentRegNo}') AND GroupID <> {groupId} AND Status = '3';";
            int groupStudentCount = 0;

            command = new SqlCommand(checkGroupStudentQuery, con);

            try
            {
                groupStudentCount = (int)command.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error checking group student: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (groupStudentCount > 0)
            {
                MessageBox.Show("Student is already in another group.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update the student's status in the group
            string updateGroupStudentQuery = $"UPDATE GroupStudent SET Status = {(materialRadioButton1.Checked ? 3 : 4)} WHERE StudentID = (SELECT ID FROM Student WHERE RegistrationNo = '{studentRegNo}') AND GroupID = {groupId};";

            command = new SqlCommand(updateGroupStudentQuery, con);

            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Student status updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Student is not a member of the group.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error updating student status: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            con.Close();
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
    }
}
