using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using WebAPI.Models;

namespace WebAPI.Models
{
    public class DataAccessLayer:IProductRepository
    {
        public IConfiguration Configuration { get; }
        public string connectionString;
        public DataAccessLayer(IConfiguration configuration)
        {
            this.Configuration = configuration;
            connectionString = Configuration["ConnectionStrings:DefaultConnection"];
        }

        // Method to fetch active products from the database
        public IEnumerable<Product> ActiveProducts()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Products WHERE IsActive = 1 ORDER BY PostedDate DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                ProductName = reader["ProductName"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                PostedDate = Convert.ToDateTime(reader["PostedDate"]),
                                IsActive = Convert.ToInt32(reader["IsActive"])
                            };
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }

        // Method to search products based on search criteria
        public IEnumerable<Product> SearchProduct(string productName, decimal? minPrice, decimal? maxPrice, DateTime? startDate, DateTime? endDate)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Build the query dynamically based on the search criteria
                StringBuilder queryBuilder = new StringBuilder("SELECT * FROM Products WHERE IsActive = 1");

                if (!string.IsNullOrEmpty(productName))
                    queryBuilder.Append(" AND ProductName LIKE '%' + @ProductName + '%'");

                if (minPrice.HasValue)
                    queryBuilder.Append(" AND Price >= @MinPrice");

                if (maxPrice.HasValue)
                    queryBuilder.Append(" AND Price <= @MaxPrice");

                if (startDate.HasValue)
                    queryBuilder.Append(" AND PostedDate >= @StartDate");

                if (endDate.HasValue)
                    queryBuilder.Append(" AND PostedDate <= @EndDate");

                queryBuilder.Append(" ORDER BY PostedDate DESC");

                using (SqlCommand command = new SqlCommand(queryBuilder.ToString(), connection))
                {
                    if (!string.IsNullOrEmpty(productName))
                        command.Parameters.AddWithValue("@ProductName", productName);

                    if (minPrice.HasValue)
                        command.Parameters.AddWithValue("@MinPrice", minPrice.Value);

                    if (maxPrice.HasValue)
                        command.Parameters.AddWithValue("@MaxPrice", maxPrice.Value);

                    if (startDate.HasValue)
                        command.Parameters.AddWithValue("@StartDate", startDate.Value);

                    if (endDate.HasValue)
                        command.Parameters.AddWithValue("@EndDate", endDate.Value);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                ProductName = reader["ProductName"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                PostedDate = Convert.ToDateTime(reader["PostedDate"]),
                                IsActive = Convert.ToInt32(reader["IsActive"])
                            };
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }

        // Method to create a new product in the database
        public bool CreateProduct(string productName, decimal price)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Products (ProductName, Price, PostedDate, IsActive) VALUES (@ProductName, @Price, getdate(), 1)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductName", productName);
                    command.Parameters.AddWithValue("@Price", price);

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        // Method to update an existing product in the database
        public bool UpdateProduct(int productId, string productName, decimal price)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Products SET ProductName = @ProductName, Price = @Price, PostedDate = getdate() WHERE ProductId = @ProductId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    command.Parameters.AddWithValue("@ProductName", productName);
                    command.Parameters.AddWithValue("@Price", price);

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        // Method to delete a product from the database
        public bool DeleteProduct(int productId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Products SET IsActive=2,RequestType='Delete' WHERE ProductId = @ProductId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        // Method to fetch products in the approval queue
        public IEnumerable<ApprovalQueueItem> GetApprovalQueue()
        {
            List<ApprovalQueueItem> queueItems = new List<ApprovalQueueItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM ApprovalQueue ORDER BY RequestDate";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ApprovalQueueItem queueItem = new ApprovalQueueItem
                            {
                                RequestId = Convert.ToInt32(reader["RequestId"]),
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                RequestType = reader["RequestType"].ToString(),
                                RequestReason = reader["RequestReason"].ToString(),
                                RequestDate = Convert.ToDateTime(reader["RequestDate"])
                            };
                            queueItems.Add(queueItem);
                        }
                    }
                }
            }

            return queueItems;
        }

        // Method to create a new product in the database
        public bool CreateApprovalProduct(string productName, decimal price)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Products (ProductName, Price, PostedDate, IsActive,RequestType,RequestDate) VALUES (@ProductName, @Price, Getdate(), 2,'New', getdate())";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductName", productName);
                    command.Parameters.AddWithValue("@Price", price);

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        // Method to create a new product in the database
        public bool DeleteApprovalProduct(Product product)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Products (ProductName, Price, PostedDate, IsActive,RequestType,RequestDate) VALUES (@ProductName, @Price, @PostedDate, 2,'Delete', getdate())";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductName", product.ProductName);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@PostedDate", product.PostedDate);

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        // Method to update an existing product in the database
        public bool UpdateApprovalProduct(int productId, string productName, decimal price)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Products SET ProductName = @ProductName,IsActive=2,RequestType='Update',RequestDate=getdate(),RequestPrice=@RequestPrice WHERE ProductId = @ProductId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    command.Parameters.AddWithValue("@ProductName", productName);
                    command.Parameters.AddWithValue("@RequestPrice", price);

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }


        // Method to fetch active products from the database
        public IEnumerable<Product> ActiveProductsEdit(int productId)
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Products WHERE IsActive = 1 and ProductId='"+productId+"' ";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                ProductName = reader["ProductName"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                PostedDate = Convert.ToDateTime(reader["PostedDate"]),
                                IsActive = Convert.ToInt32(reader["IsActive"])
                            };
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }

        // Method to fetch active products from the database
        public IEnumerable<Product> ApprovalProductsList()
        {
            List<Product> products = new List<Product>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Products WHERE IsActive = 2 ORDER BY PostedDate DESC";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                ProductId = Convert.ToInt32(reader["ProductId"]),
                                ProductName = reader["ProductName"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                PostedDate = Convert.ToDateTime(reader["PostedDate"]),
                                IsActive = Convert.ToInt32(reader["IsActive"])
                            };
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }


    }

}