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
    
    [Required(ErrorMessage ="Title is reuired")]
    [StringLength(50)]
    public string Title {get; set;} = string.Empty;

    [Required(ErrorMessage ="ISBN is reuired")]
    [StringLength(10, MinimumLength = 3,
                ErrorMessage ="ISBN must be between 3 and 10 characters.")]
    [Display(Name ="ISBN Number")]
    public string ISBN {get; set;} = string.Empty;

    [Required(ErrorMessage ="Total Copies is reuired")]
    [Range(1,100,ErrorMessage ="Total Copies must be between 1 and 100")]
    public int TotalCopies { get; set; }
    [Required(ErrorMessage ="Available Copies is required.")]
     [Range(1,100,ErrorMessage ="Available Copies must be between 1 and 100")]
    public int AvailableCopies  { get; set; }


     // Foreign key................

    [Required(ErrorMessage = "Please select a Category.")]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }


    [Required(ErrorMessage = "Please select an Author.")]
    [Display(Name = "Author")]
    public int AuthorId {get; set;}

    [Required(ErrorMessage = "Please select a Publisher.")]
    [Display(Name = "Publisher")]
    public int PublisherId { get; set; }

    //Navigation properties....................?

     public Category? Category {get; set;}
     public Author? Author { get; set; }
     public Publisher? Publisher {get; set;}
    
    


}
}