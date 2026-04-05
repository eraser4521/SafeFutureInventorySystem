using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SafeFutureInventorySystem.Data;
using SafeFutureInventorySystem.Helpers;
using SafeFutureInventorySystem.Models;

namespace SafeFutureInventorySystem.Controllers;

 [Authorize]
public class QRCodesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<QRCodesController> _logger;

    public QRCodesController(ApplicationDbContext context, IConfiguration config, ILogger<QRCodesController> logger)
    {
        _context = context;
        _config = config;
        _logger = logger;
    }

    // Show a page to generate QR code for an inventory item
    public IActionResult Generate(int id)
    {
        var item = _context.InventoryItems.FirstOrDefault(i => i.Id == id);
        if (item == null) return NotFound();
        return View(item);
    }

    // Return the QR code image (PNG) for a given inventory item value (preview)
    public IActionResult ImageForItem(int inventoryItemId)
    {
        var item = _context.InventoryItems.FirstOrDefault(i => i.Id == inventoryItemId);
        if (item == null) return NotFound();
        // Preview: generate the same path-only value and resolve to absolute when rendering the QR
        var pathValue = QrCodeHelper.BuildInventoryDetailsPath(Url, item.Id);
        var absolute = QrCodeHelper.ResolveToAbsolute(_config, Request, pathValue);
        return GenerateImageResult(absolute);
    }

    [HttpGet]
    public IActionResult BatchLabels([FromQuery] int[] ids)
    {
        if (ids == null || ids.Length == 0)
        {
            TempData["error"] = "Select at least one item to generate QR labels.";
            return RedirectToAction("Index", "Inventory");
        }

        var items = _context.InventoryItems
            .Where(i => ids.Contains(i.Id))
            .OrderBy(i => i.Name)
            .ToList();

        if (items.Count == 0)
        {
            TempData["error"] = "The selected inventory items could not be found.";
            return RedirectToAction("Index", "Inventory");
        }

        return View(items);
    }

    [HttpGet]
    public IActionResult ExportBatchLabelsPdf([FromQuery] int[] ids, string? template = null, string? preset = null)
    {
        if (ids == null || ids.Length == 0)
        {
            TempData["error"] = "Select at least one item to export QR labels.";
            return RedirectToAction("Index", "Inventory");
        }

        var items = _context.InventoryItems
            .Where(i => ids.Contains(i.Id))
            .OrderBy(i => i.Name)
            .ToList();

        if (items.Count == 0)
        {
            TempData["error"] = "The selected inventory items could not be found.";
            return RedirectToAction("Index", "Inventory");
        }

        var normalizedTemplate = NormalizeTemplate(template);
        var layout = GetPresetLayout(preset);
        var fileName = $"QR_Labels_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

        using var memoryStream = new MemoryStream();
        using var document = new Document(PageSize.LETTER, 24f, 24f, 24f, 24f);
        PdfWriter.GetInstance(document, memoryStream);
        document.Open();

        var table = new PdfPTable(layout.Columns)
        {
            WidthPercentage = 100,
            SpacingBefore = 8f
        };

        var columnWidth = Enumerable.Repeat(1f, layout.Columns).ToArray();
        table.SetWidths(columnWidth);

        foreach (var item in items)
        {
            table.AddCell(BuildLabelCell(item, normalizedTemplate));
        }

        var remainder = items.Count % layout.Columns;
        if (remainder != 0)
        {
            for (var i = remainder; i < layout.Columns; i++)
            {
                table.AddCell(new PdfPCell { Border = Rectangle.NO_BORDER, FixedHeight = layout.CellHeight });
            }
        }

        document.Add(new Paragraph($"QR Label Export - {layout.DisplayName}",
            FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12))
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 6f
        });
        document.Add(table);
        document.Close();

        return File(memoryStream.ToArray(), "application/pdf", fileName);
    }

    private IActionResult GenerateImageResult(string value)
    {
        var pngBytes = QrCodeHelper.GeneratePng(value);
        return File(pngBytes, "image/png");
    }

    private PdfPCell BuildLabelCell(InventoryItem item, string template)
    {
        var pathValue = QrCodeHelper.BuildInventoryDetailsPath(Url, item.Id);
        var absolute = QrCodeHelper.ResolveToAbsolute(_config, Request, pathValue);
        var qrBytes = QrCodeHelper.GeneratePng(absolute, size: 220, margin: 1);
        var image = Image.GetInstance(qrBytes);
        image.ScaleToFit(120f, 120f);
        image.Alignment = Element.ALIGN_CENTER;

        var cell = new PdfPCell
        {
            Padding = 8f,
            BorderColor = new BaseColor(184, 198, 219),
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            FixedHeight = template == "template-compact" ? 190f : template == "template-detailed" ? 265f : 220f
        };

        cell.AddElement(image);
        cell.AddElement(new Paragraph(item.Name, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, template == "template-compact" ? 9f : 10f))
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingBefore = 4f,
            SpacingAfter = 2f
        });
        cell.AddElement(new Paragraph($"Item ID: {item.Id}   Qty: {item.Quantity}",
            FontFactory.GetFont(FontFactory.HELVETICA, template == "template-compact" ? 8f : 9f))
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 2f
        });
        cell.AddElement(new Paragraph(item.Category ?? "Uncategorized",
            FontFactory.GetFont(FontFactory.HELVETICA, 8f))
        {
            Alignment = Element.ALIGN_CENTER
        });

        if (template == "template-detailed")
        {
            var description = string.IsNullOrWhiteSpace(item.Description) ? "No description provided" : item.Description;
            var expiration = item.ExpirationDate?.ToString("yyyy-MM-dd") ?? "None";
            cell.AddElement(new Paragraph(description,
                FontFactory.GetFont(FontFactory.HELVETICA, 7.5f))
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingBefore = 4f,
                SpacingAfter = 2f
            });
            cell.AddElement(new Paragraph($"Expiration: {expiration}",
                FontFactory.GetFont(FontFactory.HELVETICA, 7.5f))
            {
                Alignment = Element.ALIGN_CENTER
            });
        }

        return cell;
    }

    private static string NormalizeTemplate(string? template)
    {
        return template switch
        {
            "template-compact" => "template-compact",
            "template-detailed" => "template-detailed",
            _ => "template-standard"
        };
    }

    private static LabelPresetLayout GetPresetLayout(string? preset)
    {
        return preset switch
        {
            "two-up-large" => new LabelPresetLayout("2-Up Large Labels", 2, 260f),
            "four-up-compact" => new LabelPresetLayout("4-Up Compact Labels", 4, 185f),
            _ => new LabelPresetLayout("Avery 5160 / 3-Up Address Labels", 3, 220f)
        };
    }

    private sealed record LabelPresetLayout(string DisplayName, int Columns, float CellHeight);
}
