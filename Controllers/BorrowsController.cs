using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers
{
    public class BorrowsController : Controller
    {
        private readonly AppDbContext _context;

        public BorrowsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Borrows or Search button add------------------|
        public async Task<IActionResult> Index(string? searchString)
        {
            var borrows =  _context.Borrows
                .Include(x => x.Book)
                .Include(x => x.Member)
                .AsQueryable();

        if(!string.IsNullOrWhiteSpace(searchString))
            {
               borrows = borrows.Where(x =>
                (x.Book != null && x.Book.Title.Contains(searchString)) ||
                (x.Member != null && x.Member.Name.Contains(searchString)));
            }
            ViewBag.SearchString = searchString;

            return View(await borrows.ToListAsync());
        }

        // GET: Borrows/Create
        public IActionResult Create()
        {
            ViewBag.Books = new SelectList(_context.Books, "Id", "Title");
            ViewBag.Members = new SelectList(_context.Members,"Id", "Name");

            return View();
        }

        // POST: Borrows/Create

    [HttpPost]
    [ValidateAntiForgeryToken]
     public async Task<IActionResult> Create(Borrow borrow)
        {
            if(ModelState.IsValid)
            {
                var book = await _context.Books.FindAsync(borrow.BookId);

                if(book == null)
                {
                    return NotFound();
                }

                 // Available Copy 
                if(book.AvailableCopies <= 0)
                {
                    ModelState.AddModelError("", "This Book is not available.");
                }
                else
                {
                    //Available Copy
                    book.AvailableCopies--;

                    //Borrow Record Save
                    _context.Borrows.Add(borrow);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            ViewBag.Books = new SelectList(_context.Books, "Id", "Title", borrow.BookId);
            ViewBag.Members = new SelectList(_context.Members,"Id","Name", borrow.MemberId);

            return View(borrow);
        } 
        // GET: Borrows/Return/1....................|
        public async Task<IActionResult> Return(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
        var borrow = await _context.Borrows
                    .Include(x=>x.Book)
                    .Include(x=>x.Member)
                    .FirstOrDefaultAsync(x =>x.Id == id);
        
        if(borrow == null)
            {
                return NotFound();
            }
        return View(borrow);    
        }


        // POST: Borrows/Return/1
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Return(int id)
        {
            var  borrow = await _context.Borrows
                .Include(x => x.Book)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(borrow == null)
            {
                return NotFound();
            }    
            //Return
            if(borrow.ReturnDate != null)
            {
                 return RedirectToAction(nameof(Index));
            }

            //Return Date
            borrow.ReturnDate = DateTime.Now;


             //Book and Availabl
            if(borrow.Book != null)
            {
                borrow.Book.AvailableCopies++;
            }
         
          await _context.SaveChangesAsync();
          return RedirectToAction(nameof(Index));
        }
    }

}