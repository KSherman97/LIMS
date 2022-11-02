using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using System.Text;
using System.Diagnostics;

namespace LIMS
{
    public class RegisterModel : PageModel
    {
        public void OnGet()
        {

        }

        public class RegistrationForm
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Username { get; set; }
            public string Password1 { get; set; }
            public string Password2 { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Phone { get; set; }
            public string Zip { get; set; }
        }


        public JsonResult OnPost([FromBody]RegistrationForm form)
        {
            var hasBadField = false;
            var hasDifPass = false;
            var isUnameTaken = false;
            var errorMessage = "";
            var success = false;
            try
            {
                if (form.FirstName == null || form.LastName == null || form.Username == null 
                    || form.Password1 == null || form.Password2 == null || form.Address == null 
                    || form.City == null || form.State == null || form.Zip == null || form.Phone == null)
                {
                    hasBadField = true;
                }

                // these should be validated client side as well
                if (form.Username.Length < 6 || form.Zip.Length != 5 || form.Phone.Length != 10 
                    || form.Address.Length < 2 || form.State.Length != 2 || form.City.Length < 2
                    || form.FirstName.Length < 1 || form.LastName.Length < 1)
                {
                    hasBadField = true;
                }

                if (form.Password1 != form.Password2 || form.Password1.Length < 6)
                {
                    hasDifPass = true;
                }

                var handler = new ConnectionHandler();
                string sql = "SELECT * FROM Users WHERE username=@USERNAME";
                MySqlCommand select_username = new MySqlCommand(sql, handler.Connection);
                select_username.Parameters.AddWithValue("@USERNAME", form.Username);

                using (MySqlDataReader rdr = select_username.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        isUnameTaken = true;
                    }
                }

                if (!isUnameTaken && !hasBadField && !hasDifPass)
                {
                    using (MySqlTransaction trans = handler.Connection.BeginTransaction())
                    {
                        Console.WriteLine("Register: No Error in fields");
                        // create a random salt to hash the password with to
                        // guard against rainbow tables and other site breaches
                        var rand = new Random((int)Stopwatch.GetTimestamp());
                        SHA256 hash = SHA256.Create();
                        var saltBytes = hash.ComputeHash(Encoding.ASCII.GetBytes(form.Username + rand.Next().ToString()));
                        var salt = Encoding.ASCII.GetString(saltBytes);
                        var hashedPasswordBytes = hash.ComputeHash(Encoding.ASCII.GetBytes(form.Password1 + salt));
                        var hashedPassword = Encoding.ASCII.GetString(hashedPasswordBytes);

                        sql = "INSERT INTO Users(username,password,salt,accountType,"
                                    + "firstName,lastName,address,city,zip,state,phone) " +
                              "VALUES (@USERNAME, @PASSWORD, @SALT, 'user', "
                                    + "@FIRSTNAME, @LASTNAME, @ADDRESS, @CITY, @ZIP, @STATE, @PHONE)";

                        MySqlCommand insert_user = new MySqlCommand(sql, trans.Connection);
                        
                        insert_user.Parameters.AddWithValue("@USERNAME", form.Username);
                        insert_user.Parameters.AddWithValue("@PASSWORD", hashedPassword);
                        insert_user.Parameters.AddWithValue("@SALT", salt);
                        insert_user.Parameters.AddWithValue("@FIRSTNAME", form.FirstName);
                        insert_user.Parameters.AddWithValue("@LASTNAME", form.LastName);
                        insert_user.Parameters.AddWithValue("@ADDRESS", form.Address);
                        insert_user.Parameters.AddWithValue("@CITY", form.City);
                        insert_user.Parameters.AddWithValue("@ZIP", form.Zip);
                        insert_user.Parameters.AddWithValue("@STATE", form.State);
                        insert_user.Parameters.AddWithValue("@PHONE", form.Phone);
                        insert_user.ExecuteNonQuery();
                        trans.Commit();
                        success = true;
                    }
                } 
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
                success = false;
                Console.WriteLine(ex);
            }

            return new JsonResult($"{{\"hasBadField\":\"{hasBadField}\","
                                  + $"\"hasDifferentPasswords\":\"{hasDifPass}\","
                                  + $"\"isUsernameTaken\":\"{isUnameTaken}\","
                                  + $"\"success\":\"{success}\"}}", new System.Text.Json.JsonSerializerOptions());
        }
    }
}