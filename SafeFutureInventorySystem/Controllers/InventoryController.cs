using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using SafeFutureInventorySystem.Data;
using SafeFutureInventorySystem.Helpers;
using SafeFutureInventorySystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace SafeFutureInventorySystem.Controllers
{
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<InventoryController> _logger;
        private readonly IConfiguration _config;

        public InventoryController(ApplicationDbContext context, ILogger<InventoryController> logger, IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _config = config;
        }

        public IActionResult Index(string searchTerm, string expirationFilter,
            DateTime? fromDate, DateTime? toDate, string sortBy = "Name",
            string sortOrder = "asc", int page = 1, int pageSize = 10)
        {
            try
            {
                var inventoryItems = _context.InventoryItems.AsQueryable();
                var lowStockCount = inventoryItems.Count(i => i.Quantity > 0 && i.LowStockThreshold > 0 && i.Quantity <= i.LowStockThreshold);
                var noStockCount = inventoryItems.Count(i => i.Quantity <= 0);

                var query = inventoryItems;

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(i =>
                        i.Name.Contains(searchTerm) ||
                        (i.Description != null && i.Description.Contains(searchTerm))); 
                }

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

                        case "LowStock":
                            query = query.Where(i => i.Quantity > 0 &&
                                                   i.LowStockThreshold > 0 &&
                                                   i.Quantity <= i.LowStockThreshold);
                            break;

                        case "NoStock":
                            query = query.Where(i => i.Quantity <= 0);
                            break;
                    }
                }

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

                var totalCount = query.Count();

                pageSize = Math.Max(5, Math.Min(pageSize, 100));

                var items = query.Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .Include(i => i.DonationLogs)
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
                    TotalCount = totalCount,
                    LowStockCount = lowStockCount,
                    NoStockCount = noStockCount
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading inventory index.");
                return BuildErrorResult("We couldn't load the inventory data.", ex.ToString(), 500);
            }
        }

        public IActionResult QrCode(int id)
        {
            try
            {
                var item = _context.InventoryItems.Find(id);
                if (item == null) return ItemNotFoundResult(id);

                return View("QrCode", item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing QR code view for item {ItemId}", id);
                return BuildErrorResult("We couldn't generate the QR code for this item.", ex.ToString(), 500);
            }
        }

        public IActionResult QrCodeImage(int id)
        {
            try
            {
                var item = _context.InventoryItems.Find(id);
                if (item == null) return ItemNotFoundResult(id);

                var pathValue = QrCodeHelper.BuildInventoryDetailsPath(Url, item.Id);
                var absolute = QrCodeHelper.ResolveToAbsolute(_config, Request, pathValue);
                var pngBytes = QrCodeHelper.GeneratePng(absolute);

                return File(pngBytes, "image/png", $"qrcode-{item.Id}.png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code image for item {ItemId}", id);
                return BuildErrorResult("We couldn't generate the QR code for this item.", ex.ToString(), 500);
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(InventoryItem item,
    string? DonorName, string? DonorPhone, string? DonorEmail)
        {
            ModelState.Remove("DonationLogs");
            ModelState.Remove("AdjustmentLogs");

            if (!ModelState.IsValid)
                return View(item);

            try
            {
                // Step 1: Try to find a duplicate by barcode
                InventoryItem? existing = null;

                if (!string.IsNullOrWhiteSpace(item.Barcode))
                {
                    existing = _context.InventoryItems
                        .FirstOrDefault(i => i.Barcode == item.Barcode);
                }

                // Step 2: If no barcode match, try name only (case-insensitive, trimmed)
                if (existing == null && !string.IsNullOrWhiteSpace(item.Name))
                {
                    var nameLower = item.Name.Trim().ToLower();
                    existing = _context.InventoryItems
                        .FirstOrDefault(i => i.Name.ToLower() == nameLower);
                }

                if (existing != null)
                {
                    // MERGE into existing item
                    int oldQty = existing.Quantity;
                    existing.Quantity += item.Quantity;
                    if (item.LowStockThreshold > 0)
                    {
                        existing.LowStockThreshold = item.LowStockThreshold;
                    }
                    existing.LastUpdated = DateTime.Now;

                    _context.DonationLogs.Add(new DonationLog
                    {
                        InventoryItemId = existing.Id,
                        QuantityDonated = item.Quantity,
                        DonationDate = DateTime.Now,
                        DonorName = DonorName,
                        Notes = $"Merged into existing stock. Previous qty: {oldQty}"
                    });

                    _context.SaveChanges();

                    TempData["SuccessMessage"] =
                        $"'{existing.Name}' already exists — {item.Quantity} unit(s) merged into existing stock (new total: {existing.Quantity}).";
                    return RedirectToAction(nameof(Details), new { id = existing.Id });
                }
                else
                {
                    // Brand new item
                    item.DateAdded = DateTime.Now;
                    _context.InventoryItems.Add(item);
                    _context.SaveChanges();

                    _context.DonationLogs.Add(new DonationLog
                    {
                        InventoryItemId = item.Id,
                        QuantityDonated = item.Quantity,
                        DonationDate = DateTime.Now,
                        DonorName = DonorName,
                        Notes = "Initial donation — item created."
                    });

                    _context.SaveChanges();

                    TempData["SuccessMessage"] = $"Item '{item.Name}' added successfully!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory item {Name}", item.Name);
                ModelState.AddModelError(string.Empty, "We couldn't save the item. Please try again.");
                return View(item);
            }
        }


        public IActionResult Details(int id, string searchTerm, string expirationFilter,
            DateTime? fromDate, DateTime? toDate, string sortBy = "Name",
            string sortOrder = "asc", int page = 1, int pageSize = 10)
        {
            try
            {
                var item = _context.InventoryItems
                    .Include(i => i.DonationLogs)
                    .Include(i => i.AdjustmentLogs)
                    .FirstOrDefault(i => i.Id == id);

                if (item == null)
                    return ItemNotFoundResult(id);

                ViewBag.AdjustmentHistory = item.AdjustmentLogs
                    .OrderByDescending(l => l.AdjustmentDate)
                    .Take(10)
                    .ToList();

                ViewBag.DonationHistory = item.DonationLogs
                    .OrderByDescending(d => d.DonationDate)
                    .ToList();

                ViewBag.ReturnSearchTerm = searchTerm;
                ViewBag.ReturnExpirationFilter = expirationFilter;
                ViewBag.ReturnFromDate = fromDate;
                ViewBag.ReturnToDate = toDate;
                ViewBag.ReturnSortBy = sortBy;
                ViewBag.ReturnSortOrder = sortOrder;
                ViewBag.ReturnPage = page;
                ViewBag.ReturnPageSize = pageSize;

                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading details for item {ItemId}", id);
                return BuildErrorResult("We couldn't load this inventory item.", ex.ToString(), 500);
            }
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, string searchTerm, string expirationFilter,
            DateTime? fromDate, DateTime? toDate, string sortBy = "Name",
            string sortOrder = "asc", int page = 1, int pageSize = 10)
        {
            try
            {
                var item = _context.InventoryItems.FirstOrDefault(i => i.Id == id);
                if (item == null)
                    return ItemNotFoundResult(id);

                ViewBag.ReturnSearchTerm = searchTerm;
                ViewBag.ReturnExpirationFilter = expirationFilter;
                ViewBag.ReturnFromDate = fromDate;
                ViewBag.ReturnToDate = toDate;
                ViewBag.ReturnSortBy = sortBy;
                ViewBag.ReturnSortOrder = sortOrder;
                ViewBag.ReturnPage = page;
                ViewBag.ReturnPageSize = pageSize;

                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading edit page for item {ItemId}", id);
                return BuildErrorResult("We couldn't load this inventory item for editing.", ex.ToString(), 500);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, string name, string description, string category, int quantity, DateTime? expirationDate,
            int lowStockThreshold, string searchTerm, string expirationFilter,
            DateTime? fromDate, DateTime? toDate, string sortBy = "Name",
            string sortOrder = "asc", int page = 1, int pageSize = 10)
        {
            try
            {
                var item = _context.InventoryItems.FirstOrDefault(i => i.Id == id);
                if (item == null)
                    return ItemNotFoundResult(id);

                name = name?.Trim() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(name))
                {
                    ModelState.AddModelError(nameof(InventoryItem.Name), "Item Name is required.");

                    var invalidModel = new InventoryItem
                    {
                        Id = item.Id,
                        Name = name,
                        Description = description,
                        Category = category,
                        Quantity = quantity,
                        ExpirationDate = expirationDate,
                        LowStockThreshold = lowStockThreshold,
                        DateAdded = item.DateAdded,
                        LastUpdated = item.LastUpdated,
                        Barcode = item.Barcode,
                        DonationLogs = item.DonationLogs,
                        AdjustmentLogs = item.AdjustmentLogs
                    };

                    ViewBag.ReturnSearchTerm = searchTerm;
                    ViewBag.ReturnExpirationFilter = expirationFilter;
                    ViewBag.ReturnFromDate = fromDate;
                    ViewBag.ReturnToDate = toDate;
                    ViewBag.ReturnSortBy = sortBy;
                    ViewBag.ReturnSortOrder = sortOrder;
                    ViewBag.ReturnPage = page;
                    ViewBag.ReturnPageSize = pageSize;

                    return View(invalidModel);
                }

                if (quantity < 0)
                {
                    ModelState.AddModelError(nameof(InventoryItem.Quantity), "Quantity cannot be negative.");

                    var invalidModel = new InventoryItem
                    {
                        Id = item.Id,
                        Name = name,
                        Description = description,
                        Category = category,
                        Quantity = quantity,
                        ExpirationDate = expirationDate,
                        LowStockThreshold = lowStockThreshold,
                        DateAdded = item.DateAdded,
                        LastUpdated = item.LastUpdated,
                        Barcode = item.Barcode,
                        DonationLogs = item.DonationLogs,
                        AdjustmentLogs = item.AdjustmentLogs
                    };

                    ViewBag.ReturnSearchTerm = searchTerm;
                    ViewBag.ReturnExpirationFilter = expirationFilter;
                    ViewBag.ReturnFromDate = fromDate;
                    ViewBag.ReturnToDate = toDate;
                    ViewBag.ReturnSortBy = sortBy;
                    ViewBag.ReturnSortOrder = sortOrder;
                    ViewBag.ReturnPage = page;
                    ViewBag.ReturnPageSize = pageSize;

                    return View(invalidModel);
                }

                if (lowStockThreshold < 0)
                {
                    ModelState.AddModelError(nameof(InventoryItem.LowStockThreshold), "Low-stock threshold cannot be negative.");

                    var invalidModel = new InventoryItem
                    {
                        Id = item.Id,
                        Name = name,
                        Description = description,
                        Category = category,
                        Quantity = quantity,
                        ExpirationDate = expirationDate,
                        LowStockThreshold = lowStockThreshold,
                        DateAdded = item.DateAdded,
                        LastUpdated = item.LastUpdated,
                        Barcode = item.Barcode,
                        DonationLogs = item.DonationLogs,
                        AdjustmentLogs = item.AdjustmentLogs
                    };

                    ViewBag.ReturnSearchTerm = searchTerm;
                    ViewBag.ReturnExpirationFilter = expirationFilter;
                    ViewBag.ReturnFromDate = fromDate;
                    ViewBag.ReturnToDate = toDate;
                    ViewBag.ReturnSortBy = sortBy;
                    ViewBag.ReturnSortOrder = sortOrder;
                    ViewBag.ReturnPage = page;
                    ViewBag.ReturnPageSize = pageSize;

                    return View(invalidModel);
                }

                var changeNotes = new List<string>();
                var oldQuantity = item.Quantity;

                if (item.Quantity != quantity)
                {
                    changeNotes.Add($"Quantity: {item.Quantity} -> {quantity}");
                }

                if (!string.Equals(item.Name, name, StringComparison.Ordinal))
                {
                    changeNotes.Add($"Name: '{item.Name}' -> '{name}'");
                }

                if (!string.Equals(item.Description ?? string.Empty, description ?? string.Empty, StringComparison.Ordinal))
                {
                    changeNotes.Add("Description updated");
                }

                if (!string.Equals(item.Category ?? string.Empty, category ?? string.Empty, StringComparison.Ordinal))
                {
                    changeNotes.Add($"Category: '{(string.IsNullOrWhiteSpace(item.Category) ? "None" : item.Category)}' -> '{(string.IsNullOrWhiteSpace(category) ? "None" : category)}'");
                }

                if (item.ExpirationDate != expirationDate)
                {
                    changeNotes.Add($"Expiration: '{FormatDateForLog(item.ExpirationDate)}' -> '{FormatDateForLog(expirationDate)}'");
                }

                if (item.LowStockThreshold != lowStockThreshold)
                {
                    changeNotes.Add($"Low-stock threshold: {item.LowStockThreshold} -> {lowStockThreshold}");
                }

                item.Name = name;
                item.Description = description;
                item.Category = category;
                item.Quantity = quantity;
                item.ExpirationDate = expirationDate;
                item.LowStockThreshold = lowStockThreshold;
                item.LastUpdated = DateTime.Now;

                if (changeNotes.Count > 0)
                {
                    _context.AdjustmentLogs.Add(new InventoryAdjustmentLog
                    {
                        InventoryItemId = item.Id,
                        OldQuantity = oldQuantity,
                        NewQuantity = quantity,
                        Reason = "Item metadata updated: " + string.Join("; ", changeNotes),
                        AdjustmentDate = DateTime.Now,
                        AdjustedBy = User.Identity?.Name ?? "Admin"
                    });
                }

                _context.SaveChanges();

                TempData["SuccessMessage"] = changeNotes.Count > 0
                    ? $"Item '{item.Name}' updated successfully."
                    : $"No changes were made to '{item.Name}'.";
                return RedirectToAction(nameof(Details), new { id, searchTerm, expirationFilter, fromDate, toDate, sortBy, sortOrder, page, pageSize });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating item metadata for item {ItemId}", id);
                TempData["error"] = "We couldn't update this item.";
                return RedirectToAction(nameof(Edit), new { id, searchTerm, expirationFilter, fromDate, toDate, sortBy, sortOrder, page, pageSize });
            }
        }

        private static string FormatDateForLog(DateTime? date)
        {
            return date.HasValue ? date.Value.ToString("MM/dd/yyyy") : "None";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AdjustQuantity(int id, int newQuantity, string reason)
        {
            try
            {
                var item = _context.InventoryItems.Find(id);
                if (item == null)
                {
                    return new JsonResult(new { success = false, message = "Item not found" });
                }

                if (newQuantity < 0)
                {
                    return new JsonResult(new { success = false, message = "Quantity cannot be negative" });
                }

                var oldQuantity = item.Quantity;
                item.Quantity = newQuantity;
                item.LastUpdated = DateTime.Now;

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
                _logger.LogError(ex, "Error adjusting quantity for item {ItemId}", id);
                return new JsonResult(new { success = false, message = "An error occurred while adjusting the quantity." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateLowStockThreshold(int id, int lowStockThreshold, string searchTerm, string expirationFilter,
            DateTime? fromDate, DateTime? toDate, string sortBy = "Name",
            string sortOrder = "asc", int page = 1, int pageSize = 10)
        {
            try
            {
                if (lowStockThreshold < 0)
                {
                    TempData["error"] = "Low-stock threshold cannot be negative.";
                    return RedirectToAction(nameof(Details), new { id, searchTerm, expirationFilter, fromDate, toDate, sortBy, sortOrder, page, pageSize });
                }

                var item = _context.InventoryItems.Find(id);
                if (item == null)
                {
                    return ItemNotFoundResult(id);
                }

                item.LowStockThreshold = lowStockThreshold;
                item.LastUpdated = DateTime.Now;
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Low-stock threshold updated for '{item.Name}'.";
                return RedirectToAction(nameof(Details), new { id, searchTerm, expirationFilter, fromDate, toDate, sortBy, sortOrder, page, pageSize });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating low-stock threshold for item {ItemId}", id);
                TempData["error"] = "We couldn't update the low-stock threshold.";
                return RedirectToAction(nameof(Details), new { id, searchTerm, expirationFilter, fromDate, toDate, sortBy, sortOrder, page, pageSize });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
                _logger.LogError(ex, "Error removing items {@ItemIds}", itemIds);
                return new JsonResult(new { success = false, message = "An error occurred while removing items." });
            }
        }

        // ── FULL INVENTORY EXPORT (Header "Create PDF" button) ──────────────────────
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("Inventory/ExportFullPdf")]
        public IActionResult ExportFullInventoryPdf()
        {
            try
            {
                var items = _context.InventoryItems
                    .Include(i => i.DonationLogs)
                    .Include(i => i.AdjustmentLogs)
                    .OrderBy(i => i.Category)
                    .ThenBy(i => i.Name)
                    .ToList();

                if (!items.Any())
                {
                    TempData["error"] = "No inventory items to export.";
                    return RedirectToAction("Index");
                }

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4.Rotate(), 15, 15, 15, 15);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // ── Header Banner ──
                    PdfPTable headerTable = new PdfPTable(2);
                    headerTable.WidthPercentage = 100;
                    headerTable.SetWidths(new float[] { 55, 45 });

                    headerTable.AddCell(new PdfPCell(new Phrase("Safe Future Foundation\nComplete Inventory Report",
                        FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.WHITE)))
                    {
                        BackgroundColor = new BaseColor(30, 80, 140),
                        Padding = 12,
                        Border = PdfPCell.NO_BORDER
                    });
                    headerTable.AddCell(new PdfPCell(new Phrase(
                        $"Generated: {DateTime.Now:MMMM dd, yyyy  HH:mm:ss}\nTotal Items: {items.Count}    Total Units: {items.Sum(i => i.Quantity)}",
                        FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.WHITE)))
                    {
                        BackgroundColor = new BaseColor(30, 80, 140),
                        Padding = 12,
                        HorizontalAlignment = Element.ALIGN_RIGHT,
                        Border = PdfPCell.NO_BORDER
                    });
                    document.Add(headerTable);
                    document.Add(new Paragraph("\n"));

                    // ── Main Table ──
                    PdfPTable table = new PdfPTable(10);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 5, 14, 16, 7, 9, 10, 11, 9, 10, 9 });

                    string[] headers = { "ID", "Name", "Description", "Qty", "Threshold", "Category", "Expiration", "Status", "Date Added", "Donor" };
                    bool altHeader = false;
                    foreach (string header in headers)
                    {
                        table.AddCell(new PdfPCell(new Phrase(header,
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE)))
                        {
                            BackgroundColor = altHeader ? new BaseColor(58, 120, 202) : new BaseColor(46, 100, 176),
                            HorizontalAlignment = Element.ALIGN_CENTER,
                            Padding = 7,
                            BorderColor = new BaseColor(255, 255, 255),
                            BorderWidth = 0.5f
                        });
                        altHeader = !altHeader;
                    }

                    bool altRow = false;
                    foreach (var item in items)
                    {
                        var rowBg = altRow ? new BaseColor(240, 246, 255) : BaseColor.WHITE;
                        var lastDonor = item.DonationLogs?.OrderByDescending(d => d.DonationDate)
                            .FirstOrDefault()?.DonorName ?? "—";

                        table.AddCell(new PdfPCell(new Phrase(item.Id.ToString(),
                            FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                        { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = rowBg });
                        table.AddCell(new PdfPCell(new Phrase(item.Name,
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8)))
                        { Padding = 5, BackgroundColor = rowBg });
                        table.AddCell(new PdfPCell(new Phrase(item.Description ?? "—",
                            FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                        { Padding = 5, BackgroundColor = rowBg });
                        table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(),
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9)))
                        { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = new BaseColor(230, 245, 255) });
                        table.AddCell(new PdfPCell(new Phrase(
                            item.LowStockThreshold > 0 ? item.LowStockThreshold.ToString() : "Disabled",
                            FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                        { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = rowBg });
                        table.AddCell(new PdfPCell(new Phrase(item.Category ?? "Uncategorized",
                            FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                        { Padding = 5, BackgroundColor = rowBg });
                        table.AddCell(new PdfPCell(new Phrase(
                            item.ExpirationDate?.ToString("yyyy-MM-dd") ?? "No Expiration",
                            FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                        { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = rowBg });
                        table.AddCell(new PdfPCell(new Phrase(
                            item.IsLowStock ? "Low Stock" : item.ExpirationStatus,
                            FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                        { Padding = 5, HorizontalAlignment = Element.ALIGN_CENTER, BackgroundColor = rowBg });
                        table.AddCell(new PdfPCell(new Phrase(item.DateAdded.ToString("yyyy-MM-dd"),
                            FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                        { Padding = 5, BackgroundColor = rowBg });
                        table.AddCell(new PdfPCell(new Phrase(lastDonor,
                            FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                        { Padding = 5, BackgroundColor = rowBg });

                        altRow = !altRow;
                    }
                    document.Add(table);

                    // ── Summary by Category ──
                    document.Add(new Paragraph("\n"));
                    PdfPTable summaryTable = new PdfPTable(2);
                    summaryTable.WidthPercentage = 45;
                    summaryTable.HorizontalAlignment = Element.ALIGN_LEFT;
                    summaryTable.SpacingAfter = 15;

                    summaryTable.AddCell(new PdfPCell(new Phrase("INVENTORY SUMMARY BY CATEGORY",
                        FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE)))
                    {
                        BackgroundColor = new BaseColor(46, 100, 176),
                        Padding = 7,
                        Colspan = 2,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    });

                    foreach (var cat in items.GroupBy(i => i.Category ?? "Uncategorized")
                        .OrderByDescending(g => g.Sum(i => i.Quantity)))
                    {
                        summaryTable.AddCell(new PdfPCell(new Phrase(cat.Key,
                            FontFactory.GetFont(FontFactory.HELVETICA, 9)))
                        { Padding = 5 });
                        summaryTable.AddCell(new PdfPCell(new Phrase(
                            $"{cat.Count()} items  ({cat.Sum(i => i.Quantity)} units)",
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9)))
                        { Padding = 5, HorizontalAlignment = Element.ALIGN_RIGHT });
                    }
                    document.Add(summaryTable);

                    // ── Tax Footer ──
                    document.Add(new Paragraph(
                        "This report is for internal record-keeping and may be used for tax and audit purposes. " +
                        "All donation information is documented for tax deductibility verification.",
                        FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8))
                    {
                        Alignment = Element.ALIGN_CENTER,
                        SpacingBefore = 10
                    });

                    document.Close();
                    return File(memoryStream.ToArray(), "application/pdf",
                        $"Inventory_Complete_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting full inventory to PDF.");
                TempData["error"] = "Error exporting inventory. Please try again.";
                return RedirectToAction("Index");
            }
        }

        // ── SELECTED ITEMS EXPORT (Table "Export to PDF" button) ────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Inventory/ExportSelectedPdf")]
        public IActionResult ExportSelectedItemsPdf([FromBody] int[] selectedIds)
        {
            try
            {
                if (selectedIds == null || selectedIds.Length == 0)
                    return new JsonResult(new { success = false, message = "No items selected for export" });

                var items = _context.InventoryItems
                    .Include(i => i.DonationLogs)
                    .Include(i => i.AdjustmentLogs)
                    .Where(i => selectedIds.Contains(i.Id))
                    .OrderBy(i => i.Name)
                    .ToList();

                if (!items.Any())
                    return new JsonResult(new { success = false, message = "Selected items not found" });

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4, 20, 20, 20, 20);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // ── Report Header ──
                    document.Add(new Paragraph("Safe Future Foundation",
                        FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, new BaseColor(30, 80, 140)))
                    { Alignment = Element.ALIGN_CENTER });

                    document.Add(new Paragraph("Selected Items Detailed Report",
                        FontFactory.GetFont(FontFactory.HELVETICA, 13, new BaseColor(80, 80, 80)))
                    { Alignment = Element.ALIGN_CENTER, SpacingAfter = 5 });

                    document.Add(new Paragraph(
                        $"Generated: {DateTime.Now:MMMM dd, yyyy  HH:mm:ss}  |  " +
                        $"Items: {items.Count}  |  Total Units: {items.Sum(i => i.Quantity)}",
                        FontFactory.GetFont(FontFactory.HELVETICA, 9))
                    { Alignment = Element.ALIGN_CENTER, SpacingAfter = 20 });

                    // ── One card-style block per item ──
                    foreach (var item in items)
                    {
                        // Item title row
                        PdfPTable titleRow = new PdfPTable(2);
                        titleRow.WidthPercentage = 100;
                        titleRow.SetWidths(new float[] { 65, 35 });
                        titleRow.SpacingBefore = 10;

                        titleRow.AddCell(new PdfPCell(new Phrase($"  #{item.Id}  {item.Name}",
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE)))
                        {
                            BackgroundColor = new BaseColor(46, 100, 176),
                            Padding = 9,
                            Border = PdfPCell.NO_BORDER,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        });

                        string expLabel = item.ExpirationDate.HasValue
                            ? (item.ExpirationDate.Value < DateTime.Now ? "⚠ EXPIRED" : item.ExpirationDate.Value.ToString("yyyy-MM-dd"))
                            : "No Expiration";
                        var expBg = item.ExpirationDate.HasValue && item.ExpirationDate.Value < DateTime.Now
                            ? new BaseColor(180, 30, 30) : new BaseColor(46, 100, 176);

                        titleRow.AddCell(new PdfPCell(new Phrase($"Expiration: {expLabel}",
                            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE)))
                        {
                            BackgroundColor = expBg,
                            Padding = 9,
                            Border = PdfPCell.NO_BORDER,
                            HorizontalAlignment = Element.ALIGN_RIGHT,
                            VerticalAlignment = Element.ALIGN_MIDDLE
                        });
                        document.Add(titleRow);

                        // Item details grid
                        PdfPTable details = new PdfPTable(4);
                        details.WidthPercentage = 100;
                        details.SpacingAfter = 5;

                        var fields = new List<(string, string)>
                {
                    ("Description", item.Description ?? "—"),
                    ("Category",    item.Category ?? "Uncategorized"),
                    ("Quantity",    item.Quantity.ToString()),
                    ("Date Added",  item.DateAdded.ToString("yyyy-MM-dd")),
                    ("Last Updated",item.LastUpdated?.ToString("yyyy-MM-dd HH:mm") ?? "Never"),
                    ("Donors",      (item.DonationLogs?.Count ?? 0).ToString()),
                    ("Adjustments", (item.AdjustmentLogs?.Count ?? 0).ToString())
                };

                        foreach (var (label, val) in fields)
                        {
                            details.AddCell(new PdfPCell(new Phrase(label + ":",
                                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9)))
                            { BackgroundColor = new BaseColor(237, 244, 255), Padding = 5 });
                            details.AddCell(new PdfPCell(new Phrase(val,
                                FontFactory.GetFont(FontFactory.HELVETICA, 9)))
                            { Padding = 5 });
                        }
                        document.Add(details);

                        // Donation History sub-table
                        if (item.DonationLogs?.Any() == true)
                        {
                            document.Add(new Paragraph("Donation History",
                                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10))
                            { SpacingBefore = 6, SpacingAfter = 4 });

                            PdfPTable donTable = new PdfPTable(4);
                            donTable.WidthPercentage = 100;
                            donTable.SetWidths(new float[] { 20, 15, 25, 40 });
                            donTable.SpacingAfter = 4;

                            foreach (string h in new[] { "Date", "Qty", "Donor", "Notes" })
                            {
                                donTable.AddCell(new PdfPCell(new Phrase(h,
                                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.WHITE)))
                                {
                                    BackgroundColor = new BaseColor(140, 170, 210),
                                    Padding = 4,
                                    HorizontalAlignment = Element.ALIGN_CENTER
                                });
                            }

                            foreach (var d in item.DonationLogs.OrderByDescending(d => d.DonationDate).Take(10))
                            {
                                donTable.AddCell(new PdfPCell(new Phrase(d.DonationDate.ToString("yyyy-MM-dd"),
                                    FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                                { Padding = 4 });
                                donTable.AddCell(new PdfPCell(new Phrase($"+{d.QuantityDonated}",
                                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8)))
                                { Padding = 4, HorizontalAlignment = Element.ALIGN_CENTER });
                                donTable.AddCell(new PdfPCell(new Phrase(d.DonorName ?? "—",
                                    FontFactory.GetFont(FontFactory.HELVETICA, 8)))
                                { Padding = 4 });
                                donTable.AddCell(new PdfPCell(new Phrase(d.Notes ?? "—",
                                    FontFactory.GetFont(FontFactory.HELVETICA, 7)))
                                { Padding = 4 });
                            }
                            document.Add(donTable);
                        }

                        document.Add(new Paragraph("\n"));
                    }

                    // ── Footer ──
                    document.Add(new Paragraph(
                        "This detailed report includes donation history for tax and audit purposes. " +
                        "All information is documented and can be verified through the system.",
                        FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8))
                    { Alignment = Element.ALIGN_CENTER, SpacingBefore = 15 });

                    document.Close();
                    return File(memoryStream.ToArray(), "application/pdf",
                        $"Selected_Items_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting selected items to PDF.");
                return new JsonResult(new { success = false, message = "Error generating PDF. Please try again." });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("ExportCsv")]
        public IActionResult ExportInventoryCsv()
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
                using (TextWriter writer = new StreamWriter(memoryStream, System.Text.Encoding.UTF8))
                {
                    // Write CSV headers
                    string[] headers = { "ID", "Name", "Description", "Quantity", "Low Stock Threshold", "Low Stock Alert", "Category", "Expiration Date", "Date Added" };
                    writer.WriteLine(string.Join(",", headers.Select(h => $"\"{h}\"")));

                    // Write data rows
                    foreach (var item in items)
                    {
                        var row = new[]
                        {
                            item.Id.ToString(),
                            $"\"{item.Name}\"",
                            $"\"{item.Description ?? ""}\"",
                            item.Quantity.ToString(),
                            item.LowStockThreshold.ToString(),
                            item.IsLowStock ? "\"Yes\"" : "\"No\"",
                            $"\"{item.Category ?? ""}\"",
                            item.ExpirationDate?.ToString("yyyy-MM-dd") ?? "N/A",
                            item.DateAdded.ToString("yyyy-MM-dd")
                        };
                        writer.WriteLine(string.Join(",", row));
                    }

                    writer.Flush();
                    byte[] csvBytes = memoryStream.ToArray();
                    string fileName = $"Inventory_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                    return File(csvBytes, "text/csv", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting inventory to CSV.");
                TempData["error"] = "Error exporting inventory. Please try again.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [Route("ExportExcel")]
        public IActionResult ExportInventoryExcel()
        {
            try
            {
                var items = _context.InventoryItems
                    .Include(i => i.DonationLogs)
                    .Include(i => i.AdjustmentLogs)
                    .OrderBy(i => i.Name)
                    .ToList();

                if (!items.Any())
                {
                    TempData["error"] = "No inventory items to export.";
                    return RedirectToAction("Index");
                }

                ExcelPackage.License.SetNonCommercialOrganization("Safe Future Foundation");

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Inventory");

                var headers = new[]
                {
                    "ID",
                    "Name",
                    "Description",
                    "Quantity",
                    "Low Stock Threshold",
                    "Low Stock Alert",
                    "Category",
                    "Barcode",
                    "Expiration Date",
                    "Status",
                    "Date Added",
                    "Last Updated",
                    "Latest Donor",
                    "Donation Count",
                    "Adjustment Count"
                };

                for (var col = 0; col < headers.Length; col++)
                {
                    worksheet.Cells[1, col + 1].Value = headers[col];
                }

                using (var headerRange = worksheet.Cells[1, 1, 1, headers.Length])
                {
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(46, 100, 176));
                    headerRange.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                for (var rowIndex = 0; rowIndex < items.Count; rowIndex++)
                {
                    var item = items[rowIndex];
                    var row = rowIndex + 2;
                    var latestDonation = item.DonationLogs
                        .OrderByDescending(d => d.DonationDate)
                        .FirstOrDefault();

                    worksheet.Cells[row, 1].Value = item.Id;
                    worksheet.Cells[row, 2].Value = item.Name;
                    worksheet.Cells[row, 3].Value = item.Description;
                    worksheet.Cells[row, 4].Value = item.Quantity;
                    worksheet.Cells[row, 5].Value = item.LowStockThreshold;
                    worksheet.Cells[row, 6].Value = item.IsLowStock ? "Yes" : "No";
                    worksheet.Cells[row, 7].Value = item.Category;
                    worksheet.Cells[row, 8].Value = item.Barcode;
                    worksheet.Cells[row, 9].Value = item.ExpirationDate?.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 10].Value = item.ExpirationStatus;
                    worksheet.Cells[row, 11].Value = item.DateAdded.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 12].Value = item.LastUpdated?.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cells[row, 13].Value = latestDonation?.DonorName;
                    worksheet.Cells[row, 14].Value = item.DonationLogs.Count;
                    worksheet.Cells[row, 15].Value = item.AdjustmentLogs.Count;
                }

                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                worksheet.View.FreezePanes(2, 1);

                var excelBytes = package.GetAsByteArray();
                var fileName = $"Inventory_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(
                    excelBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting inventory to Excel.");
                TempData["error"] = "Error exporting inventory to Excel. Please try again.";
                return RedirectToAction("Index");
            }
        }


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
                        expirationStatus = item.ExpirationStatus,
                        lowStockThreshold = item.LowStockThreshold,
                        isLowStock = item.IsLowStock
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving item details for item {ItemId}", id);
                return new JsonResult(new { success = false, message = "An error occurred while retrieving item details." });
            }
        }

        [HttpGet]
        public JsonResult SearchItems(string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term))
                    return new JsonResult(new List<object>());

                var items = _context.InventoryItems
                    .Where(i => i.Name.Contains(term))
                    .Take(10)
                    .Select(i => new
                    {
                        id = i.Id,
                        label = i.Name,
                        value = i.Name,
                        quantity = i.Quantity,
                        category = i.Category ?? "Uncategorized"
                    })
                    .ToList();

                return new JsonResult(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching items with term {Term}", term);
                return new JsonResult(new List<object>());
            }
        }

        private IActionResult BuildErrorResult(string userMessage, string? detail, int statusCode)
        {
            Response.StatusCode = statusCode;
            return View("~/Views/Shared/Error.cshtml", new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier,
                UserMessage = userMessage,
                Detail = detail,
                Path = HttpContext.Request?.Path.Value,
                StatusCode = statusCode
            });
        }

        private IActionResult ItemNotFoundResult(int id)
        {
            _logger.LogWarning("Inventory item {Id} not found for request {Path}", id, HttpContext.Request?.Path.Value);
            return BuildErrorResult("We couldn't find that inventory item. If you scanned a QR code, it may be out of date.", $"Inventory item {id} was not found.", 404);
        }
    }
}
