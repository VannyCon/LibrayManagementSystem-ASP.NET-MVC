﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace LibrayManagementSystemMVC.Models
{
    public class AccountModel
    {


        /// <summary>
        /// ex.     public int id { get; set; } and   public int? student_id { get; set; } are collumn of the table
        /// [KEY] is for the Uniqueness
        /// [REQUIRED] Which is need to be have a value
        /// </summary>
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
        