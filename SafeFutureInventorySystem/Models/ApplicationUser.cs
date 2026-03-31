using Microsoft.AspNetCore.Identity;
using System;

namespace SafeFutureInventorySystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public bool MustChangePassword { get; set; } = false;
        public bool PasswordSetByAdmin { get; set; } = false;
        public DateTime? TemporaryPasswordIssuedAtUtc { get; set; }
    }
}