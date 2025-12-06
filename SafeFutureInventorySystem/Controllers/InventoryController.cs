using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeFutureInventorySystem.Data;
using SafeFutureInventorySystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

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
            string sortOrder = "asc", int page = 1, int pageSize = 10)
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

            // NEW: User Story - Configurable page size (default 10, max 100)
            pageSize = Math.Max(5, Math.Min(pageSize, 100));

            // Apply pagination
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

        // NEW: User Story - Add new item to inventory
        // GET: Inventory/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inventory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(InventoryItem item)
        {
            if (ModelState.IsValid)
            {
                item.DateAdded = DateTime.Now;
                _context.InventoryItems.Add(item);
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Item '{item.Name}' added successfully!";
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }

        // NEW: User Story - View item details/count
        // GET: Inventory/Details/5
        public IActionResult Details(int id)
        {
            var item = _context.InventoryItems.Find(id);
            if (item == null)
            {
                return NotFound();
            }

            // Get adjustment history
            var adjustmentHistory = _context.AdjustmentLogs
                .Where(l => l.InventoryItemId == id)
                .OrderByDescending(l => l.AdjustmentDate)
                .Take(10)
                .ToList();

            ViewBag.AdjustmentHistory = adjustmentHistory;
            return View(item);
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

                // NEW: Validation - ensure non-negative quantity
                if (newQuantity < 0)
                {
                    return new JsonResult(new { success = false, message = "Quantity cannot be negative" });
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

        // NEW: Export Inventory to PDF
        [HttpGet]
        [Route("Export")]
        public IActionResult ExportInventoryPdf()
        {
            try
            {
                var items = _context.InventoryItems.OrderBy(i => i.Name).ToList();

                if (!items.Any())
                {
                    TempData["error"] = "No inventory items to export.";
                    return RedirectToAction("Index");
                }

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Create document
                    Document document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // Add title
                    PdfPTable titleTable = new PdfPTable(1);
                    titleTable.WidthPercentage = 100;
                    PdfPCell titleCell = new PdfPCell(new Phrase("Inventory Report",
                        FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16, BaseColor.WHITE)))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 10,
                        BackgroundColor = new BaseColor(0, 51, 102)
                    };
                    titleTable.AddCell(titleCell);
                    document.Add(titleTable);

                    // Add date
                    Paragraph dateParagraph = new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                        FontFactory.GetFont(FontFactory.HELVETICA, 10))
                    {
                        Alignment = Element.ALIGN_RIGHT,
                        SpacingAfter = 20
                    };
                    document.Add(dateParagraph);

                    // Create data table
                    PdfPTable table = new PdfPTable(8);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 10, 18, 18, 10, 12, 12, 12, 12 });

                    // Header row
                    string[] headers = { "ID", "Name", "Description", "Quantity", "Category", "Barcode", "Expiration Date", "Date Added" };
                    foreach (string header in headers)
                    {
                        PdfPCell headerCell = new PdfPCell(new Phrase(header,
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE)))
                        {
                            BackgroundColor = new BaseColor(0, 102, 204),
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 8
                        };
                        table.AddCell(headerCell);
                    }

                    // Data rows
                    foreach (var item in items)
                    {
                        table.AddCell(new PdfPCell(new Phrase(item.Id.ToString(),
                            FontFactory.GetFont(FontFactory.HELVETICA, 9)))
                        { Padding = 6 });
                        table.AddCell(new PdfPCell(new Phrase(item.Name,
                            FontFactory.GetFont(FontFactory.HELVETICA, 9)))
                        { Padding = 6 });
                        table.AddCell(new PdfPCell(new Phrase(item.Description ?? "",
                            FontFactory.GetFont(FontFactory.HELVETICA, 9)))
                        { Padding = 6 });
                        table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(),
                            FontFactory.GetFont(FontFactory.HELVETICA, 9)))
                        { Padding = 6, HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(item.Category ?? "",
                            FontFactory.GetFont(FontFactory.HELVETICA, 9)))
                        { Padding = 6 });
                        table.AddCell(new PdfPCell(new Phrase(item.Barcode ?? "",
                            FontFactory.GetFont(FontFactory.HELVETICA, 9)))
                        { Padding = 6 });
                        table.AddCell(new PdfPCell(new Phrase(item.ExpirationDate?.ToString("yyyy-MM-dd") ?? "N/A",
                            FontFactory.GetFont(FontFactory.HELVETICA, 9)))
                        { Padding = 6, HorizontalAlignment = Element.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(item.DateAdded.ToString("yyyy-MM-dd"),
                            FontFactory.GetFont(FontFactory.HELVETICA, 9)))
                        { Padding = 6 });
                    }

                    document.Add(table);

                    // Add summary
                    Paragraph summary = new Paragraph($"\nTotal Items: {items.Count}",
                        FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11))
                    {
                        SpacingBefore = 20
                    };
                    document.Add(summary);

                    document.Close();
                    byte[] pdfBytes = memoryStream.ToArray();
                    string fileName = $"Inventory_Export_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                    return File(pdfBytes, "application/pdf", fileName);
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error exporting inventory: {ex.Message}";
                return RedirectToAction("Index");
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
                        barcode = item.Barcode,
                        expirationStatus = item.ExpirationStatus
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
