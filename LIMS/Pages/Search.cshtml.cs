using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;

namespace LIMS
{
    public class SearchModel : PageModel
    {
        public string Filter { get; private set; }
        public string Search { get; private set; }
        public List<Dictionary<string, string>> Results { get; private set; } = new List<Dictionary<string, string>>();

        public void OnGet(string search, string filter)
        {

            // Manually Sanitize the filter
            // Don't even try to use Parameters with a Column name, trust me! :(
            if (filter == "author" || filter == "genre")
            {
                Filter = filter;
            } else
            {
                Filter = "title";
            }

            if (search == null)
            {
                Search = "";
            } else
            {
                Search = search;
            }

            try
            {
                var handler = new ConnectionHandler();
                MySqlConnection connection = handler.Connection;
                string sql = "SELECT imagePath, title, author, genre, summary, ISBN FROM Books WHERE " + Filter + " LIKE @SEARCH";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@SEARCH", '%' + Search + '%');
                MySqlDataReader rdr = cmd.ExecuteReader();
                
                while (rdr.Read())
                {
                    string summary = (string)rdr[4];
                    if (summary.Length >= 150)
                    {
                        summary = summary.Remove(150, summary.Length-1-150) + "...";
                    }

                    Dictionary<string, string> row = new Dictionary<string, string>
                    {
                        { "imagePath", (string)rdr[0] },
                        { "title", (string)rdr[1] },
                        { "author", (string)rdr[2] },
                        { "genre", (string)rdr[3] },
                        { "summary", summary },
                        { "ISBN", (string)rdr[5] }
                    };
                    Results.Add(row);
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
    }
}