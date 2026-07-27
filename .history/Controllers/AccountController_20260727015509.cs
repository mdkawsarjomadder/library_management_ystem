using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Manage.Internal;
using Microsoft.AspNetCore.Mvc;


namespace LibraryManagementSystem.Controllers
{   
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IWebHostEnvironment webHostEnvironment
                                )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _webHostEnvironment = webHostEnvironment;
           
        }


        // GET: Account/Register
        /*Get create */

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        //GetPost
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                Name    = model.Name,
                UserName = model.Email,
                Email = model.Email

            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);

        }


        //Login Create methid

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

            if (result.Succeeded)
            {
                TempData["Success"] = "Login SuccessFully";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }



        //Logout  create-------------------------

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["Success"] = "Logged out Successfully";

           return RedirectToAction("Login", "Account");
        }

         //Profile  create-------------------------
         [HttpGet]
         public async Task<IActionResult> Profile()
            {
             var user = await _userManager.GetUserAsync(User);

             if(user == null)
            {
                return RedirectToAction("Login");
            }   
            return View(user);
            }

           //EditProfile Create---------------------|
           [HttpGet]
           public async Task<IActionResult> EditProfile()
            {
                var user = await _userManager.GetUserAsync(User);

                if(user == null)
            {
                return RedirectToAction("LOgin", "Account");
            }
            var model = new EditProfileViewModel
            {
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = string.IsNullOrEmpty(user.ProfileImage)
                 ?
                  null : "/uploads/profile/" + user.ProfileImage
            };
           
            return View(model);
            } 
           // POST: Account/EditProfile----------------

           [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            model.Email = user.Email;
            model.ProfileImage = string.IsNullOrEmpty(user.ProfileImage)
                ? null
                : "/uploads/profile/" + user.ProfileImage;

            return View(model);
        }

        user.Name = model.Name;
        user.PhoneNumber = model.PhoneNumber;

        // Upload Profile Image------------
        if (model.ProfileImageFile != null)
        {
        var allowedExtensions = new[] {".jpg",".jpeg", ".png"};
        var extension = Path.GetExtension(model.ProfileImageFile.FileName).ToLower();

        if (Array.IndexOf(allowedExtensions, extension) < 0)
        {
            ModelState.AddModelError("ProfileImageFile",
                        "Only JPG, JPEG and PNG images are allowed.");
           
           model.Email = user.Email;
           model.ProfileImage = string.IsNullOrEmpty(user.ProfileImage)
                    ? 
                    null :  "/uploads/profile/" + user.ProfileImage;
            return View(model);
           
        }



        if(model.ProfileImageFile.Length> 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(
                        "ProfileImageFile",
                        "Image size must be less than 5 MB."
                    );
                 model.Email = user.Email;
                model.ProfileImage = string.IsNullOrEmpty(user.ProfileImage)
                        ? null
                        : "/uploads/profile/" + user.ProfileImage;

                        return View(model);
                }


                
        string uploadFolder = Path.Combine(
            _webHostEnvironment.WebRootPath,
            "uploads",
            "profile");

        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }

        string fileName = Guid.NewGuid().ToString()
                        + Path.GetExtension(model.ProfileImageFile.FileName);

        string filePath = Path.Combine(uploadFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await model.ProfileImageFile.CopyToAsync(stream);
        }

        user.ProfileImage = fileName;
    }

    var result = await _userManager.UpdateAsync(user);

    if (result.Succeeded)
    {
        TempData["Success"] = "Profile updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

    foreach (var error in result.Errors)
    {
        ModelState.AddModelError("", error.Description);
    }

    return View(model);
}

      // Change Password (GET)
       [HttpGet]
       public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
        var user = await _userManager.GetUserAsync(User);

    if (user == null)
    {
        return RedirectToAction("Login", "Account");
    }

    var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);

    if (!passwordCheck)
    {
        ModelState.AddModelError("", "Current password is incorrect.");
        return View(model);
    }

    var result = await _userManager.ChangePasswordAsync(
        user,
        model.CurrentPassword,
        model.NewPassword
    );
  

    if (result.Succeeded)
    {
        await _signInManager.RefreshSignInAsync(user);

        TempData["Success"] = "Password changed successfully.";

        return RedirectToAction(nameof(Profile));
    }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View(model);
        }
          
    }
}