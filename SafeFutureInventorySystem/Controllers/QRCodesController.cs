using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SafeFutureInventorySystem.Models;
using SkiaSharp;
using System.Runtime.InteropServices;

namespace SafeFutureInventorySystem.Controllers;

public class QRCodesController : Controller
{
    private string InventoryFile => Path.Combine(AppContext.BaseDirectory, "inventory.txt");
    private string QRCodesFile => Path.Combine(AppContext.BaseDirectory, "qrcodes.txt");
    private readonly IConfiguration _config;
    private readonly ILogger<QRCodesController> _logger;

    public QRCodesController(IConfiguration config, ILogger<QRCodesController> logger)
    {
        _config = config;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var records = LoadBarcodeRecords();
        return View(records);
    }

    // Show a page to generate/save QR code for an inventory item
    public IActionResult Generate(int id)
    {
        var item = LoadInventory().FirstOrDefault(i => i.Id == id);
        if (item == null) return NotFound();
        return View(item);
    }

    [HttpPost]
    public IActionResult Create(int inventoryItemId)
    {
        var item = LoadInventory().FirstOrDefault(i => i.Id == inventoryItemId);
        if (item == null) return NotFound();
        // Store a path-only value (e.g. "/Inventory/Edit/12"). When scanned, the app can combine this with a base URL.
        var pathValue = Url.Action("Edit", "Inventory", new { id = item.Id }) ?? $"/Inventory/Edit/{item.Id}";
        var format = "QR_CODE";
        var records = LoadBarcodeRecords();
        var nextId = (records.Any() ? records.Max(r => r.Id) : 0) + 1;

        var record = new BarcodeRecord
        {
            Id = nextId,
            InventoryItemId = item.Id,
            Value = pathValue,
            Format = format,
            GeneratedAt = DateTime.UtcNow
        };

        records.Add(record);
        SaveBarcodeRecords(records);

        // After creating a QR record, return the user to the item's edit page for a smoother workflow
        return RedirectToAction("Edit", "Inventory", new { id = inventoryItemId });
    }

    // Return the PNG for a saved record
    public IActionResult ImageRecord(int id)
    {
        var records = LoadBarcodeRecords();
        var rec = records.FirstOrDefault(r => r.Id == id);
        if (rec == null) return NotFound();
        var absolute = ResolveToAbsolute(rec.Value);
        return GenerateImageResult(absolute);
    }

    // Return the QR code image (PNG) for a given inventory item value (preview)
    public IActionResult ImageForItem(int inventoryItemId)
    {
        var item = LoadInventory().FirstOrDefault(i => i.Id == inventoryItemId);
        if (item == null) return NotFound();
        // Preview: generate the same path-only value and resolve to absolute when rendering the QR
        var pathValue = Url.Action("Edit", "Inventory", new { id = item.Id }) ?? $"/Inventory/Edit/{item.Id}";
        var absolute = ResolveToAbsolute(pathValue);
        return GenerateImageResult(absolute);
    }

    private IActionResult GenerateImageResult(string value)
    {
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

        var pixelData = writer.Write(value);

        // Create a new SkiaSharp bitmap and draw the QR code
        using (var surface = SKSurface.Create(new SKImageInfo(pixelData.Width, pixelData.Height)))
        {
            using (var canvas = surface.Canvas)
            {
                // Create bitmap and copy pixel data
                using (var bitmap = new SKBitmap(new SKImageInfo(pixelData.Width, pixelData.Height)))
                {
                    var ptr = bitmap.GetPixels();
                    Marshal.Copy(pixelData.Pixels, 0, ptr, pixelData.Pixels.Length);
                    
                    // Draw bitmap to canvas (white background, black QR code)
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
                return File(ms.ToArray(), "image/png");
            }
        }
    }

    private List<InventoryItem> LoadInventory()
    {
        var items = new List<InventoryItem>();
        try
        {
            var lines = System.IO.File.ReadAllLines(InventoryFile).Skip(1);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length == 4)
                {
                    items.Add(new InventoryItem
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Quantity = int.Parse(parts[2]),
                        DateAdded = DateTime.Parse(parts[3])
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading inventory file at {InventoryFile}", InventoryFile);
        }
        return items;
    }

    private List<BarcodeRecord> LoadBarcodeRecords()
    {
        var records = new List<BarcodeRecord>();
        try
        {
            if (!System.IO.File.Exists(QRCodesFile))
            {
                // create header
                System.IO.File.WriteAllText(QRCodesFile, "Id,InventoryItemId,Value,Format,GeneratedAt\n");
            }

            var lines = System.IO.File.ReadAllLines(QRCodesFile).Skip(1);
            foreach (var line in lines)
            {
                var parts = line.Split(',');
                if (parts.Length == 5)
                {
                    records.Add(new BarcodeRecord
                    {
                        Id = int.Parse(parts[0]),
                        InventoryItemId = int.Parse(parts[1]),
                        Value = parts[2],
                        Format = parts[3],
                        GeneratedAt = DateTime.Parse(parts[4])
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reading QR codes file at {QRCodesFile}", QRCodesFile);
        }
        return records;
    }

    // Resolve a stored value (path or absolute) into an absolute URL for QR rendering.
    private string ResolveToAbsolute(string storedValue)
    {
        if (string.IsNullOrEmpty(storedValue)) return storedValue ?? string.Empty;

        // If it's already an absolute URL, return as-is
        if (storedValue.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || storedValue.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return storedValue;
        }

        // storedValue is a path (e.g. /Inventory/Edit/12)
        var configuredBase = _config?[
            "QrCodeBaseUrl"];
        if (!string.IsNullOrEmpty(configuredBase))
        {
            // ensure no double slashes
            return configuredBase.TrimEnd('/') + (storedValue.StartsWith("/") ? storedValue : "/" + storedValue);
        }

        // Fallback: build from current request
        var scheme = Request?.Scheme ?? "http";
        var host = Request?.Host.Value ?? "localhost";
        return scheme + "://" + host + (storedValue.StartsWith("/") ? storedValue : "/" + storedValue);
    }

    private void SaveBarcodeRecords(List<BarcodeRecord> records)
    {
        var lines = new List<string> { "Id,InventoryItemId,Value,Format,GeneratedAt" };
        lines.AddRange(records.Select(r => $"{r.Id},{r.InventoryItemId},{r.Value},{r.Format},{r.GeneratedAt:o}"));
        System.IO.File.WriteAllLines(QRCodesFile, lines);
    }
}
