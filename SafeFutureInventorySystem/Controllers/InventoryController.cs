using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SafeFutureInventorySystem.Data;
using SafeFutureInventorySystem.Helpers;
using SafeFutureInventorySystem.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace SafeFutureInventorySystem.Controllers
{
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
                var query = _context.InventoryItems.AsQueryable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    query = query.Where(i =>
                        i.Name.Contains(searchTerm) ||
                        (i.Description != null && i.Description.Contains(searchTerm)) ||
                        (i.Barcode != null && i.Barcode.Contains(searchTerm)));
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
        public IActionResult Create(InventoryItem item)
        {
            if (!ModelState.IsValid)
            {
                return View(item);
            }

            try
            {
                item.DateAdded = DateTime.Now;
                _context.InventoryItems.Add(item);
                _context.SaveChanges();

                TempData["SuccessMessage"] = $"Item '{item.Name}' added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory item {Name}", item.Name);
                ModelState.AddModelError(string.Empty, "We couldn't save the item. Please try again.");
                return View(item);
            }
        }

        public IActionResult Details(int id)
        {
            try
            {
                var item = _context.InventoryItems.Find(id);
                if (item == null)
                {
                    return ItemNotFoundResult(id);
                }

                var adjustmentHistory = _context.AdjustmentLogs
                    .Where(l => l.InventoryItemId == id)
                    .OrderByDescending(l => l.AdjustmentDate)
                    .Take(10)
                    .ToList();

                ViewBag.AdjustmentHistory = adjustmentHistory;
                return View(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading details for item {ItemId}", id);
                return BuildErrorResult("We couldn't load this inventory item.", ex.ToString(), 500);
            }
        }

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
                    Document document = new Document(PageSize.A4.Rotate(), 10, 10, 10, 10);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

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

                    Paragraph dateParagraph = new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                        FontFactory.GetFont(FontFactory.HELVETICA, 10))
                    {
                        Alignment = Element.ALIGN_RIGHT,
                        SpacingAfter = 20
                    };
                    document.Add(dateParagraph);

                    PdfPTable table = new PdfPTable(8);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 10, 18, 18, 10, 12, 12, 12, 12 });

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
                _logger.LogError(ex, "Error exporting inventory to PDF.");
                TempData["error"] = "Error exporting inventory. Please try again.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
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
                    string[] headers = { "ID", "Name", "Description", "Quantity", "Category", "Barcode", "Expiration Date", "Date Added" };
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
                            $"\"{item.Category ?? ""}\"",
                            $"\"{item.Barcode ?? ""}\"",
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
        [Route("ExportExcel")]


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
                {
                    return new JsonResult(new System.Collections.Generic.List<object>());
                }

                var items = _context.InventoryItems
                    .Where(i => i.Name.Contains(term))
                    .Take(10)
                    .Select(i => new
                    {
                        id = i.Id,
                        label = i.Name,
                        value = i.Name,
                        quantity = i.Quantity
                    })
                    .ToList();

                return new JsonResult(items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching items with term {Term}", term);
                return new JsonResult(new System.Collections.Generic.List<object>());
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
