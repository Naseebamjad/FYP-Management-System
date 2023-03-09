using DBPROJECT;
using System;
using System.Activities.Expressions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialSkin.Controls;
using MaterialSkin;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Modern_Dashboard_Design
{
    public partial class GroupFormation : Form
    {
        public GroupFormation()
        {
            InitializeComponent();
           
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }
        




        private void GroupFormation_Load(object sender, EventArgs e)
        {
          


        }







        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Add_Click(object sender, EventArgs e)
        {
            // Check if either MaterialRadioButton2 or MaterialRadioButton3 is checked
            if (!materialRadioButton1.Checked && !materialRadioButton2.Checked)
            {
                MessageBox.Show("Please select a group status.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            // Get the student ID from the database using the registration number
            string registrationNumber = StudentTextField.Text;
            int studentID;
            SqlConnection con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("SELECT ID FROM Student WHERE RegistrationNo = @RegNo", con);


            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

                    cmd.Parameters.AddWithValue("@RegNo", registrationNumber);
                    object result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        MessageBox.Show("No student was found with the specified registration number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    studentID = (int)result;
                
            

            // Check if the student is already in a group
                     bool isInGroup;


            cmd = new SqlCommand("SELECT COUNT(*) FROM GroupStudent WHERE StudentID = @StudentID", con);
                
                    cmd.Parameters.AddWithValue("@StudentID", studentID);
                    int count = (int)cmd.ExecuteScalar();
                    isInGroup = count > 0;
                
            
            if (isInGroup)
            {
                MessageBox.Show("The student is already enrolled in another group.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

           
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            string status = materialRadioButton1.Checked ? "Active" : materialRadioButton2.Checked ? "Inactive" : null;

            int statusID = -1;
            MessageBox.Show(status, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Fetch the ID of the status from the lookup table
            string query = "SELECT ID FROM lookup WHERE Category = 'STATUS' AND Value = @Status";
            cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Status", status);
            object result2 = cmd.ExecuteScalar();
            if (result2 != null)
            {
                statusID = Convert.ToInt32(result);
                //MessageBox.Show(statusID, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            if (status == "Active")
              {
                statusID = 3;
            }
            else if ( status == "InActive")
            {
                statusID = 4;
            }
            // Insert the group and group-student records into the database
            int groupID;
            cmd = new SqlCommand("INSERT INTO [Group] (Created_On) VALUES (GETDATE()); SELECT SCOPE_IDENTITY();", con);
            groupID = Convert.ToInt32(cmd.ExecuteScalar());

            cmd = new SqlCommand("INSERT INTO GroupStudent (GroupID, StudentID, Status, AssignmentDate) VALUES (@GroupID, @StudentID, @StatusID, GETDATE())", con);
            cmd.Parameters.AddWithValue("@GroupID", groupID);
            cmd.Parameters.AddWithValue("@StudentID", studentID);
            if (statusID == -1)
            {
                cmd.Parameters.AddWithValue("@StatusID", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@StatusID", statusID);
            }
            cmd.ExecuteNonQuery();



            MessageBox.Show("Group created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
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
        s.ID AS StudentID, 
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

        private void label5_Click(object sender, EventArgs e)
        {

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
