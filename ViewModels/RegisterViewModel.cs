using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

         [Required(ErrorMessage = "Password is required.")]
        [StringLength(10,
        MinimumLength = 8,
        ErrorMessage = "Password must be between 8 and 10 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password",
        ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; } = "";
    }
}