using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LibrayManagementSystemMVC.Models
{
    public class IndexViewModel
    {
        [Key]

        public List<Books> books { get; set; }
        public List<AccountModel> studentinfo { get; set; }
    }
}
        