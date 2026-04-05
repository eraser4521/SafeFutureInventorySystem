using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SafeFutureInventorySystem.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Range(0, int.MaxValue)]
        public int LowStockThreshold { get; set; }

        [StringLength(50)]
        public string? Barcode { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; }

        public DateTime? LastUpdated { get; set; }

        public string? Category { get; set; }

        // Navigation collections
        public List<DonationLog> DonationLogs { get; set; } = new();
        public List<InventoryAdjustmentLog> AdjustmentLogs { get; set; } = new();

        public string ExpirationStatus
        {
            get
            {
                if (!ExpirationDate.HasValue)
                    return "No Expiration";

                var daysUntilExpiration = (ExpirationDate.Value - DateTime.Now).Days;

                if (daysUntilExpiration < 0)
                    return "Expired";
                else if (daysUntilExpiration <= 7)
                    return "Expiring Soon";
                else if (daysUntilExpiration <= 30)
                    return "Expiring This Month";
                else
                    return "Good";
            }
        }

        public bool IsLowStock => LowStockThreshold > 0 && Quantity <= LowStockThreshold;
    }
}
