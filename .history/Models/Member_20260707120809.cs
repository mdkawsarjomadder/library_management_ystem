using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class Member
    {
        [Key]
        public int Id {get; set;}

        [Required]
        public string Name {get; set;} = string.Empty;
        public string Email {get; set;} = string.Empty;
        public string Phone {get; set;} = string.Empty;
        public string Address {get; set;} = string.Empty;



    }
}