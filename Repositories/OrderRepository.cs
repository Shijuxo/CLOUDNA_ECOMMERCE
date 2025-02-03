using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CLOUDNA_ECOMMERCE.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using CLOUDNA_ECOMMERCE.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly string _connectionString;

    // Constructor to initialize the connection string from the configuration
    public OrderRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // Method to get the most recent order for a customer
    public async Task<OrderResponse> GetMostRecentOrder(string userEmail, string customerId)
    {
        try
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(); // Open a connection to the database

            // Step 1: Validate the customer by checking if the customer ID and email match
            Customer customer = null;
            using (SqlCommand cmd = new SqlCommand("SELECT CustomerId, FirstName, LastName, Email FROM CUSTOMERS WHERE CustomerId = @CustomerId AND Email = @Email", connection))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                cmd.Parameters.AddWithValue("@Email", userEmail);

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    customer = new Customer
                    {
                        CustomerId = reader["CustomerId"].ToString(),
                        FirstName = reader["FirstName"].ToString(),
                        LastName = reader["LastName"].ToString(),
                        Email = reader["Email"].ToString()
                    };
                }
            }

            if (customer == null)
                return null; // If no matching customer is found, return null

            // Step 2: Get the most recent order for the validated customer
            Order order = null;
            using (SqlCommand cmd = new SqlCommand(@"SELECT TOP 1 ORDERID, ORDERDATE, DELIVERYEXPECTED 
                                                     FROM ORDERS 
                                                     WHERE CUSTOMERID = @CustomerId 
                                                     ORDER BY ORDERDATE DESC", connection))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customerId);

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    order = new Order
                    {
                        OrderNumber = Convert.ToInt32(reader["ORDERID"]),
                        OrderDate = Convert.ToDateTime(reader["ORDERDATE"]),
                        DeliveryExpected = Convert.ToDateTime(reader["DELIVERYEXPECTED"]),
                        OrderItems = new List<OrderItem>()
                    };
                }
            }

            if (order == null)
                return new OrderResponse { Customer = customer, Order = null }; // If no orders are found, return the customer info and null order

            // Step 3: Get the items in the order
            using (SqlCommand cmd = new SqlCommand(@"SELECT p.PRODUCTNAME, oi.QUANTITY, oi.PRICE, o.CONTAINSGIFT 
                                                     FROM ORDERITEMS oi
                                                     INNER JOIN PRODUCTS p ON oi.PRODUCTID = p.PRODUCTID
                                                     INNER JOIN ORDERS o ON oi.ORDERID = o.ORDERID
                                                     WHERE oi.ORDERID = @OrderId", connection))
            {
                cmd.Parameters.AddWithValue("@OrderId", order.OrderNumber);

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    var productName = reader["PRODUCTNAME"].ToString();
                    bool containsGift = Convert.ToBoolean(reader["CONTAINSGIFT"]);

                    order.OrderItems.Add(new OrderItem
                    {
                        Product = containsGift ? "Gift" : productName,
                        Quantity = Convert.ToInt32(reader["QUANTITY"]),
                        Price = Convert.ToDecimal(reader["PRICE"])
                    });
                }
            }

            // Step 4: Add a dummy delivery address (you can update this later)
            order.DeliveryAddress = " 2nd floor, Kabani Building, Technopark Phase 4, Technocity, Thiruvananthapuram, Kerala 695317"; // Dummy data for now

            return new OrderResponse { Customer = customer, Order = order }; // Return the customer and their most recent order
        }
        catch (SqlException ex)
        {
            // Step 5: Log SQL specific errors
            Console.WriteLine($"SQL Error: {ex.Message}");
            // Optionally, log to a file or external service like Serilog or NLog
            throw new Exception("An error occurred while fetching the order data. Please try again later.");
        }
        catch (Exception ex)
        {
            // Step 6: Catch any other exceptions
            Console.WriteLine($"General Error: {ex.Message}");
            // Optionally, log to a file or external service
            throw new Exception("An unexpected error occurred. Please try again later.");
        }
    }
}
