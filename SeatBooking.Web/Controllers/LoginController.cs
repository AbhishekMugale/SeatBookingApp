using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SeatBooking.Repository.Business_Services.Business_Contracts;
using SeatBooking.Web.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using SeatBooking.DAL.Models;

namespace SeatBooking.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private AuthenticationProperties? properties;

        public LoginController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var authenticationResult = await _userService.Authenticate(model.Username, model.Password);

                if (authenticationResult.IsAuthenticated)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Username),
                        new Claim(ClaimTypes.Role, authenticationResult.RoleName)
                    };
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    return RedirectToAction("Dashboard", "Login");
                }
                else
                {
                    TempData["ErrorMessage"] = authenticationResult.ErrorMessage ?? "Invalid username or password.";
                    ModelState.AddModelError(string.Empty, TempData["ErrorMessage"].ToString());
                }
            }

            return View(model);
        }

        [Authorize] // Requires authentication for this action
        public IActionResult Dashboard()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("AdminDashboard", "Employee");
                else if (User.IsInRole("Employee"))
                    return RedirectToAction("EmployeeDashboard", "Employee");
            }

            // Default redirection if role is not recognized
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
       
    }
}








