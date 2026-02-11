using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SafeFutureInventorySystem.Models;
using System.Linq;
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

        // GET: /Admin or /Admin/Index
        public IActionResult Index()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        // POST: /Admin/Delete
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

            // Prevent deleting yourself
            var me = await _userManager.GetUserAsync(User);
            if (me != null && me.Id == id)
            {
                TempData["Error"] = "You cannot delete your own admin account.";
                return RedirectToAction(nameof(Index));
            }

            // Prevent deleting the last remaining admin
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var admins = await _userManager.GetUsersInRoleAsync("Admin");
                if (admins.Count <= 1)
                {
                    TempData["Error"] = "You cannot delete the last remaining admin.";
                    return RedirectToAction(nameof(Index));
                }
            }

            var result = await _userManager.DeleteAsync(user);

            TempData["Success"] = result.Succeeded ? "User deleted." : "Could not delete user.";
            return RedirectToAction(nameof(Index));
        }
    }
}
