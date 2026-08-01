using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace library_management_ystem.Views.Home
{
    public class Contact : PageModel
    {
        private readonly ILogger<Contact> _logger;

        public Contact(ILogger<Contact> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}