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
    // GET: Books/Edit/1
     public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
        var book = await _context.Books.FindAsync(id);
           if(book == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new SelectList(
                _context.Categories,
                 "Id", 
                 "Name",
             book.CategoryId);

            ViewBag.Authors = new SelectList(
                _context.Authors,
                     "Id", 
                     "Name",
             book.AuthorId);
            ViewBag.Publishers = new SelectList(
                _context.Publishers, 
                        "Id", 
                        "Name", 
            book.PublisherId);


            return View(book);
        }

      // POST: Books/Edit/1

      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Edit(int id, Book book)
        {
            if(id != book.Id)
            {
                return NotFound();                
            }
            if(ModelState.IsValid)
            {
               _context.Update(book);
            await _context.SaveChangesAsync();

              return RedirectToAction(nameof(Index));
            }
         ViewBag.Categories = new SelectList(
            _context.Categories, 
            "Id", 
            "Name",
             book.CategoryId);
        ViewBag.Authors = new SelectList(
            _context.Authors,
             "Id",
             "Name",
             book.AuthorId);
        ViewBag.Publishers = new SelectList(
            _context.Publishers,
             "Id", 
             "Name",
             book.PublisherId);


            return View(book);
        }

    // Delete:Books/Delete/1...........................||
     
      public async Task<IActionResult> Delete(int? id)
        {
            if(id == null)
            {
            return NotFound();
            }
        var book = await _context.Books.Include(x =>x.Category)
        .Include(x => x.Author)
        .Include(x => x.Publisher)
        .FirstOrDefaultAsync(x =>x.Id == id);

        if(book == null)
            {
                return NotFound();
            }
        return View(book);
        }

    // POST: Books/Delete/1----------------------------||
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]



    public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if(book != null )
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
          
        }
    


        }   // {public class BooksController : Controller}
        }