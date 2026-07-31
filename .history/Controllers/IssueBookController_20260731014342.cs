using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class IssueBookController: Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IssueBookController(
            AppDbContext context,
            UserManager <ApplicationUser> userManager
        )
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var issuesBook = await _context.IssueBooks
                           .Include(x => x.Book)
                           .Include(x => x.User)
                           .ToListAsync();
            
            return View(issuesBook);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Books = new SelectList(
                _context.Books.Where(x => x.AvailableCopies > 0),
                 "Id", "Title" );
            ViewBag.Users  = new SelectList(
                _userManager.Users,"Id","Name"
            );

            return View();
        }

        //Create post 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IssueBook issueBook)
        {
            ViewBag.Books = new SelectList(
                _context.Books.Where(x => x.AvailableCopies > 0),
                    "Id",
                    "Title", issueBook.BookId);
                    
            ViewBag.Users = new SelectList(
                _userManager.Users,
                 "Id",
                 "Name", issueBook.UserId);

            if(!ModelState.IsValid)
            {
                return View(issueBook);
            }
            issueBook.IssueDate = DateTime.Now;

            var book = await _context.Books.FindAsync(issueBook.BookId);

            if(book == null)
            {
                ModelState.AddModelError("", "Book not found ");
                 return View(issueBook);
            }

            if(book.AvailableCopies <= 0)
            {
                ModelState.AddModelError("", "This book is not  available");
                return View(issueBook);
            }

            // Available Copies 
           book.AvailableCopies--;

            _context.Books.Update(book);

            //Issue Save
            _context.IssueBooks.Add(issueBook);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Book Issue  SuccessFully";

            return RedirectToAction(nameof(Index));


        }

        //Return action create------------------------

        public async Task<IActionResult> Return (int id)
        {
            var issueBook = await _context.IssueBooks
                            .Include(x =>x.Book)
                            .FirstOrDefaultAsync(x => x.Id == id);
            if(issueBook == null)
            {
                return NotFound();
            }

          
        if(issueBook.IsReturned)
            {
                TempData["Success"] = "Book already returned";
                return RedirectToAction(nameof(Index));
            }
            issueBook.IsReturned = true;
            issueBook.ReturnDate = DateTime.Now;

             // Fine Calculate
        if (issueBook.ReturnDate.Value.Date > issueBook.DueDate.Date)
            {
                var lateDays = (issueBook.ReturnDate.Value.Date - issueBook.DueDate.Date).Days;
                issueBook.Fine = lateDays * 10;
            }

            if(issueBook.Book != null)
            {
                issueBook.Book.AvailableCopies++;
            }
            await _context.SaveChangesAsync();
            TempData["Success"] = "Book returned successfully";

            return RedirectToAction(nameof(Index));

        }
    }
}