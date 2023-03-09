using DBPROJECT;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Modern_Dashboard_Design
{
    public partial class EvaluationMarking : Form
    {
        public EvaluationMarking()
        {
            InitializeComponent();

            // Set the format of the DateTimePicker
            EvaluationDateTimePicker.ShowUpDown = true;
            EvaluationDateTimePicker.Format = DateTimePickerFormat.Custom;
            EvaluationDateTimePicker.CustomFormat = "dd/MM/yyyy";

            // Set the font of the DateTimePicker
            EvaluationDateTimePicker.Font = new Font("Arial", 12);

            // Set the foreground and background colors of the DateTimePicker
            EvaluationDateTimePicker.ForeColor = Color.FromArgb(46, 53, 73);
            EvaluationDateTimePicker.BackColor = Color.FromArgb(255, 255, 255);
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void SearchCriteria_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a search option.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string searchText = SearchCriteria.Text;
            string searchBy = comboBox1.SelectedItem.ToString();

            // Check if search text is empty
            if (searchText.Length == 0)
            {
                dataGridView1.DataSource = null;
                return;
            }

            // Construct the SQL query based on the selected search option
            string query = "";
            if (searchBy == "Reg No")
            {
                query = @"
SELECT

g.Created_On AS GroupCreationDate,

p.Title AS ProjectTitle,
p.Description AS ProjectDescription,
e.Id AS EvaluationID,
e.Name As EvaluationName,
e.TotalMarks as TotalMarks,
e.TotalWeightage as TotalWeight,
ge.ObtainedMarks,
ge.EvaluationDate,
s.RegistrationNo AS StudentRegistrationNo,
CONCAT(pers.FirstName, ' ', pers.LastName) AS StudentName
FROM
[Group] g
LEFT JOIN GroupProject gp ON g.ID = gp.GroupID
LEFT JOIN Project p ON gp.ProjectID = p.Id
LEFT JOIN GroupEvaluation ge ON g.ID = ge.GroupID
LEFT JOIN Evaluation e ON ge.EvaluationID = e.Id
LEFT JOIN GroupStudent gs ON g.ID = gs.GroupId
LEFT JOIN Student s ON gs.StudentId = s.Id
LEFT JOIN Person pers ON s.Id = pers.Id
WHERE
s.RegistrationNo = @SearchText AND gs.Status = '3';";
            }
            else if (searchBy == "Evaluation Name")
            {
                query = @"
SELECT
g.Created_On AS GroupCreationDate,
p.Title AS ProjectTitle,
p.Description AS ProjectDescription,
e.Id AS EvaluationID,
e.Name As EvaluationName,
e.TotalMarks as TotalMarks,
e.TotalWeightage as TotalWeight,
ge.ObtainedMarks,
ge.EvaluationDate,
s.RegistrationNo AS StudentRegistrationNo,
CONCAT(pers.FirstName, ' ', pers.LastName) AS StudentName
FROM
Evaluation e
LEFT JOIN GroupEvaluation ge ON e.Id = ge.EvaluationID
LEFT JOIN [Group] g ON ge.GroupID = g.ID
LEFT JOIN GroupProject gp ON g.ID = gp.GroupID
LEFT JOIN Project p ON gp.ProjectID = p.Id
LEFT JOIN GroupStudent gs ON g.ID = gs.GroupId
LEFT JOIN Student s ON gs.StudentId = s.Id
LEFT JOIN Person pers ON s.Id = pers.Id
WHERE
e.Name = @SearchText AND gs.Status = '3';";
            }
            // Execute the query and populate the data grid view with the results
            try
            {
                SqlConnection con = Configuration.getInstance().getConnection();
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@SearchText", searchText);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Bind the data table to the data grid view
                dataGridView1.DataSource = dataTable;

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

        private void ObtainedMarksTextField_Click(object sender, EventArgs e)
        {

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Assigned_Click(object sender, EventArgs e)
        {
            // Check if all input values are filled
            if (StudentRegNoTextField.Text == "" || EvaluationNameTextField.Text == "" || ObtainedMarksTextField.Text == "" || EvaluationDateTimePicker.Value == null)
            {
                MessageBox.Show("Please fill all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string studentRegNo = StudentRegNoTextField.Text;
            string evaluationName = EvaluationNameTextField.Text;
            int obtainedMarks = int.Parse(ObtainedMarksTextField.Text);

            // Check if student Reg No and evaluation name are valid
            bool studentExists = false;
            bool evaluationExists = false;
            int groupId = 0;
            int evaluationId = 0;
            try
            {
                SqlConnection con = Configuration.getInstance().getConnection();
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand command = new SqlCommand("SELECT * FROM Student WHERE RegistrationNo=@StudentRegNo;", con);
                command.Parameters.AddWithValue("@StudentRegNo", studentRegNo);
                SqlDataReader reader = command.ExecuteReader();
                studentExists = reader.HasRows;
                if (studentExists)
                {
                    reader.Read();
                    int studentI = reader.GetInt32(0);
                    reader.Close();

                    command = new SqlCommand("SELECT GroupId FROM GroupStudent WHERE StudentId=@StudentId AND Status='3';", con);
                    command.Parameters.AddWithValue("@StudentId", studentI);
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        groupId = reader.GetInt32(0);
                        reader.Close();
                    }
                    else
                    {
                        reader.Close();
                        MessageBox.Show("Student is not a member of any active group.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                reader.Close();

                command = new SqlCommand("SELECT * FROM Evaluation WHERE Name=@EvaluationName;", con);
                command.Parameters.AddWithValue("@EvaluationName", evaluationName);
                reader = command.ExecuteReader();
                evaluationExists = reader.HasRows;
                if (evaluationExists)
                {
                    reader.Read();
                    evaluationId = reader.GetInt32(0);
                }
                reader.Close();

                command = new SqlCommand("SELECT * FROM GroupEvaluation WHERE GroupID=@GroupId AND EvaluationID=@EvaluationId", con);
                command.Parameters.AddWithValue("@GroupId", groupId);
                command.Parameters.AddWithValue("@EvaluationId", evaluationId);
                reader = command.ExecuteReader();
                bool groupEvaluated = reader.HasRows;
                reader.Close();

                if (groupEvaluated)
                {
                    MessageBox.Show("Group has already been evaluated for this evaluation.", "Evaluation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get the student ID
                command = new SqlCommand("SELECT ID FROM Student WHERE RegistrationNo=@StudentRegNo;", con);
                command.Parameters.AddWithValue("@StudentRegNo", studentRegNo);
                int studentId = (int)command.ExecuteScalar();

                // Get the group ID
                command = new SqlCommand("SELECT GroupId FROM GroupStudent WHERE StudentId=@StudentId AND Status='3';", con);
                command.Parameters.AddWithValue("@StudentId", studentId);
                int groupIdd = (int)command.ExecuteScalar();

                // Get the evaluation ID
                command = new SqlCommand("SELECT ID FROM Evaluation WHERE Name=@EvaluationName;", con);
                command.Parameters.AddWithValue("@EvaluationName", evaluationName);
                int evaluationIdd = (int)command.ExecuteScalar();

                

                command = new SqlCommand("SELECT * FROM GroupProject WHERE GroupID=@GroupId", con);
                command.Parameters.AddWithValue("@GroupId", groupIdd);
                
                 reader = command.ExecuteReader();
                bool projectAssigned = reader.HasRows;
                reader.Close();

                if (!projectAssigned)
                {
                    MessageBox.Show("Project has not been assigned to the group.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking if student or evaluation exists: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!studentExists)
            {
                MessageBox.Show("Student with registration number " + studentRegNo + " does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!evaluationExists)
            {
                MessageBox.Show("Evaluation with name " + evaluationName + " does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if obtained marks is valid
            try
            {
                SqlConnection con = Configuration.getInstance().getConnection();
                SqlCommand command = new SqlCommand("SELECT TotalMarks FROM Evaluation WHERE Id=@EvaluationId;", con);
                command.Parameters.AddWithValue("@EvaluationId", evaluationId);
                int totalMarks = (int)command.ExecuteScalar();
                if (obtainedMarks > totalMarks)
                {
                    MessageBox.Show("Obtained marks cannot be greater than total marks (" + totalMarks + ").", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking if obtained marks are valid: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Add the new GroupEvaluation row to the database
            try
            {
                SqlConnection con = Configuration.getInstance().getConnection();
                SqlCommand command = new SqlCommand("INSERT INTO GroupEvaluation (GroupID, EvaluationID, ObtainedMarks, EvaluationDate) VALUES (@GroupId, @EvaluationId, @ObtainedMarks, @EvaluationDate);", con);
                command.Parameters.AddWithValue("@GroupId", groupId);
                command.Parameters.AddWithValue("@EvaluationId", evaluationId);
                command.Parameters.AddWithValue("@ObtainedMarks", obtainedMarks);
                command.Parameters.AddWithValue("@EvaluationDate", EvaluationDateTimePicker.Value);
                command.ExecuteNonQuery();

                MessageBox.Show("Evaluation added to group successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding evaluation to group: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        ///
        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            StudentRegNoTextField.Text = "";
            ObtainedMarksTextField.Text = "";
            EvaluationNameTextField.Text = "";
        }

        private void EditEvaluation_Click(object sender, EventArgs e)
        { 
            SqlConnection con = Configuration.getInstance().getConnection();
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            SqlDataReader reader = null;

            try
            {

                MessageBox.Show("Remeber that this will only work when already group has been evaluated", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string studentRegNo = StudentRegNoTextField.Text;
                string evaluationName = EvaluationNameTextField.Text;
                int obtainedMarks = int.Parse(ObtainedMarksTextField.Text);
                string evaluationDate = EvaluationDateTimePicker.Value.ToString("yyyy-MM-dd HH:mm:ss");

                if (StudentRegNoTextField.Text == "" || EvaluationNameTextField.Text == "" || ObtainedMarksTextField.Text == "" || EvaluationDateTimePicker.Value == null)
                {
                    MessageBox.Show("Please fill all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                SqlCommand command = new SqlCommand("SELECT ID FROM Student WHERE RegistrationNo=@StudentRegNo;", con);
                command.Parameters.AddWithValue("@StudentRegNo", studentRegNo);
                int studentId = (int)command.ExecuteScalar();

                // Get the group ID
                command = new SqlCommand("SELECT GroupId FROM GroupStudent WHERE StudentId=@StudentId AND Status='3';", con);
                command.Parameters.AddWithValue("@StudentId", studentId);
                int groupId = (int)command.ExecuteScalar();

                // Get the evaluation ID
                command = new SqlCommand("SELECT ID FROM Evaluation WHERE Name=@EvaluationName;", con);
                command.Parameters.AddWithValue("@EvaluationName", evaluationName);
                int evaluationId = (int)command.ExecuteScalar();

                // Check if the group has been evaluated for the given evaluation
                command = new SqlCommand("SELECT * FROM GroupEvaluation WHERE GroupID=@GroupId AND EvaluationID=@EvaluationId;", con);
                command.Parameters.AddWithValue("@GroupId", groupId);
                command.Parameters.AddWithValue("@EvaluationId", evaluationId);
                reader = command.ExecuteReader();
                bool groupEvaluated = reader.HasRows;
                reader.Close();

                if (!groupEvaluated)
                {
                    MessageBox.Show("This group has not been evaluated yet.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get the total marks for the evaluation
                command = new SqlCommand("SELECT TotalMarks FROM Evaluation WHERE Id=@EvaluationId;", con);
                command.Parameters.AddWithValue("@EvaluationId", evaluationId);
                int totalMarks = (int)command.ExecuteScalar();

                if (obtainedMarks > totalMarks)
                {
                    MessageBox.Show("Obtained marks cannot be greater than total marks.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update the obtained marks and evaluation date
                command = new SqlCommand("UPDATE GroupEvaluation SET ObtainedMarks=@ObtainedMarks, EvaluationDate=@EvaluationDate WHERE GroupID=@GroupId AND EvaluationID=@EvaluationId;", con);
                command.Parameters.AddWithValue("@GroupId", groupId);
                command.Parameters.AddWithValue("@EvaluationId", evaluationId);
                command.Parameters.AddWithValue("@ObtainedMarks", obtainedMarks);
                command.Parameters.AddWithValue("@EvaluationDate", evaluationDate);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Evaluation updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error updating evaluation.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
               
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

        private void SearchInactiveByRegNo_TextChanged(object sender, EventArgs e)
        {

            string searchText = SearchInactiveByRegNo.Text;


            // Check if search text is empty
            if (searchText.Length == 0)
            {
                dataGridView1.DataSource = null;
                return;
            }

            // Construct the SQL query based on the selected search option
            string query = "";

            query = @"
         SELECT 
    g.ID AS GroupID,
    g.Created_On AS GroupCreationDate,
    p.Id AS ProjectID,
    p.Title AS ProjectTitle,
    p.Description AS ProjectDescription,
    e.Id AS EvaluationID,
    e.Name As EvaluationName,
    e.TotalMarks as TotalMarks,
    e.TotalWeightage as TotalWeight,
    ge.ObtainedMarks,
    ge.EvaluationDate,
    s.RegistrationNo AS StudentRegistrationNo,
    CONCAT(pers.FirstName, ' ', pers.LastName) AS StudentName
FROM 
    [Group] g
    LEFT JOIN GroupProject gp ON g.ID = gp.GroupID
    LEFT JOIN Project p ON gp.ProjectID = p.Id
    LEFT JOIN GroupEvaluation ge ON g.ID = ge.GroupID
    LEFT JOIN Evaluation e ON ge.EvaluationID = e.Id
    LEFT JOIN GroupStudent gs ON g.ID = gs.GroupId
    LEFT JOIN Student s ON gs.StudentId = s.Id
    LEFT JOIN Person pers ON s.Id = pers.Id
WHERE 
    s.RegistrationNo = @SearchText AND gs.Status = '4';
";


            // Execute the query and populate the data grid view with the results
            try
            {
                SqlConnection con = Configuration.getInstance().getConnection();
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand command = new SqlCommand(query, con);
                command.Parameters.AddWithValue("@SearchText", searchText);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Bind the data table to the data grid view
                dataGridView1.DataSource = dataTable;

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

        private void SearchInactiveByRegNo_Click(object sender, EventArgs e)
        {

        }

        private void SearchCriteria_Click(object sender, EventArgs e)
        {

        }
    }
}
