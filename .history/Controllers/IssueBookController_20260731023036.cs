using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using ClosedXML.Excel;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

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

        //Download Create...........
        public async Task<IActionResult> ExportExcel()
{
    var issueBooks = await _context.IssueBooks
        .Include(x => x.Book)
        .Include(x => x.User)
        .ToListAsync();

    using (var workbook = new XLWorkbook())
    {
        var worksheet = workbook.Worksheets.Add("Issue Books");

        // Header
        worksheet.Cell(1, 1).Value = "Member";
        worksheet.Cell(1, 2).Value = "Book";
        worksheet.Cell(1, 3).Value = "Issue Date";
        worksheet.Cell(1, 4).Value = "Due Date";
        worksheet.Cell(1, 5).Value = "Return Date";
        worksheet.Cell(1, 6).Value = "Status";
        worksheet.Cell(1, 7).Value = "Fine (৳)";

        int row = 2;

        foreach (var item in issueBooks)
        {
            worksheet.Cell(row, 1).Value = item.User?.Name;
            worksheet.Cell(row, 2).Value = item.Book?.Title;
            worksheet.Cell(row, 3).Value = item.IssueDate.ToString("dd MMM yyyy");
            worksheet.Cell(row, 4).Value = item.DueDate.ToString("dd MMM yyyy");
            worksheet.Cell(row, 5).Value = item.ReturnDate?.ToString("dd MMM yyyy") ?? "-";
            worksheet.Cell(row, 6).Value = item.IsReturned ? "Returned" : "Issued";
            worksheet.Cell(row, 7).Value = item.Fine;

            row++;
        }

        worksheet.Columns().AdjustToContents();

        using (var stream = new MemoryStream())
        {
            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"IssueBooks_{DateTime.Now:yyyyMMdd}.xlsx");
                }
            }
        }

        //PDF------------
        public async Task<IActionResult> ExportPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var issueBooks = await _context.IssueBooks
                .Include(x => x.Book)
                .Include(x => x.User)
                .ToListAsync();

            var pdf = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);

                    page.Header()
                        .Text("Library Management System\nIssue Book Report")
                        .SemiBold()
                        .FontSize(20);

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Member").Bold();
                            header.Cell().Text("Book").Bold();
                            header.Cell().Text("Issue Date").Bold();
                            header.Cell().Text("Due Date").Bold();
                            header.Cell().Text("Return Date").Bold();
                            header.Cell().Text("Status").Bold();
                            header.Cell().Text("Fine").Bold();
                        });

                        foreach (var item in issueBooks)
                        {
                            table.Cell().Text(item.User?.Name ?? "");
                            table.Cell().Text(item.Book?.Title ?? "");
                            table.Cell().Text(item.IssueDate.ToString("dd MMM yyyy"));
                            table.Cell().Text(item.DueDate.ToString("dd MMM yyyy"));
                            table.Cell().Text(item.ReturnDate?.ToString("dd MMM yyyy") ?? "-");
                            table.Cell().Text(item.IsReturned ? "Returned" : "Issued");
                            table.Cell().Text($"৳ {item.Fine}");
                        }
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated: {DateTime.Now:dd MMM yyyy}");
                });
            });

            var pdfBytes = pdf.GeneratePdf();

            return File(
                pdfBytes,
                "application/pdf",
                $"IssueBooks_{DateTime.Now:yyyyMMdd}.pdf");
        }
    }
}
