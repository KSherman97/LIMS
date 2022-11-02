using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Http;

namespace LIMS
{
    public class ProfileModel : PageModel
    {

        public string Username { get; private set;}
        public string FirstName { get; private set; }
        public string LastName { get; private set;}
        public string Address { get; private set;}
        public string City { get; private set;}
        public string State { get; private set;}
        public string Zip { get; private set;}
        public string Phone { get; private set;}
        public class ProfileForm
        {
            public string Action { get; set; }
            public string Data { get; set; }
        }


        public JsonResult handleRequest(ProfileForm form)
        {
            var success = false;
            try
            {
                var handler = new ConnectionHandler();
                using (MySqlConnection connection = handler.Connection)
                {
                    string sql = "SELECT requestId FROM BookRequests WHERE ISBN=@ISBN AND userId=@USERID";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);

                    cmd.Parameters.AddWithValue("@ISBN", form.Data);
                    cmd.Parameters.AddWithValue("@USERID", HttpContext.Session.GetString("userId"));
                    int requestId = -1;

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            requestId = (int)rdr[0];
                        }
                    }

                    // if there is already a request of this book from the user, ignore this
                    if (requestId != -1)
                    {
                        success = true;
                    }
                    else
                    {
                        cmd.CommandText = "INSERT INTO BookRequests(userId, ISBN, dateRequested) VALUES (@USERID, @ISBN, CURDATE())";
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

        public JsonResult OnPost([FromBody]ProfileForm form)
        {
            JsonResult res = new JsonResult("{\"success\":\"False\"," +
                                            "\"reason\":\"none\"}", new System.Text.Json.JsonSerializerOptions());
            if (form.Action == "request")
            {
                res = handleRequest(form);
            }

            return res;
        }
        
        public void OnGet()
        {
            try
            {
                var handler = new ConnectionHandler();
                using (MySqlConnection connection = handler.Connection)
                {
                    string sql = "SELECT username, firstName, lastName, address, city, state, zip, phone FROM Users WHERE userId=@USERID";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);

                    cmd.Parameters.AddWithValue("@USERID", HttpContext.Session.GetString("userId"));

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            this.Username = rdr[0].ToString();
                            this.FirstName = rdr[1].ToString();
                            this.LastName = rdr[2].ToString();
                            this.Address = rdr[3].ToString();
                            this.City = rdr[4].ToString();
                            this.State = rdr[5].ToString();
                            this.Zip = rdr[6].ToString();
                            this.Phone = rdr[7].ToString();
                        }
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