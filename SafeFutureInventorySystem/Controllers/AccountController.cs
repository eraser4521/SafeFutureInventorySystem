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

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Inventory");

            ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
            return View("Login");
        }

        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            email = (email ?? "").Trim();
            password = (password ?? "").Trim();

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Email and password are required.";
                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
                return View("Login");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ViewBag.Error = "Invalid email or password.";
                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
                return View("Login");
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                password,
                isPersistent: false,
                lockoutOnFailure: true);

            if (result.IsLockedOut)
            {
                ViewBag.Error = "This account is temporarily locked. Try again later.";
                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
                return View("Login");
            }

            if (!result.Succeeded)
            {
                ViewBag.Error = "Invalid email or password.";
                ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;
                return View("Login");
            }

            if (user.MustChangePassword)
                return RedirectToAction(nameof(ForceChangePassword));

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Inventory");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ForceChangePassword()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction(nameof(Login));

            if (!user.MustChangePassword)
                return RedirectToAction("Index", "Inventory");

            ViewBag.Email = user.Email ?? "";
            return View("ForceChangePassword");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForceChangePassword(string newPassword, string confirmPassword)
        {
            newPassword = (newPassword ?? "").Trim();
            confirmPassword = (confirmPassword ?? "").Trim();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction(nameof(Login));

            if (!user.MustChangePassword)
                return RedirectToAction("Index", "Inventory");

            ViewBag.Email = user.Email ?? "";

            if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ViewBag.Error = "All fields are required.";
                return View("ForceChangePassword");
            }

            if (newPassword.Length < 6)
            {
                ViewBag.Error = "New password must be at least 6 characters.";
                return View("ForceChangePassword");
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match.";
                return View("ForceChangePassword");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                ViewBag.Error = "Could not update password.";
                return View("ForceChangePassword");
            }

            user.MustChangePassword = false;
            user.PasswordSetByAdmin = false;
            user.TemporaryPasswordIssuedAtUtc = null;

            await _userManager.UpdateAsync(user);
            await _signInManager.RefreshSignInAsync(user);

            return RedirectToAction("Index", "Inventory");
        }

        [HttpGet, AllowAnonymous]
        public IActionResult Register()
        {
            TempData["Error"] = "Self registration is disabled. Please contact an admin.";
            return RedirectToAction(nameof(Login));
        }

        [HttpPost, AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Register(string firstName, string lastName, string email, string password, string confirmPassword)
        {
            TempData["Error"] = "Self registration is disabled. Please contact an admin.";
            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> LogoutLink()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
    }
}
