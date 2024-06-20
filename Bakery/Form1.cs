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

namespace Bakery
{
    public partial class Form1 : Form
    {

        private Form2 _next;
        private Form3 _next1;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a connection string for the Bakery database
                string connectionString = "Data Source=MAGE_PACA\\SQLEXPRESS;Initial Catalog=bakery;Integrated Security=True;Encrypt=False";

                // Create a new SqlConnection object using the connection string
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Open the connection
                    connection.Open();

                    // Create a new SqlCommand object to insert a new customer
                    using (SqlCommand command = new SqlCommand("INSERT INTO Customers (CustomerID, FirstName, LastName, Email, PhoneNumber, Address) VALUES (@CustomerID, @FirstName, @LastName, @Email, @PhoneNumber, @Address)", connection))
                    {
                        // Add parameters to the command to prevent SQL injection attacks
                        command.Parameters.AddWithValue("@CustomerID", txtCustomerID.Text);
                        command.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                        command.Parameters.AddWithValue("@LastName", txtLastName.Text);
                        command.Parameters.AddWithValue("@Email", txtEmail.Text);
                        command.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text);
                        command.Parameters.AddWithValue("@Address", txtAddress.Text);

                        // Execute the command to insert the new customer
                        command.ExecuteNonQuery();
                    }

                    // Close the connection
                    connection.Close();

                    // Display a message to confirm that the customer was inserted
                    MessageBox.Show("Customer inserted successfully!");
                }
            }
            catch (Exception ex)
            {
                // Display an error message if there was a problem inserting the customer
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
            _next = new Form2(this);
            _next.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Hide();
            _next1 = new Form3(this);
            _next1.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Create a new instance of the TotalSalesViewForm
            var totalSalesViewForm = new TotalSalesViewForm();

            // Create a new SqlConnection object
            using (var connection = new SqlConnection("Data Source=MAGE_PACA\\SQLEXPRESS;Initial Catalog=bakery;Integrated Security=True;Encrypt=False"))
            {
                // Create a new SqlCommand object
                using (var command = new SqlCommand("SELECT e.FirstName + ' ' + e.LastName AS EmployeeName, SUM(od.TotalAmount) AS TotalSales FROM OrderDetails od INNER JOIN Orders o ON od.OrderID = o.OrderID INNER JOIN Employees e ON o.EmployeeID = e.EmployeeID GROUP BY e.FirstName, e.LastName", connection))
                {
                    // Open the connection
                    connection.Open();

                    // Create a new SqlDataAdapter object
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        // Create a new DataTable object
                        var dataTable = new DataTable();

                        // Fill the DataTable with data from the database
                        adapter.Fill(dataTable);

                        // Set the DataGridView's DataSource property to the DataTable
                        totalSalesViewForm.dataGridView1.DataSource = dataTable;
                    }
                }
            }

            // Show the TotalSalesViewForm
            totalSalesViewForm.Show();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
