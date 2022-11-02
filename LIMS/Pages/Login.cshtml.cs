using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Web;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace LIMS
{
    public class LoginModel : PageModel
    {
        public class LoginForm
        {
            public string Action { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public string Action { get; set; }
        // Is the login information valid?
        public bool IsValid { get; private set; } = false;

        public void OnGet(string action)
        {
            this.Action = action;
            if (action == "logout")
            {
                HttpContext.Session.Clear();
            }
            else if (action == "register")
            {
                // Nothing to do currently
            }
        }

        public JsonResult OnPost([FromBody]LoginForm form)
        {
            Console.WriteLine("Login attempt...");
            var success = false;
            var reason = "unknown";

            if (form.Username != null && form.Password != null)
            {
                try
                {
                    var handler = new ConnectionHandler();
                    MySqlConnection connection = handler.Connection;
                    string sql = "SELECT userid, password, salt, firstName, accountType FROM Users WHERE username=@USERNAME";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    cmd.Parameters.AddWithValue("@USERNAME", form.Username);
                    using MySqlDataReader rdr = cmd.ExecuteReader();
                    if (rdr.Read())
                    {
                        var userid = rdr[0].ToString();
                        var hashedPassword = (string)rdr[1];
                        var salt = (string)rdr[2];
                        var firstName = (string)rdr[3];
                        var accountType = (string)rdr[4];
                        SHA256 hash = SHA256.Create();
                        // Salt the Password, convert it to a byte[], compute the hash, then convert back to string for comparison
                        var computedHash = Encoding.ASCII.GetString(hash.ComputeHash(Encoding.ASCII.GetBytes(form.Password + salt)));


                        if (computedHash.ToString() == hashedPassword)
                        {
                            // Set up the user's session
                            HttpContext.Session.SetString("userId", userid);
                            HttpContext.Session.SetString("firstName", firstName);
                            HttpContext.Session.SetString("accountType", accountType);
                            Console.WriteLine("Login attempt Succeeded");
                            success = true; 
                            reason = "none";
                        }
                        else
                        {
                            Console.WriteLine("Login attempt failed: mismatch info!");
                            reason = "incorrect";
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Login attempt failed: exception!");
                    Console.WriteLine(ex);
                }
            }
            else
            {
                reason = "incorrect";
                Console.WriteLine("Login attempt failed: NULL FIELD");
            }

            return new JsonResult($"{{\"success\":\"{success}\"," +
                                    $"\"reason\":\"{reason}\"}}", new System.Text.Json.JsonSerializerOptions());
        }
    }
}