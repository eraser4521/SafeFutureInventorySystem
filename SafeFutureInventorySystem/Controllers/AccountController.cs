using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SafeFutureInventorySystem.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet, AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl ?? "/";
            return View("~/Views/Account.cshtml");
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            const string demoUser = "test";
            const string demoPass = "1234";

            if (username == demoUser && password == demoPass)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Admin")
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return LocalRedirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl!);
            }

            ViewBag.Error = "Invalid username or password.";
            ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
            return View("~/Views/Account.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
