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

        [Required(ErrorMessage = "Member name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Only letters and spaces are allowed.")]
        [Display(Name = "Name")]
        public string Name {get; set;} = string.Empty;


        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email {get; set;} = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^01[3-9]\d{8}$",
        ErrorMessage = "Enter a valid 11-digit Bangladeshi phone number.")]
        [Display(Name = "Phone Number")]
        public string Phone {get; set;} = string.Empty;


        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        [RegularExpression(
        @"^[A-Z][A-Za-z0-9\s,.\-]*$",
        ErrorMessage = "Address must start with an uppercase letter."
        )]
        [Display(Name = "Address")]
        public string Address {get; set;} = string.Empty;



    }
}