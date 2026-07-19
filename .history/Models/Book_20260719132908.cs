using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
public class Book
{
    public int Id {get; set;}
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(50, ErrorMessage = "Title cannot exceed 50 characters.")]
        [RegularExpression(
        @"^[A-Z][A-Za-z0-9\s,.\-]*$",
        ErrorMessage = "Title must start with an uppercase letter."
        )]
     public string Title {get; set;} = string.Empty;

        [Required(ErrorMessage = "ISBN is required.")]
        [StringLength(5, MinimumLength = 5,
        ErrorMessage = "ISBN must be exactly 5 digits.")]
        [RegularExpression(@"^\d{5}$",
        ErrorMessage = "ISBN must contain exactly 5 digits.")]
    public string ISBN {get; set;} = string.Empty;

        [Required(ErrorMessage ="Total Copies is reuired")]
        [Range(1,100,ErrorMessage ="Total Copies must be between 1 and 100")]
    public int TotalCopies { get; set; }

        [Required(ErrorMessage ="Available Copies is required.")]
        [Range(1,100,ErrorMessage ="Available Copies must be between 1 and 100")]
    public int AvailableCopies  { get; set; }


     // Foreign key................

       [Range(1, int.MaxValue, ErrorMessage = "Please select a Category.")]
    public int CategoryId { get; set; }

       [Range(1, int.MaxValue, ErrorMessage = "Please select an Author.")] public int CategoryCount { get; set;}   
    public int AuthorId {get; set;}
    

       [Range(1, int.MaxValue, ErrorMessage = "Please select a Publisher.")]
    public int PublisherId { get; set; }

    //Navigation properties....................?

     public Category? Category {get; set;}
     public Author? Author { get; set; }
     public Publisher? Publisher {get; set;}
    
    


}
}