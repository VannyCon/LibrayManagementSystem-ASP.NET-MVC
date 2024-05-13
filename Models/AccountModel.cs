using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LibrayManagementSystemMVC.Models
{
    public class AccountModel
    {
        [Key]
        public int id { get; set; }
        public int student_id { get; set; }
        [Required]
        public string username { get; set; }
        public string password { get; set; }
        public string fullname { get; set; }
        public string authorization { get; set; }

    }
}
        