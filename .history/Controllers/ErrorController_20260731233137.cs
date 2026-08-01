using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace library_management_ystem.Controllers
{
    public class ErrorController:Controller
    {
        public IActionResult Error404()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}