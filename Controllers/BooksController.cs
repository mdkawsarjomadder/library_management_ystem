using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;
        public BooksController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
            .Include(x =>x.Category)
            .Include(x =>x.Author)
            .Include(x => x.Publisher)
            .ToListAsync();
            return View(books);
        }

      // Create GET Method.......  {GET: Books/Create}
        public IActionResult Create()
        {
            return View();
        }

        //Create Post Method -----------{post: Books/create}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if(ModelState.IsValid)
            {
                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));               
            }
             return View(book);
        }    


    }
}