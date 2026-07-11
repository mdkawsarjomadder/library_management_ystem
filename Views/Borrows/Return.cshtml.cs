using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace library_management_ystem.Views.Borrows
{
    public class Return : PageModel
    {
        private readonly ILogger<Return> _logger;

        public Return(ILogger<Return> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}