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
        // POST: Members/Edit/1................................||
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, Member member)
        {
            if(id != member.Id)
            {
                 return NotFound();
            }
            if (ModelState.IsValid)
            {
                _context.Update(member);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }
                return View(member);
        }
        // GET: Members/Delete/1-------------------||
        public async Task<IActionResult> Delete(int? id)
        {
             if(id == null)
            {
                return NotFound();
            }
          var member = await _context.Members.FirstOrDefaultAsync(x => x.Id == id);

          if(member == null)
            {
                return NotFound();
            }
            return View(member);
        }

    // POST: Members/Delete/1..........................|
    [HttpPost,ActionName("Delete")]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult>DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if(member != null)
            {
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        


       } // MembersController : Controller    
    }// LibraryManagementSystem : Controllers