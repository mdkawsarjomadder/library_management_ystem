using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Include(x => x.Publisher)
                .ToListAsync();

            return View(books);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
        ViewBag.Authors = new SelectList(_context.Authors, "Id", "Name");
        ViewBag.Publishers = new SelectList(_context.Publishers, "Id", "Name");

        return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
        if (ModelState.IsValid)
        {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
        }

        ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
        ViewBag.Authors = new SelectList(_context.Authors, "Id", "Name");
        ViewBag.Publishers = new SelectList(_context.Publishers, "Id", "Name");

        return View(book);
        }
        }
        }