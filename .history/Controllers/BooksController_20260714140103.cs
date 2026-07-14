using System.Drawing;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace LibraryManagementSystem.Controllers
{
    public class BooksController : Controller
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Books (Search Included)
        

         const int pageSize = 10;
        public async Task<IActionResult> Index(string? searchString, int page = 1)
        {
            var books = _context.Books
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Include(x => x.Publisher)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                books = books.Where(x => x.Title.Contains(searchString));
            }
             //Total Record Count...
             int totalBooks = await books.CountAsync();

             //total Page.....
             int totalPages = (int)Math.Ceiling((double)totalBooks / pageSize);
                
                     // Pagination
             var pagedBooks = await books
                              .OrderBy(x => x.Id)
                              .Skip((page -1 ) * pageSize)
                             .Take(pageSize)
                             .ToListAsync();



        ViewBag.SearchString = searchString;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(pagedBooks);
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
            if (id == null)
                return NotFound();

            var book = await _context.Books.FindAsync(id);

            if (book == null)
                return NotFound();

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            ViewBag.Authors = new SelectList(_context.Authors, "Id", "Name", book.AuthorId);
            ViewBag.Publishers = new SelectList(_context.Publishers, "Id", "Name", book.PublisherId);

            return View(book);
        }

        // POST: Books/Edit/1

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {
            if (id != book.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(book);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", book.CategoryId);
            ViewBag.Authors = new SelectList(_context.Authors, "Id", "Name", book.AuthorId);
            ViewBag.Publishers = new SelectList(_context.Publishers, "Id", "Name", book.PublisherId);

            return View(book);
        }

        // GET: Books/Delete/1

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var book = await _context.Books
                .Include(x => x.Category)
                .Include(x => x.Author)
                .Include(x => x.Publisher)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        // POST: Books/Delete/1
       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);

            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
  
     
    //BooksController in a ExportExcel Cretate ............|

        public IActionResult ExportToExcel()
        {
            var books = _context.Books.ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


            using (var package = new ExcelPackage())
            {
                var workSheet = package.Workbook.Worksheets.Add("Book");

                //header..................|
                workSheet.Cells[1,1].Value ="ID";
                workSheet.Cells[1,2].Value ="Title";
                workSheet.Cells[1,3].Value ="Author";
                workSheet.Cells[1,4].Value ="Category";
                workSheet.Cells[1,5].Value ="Total Copies";
                workSheet.Cells[1,6].Value ="Available Copies";
          
            using (var range = workSheet.Cells[1,1,1,6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                         range.Style.Font.Color.SetColor(Color.White);

                }
            int row = 2;

            foreach(var book in books)
            {
                  workSheet.Cells[row, 1].Value = book.Id;
                  workSheet.Cells[row, 2].Value = book.Title;
                  workSheet.Cells[row, 3].Value = book.Author;
                  workSheet.Cells[row, 4].Value = book.Category;
                  workSheet.Cells[row, 5].Value = book.TotalCopies;
                  workSheet.Cells[row, 6].Value = book.AvailableCopies;

                  row ++;

            }
           
        }

        }
    }  // public End

 }