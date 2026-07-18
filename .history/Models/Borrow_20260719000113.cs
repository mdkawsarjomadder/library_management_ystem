using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class Borrow
    {
        [Key]
        public int Id { get; set; }

        [Required] 
        [Range(1, int.MaxValue, ErrorMessage = "Please select a Book.")]
        [Display(Name = "Book")]
        public int BookId { get; set; } // Book_Foreign Key.........!


        [ForeignKey("BookId")]
        public Book? Book { get; set; }

        [Required]   //Member Foreign key..................|
        [Range(1, int.MaxValue, ErrorMessage = "Please select a Member.")]
        [Display(Name = "Member")]
        public int MemberId { get; set; }

        [ForeignKey("MemberId")]
        public Member? Member { get; set; }

       //borrow Information...........|  
        [Required(ErrorMessage = "Borrow Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Borrow Date")]
        public DateTime BorrowDate { get; set; }


        [Required]  //due borrow Date Time......................|
        public DateTime DueDate { get; set; }

        public DateTime? ReturnDate { get; set; } // null return...............|


    }
}