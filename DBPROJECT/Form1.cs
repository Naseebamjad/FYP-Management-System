using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Bunifu.UI.WinForms;
using MaterialSkin.Controls;
using MaterialSkin;
using System.Data.SqlClient;
using DBPROJECT;
using System.Text.RegularExpressions;

namespace Modern_Dashboard_Design
{
    public partial class Form1 : Form
    {

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]

        private static extern IntPtr CreateRoundRectRgn
     (
          int nLeftRect,
          int nTopRect,
          int nRightRect,
          int nBottomRect,
          int nWidthEllipse,
         int nHeightEllipse

      );

        public Form1()
        {
            InitializeComponent();
         //  */ Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 25, 25));
         //   pnlNav.Height = btnDashboard.Height;
         //   pnlNav.Top = btnDashboard.Top;
        //   pnlNav.Left = btnDashboard.Left;
        //    btnDashboard.BackColor = Color.FromArgb(46, 51, 73);
        //    MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
        //    materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
        //    materialSkinManager.ColorScheme = new ColorScheme(Primary.LightBlue500, Primary.LightBlue700, Primary.LightBlue200, Accent.Red100, TextShade.WHITE);

       //     MaterialFlatButton materialFlatButton1 = new MaterialFlatButton();
       //     materialFlatButton1.AutoSize = true;
        //    materialFlatButton1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
         //   materialFlatButton1.Depth = 0;
         //   materialFlatButton1.Location = new System.Drawing.Point(12, 80);
        //    materialFlatButton1.Margin = new Padding(4, 6, 4, 6);
         //   materialFlatButton1.MouseState = MaterialSkin.MouseState.HOVER;
         //   materialFlatButton1.Name = "materialFlatButton1";
         //   materialFlatButton1.Primary = true;
         //   materialFlatButton1.Size = new System.Drawing.Size(108, 36);
          //  materialFlatButton1.TabIndex = 0;
          //  materialFlatButton1.Text = "Add";
          //  materialFlatButton1.UseVisualStyleBackColor = true;
          //  materialFlatButton1.Click += MaterialFlatButton1_Click;
           // Controls.Add(materialFlatButton1); 
        }

        private void MaterialFlatButton1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Button was clicked!");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Add.BackColor = Color.FromArgb(255, 255, 255);
            Add.ForeColor = Color.FromArgb(46, 53, 73);
            dateTimePicker1.ShowUpDown = true;
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "dd/MM/yyyy";
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;


            dateTimePicker1.Font = new Font("Arial", 12);

           
            dateTimePicker1.ForeColor = Color.FromArgb(46, 53, 73);
            dateTimePicker1.BackColor = Color.FromArgb(255, 255, 255);

            List<string> genderValues = new List<string>();
            SqlConnection con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("SELECT Value FROM Lookup WHERE Category = 'Gender'", con);
            

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            

               

                try
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        genderValues.Add(reader["Value"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                con.Close();
            

            genderComboBox.DataSource = genderValues;
            
        }


        private void FlowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void BtnDashboard_Click(object sender, EventArgs e)
        {
            pnlNav.Height = btnDashboard.Height;
            pnlNav.Top = btnDashboard.Top;
            pnlNav.Left = btnDashboard.Left;
            btnDashboard.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void BtnAnalytics_Click(object sender, EventArgs e)
        {
            pnlNav.Height = btnAnalytics.Height;
            pnlNav.Top = btnAnalytics.Top;
            btnAnalytics.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void BtnCalander_Click(object sender, EventArgs e)
        {
            pnlNav.Height = btnCalander.Height;
            pnlNav.Top = btnCalander.Top;
            btnCalander.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void BtnContactUs_Click(object sender, EventArgs e)
        {
            pnlNav.Height = btnContactUs.Height;
            pnlNav.Top = btnContactUs.Top;
            btnContactUs.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            pnlNav.Height = btnSettings.Height;
            pnlNav.Top = btnSettings.Top;
            btnSettings.BackColor = Color.FromArgb(46, 51, 73);
        }

        private void BtnDashboard_Leave(object sender, EventArgs e)
        {
            btnDashboard.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void BtnAnalytics_Leave(object sender, EventArgs e)
        {
            btnAnalytics.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void BtnCalander_Leave(object sender, EventArgs e)
        {
            btnCalander.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void BtnContactUs_Leave(object sender, EventArgs e)
        {
            btnContactUs.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void BtnSettings_Leave(object sender, EventArgs e)
        {
            btnSettings.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void Label11_Click(object sender, EventArgs e)
        {

        }

        private void Label14_Click(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void materialSingleLineTextField1_Click(object sender, EventArgs e)
        {
            
           
            
        }

        private void materialSingleLineTextField2_Click(object sender, EventArgs e)
        {



        }

        private void button3_Click(object sender, EventArgs e)
        {
            // hide this form 
            this.Hide();
            Advisor f2 = new Advisor();
            f2.Show();


        }

        private void materialSingleLineTextField7_Click(object sender, EventArgs e)
        {
            MaterialSkinManager skinManager = MaterialSkinManager.Instance;
            skinManager.ColorScheme = new ColorScheme(
                  Primary.Blue100, // Set your primary color
                  Primary.Blue100,
                  Primary.Blue100,
                  Accent.Blue200,
                  TextShade.WHITE
);

            RegNoTextField.BackColor = skinManager.ColorScheme.PrimaryColor;
           // RegNoTextField.Text = "REG NO:";
            RegNoTextField.Font = new Font(FirstNameTextField.Font.FontFamily, 12, FontStyle.Bold);
            RegNoTextField.Width = 200;
            RegNoTextField.Height = 70;


            

            
        }

        private void materialSingleLineTextField3_Click(object sender, EventArgs e)
        {

        }

        private void materialRaisedButton1_Click_1(object sender, EventArgs e)
        {

        }

        private void Add_Click(object sender, EventArgs e)
        {
            // first check all field must  be filled
            RegNoTextField.Enabled = true;
            if (FirstNameTextField.Text != "" && LastNameTextField.Text != "" && genderComboBox.SelectedItem != null &&  EmailTextField.Text != "" && ContactTextField.Text != "" && RegNoTextField.Text != "")
            {
                string selectedGender = genderComboBox.SelectedItem.ToString();
                if (isStudentExist(RegNoTextField.Text))
                {
                    MessageBox.Show("This Student Already Exist", "Add Student", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else if (isEmailExist(EmailTextField.Text))
                {
                    MessageBox.Show("This Email Already Exist", "Add Student", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                }
                else
                {
                    int genderId =0;
                    if (selectedGender == "Male")
                    {
                         genderId = 1;
                    }
                    else  if (selectedGender == "Female")
                    {
                        genderId = 2;
                    }
                    if (insertStudent(RegNoTextField.Text , FirstNameTextField.Text , LastNameTextField.Text , EmailTextField.Text, ContactTextField.Text, genderId))
                    {
                        MessageBox.Show("New Student Added", "Add Student", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error", "Add Student", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                  
                }
            }
            else
            {
                MessageBox.Show("Empty Fields", "Add Student", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private bool insertStudent(String regNo, String FirstName, String LastName, String Email, String Contact, int Gender)
        {

            SqlConnection con = Configuration.getInstance().getConnection();
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
           
            SqlCommand cmd = new SqlCommand("INSERT INTO Person (FirstName, LastName, Contact, Email, DateOfBirth, Gender) " +
                "OUTPUT INSERTED.ID " +
                "VALUES (@FirstName, @LastName, @Contact, @Email, @DOB, @Gender)", con);
            cmd.Parameters.AddWithValue("@FirstName", FirstName);
            cmd.Parameters.AddWithValue("@LastName", LastName);
            cmd.Parameters.AddWithValue("@Contact", Contact);
            cmd.Parameters.AddWithValue("@Email", Email);
            cmd.Parameters.AddWithValue("@DOB", dateTimePicker1.Value);
            cmd.Parameters.AddWithValue("@Gender", Gender);

            int insertedPersonID = (int)cmd.ExecuteScalar();

            cmd = new SqlCommand("INSERT INTO Student (RegistrationNo, ID) " +
                "VALUES (@RegistrationNo, @ID)", con);
            cmd.Parameters.AddWithValue("@RegistrationNo", regNo);
            cmd.Parameters.AddWithValue("@ID", insertedPersonID);

            try
            {
                cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                con.Close();
                return false;
            }
           
        }



        private bool isStudentExist(    string regNo)
        {
            
            bool exist = false;
            SqlConnection con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Student WHERE RegistrationNo = @regNo", con);
            cmd.Parameters.AddWithValue("@regNo", regNo);

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    exist = true;
                }
                else
                {
                    exist = false;
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

            return exist;

        }
        private bool isEmailExist(string email)
        {
            bool exist = false;
            SqlConnection con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Person WHERE Email = @email", con);
            cmd.Parameters.AddWithValue("@email", email);

            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    exist = true;
                }
                else
                {
                    exist = false;
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

            return exist;
        }


        private void genderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ShowStu_Click(object sender, EventArgs e)
        {
            SqlConnection con = Configuration.getInstance().getConnection();
            string query = "SELECT Person.FirstName, Person.LastName, Person.Email,Person.Contact , Person.DateOfBirth, Student.RegistrationNo, Lookup.Value as Gender " +
                               "FROM Person " +
                               "INNER JOIN Student ON Person.Id = Student.Id " +
                               "INNER JOIN Lookup ON Person.Gender = Lookup.Id " +
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

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked column is the "Edit" column and if the clicked row is valid
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView1.Columns["Edit"].Index)
            {
                // Get the data from the selected row
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                string title = selectedRow.Cells["Title"].Value.ToString();
                string advisorName = selectedRow.Cells["AdvisorName"].Value.ToString();
                string advisorRole = selectedRow.Cells["AdvisorRole"].Value.ToString();

               
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 7)
            {
                string registrationNo = dataGridView1.Rows[e.RowIndex].Cells["RegistrationNo"].Value.ToString();

                SqlConnection con = Configuration.getInstance().getConnection();
                string query = "SELECT Person.*, Student.RegistrationNo " +
                               "FROM Person " +
                               "INNER JOIN Student ON Person.Id = Student.Id " +
                               "WHERE Student.RegistrationNo = @registrationNo";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@registrationNo", registrationNo);

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
                        RegNoTextField.Text = reader["RegistrationNo"].ToString();
                        FirstNameTextField.Text = reader["FirstName"].ToString();
                        LastNameTextField.Text = reader["LastName"].ToString();
                        EmailTextField.Text = reader["Email"].ToString();
                        ContactTextField.Text = reader["Contact"].ToString();
                        DateTime dateOfBirth;
                        if (DateTime.TryParse(reader["DateOfBirth"].ToString(), out dateOfBirth))
                        {
                            dateTimePicker1.Value = dateOfBirth;
                        }



                        // Set the ReadOnly property of the RegNoTextField to true
                        RegNoTextField.Enabled = false;
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





     /*   private void deleteRecord(string registrationNo)
        {
            SqlConnection con = Configuration.getInstance().getConnection();
            string query = "DELETE FROM Student WHERE RegistrationNo = @registrationNo; " +
                           "DELETE FROM Person WHERE Id = (SELECT Id FROM Student WHERE RegistrationNo = @registrationNo)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@registrationNo", registrationNo);

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this record?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Record deleted successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error deleting record!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        }*/

        private void SearchStu_Click(object sender, EventArgs e)
        {
            string registrationNo = RegNoTextField.Text;
            RegNoTextField.Enabled = true;
            SqlConnection con = Configuration.getInstance().getConnection();
            string query = "SELECT Person.FirstName, Person.LastName, Person.Email, Person.Contact, Person.DateOfBirth, Student.RegistrationNo, Lookup.Value as Gender " +
                           "FROM Person " +
                           "INNER JOIN Student ON Person.Id = Student.Id " +
                           "INNER JOIN Lookup ON Person.Gender = Lookup.Id " +
                           "WHERE Lookup.Category = 'Gender' AND Student.RegistrationNo = @registrationNo";

            /*string query = "SELECT Person.FirstName, Person.LastName, Person.Email, Person.Contact, Person.DateOfBirth, Student.RegistrationNo, Lookup.Value as Gender " +
                          "FROM Person " +
                          "INNER JOIN Student ON Person.Id = Student.Id " +
                          "INNER JOIN Lookup ON Person.Gender = Lookup.Id " +
                          "WHERE Lookup.Category = 'Gender' AND Student.RegistrationNo = @registrationNo"; */
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@registrationNo", registrationNo);

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

                    //dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
                    MessageBox.Show("Student not found!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void materialFlatButton3_Click(object sender, EventArgs e)
        {
            if (FirstNameTextField.Text != "" && LastNameTextField.Text != "" && genderComboBox.SelectedItem != null && EmailTextField.Text != "" && ContactTextField.Text != "" && RegNoTextField.Text != "")
            {
                string registrationNo = RegNoTextField.Text;
                string firstName = FirstNameTextField.Text;
                string lastName = LastNameTextField.Text;
                string contactNo = ContactTextField.Text;
                string email = EmailTextField.Text;
              
                string selectedGender = genderComboBox.SelectedItem.ToString();
                int gender = 0;
                if (selectedGender == "Male")
                {
                    gender = 1;
                }
                else if (selectedGender == "Female")
                {
                    gender = 2;
                }
              

                SqlConnection con = Configuration.getInstance().getConnection();
                string query = "UPDATE Person " +
                               "SET FirstName = @firstName, LastName = @lastName, Contact = @contactNo, Email = @email, Gender = @gender, DateOfBirth = @dob " +
                               "WHERE Id = (SELECT Id FROM Student WHERE RegistrationNo = @registrationNo)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@firstName", firstName);
                cmd.Parameters.AddWithValue("@lastName", lastName);
                cmd.Parameters.AddWithValue("@contactNo", contactNo);
                cmd.Parameters.AddWithValue("@email", email);

                cmd.Parameters.AddWithValue("@dob", dateTimePicker1.Value);
                cmd.Parameters.AddWithValue("@registrationNo", registrationNo);
                cmd.Parameters.AddWithValue("@gender", gender);

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                try
                {
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        RegNoTextField.Text = "";
                        FirstNameTextField.Text = "";
                        LastNameTextField.Text = "";
                        ContactTextField.Text = "";
                        EmailTextField.Text = "";
                       
                        RegNoTextField.Enabled = true;
                        MessageBox.Show("Record updated successfully!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error updating record NO RECORD EXIST!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            else
            {
                MessageBox.Show("Empty Fields", "Add Student", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void ClearField_Click(object sender, EventArgs e)
        {
            RegNoTextField.Text = "";
            FirstNameTextField.Text = "";
            LastNameTextField.Text = "";
            ContactTextField.Text = "";
            EmailTextField.Text = "";
            
            RegNoTextField.Enabled = true;
        }

        private void RegNoTextField_TextChanged(object sender, EventArgs e)
        {
            string partialRegNo = RegNoTextField.Text;

            if (string.IsNullOrEmpty(partialRegNo))
            {
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                return;
            }
            SqlConnection con = Configuration.getInstance().getConnection();
            string query = "SELECT Person.FirstName, Person.LastName, Person.Email,Person.Contact , Person.DateOfBirth, Student.RegistrationNo, Lookup.Value as Gender " +
                               "FROM Person " +
                               "INNER JOIN Student ON Person.Id = Student.Id " +
                               "INNER JOIN Lookup ON Person.Gender = Lookup.Id " +
                               "WHERE Lookup.Category = 'Gender' AND Student.RegistrationNo LIKE @partialRegNo + '%'";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@partialRegNo", partialRegNo);

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

            }


        }

        private void FirstNameTextField_TextChanged(object sender, EventArgs e)
        {
            string partialFirstName = RegNoTextField.Text;

            if (string.IsNullOrEmpty(partialFirstName))
            {

                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                return;
            }

            
            SqlConnection con = Configuration.getInstance().getConnection();
            string query = "SELECT Person.FirstName, Person.LastName, Person.Email,Person.Contact , Person.DateOfBirth, Student.RegistrationNo, Lookup.Value as Gender " +
                       "FROM Person " +
                       "INNER JOIN Student ON Person.Id = Student.Id " +
                       "INNER JOIN Lookup ON Person.Gender = Lookup.Id " +
                       "WHERE Lookup.Category = 'Gender' AND Person.FirstName LIKE @partialFirstName + '%'";

            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@partialFirstName", partialFirstName + "%");

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

            }

        }

        private void btnDashboard_Click_1(object sender, EventArgs e)
        {
            Form Manage_Projects = new Form1();
            this.Hide();
            Manage_Projects.Show();
        }

        private void btnAnalytics_Click_1(object sender, EventArgs e)
        {
            
            Form Manage_Projects = new Manage_Projects();
            this.Hide();
            Manage_Projects.Show();
        }

        private void btnCalander_Click_1(object sender, EventArgs e)
        {
            Form Groups = new Groups();
            this.Hide();
            Groups.Show();
        }

        private void btnContactUs_Click_1(object sender, EventArgs e)
        {
            Form ProjectAssign = new ProjectAssign();
            this.Hide();
            ProjectAssign.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form ManageEvaluation = new ManageEvaluation();
            this.Hide();
            ManageEvaluation.Show();
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            Form Advisor = new Advisor();
            this.Hide();
            Advisor.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form Advisor = new EvaluationMarking();
            this.Hide();
            Advisor.Show();
        }
    }

}
