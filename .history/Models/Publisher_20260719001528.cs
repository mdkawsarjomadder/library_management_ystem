using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class Publisher
    {
        public int Id {get; set;}

        [Required(ErrorMessage = "Publisher name is required.")]
        [StringLength(100, ErrorMessage = "Publisher name cannot exceed 100 characters.")]
        [Display(Name = "Publisher Name")]
        public string Name {get; set;} = string.Empty;
    }
}