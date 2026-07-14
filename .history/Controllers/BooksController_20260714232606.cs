using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;
using System.Drawing;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Reflection.Metadata;
using System.Diagnostics.Metrics;

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
            var books = await _context.Books.FindAsync(id);

            if (books != null)
            {
                _context.Books.Remove(books);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
  
     
    //BooksController in a ExportExcel Cretate ............|

        public IActionResult ExportToExcel()
    {
        // EPPlus 8 License
        ExcelPackage.License.SetNonCommercialPersonal("Your Name");

        // Load related data
        var books = _context.Books
            .Include(b => b.Author)
            .Include(b => b.Category)
            .ToList();

        using (var package = new ExcelPackage())
        {
            var workSheet = package.Workbook.Worksheets.Add("Books");

            // Header
            workSheet.Cells[1, 1].Value = "ID";
            workSheet.Cells[1, 2].Value = "Title";
            workSheet.Cells[1, 3].Value = "Author";
            workSheet.Cells[1, 4].Value = "Category";
            workSheet.Cells[1, 5].Value = "Total Copies";
            workSheet.Cells[1, 6].Value = "Available Copies";

            // Header Style
            using (var range = workSheet.Cells[1, 1, 1, 6])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.DarkBlue);
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
            }

            // Data
            int row = 2;

            foreach (var book in books)
            {
                workSheet.Cells[row, 1].Value = book.Id;
                workSheet.Cells[row, 2].Value = book.Title;
                workSheet.Cells[row, 3].Value = book.Author?.Name;
                workSheet.Cells[row, 4].Value = book.Category?.Name;
                workSheet.Cells[row, 5].Value = book.TotalCopies;
                workSheet.Cells[row, 6].Value = book.AvailableCopies;

                row++;
            }

            // Auto fit columns
            workSheet.Cells.AutoFitColumns();

            // Save to MemoryStream
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            string fileName = $"Books_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(
                stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
    }
       
        //ExportToPdf in Download.........

       public IActionResult ExportToPdf()
{
    var books = _context.Books
        .Include(x => x.Author)
        .Include(x => x.Category)
        .ToList();

    var pdf = QuestPDF.Fluent.Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Margin(25);

            // Header
            page.Header().Row(row =>
            {
                 row.RelativeItem()
                .Text("📚 Library Book Report")
                .FontSize(22)
                .Bold();

                row.ConstantItem(180)
                    .AlignRight()
                    .Text($"Date: {DateTime.Now : dd MMM yyyy}");
            });
               
        

            // Content
            page.Content().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(40);   // ID
                    columns.RelativeColumn(4);    // Title
                    columns.RelativeColumn(3);    // Author
                    columns.RelativeColumn(3);    // Category
                    columns.ConstantColumn(70);   // Total
                    columns.ConstantColumn(80);   // Available
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Border(1).Background(Colors.Blue.Darken2)
                        .Padding(5).Text("ID").FontColor(Colors.White).Bold();

                    header.Cell().Border(1).Background(Colors.Blue.Darken2)
                        .Padding(5).Text("Title").FontColor(Colors.White).Bold();

                    header.Cell().Border(1).Background(Colors.Blue.Darken2)
                        .Padding(5).Text("Author").FontColor(Colors.White).Bold();

                    header.Cell().Border(1).Background(Colors.Blue.Darken2)
                        .Padding(5).Text("Category").FontColor(Colors.White).Bold();

                    header.Cell().Border(1).Background(Colors.Blue.Darken2)
                        .Padding(5).Text("Total").FontColor(Colors.White).Bold();

                    header.Cell().Border(1).Background(Colors.Blue.Darken2)
                        .Padding(5).Text("Available").FontColor(Colors.White).Bold();
                });

                // Data
                foreach (var book in books)
                {
                    table.Cell().Border(1).Padding(5).Text(book.Id.ToString());

                    table.Cell().Border(1).Padding(5).Text(book.Title);

                    table.Cell().Border(1).Padding(5)
                        .Text(book.Author?.Name ?? "");

                    table.Cell().Border(1).Padding(5)
                        .Text(book.Category?.Name ?? "");

                    table.Cell().Border(1).Padding(5)
                        .AlignCenter()
                        .Text(book.TotalCopies.ToString());

                    table.Cell().Border(1).Padding(5)
                        .AlignCenter()
                        .Text(book.AvailableCopies.ToString());
                }
            });

            // Footer
            page.Footer().Row(row =>
            {

                 row.RelativeItem()
                 .Text($"Generated on: {DateTime.Now : dd MMM yyyy hh:mm tt}");
                row.ConstantItem(100)
                .AlignRight()
                .Text(x =>{
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
                
                
        });
    });

    var bytes = pdf.GeneratePdf();

    return File(bytes,
        "application/pdf",
        "BooksReport.pdf");
}
       
       }  // public End

 }