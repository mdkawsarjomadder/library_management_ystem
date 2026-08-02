using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.ComponentModel;
using QuestPDF.Fluent;
using DocumentFormat.OpenXml.Spreadsheet;

namespace LibraryManagementSystem.Controllers
{    
    [Authorize]
    public class BorrowsController : Controller
    { 
        private readonly AppDbContext _context;

        public BorrowsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Borrows or Search button add--- And Next Button Create---------------|
        const int pageSize = 10;
        public async Task<IActionResult> Index(string? searchString, int page = 1)
        {
            var borrows = _context.Borrows
                .Include(x => x.Book)
                .Include(x => x.Member)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
               borrows = borrows.Where(x =>
                x.Book!.Title.Contains(searchString) ||
                x.Member!.Name.Contains(searchString));
            }

            int totalRecords = await borrows.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var date = await borrows.OrderByDescending(x => x.BorrowDate)
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();
                                
            ViewBag.CurrentPage = page;            
            ViewBag.TotalPages = totalPages;            
            ViewBag.SearchString = searchString;

            return View(date);
        }

        // GET: Borrows/Create
        public IActionResult Create()
        {
            ViewBag.Books = new SelectList(_context.Books, "Id", "Title");
            ViewBag.Members = new SelectList(_context.Members, "Id", "Name");

            return View();
        }

        // POST: Borrows/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Borrow borrow)
        {
            if (ModelState.IsValid)
            {
                var book = await _context.Books.FindAsync(borrow.BookId);

                if (book == null)
                {
                    return NotFound();
                }

                // Available Copy Check
                if (book.AvailableCopies <= 0)
                {
                    ModelState.AddModelError("", "This Book is not available.");
                }
                else
                {
                    // Decrease Available Copy
                    book.AvailableCopies--;

                    // Borrow Record Save
                    _context.Borrows.Add(borrow);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Borrowed successfully.";
                    return RedirectToAction(nameof(Index));
                }
            }
            
            ViewBag.Books = new SelectList(_context.Books, "Id", "Title", borrow.BookId);
            ViewBag.Members = new SelectList(_context.Members, "Id", "Name", borrow.MemberId);

            return View(borrow);
        } 

        // GET: Borrows/Details/1
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var borrow = await _context.Borrows
                                    .Include(x => x.Book)
                                    .Include(x => x.Member)
                                    .FirstOrDefaultAsync(x => x.Id == id);
            
            if (borrow == null)
            {
                 return NotFound();
            }
            
            return View(borrow);
        }

        // GET: Borrows/Return/1
        public async Task<IActionResult> Return(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var borrow = await _context.Borrows
                                .Include(x => x.Book)
                                .Include(x => x.Member)
                                .FirstOrDefaultAsync(x => x.Id == id);
        
            if (borrow == null)
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
            var borrow = await _context.Borrows
                .Include(x => x.Book)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (borrow == null)
            {
                return NotFound();
            }    

            // Return check
            if (borrow.ReturnDate != null)
            {
                 return RedirectToAction(nameof(Index));
            }

            // Return Date
            borrow.ReturnDate = DateTime.Now;

            // Increment Book Available Copies
            if (borrow.Book != null)
            {
                borrow.Book.AvailableCopies++;
            }
         
            await _context.SaveChangesAsync();
            TempData["Success"] = "Book returned successfully";
            return RedirectToAction(nameof(Index));
        }

        //----------------- Excel Export Borrow  Create-------------?
            public IActionResult ExcelExport()
    {
        // EPPlus 8 License
        ExcelPackage.License.SetNonCommercialPersonal("Your Name");

        // Load related data
        var borrows = _context.Borrows
            .Include(b => b.Book)
            .Include(b => b.Member)
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

            foreach (var borrow in borrows)
            {
                workSheet.Cells[row, 1].Value = borrow.Id;
                workSheet.Cells[row, 2].Value = borrow.Book?.Title;;
                workSheet.Cells[row, 3].Value = borrow.Member?.Name;
                workSheet.Cells[row, 4].Value = borrow.BorrowDate.ToString("dd-MMM-yy");
                workSheet.Cells[row, 5].Value = borrow.DueDate.ToString("dd-MMM-yy");
                workSheet.Cells[row, 6].Value = borrow.Book?.AvailableCopies;

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
       
        //----------------- PDF Export Borrow  Create-------------?
          public IActionResult Pdf()
{
    var borrows = _context.Borrows
        .Include(x => x.Book)
        .Include(x => x.Member)
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
                 .PaddingBottom(15)
                .Text("📚 Library Book Report")
                .FontSize(22)
                .Bold();

                row.ConstantItem(180)
                    .AlignRight()
                    .Text($"Date: {DateTime.Now : dd MMM yy}");
            });
               
        

            // Content
            page.Content().Table(table =>
            {   
                  
                table.ColumnsDefinition(columns =>
                {

                        columns.ConstantColumn(40); //id
                        columns.RelativeColumn(4); //Title
                        columns.RelativeColumn(3); //Author
                        columns.RelativeColumn(3); //Category
                        columns.ConstantColumn(70); // total
                        columns.ConstantColumn(80); //Acilable
             
                });

                // Header
               table.Header(header =>
{
    // ID
    header.Cell()
        .Border(1)
        .Background("#212529")   // Bootstrap Dark
        .PaddingVertical(8)
        .PaddingHorizontal(6)
        .AlignCenter()
        .AlignMiddle()
        .Text("ID")
       
        .Bold();

    // Title
    header.Cell()
        .Border(1)
        .Background("#212529")
        .PaddingVertical(8)
        .PaddingHorizontal(6)
        .Text("Title")
     
        .Bold();

    // Author
    header.Cell()
        .Border(1)
        .Background("#212529")
        .PaddingVertical(8)
        .PaddingHorizontal(6)
        .Text("Author")

        .Bold();

    // Category
            header.Cell()
                .Border(1)
                .Background("#212529")
                .PaddingVertical(8)
                .PaddingHorizontal(6)
                .Text("Category")
               
                .Bold();

            // Total
            header.Cell()
                .Border(1)
                .Background("#212529")
                .PaddingVertical(8)
                .PaddingHorizontal(6)
                .AlignCenter()
                .Text("Total")
                
                .Bold();

            // Available
            header.Cell()
                .Border(1)
                .Background("#212529")
                .PaddingVertical(8)
                .PaddingHorizontal(6)
                .AlignCenter()
                .Text("Available")
                
                .Bold();
        });
                // Data
               
        int serial = 1;

            foreach (var borrow in borrows)
            {
                table.Cell().Border(1).Padding(5).AlignCenter().Text(serial.ToString());

                table.Cell().Border(1).Padding(5).Text(borrow.Book?.Title ?? "");
                table.Cell().Border(1).Padding(5).Text(borrow.BorrowDate.ToString("dd-MMM-dd"));

                table.Cell().Border(1).Padding(5).Text(borrow.DueDate.ToString("dd-MMM-yy"));

                table.Cell().Border(1).Padding(5).AlignCenter().Text(borrow.ReturnDate.ToString());


            serial++;
            }

            });

            // Footer and Date add....
            page.Footer().Row(row =>
            {

                 row.RelativeItem()
                 .Text($"Generated on: {DateTime.Now : dd MMM yy hh:mm tt}");
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
       
        //-----------------Print Borrow  Create-------------?
        public async Task<IActionResult> Print()
        {
           var borrows = await _context.Borrows
                  .Include(x => x.Book)
                  .Include(x => x.Member)
                  .OrderByDescending(b => b.BorrowDate)
                   .ToListAsync();

            return View(borrows);
        }
    }
}