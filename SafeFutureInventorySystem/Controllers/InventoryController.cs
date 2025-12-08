using Microsoft.AspNetCore.Mvc;
using SafeFutureInventorySystem.Models;
using SkiaSharp;
using System.Runtime.InteropServices;

namespace SafeFutureInventorySystem.Controllers;

public class InventoryController : Controller
{
    private readonly ILogger<InventoryController> _logger;
    private readonly string _inventoryFilePath;

    public InventoryController(ILogger<InventoryController> logger)
    {
        _logger = logger;
        _inventoryFilePath = Path.Combine(AppContext.BaseDirectory, "inventory.txt");
    }

    public IActionResult Index()
    {
        if (!TryLoadItems(out var items, out var errorMessage, out var technicalDetail))
        {
            ViewBag.Error = errorMessage ?? "We couldn't read the inventory file.";
            ViewBag.ErrorDetail = technicalDetail;
            return View(new List<InventoryItem>());
        }

        var sortedItems = items.OrderBy(item => item.DateAdded).ToList();

        return View(sortedItems);
    }

    // Show a page with the QR code for the given item id
    public IActionResult QrCode(int id)
    {
        if (!TryLoadItems(out var items, out var errorMessage, out var technicalDetail))
        {
            return InventoryLoadFailed(errorMessage, technicalDetail);
        }

        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null) return ItemNotFoundResult(id);
        // Render the QR code view
        return View("QrCode", item);
    }

    // Return the QR code image (PNG) for the given item id
    public IActionResult QrCodeImage(int id)
    {
        if (!TryLoadItems(out var items, out var errorMessage, out var technicalDetail))
        {
            return InventoryLoadFailed(errorMessage, technicalDetail);
        }

        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null) return ItemNotFoundResult(id);

        // Encode a URL that links to the Edit page for this item so scanning the QR opens the edit UI
        var qrValue = Url.Action("Edit", "Inventory", new { id = item.Id }, Request.Scheme) ?? $"INV-{item.Id:00000}";

        var writer = new ZXing.BarcodeWriterPixelData
        {
            Format = ZXing.BarcodeFormat.QR_CODE,
            Options = new ZXing.Common.EncodingOptions
            {
                Height = 250,
                Width = 250,
                Margin = 1
            }
        };

        var pixelData = writer.Write(qrValue);

        // Create a new SkiaSharp bitmap and draw the barcode
        using (var surface = SKSurface.Create(new SKImageInfo(pixelData.Width, pixelData.Height)))
        {
            using (var canvas = surface.Canvas)
            {
                // Create bitmap and copy pixel data
                using (var bitmap = new SKBitmap(new SKImageInfo(pixelData.Width, pixelData.Height)))
                {
                    var ptr = bitmap.GetPixels();
                    Marshal.Copy(pixelData.Pixels, 0, ptr, pixelData.Pixels.Length);
                    
                    // Draw bitmap to canvas (white background, black barcode)
                    canvas.Clear(SKColors.White);
                    canvas.DrawBitmap(bitmap, 0, 0);
                }
            }

            // Encode as PNG and return
            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var ms = new MemoryStream())
            {
                data.SaveTo(ms);
                return File(ms.ToArray(), "image/png", $"qrcode-{item.Id}.png");
            }
        }
    }

    private bool TryLoadItems(out List<InventoryItem> items, out string? userMessage, out string? technicalDetail)
    {
        items = new List<InventoryItem>();
        userMessage = null;
        technicalDetail = null;

        try
        {
            if (!System.IO.File.Exists(_inventoryFilePath))
            {
                userMessage = "Inventory data could not be found.";
                technicalDetail = $"Missing file: {_inventoryFilePath}";
                _logger.LogWarning("Inventory file not found at {Path}", _inventoryFilePath);
                return false;
            }

            var lines = System.IO.File.ReadAllLines(_inventoryFilePath).Skip(1);

            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length == 4)
                {
                    var item = new InventoryItem
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Quantity = int.Parse(parts[2]),
                        DateAdded = DateTime.Parse(parts[3])
                    };
                    items.Add(item);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            userMessage = "We couldn't read the inventory list. Please try again or contact support.";
            technicalDetail = ex.ToString();
            _logger.LogError(ex, "Error reading inventory file at {Path}", _inventoryFilePath);
            return false;
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

    private IActionResult InventoryLoadFailed(string? userMessage, string? detail)
    {
        return BuildErrorResult(userMessage ?? "We couldn't load the inventory data.", detail, 500);
    }

    private IActionResult ItemNotFoundResult(int id)
    {
        _logger.LogWarning("Inventory item {Id} not found for request {Path}", id, HttpContext.Request?.Path.Value);
        return BuildErrorResult("We couldn't find that inventory item. If you scanned a QR code, it may be out of date.", $"Inventory item {id} was not found.", 404);
    }

    // Show details page for an item
    public IActionResult Details(int id)
    {
        if (!TryLoadItems(out var items, out var errorMessage, out var technicalDetail))
        {
            return InventoryLoadFailed(errorMessage, technicalDetail);
        }

        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null) return ItemNotFoundResult(id);
        return View(item);
    }

    // Edit page (GET)
    public IActionResult Edit(int id)
    {
        if (!TryLoadItems(out var items, out var errorMessage, out var technicalDetail))
        {
            return InventoryLoadFailed(errorMessage, technicalDetail);
        }

        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null) return ItemNotFoundResult(id);
        return View(item);
    }

    // Edit handler (POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(InventoryItem model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (!TryLoadItems(out var items, out var loadError, out var loadDetail))
        {
            ViewBag.Error = loadError ?? "We couldn't load the inventory data.";
            ViewBag.ErrorDetail = loadDetail;
            return View(model);
        }

        var existing = items.FirstOrDefault(i => i.Id == model.Id);
        if (existing == null) return ItemNotFoundResult(model.Id);

        // Update editable fields (allow name and quantity edits)
        existing.Name = model.Name;
        existing.Quantity = model.Quantity;

        if (!TrySaveItems(items, out var saveDetail))
        {
            ViewBag.Error = "We couldn't save your changes. Please try again.";
            ViewBag.ErrorDetail = saveDetail;
            return View(model);
        }

        return RedirectToAction("Details", new { id = model.Id });
    }

    private bool TrySaveItems(List<InventoryItem> items, out string? technicalDetail)
    {
        technicalDetail = null;

        try
        {
            var lines = new List<string> { "Id,Name,Quantity,DateAdded" };
            lines.AddRange(items.Select(i => $"{i.Id},{EscapeCsv(i.Name)},{i.Quantity},{i.DateAdded:yyyy-MM-dd}"));
            System.IO.File.WriteAllLines(_inventoryFilePath, lines);
            return true;
        }
        catch (Exception ex)
        {
            technicalDetail = ex.ToString();
            _logger.LogError(ex, "Error saving inventory file at {Path}", _inventoryFilePath);
            return false;
        }
    }

    // Simple CSV escape for commas/newlines in names
    private string EscapeCsv(string input)
    {
        if (input == null) return string.Empty;
        if (input.Contains(',') || input.Contains('"') || input.Contains('\n'))
        {
            return '"' + input.Replace("\"", "\"\"") + '"';
        }
        return input;
    }
}
