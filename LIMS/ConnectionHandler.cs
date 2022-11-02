using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace LIMS
{
    public class ConnectionHandler
    {
        // create the properties we will use when loading the data from the json file
        public static string ServerAddress { get; set; }
        public static string ServerPort { get; set; }
        public static string Database { get; set; }
        public static string User { get; set; }
        public static string Password { get; set; }

        public static string ConnectionString { get; private set; }

        public static string _filePath = "DB_Config.json";

        // Since the server is multithreaded, I believe we need multiple unique connection => shouldn't be static
        public MySqlConnection Connection;

        // Default Constructor
        public ConnectionHandler()
        {
            OpenConnection();
        }

        // Finalizer - ensures connection is closed when the object leaves scope
        ~ConnectionHandler()
        {
            if (Connection != null)
            {
                Connection.Close();
            }
        }


        public void ReadJson()
        {
            if (ConnectionString == null)
            {
                try
                {
                    string JsonData;

                    // read the entire json file
                    using (var reader = new StreamReader(_filePath))
                    {
                        JsonData = reader.ReadToEnd();
                    }

                    // parse the json data 
                    dynamic json = JValue.Parse(JsonData);

                    // assigned the parsed data to the specified variable
                    ServerAddress = json.Host;
                    ServerPort = json.Port;
                    Database = json.Database;
                    User = json.User;
                    Password = json.Password;
                    ConnectionString = String.Format("server={0};port={1};uid={2};pwd={3};database={4}", 
                                                    ServerAddress, ServerPort, User, Password, Database);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }


        // this method will allow us to quickly open a connection
        private void OpenConnection()
        {
            if (ConnectionString == null)
            {
                ReadJson();
            }
            Connection = new MySqlConnection(ConnectionString);
            Connection.Open();
        }

        /* I commented this out in case someone needs to use this, currently using a local MySQL Database seems like our best option to *at least* test the project.*/
        //protected static SqlConnection conn;

        //public void readJson()
        //{
        //    try
        //    {
        //        string JsonData;

        //        // read the entire json file
        //        using (var reader = new StreamReader(_filePath))
        //        {
        //            JsonData = reader.ReadToEnd();
        //        }

        //        // parse the json data 
        //        dynamic json = JValue.Parse(JsonData);

        //        // assigned the parsed data to the specified variable
        //        ServerAddress = json.Host;
        //        ServerPort = json.Port;
        //        Database = json.Database;
        //        User = json.User;
        //        Password = json.Password;

        //        connString = 
        //            "Data Source="+ServerAddress+":"+ServerPort+";"+
        //            "Initial Catalog="+Database+";"+
        //            "User id"+User+";"+
        //            "Password"+Password+";";
        //    }
        //    catch (Exception e)
        //    {
        //        // unused
        //    }
        //}


        //// this method will allow us to quickly open a connection
        //public void openConnection()
        //{
        //    // create a new instance
        //    // set the connection string
        //    // then open the connection
        //    conn = new SqlConnection();
        //    conn.ConnectionString = connString;
        //    conn.Open();
        //}

        //// this method will allow us to quickly close a connection
        //public void closeConnection()
        //{   
        //    // if a connection has been defined and it does not contain the closed flag, then we can close the connection
        //    if(conn != null && !conn.State.HasFlag(System.Data.ConnectionState.Closed))
        //    {
        //        conn.Close();
        //    }
        //}

    }
}
