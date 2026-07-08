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
    
    [Required]
    public string Title {get; set;} = string.Empty;
    public string ISBN {get; set;} = string.Empty;
    public int TotalCopies { get; set; }
    public int AvaliableCopies { get; set; }


     // Foreign key................
    public int CategoryId { get; set; }
    public int AuthorId {get; set;}
    public int PublisherId { get; set; }

    //Navigation properties....................?

     public Category? Category {get; set;}
     public Author? Author { get; set; }
     public Publisher? Publisher {get; set;}
    
    


}
}