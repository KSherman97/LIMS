using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace LIMS.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public List<CarouselItem> CarouselList { set; get; } = new List<CarouselItem>();

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public class CarouselItem
        {
            public string ImagePath { set; get; }
            public string ISBN { set; get; }
        }

        public void OnGet()
        {
            List<CarouselItem> list = new List<CarouselItem>();
            try
            {
                var handler = new ConnectionHandler();
                using (MySqlConnection connection = handler.Connection)
                {

                    string sql = "SELECT isbn, imagePath FROM Books";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);

                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read()) 
                        { 
                            CarouselItem item = new CarouselItem();
                            item.ISBN = rdr[0].ToString();
                            item.ImagePath = rdr[1].ToString();
                            list.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Random rand = new Random((int)DateTime.Now.Ticks);
            
            // Randomly grab 3 books to display in the carousel
            for (int i = 0; i < 3; i++)
            {
                if (list.Count > 0)
                {
                    var index = rand.Next(0,list.Count-1);
                    this.CarouselList.Add(list[index]);
                    Console.WriteLine($"isbn: {list[index].ISBN}, imagePath: {list[index].ImagePath}");
                    list.RemoveAt(index);
                }
            }
            
            // if there were not enough items, add placeholders
            while (this.CarouselList.Count < 3)
            {
                CarouselItem item = new CarouselItem();
                item.ISBN = "0000000000000";
                item.ImagePath = "placeholder.gif";
                this.CarouselList.Add(item);
            }
        }
    }
}
