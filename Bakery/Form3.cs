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
    public partial class Form3 : Form
    {
        private readonly Form1 _previous;
        public Form3(Form1 previous)
        {
            InitializeComponent();
            _previous = previous;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Hide();
            _previous.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Create a connection to the database
            using (SqlConnection connection = new SqlConnection("Data Source=MAGE_PACA\\SQLEXPRESS;Initial Catalog=bakery;Integrated Security=True;Encrypt=False"))
            {
                // Open the connection
                connection.Open();

                // Create a transaction to ensure data consistency
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert data into the Orders table
                        string queryOrders = @"
                    INSERT INTO Orders (OrderID, CustomerID, OrderDate)
                    VALUES (@OrderID, @CustomerID, @OrderDate);
                ";

                        using (SqlCommand command = new SqlCommand(queryOrders, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@OrderID", int.Parse(txtOrderID.Text));
                            command.Parameters.AddWithValue("@CustomerID", int.Parse(txtCustomerID.Text));
                            
                            command.Parameters.AddWithValue("@OrderDate", DateTime.Parse(txtOrderDate.Text));

                            command.ExecuteNonQuery();
                        }

                        // Insert data into the OrderDetails table
                        string queryOrderDetails = @"
                    INSERT INTO OrderDetails (OrderID, ProductID, Quantity)
                    VALUES (@OrderID, @ProductID, @Quantity);
                ";

                        using (SqlCommand command = new SqlCommand(queryOrderDetails, connection, transaction))
                        {
                            command.Parameters.AddWithValue("@OrderID", int.Parse(txtOrderID.Text));
                            command.Parameters.AddWithValue("@ProductID", int.Parse(txtProductID.Text));
                            command.Parameters.AddWithValue("@Quantity", int.Parse(txtQuantity.Text));
                           

                            command.ExecuteNonQuery();
                        }

                        // Commit the transaction
                        transaction.Commit();

                        // Display a success message
                        MessageBox.Show("Data inserted successfully!");
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if an error occurs
                        transaction.Rollback();

                        // Display an error message
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
            }
        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }
}
