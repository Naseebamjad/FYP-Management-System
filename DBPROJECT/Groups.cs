using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using MaterialSkin.Controls;
using MaterialSkin;
using System.Data.SqlClient;
using System.Windows.Forms;
using DBPROJECT;

namespace Modern_Dashboard_Design
{
    public partial class Groups : Form
    {
        public Groups()
        {
            InitializeComponent();
        }

        private void RegNoTextField_Click(object sender, EventArgs e)
        {

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
            g.ID AS GroupID, 
            g.Created_On as GroupCreationDate, 
            (SELECT value From Lookup Where Category = 'Status' and id = gs.Status),
            gs.AssignmentDate as StudentAssignDate, 
            s.RegistrationNo, 
            p.FirstName, 
            p.LastName
        FROM 
            [Group] g
            LEFT JOIN GroupStudent gs ON g.ID = gs.GroupID
            LEFT JOIN Student s ON gs.StudentID = s.ID
            LEFT JOIN Person p ON s.ID = p.ID
        WHERE 
            g.ID = @SearchText;";
            }
            else if (searchBy == "REG NO")
            {
                // Show group student data for the specified student registration number
                query = @"
        SELECT 
              g.ID AS GroupID, 
            g.Created_On as GroupCreationDate, 
            (SELECT value From Lookup Where Category = 'Status' and id = gs.Status) as Status,
            gs.AssignmentDate as StudentAssignDate, 
            s.RegistrationNo, 
            p.FirstName, 
            p.LastName
        FROM 
            [Group] g
            LEFT JOIN GroupStudent gs ON g.ID = gs.GroupID
            LEFT JOIN Student s ON gs.StudentID = s.ID
            LEFT JOIN Person p ON s.ID = p.ID
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
                dataGridView1.Columns.Clear();
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();



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
                dataGridView1.DataSource = dataTable;

                dataGridView1.DataSource = dataTable;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error executing query: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RegNoTextField_Click_1(object sender, EventArgs e)
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

        private void materialFlatButton1_Click(object sender, EventArgs e)
        {
            Form Manage_Projects = new GroupFormation();
            this.Hide();
            Manage_Projects.Show();
        }

        private void Add_Click(object sender, EventArgs e)
        {
            Form Manage_Projects = new ExistingGroupcs();
            this.Hide();
            Manage_Projects.Show();
        }
        // Clear the data grid view




    }
}
