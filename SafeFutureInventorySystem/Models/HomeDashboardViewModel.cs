using System.Collections.Generic;

namespace SafeFutureInventorySystem.Models
{
    public class HomeDashboardViewModel
    {
        public int TotalItems { get; set; }
        public int LowStockCount { get; set; }
        public int NoStockCount { get; set; }
        public int ExpiredCount { get; set; }
        public int ExpiringSoonCount { get; set; }
        public List<InventoryItem> AttentionItems { get; set; } = new();
        public List<HomeDashboardActivityItem> RecentActivity { get; set; } = new();
    }

    public class HomeDashboardActivityItem
    {
        public int InventoryItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public DateTime ActivityDate { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string PerformedBy { get; set; } = string.Empty;
        public string ActivityIcon { get; set; } = "fa-history";
        public string ActivityBadgeClass { get; set; } = "badge-primary";
        public string ActivityMeta { get; set; } = string.Empty;
    }
}
