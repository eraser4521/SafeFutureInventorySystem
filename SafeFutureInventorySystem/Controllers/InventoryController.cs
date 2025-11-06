using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeFutureInventorySystem.Data;
using SafeFutureInventorySystem.Models;

namespace SafeFutureInventorySystem.Controllers
{
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InventoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Inventory
        public IActionResult Index(string searchTerm, string expirationFilter,
            DateTime? fromDate, DateTime? toDate, string sortBy = "Name",
            string sortOrder = "asc", int page = 1)
        {
            var query = _context.InventoryItems.AsQueryable();

            // USER STORY 4: Search by name
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(i =>
                    i.Name.Contains(searchTerm) ||
                    (i.Description != null && i.Description.Contains(searchTerm)) ||
                    (i.Barcode != null && i.Barcode.Contains(searchTerm)));
            }

            // USER STORY 2: Filter by expiration date
            if (!string.IsNullOrWhiteSpace(expirationFilter))
            {
                var today = DateTime.Now.Date;

                switch (expirationFilter)
                {
                    case "Expired":
                        query = query.Where(i => i.ExpirationDate.HasValue &&
                                               i.ExpirationDate.Value < today);
                        break;

                    case "ExpiringSoon":
                        var weekFromNow = today.AddDays(7);
                        query = query.Where(i => i.ExpirationDate.HasValue &&
                                               i.ExpirationDate.Value >= today &&
                                               i.ExpirationDate.Value <= weekFromNow);
                        break;

                    case "ExpiringThisMonth":
                        var monthFromNow = today.AddDays(30);
                        query = query.Where(i => i.ExpirationDate.HasValue &&
                                               i.ExpirationDate.Value >= today &&
                                               i.ExpirationDate.Value <= monthFromNow);
                        break;

                    case "Good":
                        var thirtyDaysFromNow = today.AddDays(30);
                        query = query.Where(i => !i.ExpirationDate.HasValue ||
                                               i.ExpirationDate.Value > thirtyDaysFromNow);
                        break;

                    case "NoExpiration":
                        query = query.Where(i => !i.ExpirationDate.HasValue);
                        break;
                }
            }

            // Date range filter
            if (fromDate.HasValue)
            {
                query = query.Where(i => i.ExpirationDate.HasValue &&
                                        i.ExpirationDate.Value >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(i => i.ExpirationDate.HasValue &&
                                        i.ExpirationDate.Value <= toDate.Value);
            }

            // Sorting
            query = sortBy switch
            {
                "ExpirationDate" => sortOrder == "desc"
                    ? query.OrderByDescending(i => i.ExpirationDate)
                    : query.OrderBy(i => i.ExpirationDate),
                "Quantity" => sortOrder == "desc"
                    ? query.OrderByDescending(i => i.Quantity)
                    : query.OrderBy(i => i.Quantity),
                "DateAdded" => sortOrder == "desc"
                    ? query.OrderByDescending(i => i.DateAdded)
                    : query.OrderBy(i => i.DateAdded),
                _ => sortOrder == "desc"
                    ? query.OrderByDescending(i => i.Name)
                    : query.OrderBy(i => i.Name)
            };

            // Get total count before pagination
            var totalCount = query.Count();

            // Apply pagination
            var pageSize = 10;
            var items = query.Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

            var viewModel = new InventoryFilterViewModel
            {
                Items = items,
                SearchTerm = searchTerm,
                ExpirationFilter = expirationFilter,
                FromDate = fromDate,
                ToDate = toDate,
                SortBy = sortBy,
                SortOrder = sortOrder,
                PageNumber = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return View(viewModel);
        }

        // USER STORY 3: Adjust quantity
        [HttpPost]
        public JsonResult AdjustQuantity(int id, int newQuantity, string reason)
        {
            try
            {
                var item = _context.InventoryItems.Find(id);
                if (item == null)
                {
                    return new JsonResult(new { success = false, message = "Item not found" });
                }

                var oldQuantity = item.Quantity;
                item.Quantity = newQuantity;
                item.LastUpdated = DateTime.Now;

                // Log the adjustment
                var log = new InventoryAdjustmentLog
                {
                    InventoryItemId = id,
                    OldQuantity = oldQuantity,
                    NewQuantity = newQuantity,
                    Reason = reason,
                    AdjustmentDate = DateTime.Now,
                    AdjustedBy = User.Identity?.Name ?? "Admin"
                };

                _context.AdjustmentLogs.Add(log);
                _context.SaveChanges();

                return new JsonResult(new
                {
                    success = true,
                    message = "Quantity updated successfully",
                    newQuantity = newQuantity
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        // USER STORY 1: Remove selected items
        [HttpPost]
        public JsonResult RemoveItems([FromBody] int[] itemIds)
        {
            try
            {
                if (itemIds == null || itemIds.Length == 0)
                {
                    return new JsonResult(new { success = false, message = "No items selected" });
                }

                var itemsToRemove = _context.InventoryItems
                    .Where(i => itemIds.Contains(i.Id))
                    .ToList();

                _context.InventoryItems.RemoveRange(itemsToRemove);
                _context.SaveChanges();

                return new JsonResult(new
                {
                    success = true,
                    message = $"{itemsToRemove.Count} item(s) removed successfully",
                    count = itemsToRemove.Count
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        // Get item details
        [HttpGet]
        public JsonResult GetItemDetails(int id)
        {
            try
            {
                var item = _context.InventoryItems.Find(id);
                if (item == null)
                {
                    return new JsonResult(new { success = false, message = "Item not found" });
                }

                return new JsonResult(new
                {
                    success = true,
                    item = new
                    {
                        id = item.Id,
                        name = item.Name,
                        quantity = item.Quantity,
                        description = item.Description,
                        barcode = item.Barcode
                    }
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        // Search autocomplete
        [HttpGet]
        public JsonResult SearchItems(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return new JsonResult(new System.Collections.Generic.List<object>());
            }

            var items = _context.InventoryItems
                .Where(i => i.Name.Contains(term))
                .Take(10)
                .Select(i => new {
                    id = i.Id,
                    label = i.Name,
                    value = i.Name,
                    quantity = i.Quantity
                })
                .ToList();

            return new JsonResult(items);
        }
    }
}
