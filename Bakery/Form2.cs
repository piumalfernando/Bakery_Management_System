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
    public partial class Form2 : Form
    {
        private readonly Form1 _previous;
        public Form2(Form1 previous)
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
            int customerID;
            if (!int.TryParse(txtCustomerID.Text, out customerID))
            {
                MessageBox.Show("Please enter a valid customer ID.");
                return;
            }

            string connectionString = "Data Source=MAGE_PACA\\SQLEXPRESS;Initial Catalog=bakery;Integrated Security=True;Encrypt=False";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("GetCustomerName", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CustomerID", customerID);
                    command.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50);
                    command.Parameters["@FirstName"].Direction = ParameterDirection.Output;
                    command.Parameters.Add("@LastName", SqlDbType.NVarChar, 50);
                    command.Parameters["@LastName"].Direction = ParameterDirection.Output;
                    command.ExecuteNonQuery();
                    txtFirstName.Text = command.Parameters["@FirstName"].Value.ToString();
                    txtLastName.Text = command.Parameters["@LastName"].Value.ToString();
                }

                
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT dbo.AverageOrderValue(@CustomerID) AS AverageOrderValue";
                    command.Parameters.AddWithValue("@CustomerID", customerID);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtAverageOrderValue.Text = reader["AverageOrderValue"].ToString();
                        }
                    }
                }
            }


        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Get the ProductID from the textbox
            int productID;
            int quantity;
            if (int.TryParse(txtProductID.Text, out productID) && int.TryParse(txtQuantity.Text, out quantity))
            {
                // Call the stored procedure and get the ProductName
                string productName = GetProductName(productID);

                // Display the ProductName in the textbox
                txtProductName.Text = productName;

                // Call the function and get the isInStock value
                string isInStock = IsProductInStock(productID, quantity);

                // Display the isInStock value in the textbox
                txtIsInStock.Text = isInStock;
            }
            else
            {
                MessageBox.Show("Please enter a valid ProductID.");
            }
        }

        public string GetProductName(int productID)
        {
            string connectionString = "Data Source=MAGE_PACA\\SQLEXPRESS;Initial Catalog=bakery;Integrated Security=True;Encrypt=False";
            string procedureName = "GetProductName";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(procedureName, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.Add("@ProductName", SqlDbType.NVarChar, 50);
                command.Parameters["@ProductName"].Direction = ParameterDirection.Output;

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

                return command.Parameters["@ProductName"].Value.ToString();
            }
        }

        public string IsProductInStock(int productID, int quantity)
        {
            string connectionString = "Data Source=MAGE_PACA\\SQLEXPRESS;Initial Catalog=bakery;Integrated Security=True;Encrypt=False";
            string functionName = "IsProductInStock";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(functionName, connection);
                command.CommandType = CommandType.Text;
                command.CommandText = $"SELECT dbo.{functionName}(@ProductID, @Quantity) AS IsInStock";
                command.Parameters.AddWithValue("@ProductID", productID);
                command.Parameters.AddWithValue("@Quantity", quantity);

                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return reader.GetString(0);
                    }
                }
                connection.Close();
            }

            return "Out of Stock";
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
    }

