using System;
using System.ComponentModel.DataAnnotations;

namespace SafeFutureInventorySystem.Models
{
    public class InventoryAdjustmentLog
    {
        public int Id { get; set; }

        [Required]
        public int InventoryItemId { get; set; }

        public int OldQuantity { get; set; }

        public int NewQuantity { get; set; }

        [StringLength(500)]
        public string Reason { get; set; }

        public DateTime AdjustmentDate { get; set; }

        [StringLength(100)]
        public string AdjustedBy { get; set; }
    }
}
