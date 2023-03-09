using DBPROJECT;
using MaterialSkin;
using Newtonsoft.Json.Linq;
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
    public partial class Advisor : Form
    {


        public Advisor()
        {
            InitializeComponent();
        }

        private void Advisor_Load(object sender, EventArgs e)
        {
            Add.BackColor = Color.FromArgb(255, 255, 255);
            Add.ForeColor = Color.FromArgb(46, 53, 73);
            dateTimePicker1.ShowUpDown = true;
            // Set the format of the DateTimePicker
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";

            // Set the font of the DateTimePicker
            dateTimePicker1.Font = new Font("Arial", 12);

            // Set the foreground and background colors of the DateTimePicker
            dateTimePicker1.ForeColor = Color.FromArgb(46, 53, 73);
            dateTimePicker1.BackColor = Color.FromArgb(255, 255, 255);
            List<string> genderValues = new List<string>();
            List<string> designationValues = new List<string>();

            SqlConnection con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("SELECT Value, Category FROM Lookup WHERE Category IN ('Gender', 'Designation')", con);

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string value = reader["Value"].ToString();
                    string category = reader["Category"].ToString();

                    if (category == "GENDER")
                    {
                        genderValues.Add(value);
                    }
                    else if (category == "DESIGNATION")
                    {
                        designationValues.Add(value);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            con.Close();

            genderComboBox.DataSource = genderValues;
            DesignationComboBox.DataSource = designationValues;

        }

        private void Add_Click(object sender, EventArgs e)
        {
            if (FirstNameTextField.Text != "" && LastNameTextField.Text != "" && ContactTextField.Text != "" && EmailTextField.Text != "" && SalaryTextField.Text != "" && DesignationComboBox.SelectedItem != null && genderComboBox.SelectedItem != null)
            {
                string selectedGender = genderComboBox.SelectedItem.ToString();
                string selectedDesignation = DesignationComboBox.SelectedItem.ToString();
                int genderId = -1;
                int designationId = -1;

                SqlConnection con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("SELECT ID FROM Lookup WHERE Category = 'Gender' AND Value = @Value", con);
                cmd.Parameters.AddWithValue("@Value", selectedGender);

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        genderId = int.Parse(reader["ID"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                con.Close();

                cmd = new SqlCommand("SELECT ID FROM Lookup WHERE Category = 'Designation' AND Value = @Value", con);
                cmd.Parameters.AddWithValue("@Value", selectedDesignation);

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        designationId = int.Parse(reader["ID"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                con.Close();

                if (insertAdvisor(FirstNameTextField.Text, LastNameTextField.Text, EmailTextField.Text, ContactTextField.Text,  SalaryTextField.Text, genderId, designationId))
                {
                    MessageBox.Show("New Advisor Added", "Add Advisor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Error", "Add Advisor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Empty Fields", "Manage Advisor", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

        }
        private bool insertAdvisor(string FName, string LName, string Email, string Contact, String Salary, int GenderID, int DesignationID)
        {
            bool result = false;

            SqlConnection con = Configuration.getInstance().getConnection();
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
           

            // Check if email already exists in Person table
            SqlCommand cmdCheckEmail = new SqlCommand("SELECT COUNT(*) FROM Person WHERE Email = @Email", con);
            cmdCheckEmail.Parameters.AddWithValue("@Email", Email);
            int count = Convert.ToInt32(cmdCheckEmail.ExecuteScalar());
            if (count > 0)
            {
                MessageBox.Show("Email already exists", "Add Advisor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                con.Close();
                return false;
            }

            // Check if email already exists in Student table
            SqlCommand cmdCheckStudentEmail = new SqlCommand("SELECT COUNT(*) FROM Person INNER JOIN Student ON Person.Id = Student.Id WHERE Person.Email = @Email", con);
            cmdCheckStudentEmail.Parameters.AddWithValue("@Email", Email);
            count = Convert.ToInt32(cmdCheckStudentEmail.ExecuteScalar());
            if (count > 0)
            {
                MessageBox.Show("This email belongs to a student", "Add Advisor", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                con.Close();
                return false;
            }

            // Insert new Person record
            string insertPersonQuery = "INSERT INTO Person (FirstName, LastName, Email, Contact, DateOfBirth, Gender) " +
                                           "VALUES (@FirstName, @LastName, @Email, @Contact, @DateOfBirth, @GenderId);" +
                                           "SELECT CAST(scope_identity() AS int)";
            SqlCommand cmdInsertPerson = new SqlCommand(insertPersonQuery, con);
            cmdInsertPerson.Parameters.AddWithValue("@FirstName", FName);
            cmdInsertPerson.Parameters.AddWithValue("@LastName", LName);
            cmdInsertPerson.Parameters.AddWithValue("@Email", Email);
            cmdInsertPerson.Parameters.AddWithValue("@Contact", Contact);
            cmdInsertPerson.Parameters.AddWithValue("@DateOfBirth", dateTimePicker1.Value);
            cmdInsertPerson.Parameters.AddWithValue("@GenderId", GenderID);
            int personId = (int)cmdInsertPerson.ExecuteScalar();

            // Insert new Advisor record
            string insertAdvisorQuery = "INSERT INTO Advisor (Id, Designation, Salary) " +
                                        "VALUES (@PersonId, @DesignationId, @Salary)";
            SqlCommand cmdInsertAdvisor = new SqlCommand(insertAdvisorQuery, con);
            cmdInsertAdvisor.Parameters.AddWithValue("@PersonId", personId);
            cmdInsertAdvisor.Parameters.AddWithValue("@DesignationId", DesignationID);
            cmdInsertAdvisor.Parameters.AddWithValue("@Salary", Salary);
            int rowsAffected = cmdInsertAdvisor.ExecuteNonQuery();
            if (rowsAffected > 0)
            {
                result = true;
            }
            con.Close();
            return result;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void materialFlatButton3_Click(object sender, EventArgs e)
        {
            SqlConnection con = Configuration.getInstance().getConnection();
            string query = "SELECT  Person.FirstName, Person.LastName, Person.Email, Person.Contact, Person.DateOfBirth, Lookup.Value as Gender, LookupDesignation.Value as Designation, Advisor.Salary " +
               "FROM Person " +
               "INNER JOIN Advisor ON Person.Id = Advisor.Id " +
               "INNER JOIN Lookup ON Person.Gender = Lookup.Id " +
               "INNER JOIN Lookup LookupDesignation ON Advisor.Designation = LookupDesignation.Id " +
               "WHERE Lookup.Category = 'Gender'";

            SqlCommand cmd = new SqlCommand(query, con);


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


               // dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex ==8)
            {
                EmailTextField.Enabled = false;
                string email = dataGridView1.Rows[e.RowIndex].Cells["Email"].Value.ToString();

                SqlConnection con = Configuration.getInstance().getConnection();
                string query = "SELECT Person.*, Advisor.Designation , Advisor.Salary " +
                               "FROM Person " +
                               "INNER JOIN Advisor ON Person.Id = Advisor.Id " +
                               "WHERE Person.Email = @Email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();

                        FirstNameTextField.Text = reader["FirstName"].ToString();
                        LastNameTextField.Text = reader["LastName"].ToString();
                        EmailTextField.Text = reader["Email"].ToString();
                        ContactTextField.Text = reader["Contact"].ToString();
                        
                        SalaryTextField.Text = reader["Salary"].ToString();

                        // Set the ReadOnly property of the RegNoTextField to true
                        //RegNoTextField.Enabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Record not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            }
        }


        private void UpdateAdvisor_Click(object sender, EventArgs e)
        {
            // check if all required fields are filled
            if (FirstNameTextField.Text != "" && LastNameTextField.Text != "" && EmailTextField.Text != "" && ContactTextField.Text != ""  && SalaryTextField.Text != ""  && genderComboBox.SelectedItem != null && DesignationComboBox.SelectedItem != null)
            {
                // get the ID of the advisor to update from the advisorID TextBox
                string selectedGender = genderComboBox.SelectedItem.ToString();
                string selectedDesignation = DesignationComboBox.SelectedItem.ToString();
                int genderId = -1;
                int designationId = -1;

                SqlConnection con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("SELECT ID FROM Lookup WHERE Category = 'Gender' AND Value = @Value", con);
                cmd.Parameters.AddWithValue("@Value", selectedGender);

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        genderId = int.Parse(reader["ID"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                con.Close();

                cmd = new SqlCommand("SELECT ID FROM Lookup WHERE Category = 'Designation' AND Value = @Value", con);
                cmd.Parameters.AddWithValue("@Value", selectedDesignation);

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        designationId = int.Parse(reader["ID"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                con.Close();

                string email = EmailTextField.Text;
               

                // update the Person table
                string updatePersonQuery = "UPDATE Person SET FirstName=@FirstName, LastName=@LastName, Contact=@Contact, DateOfBirth=@DateOfBirth, Gender=@Gender WHERE Email=@Email";

                cmd = new SqlCommand(updatePersonQuery, con);
                cmd.Parameters.AddWithValue("@FirstName", FirstNameTextField.Text);
                cmd.Parameters.AddWithValue("@LastName", LastNameTextField.Text);
                cmd.Parameters.AddWithValue("@Contact", ContactTextField.Text);
                cmd.Parameters.AddWithValue("@DateOfBirth", dateTimePicker1.Value);
                cmd.Parameters.AddWithValue("@Gender", genderId);
                cmd.Parameters.AddWithValue("@Email", email);

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        // update the Advisor table
                        string updateAdvisorQuery = "UPDATE Advisor SET Designation=@Designation, Salary=@Salary WHERE Id=(SELECT Id FROM Person WHERE Email=@Email)";
                        SqlCommand cmd2 = new SqlCommand(updateAdvisorQuery, con);
                        cmd2.Parameters.AddWithValue("@Designation", designationId);
                        cmd2.Parameters.AddWithValue("@Salary", SalaryTextField.Text);
                        cmd2.Parameters.AddWithValue("@Email", email);

                        int rowsAffected2 = cmd2.ExecuteNonQuery();
                        if (rowsAffected2 > 0)
                        {
                            MessageBox.Show("Advisor updated successfully", "Update Advisor", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to update advisor", "Update Advisor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Failed to update advisor", "Update Advisor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                con.Close();
            }
            else
            {
                MessageBox.Show("Please fill all required fields", "Update Advisor", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void ClearField_Click(object sender, EventArgs e)
        {
            
            FirstNameTextField.Text = "";
            LastNameTextField.Text = "";
            ContactTextField.Text = "";
            EmailTextField.Text = "";
           
            EmailTextField.Enabled = true;
            SalaryTextField.Text = "";

        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void FirstNameTextField_TextChanged(object sender, EventArgs e)
        {

        }

        private void FirstNameTextField_Click(object sender, EventArgs e)
        {

        }

        private void EmailTextField_TextChanged(object sender, EventArgs e)
        {
            string partialEmail = EmailTextField.Text;

            if (string.IsNullOrEmpty(partialEmail))
            {
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                return;
            }

            SqlConnection con = Configuration.getInstance().getConnection();

            string query = "SELECT  Person.FirstName, Person.LastName, Person.Email, Person.Contact, Person.DateOfBirth, Lookup.Value as Gender, LookupDesignation.Value as Designation, Advisor.Salary " +
                              "FROM Person " +
                              "INNER JOIN Advisor ON Person.Id = Advisor.Id " +
                               "INNER JOIN Lookup ON Person.Gender = Lookup.Id " +
                               "WHERE Lookup.Category = 'Gender' AND Person.Email LIKE @partialEmail + '%'";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@partialEmail", partialEmail);
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
            catch (Exception ex)
            {

            }


        }

        private void EmailTextField_Click(object sender, EventArgs e)
        {
            MaterialSkinManager skinManager = MaterialSkinManager.Instance;
            skinManager.ColorScheme = new ColorScheme(
                  Primary.Blue100, // Set your primary color
                  Primary.Blue100,
                  Primary.Blue100,
                  Accent.Blue200,
                  TextShade.WHITE
);

            EmailTextField.BackColor = skinManager.ColorScheme.PrimaryColor;
            // RegNoTextField.Text = "REG NO:";
            EmailTextField.Font = new Font(FirstNameTextField.Font.FontFamily, 12, FontStyle.Bold);
            EmailTextField.Width = 200;
            EmailTextField.Height = 70;
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            string Email = EmailTextField.Text;
            EmailTextField.Enabled = true;
            SqlConnection con = Configuration.getInstance().getConnection();
            string query = "SELECT Person.FirstName, Person.LastName, Person.Email, Person.Contact, Person.DateOfBirth, Lookup.Value as Gender, " +
                "(SELECT Value FROM Lookup WHERE Category = 'Designation' AND Id = Advisor.Designation) as Designation, Advisor.Salary " +
                "FROM Person " +
                "INNER JOIN Advisor ON Person.Id = Advisor.Id " +
                "INNER JOIN Lookup ON Person.Gender = Lookup.Id " +
                "WHERE Lookup.Category = 'Gender' AND Person.Email = @Email";


            /*string query = "SELECT Person.FirstName, Person.LastName, Person.Email, Person.Contact, Person.DateOfBirth, Student.RegistrationNo, Lookup.Value as Gender " +
                          "FROM Person " +
                          "INNER JOIN Student ON Person.Id = Student.Id " +
                          "INNER JOIN Lookup ON Person.Gender = Lookup.Id " +
                          "WHERE Lookup.Category = 'Gender' AND Student.RegistrationNo = @registrationNo"; */
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@Email", Email);

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
                    MessageBox.Show("Advisor not found!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
