using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LibrayManagementSystemMVC.Models
{
    public class BooksLog
    {

        [Key]
        public int id { get; set; }
        public string? student_fullname { get; set; }

        public int student_id_fk { get; set; }
        public int book_id { get; set; }
        public string? book_title { get; set; }
        public string? book_author { get; set; }
        public string? book_category { get; set; }
        public string book_get_date { get; set; }
        public string book_due_date { get; set; }


    }
}
        