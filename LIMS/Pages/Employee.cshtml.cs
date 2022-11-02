using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;


namespace LIMS
{
    public class EmployeeModel : PageModel
    {

        public List<BookInfo> BookInfoList { get; private set; } = new List<BookInfo>();
        public List<UserInfo> UserInfoList { get; private set; } = new List<UserInfo>();

        public class EmployeeForm
        {
            public string Action { get; set; }
            public string Data { get; set; }
        }

        public class CheckoutForm
        {
            [JsonPropertyName("bookId")]
            public string BookId { get; set; }

            [JsonPropertyName("username")]
            public string Username { get; set; }
        }

        public class BookForm
        {
            [JsonPropertyName("isbn")]
            public string ISBN { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }
            
            [JsonPropertyName("genre")]
            public string Genre { get; set; }

            [JsonPropertyName("author")]
            public string Author { get; set; }

            [JsonPropertyName("summary")]
            public string Summary { get; set; }

            [JsonPropertyName("imageName")]
            public string ImageName { get; set; }

            [JsonPropertyName("image")]
            public string Image { get; set; }
        }

        public class CopyForm
        {
            [JsonPropertyName("isbn")]
            public string ISBN { get; set; }
            
            [JsonPropertyName("condition")]
            public string Condition { get; set; }
            
            [JsonPropertyName("location")]
            public string Location { get; set; }
        }

        public class BookInfo
        {
            public BookInfo() { }
            public BookInfo(string bookId, string isbn,
                string condition, string location, string availability)
            {
                this.BookId = bookId;
                this.ISBN = isbn;
                this.Condition = condition;
                this.Location = location;
                this.Availability = availability;
            }
            public string BookId { get; set; }
            public string ISBN { get; set; }
            public string Condition { get; set; }
            public string Location { get; set; }
            public string Availability { get; set; }

        }

        public class UserInfo
        {
            public UserInfo() { }
            public UserInfo(string userId, string username, 
                string firstName, string lastName, string address) 
            {
                this.UserId = userId;
                this.Username = username;
                this.FirstName = firstName;
                this.LastName = lastName;
                this.Address = address;
            }
            public string UserId { get; set; }
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }

        }

        public JsonResult HandleCheckout([FromBody]EmployeeForm eform)
        {
            var success = false;
            var reason = "unknown";

            Console.WriteLine(eform.Data);

            CheckoutForm form = JsonSerializer.Deserialize<CheckoutForm>(eform.Data);

            Console.WriteLine(form.BookId);
            Console.WriteLine(form.Username);

            try
            {
                var handler = new ConnectionHandler();
                using (MySqlConnection connection = handler.Connection)
                {
                    MySqlTransaction trans = connection.BeginTransaction();

                    string sql = "SELECT userId FROM Users WHERE username=@USERNAME";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    cmd.Transaction = trans;

                    cmd.Parameters.AddWithValue("@USERNAME", form.Username);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    // if the username exists
                    if (rdr.Read())
                    {
                        int userId = (int)rdr[0];
                        rdr.Close();

                        cmd.CommandText = "SELECT * FROM BookHistory WHERE userId=@USERID AND dateReturned IS NULL";
                        cmd.Parameters.AddWithValue("@USERID", userId);
                        rdr = cmd.ExecuteReader();

                        // User does not have a book already checkedout
                        if (!rdr.Read())
                        {
                            rdr.Close();

                            cmd.CommandText = "SELECT availability FROM BookDetails WHERE bookId=@BOOKID";
                            cmd.Parameters.AddWithValue("@BOOKID", form.BookId);
                            rdr = cmd.ExecuteReader();

                            if (rdr.Read())
                            {

                                if (rdr[0].ToString() == "available")
                                {
                                    rdr.Close();
                                    cmd.CommandText = "INSERT INTO BookHistory(userId, bookId, dateCheckout) VALUES (@USERID, @BOOKID, CURDATE())";
                                    cmd.ExecuteNonQuery();
                                    cmd.CommandText = "UPDATE BookDetails SET availability='checkedout' WHERE bookId=@BOOKID";
                                    cmd.ExecuteNonQuery();
                                    trans.Commit();
                                    success = true;
                                }
                                else if (rdr[0].ToString() == "reserved")
                                {
                                    rdr.Close();
                                    cmd.CommandText= "SELECT userId FROM Reservations WHERE bookId=@BOOKID";
                                    rdr = cmd.ExecuteReader();
                                    if (rdr.Read())
                                    {
                                        if ((int)rdr[0] == userId)
                                        {
                                            rdr.Close();
                                            cmd.CommandText = "INSERT INTO BookHistory(userId, bookId, dateCheckout) VALUES (@USERID, @BOOKID, CURDATE())";
                                            cmd.ExecuteNonQuery();
                                            cmd.CommandText = "UPDATE BookDetails SET availability='checkedout' WHERE bookId=@BOOKID";
                                            cmd.ExecuteNonQuery();
                                            cmd.CommandText = "DELETE FROM Reservations WHERE bookId=@BOOKID";
                                            cmd.ExecuteNonQuery();
                                            trans.Commit();
                                            success = true;
                                        }
                                        else
                                        {
                                            rdr.Close();
                                            reason = "reserved";
                                        }
                                    } else
                                    {
                                        // INTERNAL PROBLEM - Book is reserved but there is no row in Reservations
                                        reason = "unknown";
                                    }
                                }
                                else
                                {
                                    rdr.Close();
                                    reason = "unavailable";
                                }

                            }
                            else
                            {
                                rdr.Close();
                                reason = "badBookId";
                            }
                        }
                        else
                        {
                            rdr.Close();
                            reason = "multipleCheckout";
                        }
                    }
                    else
                    {
                        rdr.Close();
                        reason = "badUsername";
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine(ex);
            }
            if (success)
            {
                reason = "none";
            }

            return new JsonResult($"{{\"success\":\"{success}\"," +
                                    $"\"reason\":\"{reason}\"}}", new System.Text.Json.JsonSerializerOptions());
        }


        public JsonResult HandleAddBook(EmployeeForm eform)
        {
            var success = false;
            var reason = "unknown";

            Console.WriteLine(eform.Data);


            BookForm form = JsonSerializer.Deserialize<BookForm>(eform.Data);
            try
            {
                var absoluteImagePath = Path.GetFullPath(".") + "\\wwwroot\\images\\" + form.ImageName;
                var imageBase64 = form.Image.Replace("data:image/jpeg;base64,", "");
                
                
                var handler = new ConnectionHandler();
                using (MySqlConnection connection = handler.Connection)
                {
                    string sql = "SELECT * FROM Books WHERE ISBN=@ISBN";

                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@ISBN", form.ISBN);

                    var hasBookAlready = false;

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            hasBookAlready = true;
                        }
                    }
                    
                    if (!hasBookAlready)
                    {
                        cmd.CommandText = "INSERT INTO Books(ISBN,title,genre,author,summary,imagePath) VALUES (@ISBN,@TITLE,@GENRE,@AUTHOR,@SUMMARY,@IMAGEPATH)";
                        cmd.Parameters.AddWithValue("@TITLE", form.Title);
                        cmd.Parameters.AddWithValue("@GENRE", form.Genre);
                        cmd.Parameters.AddWithValue("@AUTHOR", form.Author);
                        cmd.Parameters.AddWithValue("@SUMMARY", form.Summary);
                        cmd.Parameters.AddWithValue("@IMAGEPATH", form.ImageName);
                        cmd.ExecuteNonQuery();

                        System.IO.File.WriteAllBytes(absoluteImagePath, Convert.FromBase64String(imageBase64));
                        success = true;
                    }
                    else
                    {
                        reason = "bookExists";
                    }
                }

            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine(ex);
            }
            if (success)
            {
                reason = "none";
            }

            return new JsonResult($"{{\"success\":\"{success}\"," +
                                    $"\"reason\":\"{reason}\"}}", new System.Text.Json.JsonSerializerOptions());
        }

        public JsonResult HandleAddCopy(EmployeeForm eform)
        {
            var success = false;
            var reason = "unknown";

            Console.WriteLine(eform.Data);

            CopyForm form = JsonSerializer.Deserialize<CopyForm>(eform.Data);
            try
            {
                var handler = new ConnectionHandler();
                using (MySqlConnection connection = handler.Connection)
                {
                    string sql = "INSERT INTO BookDetails(ISBN, bookCondition, location, availability) " +
                                 "VALUES (@ISBN, @CONDITION, @LOCATION, 'available')";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);

                    cmd.Parameters.AddWithValue("@ISBN", form.ISBN);
                    cmd.Parameters.AddWithValue("@CONDITION", form.Condition);
                    cmd.Parameters.AddWithValue("@LOCATION", form.Location);
                    cmd.ExecuteNonQuery();

                    success = true;
                }

            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine(ex);
            }
            if (success)
            {
                reason = "none";
            }

            return new JsonResult($"{{\"success\":\"{success}\"," +
                                    $"\"reason\":\"{reason}\"}}", new System.Text.Json.JsonSerializerOptions());
        }

        public JsonResult OnPost([FromBody]EmployeeForm form)
        {
            JsonResult res = new JsonResult("{\"success\":\"False\"," +
                                            "\"reason\":\"none\"}", new System.Text.Json.JsonSerializerOptions());
            
            Console.WriteLine($"Employee Form Action: {form.Action}");

            if (form.Action == "checkout")
            {
                res = HandleCheckout(form);
            }
            else if (form.Action == "addBook")
            {
                res = HandleAddBook(form);
            }
            else if (form.Action == "addCopy")
            {
                res = HandleAddCopy(form);
            }
            return res;
        }

        public void FetchAllUsers()
        {
            UserInfoList = new List<UserInfo>();
            try
            {
                var handler = new ConnectionHandler();
                using (MySqlConnection connection = handler.Connection)
                {
                    string sql = "SELECT * FROM Users";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string userId = rdr[0].ToString();
                            string username = rdr[1].ToString();
                            string firstName = rdr[5].ToString();
                            string lastName = rdr[6].ToString();
                            string address = rdr[7].ToString() + ", " + rdr[8].ToString() + ", " + rdr[9].ToString() + ", " + rdr[10].ToString();
                            UserInfo info = new UserInfo(userId, username, firstName, lastName, address);
                            UserInfoList.Add(info);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void FetchAllBooks()
        {
            BookInfoList = new List<BookInfo>();
            try
            {
                var handler = new ConnectionHandler();
                using (MySqlConnection connection = handler.Connection)
                {
                    string sql = "SELECT * FROM BookDetails";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            string bookId = rdr[0].ToString();
                            string isbn = rdr[1].ToString();
                            string condition = rdr[2].ToString();
                            string location = rdr[3].ToString();
                            string availability = rdr[4].ToString();
                            BookInfo info = new BookInfo(bookId, isbn, condition, location, availability);
                            BookInfoList.Add(info);
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
            FetchAllUsers();
            FetchAllBooks();
        }
    }
}