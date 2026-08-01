using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    public class ErrorController:Controller
    {
        public IActionResult Errors404()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}