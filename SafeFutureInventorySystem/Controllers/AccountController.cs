using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SafeFutureInventorySystem.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        [HttpGet, AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;

            // Use Views/Account/Login.cshtml
            return View("Login");
        }

        // POST: /Account/Login
        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
           

            if (email == demoUser && password == demoPass)
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

                return LocalRedirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl!);
            }

            // Failed login
            ViewBag.Error = "Invalid username or password.";
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

            // TODO: Save user to DB later
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
