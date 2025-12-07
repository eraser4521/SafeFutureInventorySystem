using System;
using System.Collections.Generic;

namespace SafeFutureInventorySystem.Models
{
    public class InventoryFilterViewModel
    {
        public List<InventoryItem> Items { get; set; }

        // Search parameters
        public string SearchTerm { get; set; }

        // Filter parameters
        public string ExpirationFilter { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // Sorting
        public string SortBy { get; set; }
        public string SortOrder { get; set; }

        // Pagination properties
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalCount { get; set; }

        // Computed property for total pages
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
