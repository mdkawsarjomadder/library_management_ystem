using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace library_management_ystem.ViewModels
{
    public class ChangePasswordViewModel
    {
       [Required]
       [DataType(DataType.Password)]
       [Display(Name ="Current Password")]
       public string CurrentPassword { get; set; } = string.Empty;

       [Required]
        [StringLength(10, MinimumLength = 8,
            ErrorMessage = "Password must be between 8 and 10 characters.")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")] 
        public string NewPassword { get; set; } = string.Empty;

         [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword",
            ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}