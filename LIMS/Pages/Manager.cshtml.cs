using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;


namespace LIMS
{
    public class ManagerModel : PageModel
    {
        public List<BookRequest> BookRequestList { get; private set; } = new List<BookRequest>();
        public List<Order> OrderList { get; private set; } = new List<Order>();

        public class ManagerForm
        {
            public string Action { get; set; }
            public string Data { get; set; }

        }

        public class BookRequest
        {
            public BookRequest(string isbn, string userCount)
            {
                this.ISBN = isbn;
                this.UserCount = userCount;
            }
            public string ISBN { get; set; }
            public string UserCount { get; set; }
        }

        public class Order
        {
            public Order() { }

            public Order(string orderId, string isbn, string quantity,
                string dateOrdered, string dateExpected, string dateRecieved)
            {
                this.OrderId = orderId;
                this.ISBN = isbn;
                this.Quantity = quantity;
                this.DateOrdered = dateOrdered;
                this.DateExpected = dateExpected;
                this.DateRecieved = dateRecieved;
            }

            public Order(string orderId, string dateRecieved)
            {
                this.OrderId = orderId;
                this.DateRecieved = dateRecieved;
            }

            public Order(string isbn, string quantity,
                string dateOrdered, string dateExpected)
            {
                this.ISBN = isbn;
                this.Quantity = quantity;
                this.DateOrdered = dateOrdered;
                this.DateExpected = dateExpected;
            }

            [JsonPropertyName("orderId")]
            public string OrderId { get; set; }
            
            [JsonPropertyName("isbn")]
            public string ISBN { get; set; }
            
            [JsonPropertyName("quantity")]
            public string Quantity { get; set; }
            
            [JsonPropertyName("dateOrdered")]
            public string DateOrdered { get; set; }
            
            [JsonPropertyName("dateExpected")]
            public string DateExpected { get; set; }
            
            [JsonPropertyName("dateRecieved")]
            public string DateRecieved { get; set; }
        }


        public void FetchAllBookRequests()
        {
            BookRequestList = new List<BookRequest>();
            try
            {
                var handler = new ConnectionHandler();
                using (MySqlConnection connection = handler.Connection)
                {
                    string sql = "SELECT ISBN, COUNT(userId) as userCount FROM lims.BookRequests GROUP BY ISBN ORDER BY userCount DESC;";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string isbn = rdr[0].ToString();
                            string userCount = rdr[1].ToString();
                            BookRequest request = new BookRequest(isbn, userCount);
                            BookRequestList.Add(request);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void FetchAllOrders()
        {
            OrderList = new List<Order>();
            try
            {
                var handler = new ConnectionHandler();
                using (MySqlConnection connection = handler.Connection)
                {
                    string sql = "SELECT * FROM Orders";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string bookId = rdr[0].ToString();
                            string isbn = rdr[1].ToString();
                            string quantity = rdr[2].ToString();
                            string dateOrdered = rdr[3].ToString();
                            string dateExpected = rdr[4].ToString();
                            string dateRecieved = rdr[5].ToString();
                            Order order = new Order(bookId, isbn, quantity, dateOrdered, dateExpected, dateRecieved);
                            OrderList.Add(order);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void OnGet()
        {
            FetchAllBookRequests();
            FetchAllOrders();
        }

        public JsonResult HandleAddOrder(ManagerForm form)
        {
            var success = false;
            var reason = "unknown";

            Order order = JsonSerializer.Deserialize<Order>(form.Data);

            try
            {
                var handler = new ConnectionHandler();
                using (MySqlConnection connection = handler.Connection)
                {
                    string sql = "INSERT INTO Orders(isbn,quantity,dateOrdered,dateExpected) VALUES (@ISBN,@QUANTITY,@DATEORDERED,@DATEEXPECTED)";

                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@ISBN", order.ISBN);
                    cmd.Parameters.AddWithValue("@QUANTITY", order.Quantity);
                    cmd.Parameters.AddWithValue("@DATEORDERED", order.DateOrdered);
                    cmd.Parameters.AddWithValue("@DATEEXPECTED", order.DateExpected);
                    cmd.ExecuteNonQuery();

                    success = true;
                }

            }
            catch (Exception ex)
            {
                success = false;
                reason = "unknown";
                Console.WriteLine(ex);
            }
            if (success)
            {
                reason = "none";
            }

            return new JsonResult($"{{\"success\":\"{success}\"," +
                                    $"\"reason\":\"{reason}\"}}", new System.Text.Json.JsonSerializerOptions());
        }

        public JsonResult HandleRecieveOrder(ManagerForm form)
        {
            var success = false;
            var reason = "unknown";

            Order order = JsonSerializer.Deserialize<Order>(form.Data);

            try
            {
                var handler = new ConnectionHandler();
                using (MySqlConnection connection = handler.Connection)
                {
                    string sql = "UPDATE Orders SET dateRecieved=@DATERECIEVED WHERE orderId=@ORDERID";

                    MySqlCommand cmd = new MySqlCommand(sql, connection);

                    cmd.Parameters.AddWithValue("@DATERECIEVED", order.DateRecieved);
                    cmd.Parameters.AddWithValue("@ORDERID", order.OrderId);
                    cmd.ExecuteNonQuery();

                    success = true;
                }

            }
            catch (Exception ex)
            {
                success = false;
                reason = "unknown";
                Console.WriteLine(ex);
            }
            if (success)
            {
                reason = "none";
            }

            return new JsonResult($"{{\"success\":\"{success}\"," +
                                    $"\"reason\":\"{reason}\"}}", new System.Text.Json.JsonSerializerOptions());
        }

        public JsonResult OnPost([FromBody]ManagerForm form)
        {
            JsonResult res = new JsonResult("{\"success\":\"False\"," +
                                "\"reason\":\"none\"}", new System.Text.Json.JsonSerializerOptions());

            Console.WriteLine($"Manager Form Action: {form.Action}");

            if (form.Action == "addOrder")
            {
                res = HandleAddOrder(form);
            } 
            else if (form.Action == "recieveOrder")
            {
                res = HandleRecieveOrder(form);
            }

            return res;
        }
    }
}