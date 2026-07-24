using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.ViewModels
{
    public class EditProfileViewModel
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone Number is required.")]
    [Display(Name = "Phone Number")]
    [RegularExpression(@"^01[3-9]\d{8}$",
        ErrorMessage = "Phone Number must be 11 digits and start with 01.")]
    public string PhoneNumber { get; set; } = string.Empty;
  }
 }