using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class DashboardController: Controller
    {
        private readonly AppDbContext _context;

        public  DashboardController(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {   
            ViewBag.TotalBooks = await _context.Books.CountAsync();
            ViewBag.TotalMembers= await _context.Members.CountAsync();
            ViewBag.TotalBorrows= await _context.Borrows
                        .CountAsync( x => x.ReturnDate == null);
            ViewBag.TotalOverDue= await _context.Borrows
                        .CountAsync( x => x.ReturnDate == null && 
                                    x.DueDate < DateTime.Today);

            ViewBag.AvailableBooks = _context.Books.Sum(x => x.AvailableCopies);

            ViewBag.BorrowedBooks = 
                    _context.Books.Sum(x => x.TotalCopies - x.AvailableCopies);

             
            var recentBorrows = _context.Borrows
    .Include(x => x.Book)
    .Include(x => x.Member)
    .OrderByDescending(x => x.BorrowDate)
    .Take(5)
    .ToList();

ViewBag.RecentBorrows = recentBorrows;

return View();
        }
        
    }
}