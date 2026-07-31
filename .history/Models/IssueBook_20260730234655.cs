using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Models
{
    public class IssueBook
    {
        public int Id { get; set; }

        [Required]
        [Display(Name ="Member")]
        public string UserId { get; set; } = string.Empty;

        public ApplicationUser? User { get; set; }

        [Required]
        [Display(Name ="Book")]
        public int BookId { get; set; }

        public Book? Book { get; set; }

        [Required]
        [Display(Name ="Issue Date")]
        public DateTime IssueDate { get; set; } = DateTime.Now;

        [Required]
        [Display(Name ="Due Date")]
        public DateTime DueDate  { get; set; }

        public DateTime? ReturnDate  { get; set; }

        public bool IsReturned { get; set; } = false;

        public decimal Fine { get; set; }
    }
}