using System.Collections.Generic;

namespace SafeFutureInventorySystem.Models
{
    public class HomeDashboardViewModel
    {
        public int TotalItems { get; set; }
        public int LowStockCount { get; set; }
        public int NoStockCount { get; set; }
        public int ExpiringSoonCount { get; set; }
        public List<InventoryItem> AttentionItems { get; set; } = new();
        public List<InventoryItem> RecentItems { get; set; } = new();
    }
}
