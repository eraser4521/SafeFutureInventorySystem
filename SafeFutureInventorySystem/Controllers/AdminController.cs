using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SafeFutureInventorySystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SafeFutureInventorySystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var model = new List<AdminUserRow>();

            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                var role = roles.Contains("Admin") ? "Admin" : "Volunteer";

                var name = (string.IsNullOrWhiteSpace(u.FirstName) && string.IsNullOrWhiteSpace(u.LastName))
                    ? (u.Email ?? "")
                    : ((u.FirstName ?? "") + " " + (u.LastName ?? "")).Trim();

                model.Add(new AdminUserRow
                {
                    Id = u.Id,
                    Email = u.Email ?? "",
                    Name = name,
                    Role = role,
                    MustChangePassword = u.MustChangePassword
                });
            }

            model = model
                .OrderByDescending(x => x.Role == "Admin")
                .ThenBy(x => x.Email)
                .ToList();

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateUserVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserVM vm)
        {
            if (vm == null)
            {
                TempData["Error"] = "Invalid form.";
                return RedirectToAction(nameof(Index));
            }

            vm.Email = (vm.Email ?? "").Trim();
            vm.FirstName = (vm.FirstName ?? "").Trim();
            vm.LastName = (vm.LastName ?? "").Trim();
            vm.Role = (vm.Role == "Admin") ? "Admin" : "Volunteer";

            if (string.IsNullOrWhiteSpace(vm.Email))
                ModelState.AddModelError("", "Email is required.");

            if (string.IsNullOrWhiteSpace(vm.FirstName))
                ModelState.AddModelError("", "First name is required.");

            if (string.IsNullOrWhiteSpace(vm.LastName))
                ModelState.AddModelError("", "Last name is required.");

            if (!ModelState.IsValid)
                return View(vm);

            var existing = await _userManager.FindByEmailAsync(vm.Email);
            if (existing != null)
            {
                ModelState.AddModelError("", "A user with that email already exists.");
                return View(vm);
            }

            var tempCode = GenerateTemporaryCode();

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                EmailConfirmed = true,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                MustChangePassword = true,
                PasswordSetByAdmin = true,
                TemporaryPasswordIssuedAtUtc = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, tempCode);

            if (!result.Succeeded)
            {
                foreach (var e in result.Errors)
                    ModelState.AddModelError("", e.Description);

                return View(vm);
            }

            await _userManager.AddToRoleAsync(user, vm.Role);

            TempData["Success"] = "User created successfully.";
            TempData["GeneratedPassword"] = $"Temporary code for {user.Email}: {tempCode}";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            id = (id ?? "").Trim();

            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Error"] = "Invalid user.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.Contains("Admin") ? "Admin" : "Volunteer";

            var vm = new AdminEditUserVM
            {
                Id = user.Id,
                Email = user.Email ?? "",
                FirstName = user.FirstName ?? "",
                LastName = user.LastName ?? "",
                CurrentRole = role,
                NewRole = role
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminEditUserVM vm)
        {
            if (vm == null || string.IsNullOrWhiteSpace(vm.Id))
            {
                TempData["Error"] = "Invalid user.";
                return RedirectToAction(nameof(Index));
            }

            vm.Email = (vm.Email ?? "").Trim();
            vm.FirstName = (vm.FirstName ?? "").Trim();
            vm.LastName = (vm.LastName ?? "").Trim();
            vm.NewRole = (vm.NewRole == "Admin") ? "Admin" : "Volunteer";

            if (string.IsNullOrWhiteSpace(vm.Email))
                ModelState.AddModelError("", "Email is required.");

            if (string.IsNullOrWhiteSpace(vm.FirstName))
                ModelState.AddModelError("", "First name is required.");

            if (string.IsNullOrWhiteSpace(vm.LastName))
                ModelState.AddModelError("", "Last name is required.");

            var user = await _userManager.FindByIdAsync(vm.Id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var duplicate = await _userManager.FindByEmailAsync(vm.Email);
            if (duplicate != null && duplicate.Id != user.Id)
                ModelState.AddModelError("", "Another user already uses that email.");

            var me = await _userManager.GetUserAsync(User);
            var isMe = me != null && me.Id == user.Id;

            var rolesNow = await _userManager.GetRolesAsync(user);
            var isAdminNow = rolesNow.Contains("Admin");
            var wantRole = vm.NewRole;

            if (isMe && isAdminNow && wantRole != "Admin")
                ModelState.AddModelError("", "You cannot remove Admin from your own account.");

            if (isAdminNow && wantRole != "Admin")
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                if (admins.Count <= 1)
                    ModelState.AddModelError("", "You cannot remove Admin from the last remaining admin.");
            }

            if (!ModelState.IsValid)
            {
                vm.CurrentRole = isAdminNow ? "Admin" : "Volunteer";
                return View(vm);
            }

            user.Email = vm.Email;
            user.UserName = vm.Email;
            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var e in updateResult.Errors)
                    ModelState.AddModelError("", e.Description);

                vm.CurrentRole = isAdminNow ? "Admin" : "Volunteer";
                return View(vm);
            }

            if (wantRole == "Admin" && !isAdminNow)
            {
                await _userManager.RemoveFromRoleAsync(user, "Volunteer");
                await _userManager.AddToRoleAsync(user, "Admin");
            }
            else if (wantRole == "Volunteer" && isAdminNow)
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                await _userManager.AddToRoleAsync(user, "Volunteer");
            }

            TempData["Success"] = "User updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id)
        {
            id = (id ?? "").Trim();

            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Error"] = "Invalid user.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["Error"] = "Admin accounts cannot be reset from this page.";
                return RedirectToAction(nameof(Index));
            }

            var tempCode = GenerateTemporaryCode();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, tempCode);

            if (!result.Succeeded)
            {
                TempData["Error"] = "Could not reset password.";
                return RedirectToAction(nameof(Index));
            }

            user.MustChangePassword = true;
            user.PasswordSetByAdmin = true;
            user.TemporaryPasswordIssuedAtUtc = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            TempData["Success"] = "Temporary code generated.";
            TempData["GeneratedPassword"] = $"Temporary code for {user.Email}: {tempCode}";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            id = (id ?? "").Trim();

            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Error"] = "Invalid user.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Index));
            }

            var me = await _userManager.GetUserAsync(User);
            if (me != null && me.Id == id)
            {
                TempData["Error"] = "You cannot delete your own admin account.";
                return RedirectToAction(nameof(Index));
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["Error"] = "Admin accounts cannot be deleted from this page.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            TempData["Success"] = result.Succeeded ? "User deleted." : "Could not delete user.";

            return RedirectToAction(nameof(Index));
        }

        private static string GenerateTemporaryCode()
        {
            return RandomNumberGenerator.GetInt32(100000, 1000000).ToString();
        }
    }

    public class AdminUserRow
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string Name { get; set; } = "";
        public string Role { get; set; } = "";
        public bool MustChangePassword { get; set; }
    }

    public class AdminEditUserVM
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string CurrentRole { get; set; } = "";
        public string NewRole { get; set; } = "Volunteer";
    }

    public class CreateUserVM
    {
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Role { get; set; } = "Volunteer";
    }
}