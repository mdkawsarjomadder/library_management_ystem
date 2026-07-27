using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
            var issuesBook = await _context.issueBooks
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
            ViewBag.User = new SelectList(
                _userManager.Users,"Id","Name"
            );

            return View();
        }
    }
}