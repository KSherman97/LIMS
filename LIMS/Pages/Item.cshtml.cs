using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;


namespace LIMS
{
    public class ItemModel : PageModel
    {
        public string Title { get; private set; }
        public string Author { get; private set; }
        public string Genre { get; private set; }
        public DateTime DatePublished { get; private set; }
        public string ISBN { get; private set; }
        public string Summary { get; private set; } 
        public string ImagePath { get; private set; }
        public string Log { get; private set; }
        public bool HasAvailableCopies { get; private set; }
        public bool HasBookReserved { get; private set; }
        public class Review
        {
            public Review() { }
            public Review(string reviewText, string username, int rating) 
            {
                this.ReviewText = reviewText;
                this.Username = username;
                this.Rating = rating;
            }
            public string ReviewText { get; set; }
            public string Username { get; set; }
            public int Rating { get; set; }
        }

        public List<Review> Reviews { get; private set; } = new List<Review>();

        public class ItemForm
        {
            public string Action { get; set; }
            public string ISBN { get; set; }
            public string Data { get; set; }
        }

        public JsonResult HandleReserve(ItemForm form)
        {
            // TODO: Check if the user has any books already reserved
            var success = false;
            var isUnavailable = false;
            try
            {
                var handler = new ConnectionHandler();
                using (MySqlConnection connection = handler.Connection)
                {
                    MySqlTransaction trans = connection.BeginTransaction();

                    string sql = "SELECT * FROM BookDetails WHERE ISBN=@ISBN AND availability='available'";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    cmd.Transaction = trans;

                    cmd.Parameters.AddWithValue("@ISBN", form.ISBN);
                    MySqlDataReader rdr = cmd.ExecuteReader();

                    if (rdr.Read())
                    {
                        int bookId = (int)rdr[0];
                        rdr.Close();

                        cmd.CommandText = "UPDATE BookDetails SET availability='reserved' WHERE bookId=@BOOKID";
                        cmd.Parameters.AddWithValue("@BOOKID", bookId);
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "INSERT INTO Reservations(userId, bookId, dateReserved) VALUES (@USERID, @BOOKID, CURDATE())";
                        cmd.Parameters.AddWithValue("@USERID", HttpContext.Session.GetString("userId"));
                        cmd.ExecuteNonQuery();
                        trans.Commit();
                        success = true;
                    }
                    else
                    {
                        isUnavailable = true;
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine(ex);
            }
            var reason = "unknown";
            if (success)
            {
                reason = "none";
            } 
            else if (isUnavailable)
            {
                reason = "There is no available copy of the book";
            }
            return new JsonResult($"{{\"success\":\"{success}\"," +
                                    $"\"reason\":\"{reason}\"}}", new System.Text.Json.JsonSerializerOptions());
        }

        public JsonResult HandleReview(ItemForm form)
        {
            // TODO: Check if the user has any books already reserved
            var success = false;
            try
            {
                var handler = new ConnectionHandler();
                using (MySqlConnection connection = handler.Connection)
                {
                    string sql = "SELECT reviewId FROM UserReviews WHERE ISBN=@ISBN AND userId=@USERID";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);

                    cmd.Parameters.AddWithValue("@ISBN", form.ISBN);
                    cmd.Parameters.AddWithValue("@USERID", HttpContext.Session.GetString("userId"));
                    int reviewId = -1;

                    using (MySqlDataReader rdr = cmd.ExecuteReader()) {
                        if (rdr.Read())
                        {
                            reviewId = (int)rdr[0];
                        }
                    }

                    // if there is already a review of this book from the user, update it
                    if (reviewId != -1)
                    {
                        cmd.CommandText = "UPDATE UserReviews SET reviewText=@REVIEWTEXT WHERE reviewId=@REVIEWID";
                        cmd.Parameters.AddWithValue("@REVIEWTEXT", form.Data);
                        cmd.Parameters.AddWithValue("@REVIEWID", reviewId);
                        cmd.ExecuteNonQuery();
                        success = true;
                    } 
                    else
                    {
                        cmd.CommandText = "INSERT INTO UserReviews(userId, ISBN, rating, reviewText) VALUES (@USERID, @ISBN, '0', @REVIEWTEXT)";
                        cmd.Parameters.AddWithValue("@REVIEWTEXT", form.Data);
                        cmd.ExecuteNonQuery();
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine(ex);
            }

            var reason = "unknown";
            if (success)
            {
                reason = "none";
            }
            return new JsonResult($"{{\"success\":\"{success}\"," +
                                    $"\"reason\":\"{reason}\"}}", new System.Text.Json.JsonSerializerOptions());
        }

        public JsonResult OnPost([FromBody]ItemForm form)
        {
            JsonResult res = new JsonResult("{\"success\":\"False\"}", new System.Text.Json.JsonSerializerOptions());

            Console.WriteLine("Item Form Post Action: " + form.Action);
            Console.WriteLine($"request: {{action: {form.Action}, data:{form.Data}}}");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Item Form Post: Invalid model state!");
            }
            else if (form.Action == "reserve")
            {
                res = HandleReserve(form);
            }
            else if (form.Action == "review")
            {
                res = HandleReview(form);
            }

            return res;
        }


        public void OnGet(string isbn)
        {
            if (isbn == null)
            {
                ISBN = "0000000000000";
            } else
            {
                ISBN = isbn;
            }

            Console.WriteLine("ITEM Page ISBN=" + isbn);

            try
            {
                var handler = new ConnectionHandler();
                MySqlConnection connection = handler.Connection;
                string sql = "SELECT * FROM Books WHERE ISBN=@ISBN";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@ISBN", ISBN);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        //this.ISBN = (string) rdr[0];
                        Title = (string)rdr[1];
                        Genre = (string)rdr[2];
                        Author = (string)rdr[3];
                        Summary = (string)rdr[4];
                        //DatePublished = (DateTime)rdr[5];
                        ImagePath = (string)rdr[6];
                    }
                }

                Console.WriteLine("ITEM Page ImagePath=" + ImagePath);

                cmd.CommandText =   "SELECT reviewText, username, rating " +
                                    "FROM UserReviews " +
                                    "INNER JOIN Users " +
                                    "ON UserReviews.userId=Users.userId AND ISBN=@ISBN";

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Review review = new Review((string)rdr[0], (string)rdr[1], (int)rdr[2]);
                        Reviews.Add(review);
                    }
                }
                cmd.CommandText = "SELECT * FROM BookDetails WHERE ISBN=@ISBN AND availability='available'";

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        HasAvailableCopies = true;
                    }
                    else
                    {
                        HasAvailableCopies = false;
                    }
                }

                cmd.CommandText = "SELECT * FROM Reservations WHERE userId=@USERID";
                cmd.Parameters.AddWithValue("@USERID", HttpContext.Session.GetString("userId"));

                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    if (rdr.HasRows)
                    {
                        HasBookReserved = true;
                    }
                    else
                    {
                        HasBookReserved= false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}