using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SafeFutureInventorySystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SafeFutureInventorySystem.Controllers
{
    public class AccountController : Controller
    {
        // TEMPORARY HARDCODED USERS
        public static List<User> Users = new List<User>
        {
            new User { Email = "test@test.com", Password = "1234" },
            new User { Email = "admin@admin.com", Password = "admin" }
        };

        // GET: /Account/Login
        [HttpGet, AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;

            return View("Login");
        }

[HttpPost, AllowAnonymous]
public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
{
    // Trim to remove accidental spaces
    email = email?.Trim();
    password = password?.Trim();

    // 🔍 DEBUG: if you want you can put a breakpoint here later
    // SUPER SIMPLE HARDCODED CHECK
    if (email != null 
        && email.Equals("test@test.com", StringComparison.OrdinalIgnoreCase)
        && password == "1234")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);

        return RedirectToAction("Index", "Inventory");
    }

    // If it gets here, login failed – show exactly what the server saw
    ViewBag.Error = $"Invalid username or password. (Debug: email='{email ?? "null"}', password='{password ?? "null"}')";

    ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
    return View("Login");
}


        // GET: /Account/Register
        [HttpGet, AllowAnonymous]
        public IActionResult Register()
        {
            return View("Register");
        }

        // POST: /Account/Register
        [HttpPost, AllowAnonymous]
        public IActionResult Register(string email, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View("Register");
            }

            // For now we ignore registration and keep hard-coded users only
            // Later we can add to a database or a list

            return RedirectToAction("Login");
        }

        // GET: /Account/ForgotPassword
        [HttpGet, AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }

        // POST: /Account/ForgotPassword
        [HttpPost, AllowAnonymous]
        public IActionResult ForgotPassword(string email)
        {
            ViewBag.Message = "If an account exists for that email, a reset link will be sent.";
            return View("ForgotPassword");
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
