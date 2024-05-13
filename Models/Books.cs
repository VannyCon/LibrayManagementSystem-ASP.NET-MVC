using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LibrayManagementSystemMVC.Models
{
    public class Books
    {
        [Key]
        public int id { get; set; }
        public string book_title { get; set; }
        public string book_author { get; set; }
        public string book_category { get; set; }


    }
}
        