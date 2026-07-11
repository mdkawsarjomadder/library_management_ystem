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
     public IActionResult Index()
    {
        ViewBag.TotalBooks = _context.Books.Count();
        ViewBag.TotalMembers = _context.Members.Count();
        ViewBag.TotalBorrowed = _context.Borrows.Count(x => x.ReturnDate == null);
        ViewBag.TotalReturned = _context.Borrows.Count(x => x.ReturnDate != null);
        ViewBag.AvailableBooks  = _context.Books.Sum(x => x.AvailableCopies);

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
