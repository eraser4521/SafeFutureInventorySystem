using System;
using System.ComponentModel.DataAnnotations;

namespace SafeFutureInventorySystem.Models
{
    public class DonationLog
    {
        public int Id { get; set; }

        [Required]
        public int InventoryItemId { get; set; }

        // Navigation property
        public InventoryItem? InventoryItem { get; set; }

        [Required]
        public int QuantityDonated { get; set; }

        public DateTime DonationDate { get; set; }

        [StringLength(200)]
        [Display(Name = "Donor Name")]
        public string? DonorName { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
