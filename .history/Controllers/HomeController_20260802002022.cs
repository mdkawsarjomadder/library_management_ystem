using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel 
        { 
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier 
        });
    }

    // Recent Borrow History in a Dashboard
    public async Task<IActionResult> Index()
    {
        ViewBag.TotalBooks = await _context.Books.CountAsync();
        ViewBag.TotalMembers = await _context.Members.CountAsync();
        ViewBag.TotalBorrowed = await _context.Borrows.CountAsync(x => x.ReturnDate == null);
        ViewBag.TotalReturned = await _context.Borrows.CountAsync(x => x.ReturnDate != null);
        ViewBag.AvailableBooks = await _context.Books.SumAsync(x => x.AvailableCopies);
        
        ViewBag.TotalOverDue = await _context.Borrows.CountAsync(x =>
                                x.ReturnDate == null &&
                                x.DueDate < DateTime.Today);

        ViewBag.BorrowedCount = await _context.IssueBooks
                                    .CountAsync(x => !x.IsReturned);

        ViewBag.ReturnedCount = await _context.IssueBooks
                                   .CountAsync(x => x.IsReturned);

        ViewBag.OverdueCount = await _context.IssueBooks
                                    .CountAsync(x =>
                                    !x.IsReturned &&
                                    x.DueDate < DateTime.Today);

        var recentBorrows = await _context.Borrows
                 .Include(x => x.Book)
                 .Include(x => x.Member)
                 .OrderByDescending(x => x.BorrowDate)
                 .Take(200)
                 .ToListAsync();

        ViewBag.ChartBorrowed = ViewBag.TotalBorrowed;
        ViewBag.ChartReturned = ViewBag.TotalReturned;
        ViewBag.ChartOverDue = ViewBag.TotalOverDue;


    //Month ly borrowed books chart in a dashboard-------
    var monthlyData = await _context
                    .IssueBooks.GroupBy(x => x
                    .IssueDate.Month)
                    .Select(g => new
    {
         Month = g.Key,
         Total = g.Count()
    }).ToListAsync();
    int[] monthlyIssueBooks = new int[12];
    foreach(var item in monthlyData)
        {
            monthlyIssueBooks[item.Month - 1] = item.Total;
        }


        return View(recentBorrows); 


    }

    // About page create
    public IActionResult About()
    {
        return View();
    }


    // Contact page create
    public IActionResult Contact()
    {
        return View();
    }
}