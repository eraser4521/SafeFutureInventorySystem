using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SafeFutureInventorySystem.Models;
using System.Threading.Tasks;

namespace SafeFutureInventorySystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
            return View("Login");
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            email = (email ?? "").Trim();
            password = (password ?? "").Trim();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
                return View("Login");
            }

            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (!result.Succeeded)
            {
                ViewBag.Error = "Invalid email or password.";
                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
                return View("Login");
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Inventory");
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Register()
        {
            return View("Register");
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Register(string firstName, string lastName, string email, string password, string confirmPassword)
        {
            firstName = (firstName ?? "").Trim();
            lastName = (lastName ?? "").Trim();
            email = (email ?? "").Trim();
            password = (password ?? "").Trim();
            confirmPassword = (confirmPassword ?? "").Trim();

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                ViewBag.Error = "First and last name are required.";
                return View("Register");
            }

            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View("Register");
            }

            var existing = await _userManager.FindByEmailAsync(email);
            if (existing != null)
            {
                ViewBag.Error = "That email is already registered.";
                return View("Register");
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName
            };

            var create = await _userManager.CreateAsync(user, password);

            if (!create.Succeeded)
            {
                ViewBag.Error = "Could not create account. Try a stronger password.";
                return View("Register");
            }

            await _userManager.AddToRoleAsync(user, "Volunteer");

            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index", "Inventory");
        }

        // NAVBAR LOGOUT (LINK)
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> LogoutLink()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // Optional: POST logout
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
