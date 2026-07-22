using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)

        {
            
        }

        public DbSet<Book> Books {get; set;}
        public DbSet<Category> Categories {get; set;}
        public DbSet<Author> Authors {get; set;}
        public DbSet<Publisher> Publishers  {get; set;}
        public DbSet<Member> Members {get; set;}
        public DbSet<Borrow> Borrows {get; set;}

    }
}