using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialSkin.Controls;
using MaterialSkin;
using System.Data.SqlClient;
using DBPROJECT;
using System.Windows.Forms;

namespace Modern_Dashboard_Design
{
    public partial class ManageEvaluation : Form
    {
        public ManageEvaluation()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void materialSingleLineTextField1_Click(object sender, EventArgs e)
        {

        }

        private void materialSingleLineTextField3_Click(object sender, EventArgs e)
        {

        }

        private void SearchCrieteria_TextChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a search option.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string searchText = SearchCrieteria.Text;
            string searchBy = comboBox1.SelectedItem.ToString();

            // Check if search text is empty
            if (searchText.Length == 0)
            {
                dataGridView1.DataSource = null;
                return;
            }

            // Construct the SQL query based on the selected search option
            string query = "";
            if (searchBy == "EVALUATION NAME")
            {
                query = @"
        SELECT 
            
            Name, 
            TotalMarks, 
            TotalWeightage 
        FROM 
            Evaluation 
        WHERE 
            Name LIKE @SearchTextFilter 
        ORDER BY 
            Name ASC;";
                searchText = "%" + searchText + "%"; // Add the wildcard character to search for partial matches
            }

            try
            {
                SqlConnection con = Configuration.getInstance().getConnection();

                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand command = new SqlCommand(query, con);
                if (searchBy == "EVALUATION NAME")
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
            catch (SqlException ex)
            {
                MessageBox.Show("Error executing query: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the clicked cell is in the fourth column (index 3) and not a header cell
            if (e.ColumnIndex == 3 && e.RowIndex != -1)
            {
                // Get the value of the cell that was clicked
                DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                string value = cell.Value.ToString();

                // Set the value of the clicked cell to the corresponding text box
                EvaluationTextField.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                TotalMarksTectField.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                TotalWeightageTextField.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                
            }
        }



        private void Assigned_Click(object sender, EventArgs e)
        {
            // Check if evaluation name is not empty
            if (EvaluationTextField.Text.Trim().Length == 0)
            {
                MessageBox.Show("Please enter evaluation name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if total marks is a valid number
            int totalMarks;
            if (!int.TryParse(TotalMarksTectField.Text.Trim(), out totalMarks))
            {
                MessageBox.Show("Please enter a valid total marks.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if total weightage is a valid number
            int totalWeightage;
            if (!int.TryParse(TotalWeightageTextField.Text.Trim(), out totalWeightage))
            {
                MessageBox.Show("Please enter a valid total weightage.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Add evaluation to the database
            try
            {
                SqlConnection con = Configuration.getInstance().getConnection();
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand command = new SqlCommand("SELECT * FROM Evaluation WHERE Name = @EvaluationName", con);
                command.Parameters.AddWithValue("@EvaluationName", EvaluationTextField.Text.Trim());

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    MessageBox.Show("Evaluation already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                 command = new SqlCommand("INSERT INTO Evaluation (Name, TotalMarks, TotalWeightage) VALUES (@Name, @TotalMarks, @TotalWeightage)", con);
                command.Parameters.AddWithValue("@Name", EvaluationTextField.Text.Trim());
                command.Parameters.AddWithValue("@TotalMarks", totalMarks);
                command.Parameters.AddWithValue("@TotalWeightage", totalWeightage);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Evaluation added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    // Clear the text fields after adding evaluation
                    EvaluationTextField.Clear();
                    TotalMarksTectField.Clear();
                    TotalWeightageTextField.Clear();
                }
                else
                {
                    MessageBox.Show("Evaluation could not be added.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding evaluation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ManageEvaluation_Load(object sender, EventArgs e)
        {
            

        }

        private void UpdateEvaluation_Click(object sender, EventArgs e)
        {
            // Check if evaluation name is not empty
            if (EvaluationTextField.Text.Trim().Length == 0)
            {
                MessageBox.Show("Please enter evaluation name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if total marks is a valid number
            int totalMarks;
            if (!int.TryParse(TotalMarksTectField.Text.Trim(), out totalMarks))
            {
                MessageBox.Show("Please enter a valid total marks.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if total weightage is a valid number
            int totalWeightage;
            if (!int.TryParse(TotalWeightageTextField.Text.Trim(), out totalWeightage))
            {
                MessageBox.Show("Please enter a valid total weightage.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update evaluation in the database
            try
            {
                SqlConnection con = Configuration.getInstance().getConnection();
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand command = new SqlCommand("SELECT * FROM Evaluation WHERE Name = @EvaluationName", con);
                command.Parameters.AddWithValue("@EvaluationName", EvaluationTextField.Text.Trim());

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                if (dataTable.Rows.Count == 0)
                {
                    MessageBox.Show("Evaluation does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    command = new SqlCommand("UPDATE Evaluation SET TotalMarks = @TotalMarks, TotalWeightage = @TotalWeightage WHERE Name = @EvaluationName", con);
                    command.Parameters.AddWithValue("@EvaluationName", EvaluationTextField.Text.Trim());
                    command.Parameters.AddWithValue("@TotalMarks", totalMarks);
                    command.Parameters.AddWithValue("@TotalWeightage", totalWeightage);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Evaluation updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        // Clear the text fields after updating evaluation
                        EvaluationTextField.Clear();
                        TotalMarksTectField.Clear();
                        TotalWeightageTextField.Clear();
                    }
                    else
                    {
                        MessageBox.Show("Evaluation could not be updated.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating evaluation: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            ClearFields();
        }
        private void ClearFields()
        {
            EvaluationTextField.Text = "";
            TotalMarksTectField.Text = "";
            TotalWeightageTextField.Text = "";
        }

        private void ShowEvaluation_Click(object sender, EventArgs e)
        {
            try
            {
                // Retrieve all evaluations from the database
                SqlConnection con = Configuration.getInstance().getConnection();
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand command = new SqlCommand("SELECT * FROM Evaluation", con);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                // Bind the data to the DataGridView
                dataGridView1.DataSource = dataTable;

                // Clear the text fields
                EvaluationTextField.Clear();
                TotalMarksTectField.Clear();
                TotalWeightageTextField.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();

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
            catch (Exception ex)
            {
                MessageBox.Show("Error retrieving evaluations: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
