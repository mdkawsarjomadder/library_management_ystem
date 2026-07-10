using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers
{
  public class MembersController : Controller
{
    private readonly AppDbContext _context;

    public MembersController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var members = await _context.Members.ToListAsync();
        return View(members);
    }


        // GET: Members/Create
    public IActionResult Create()
    {
        return View();
        }
    

      // POST: Members/Create
      [HttpPost]
      [ValidateAntiForgeryToken]
       public async Task<IActionResult> Create(Member member)
        {
            if(ModelState.IsValid)
            {
                _context.Members.Add(member);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        //Get member/Edit/1

        public async Task<IActionResult> Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
           var member = await _context.Members.FindAsync(id);
           if(member == null)
            {
                return NotFound();
            }
            return View(member);
        }


       } // MembersController : Controller    
    }// LibraryManagementSystem : Controllers