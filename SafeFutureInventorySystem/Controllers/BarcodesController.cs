using Microsoft.AspNetCore.Mvc;
using SafeFutureInventorySystem.Models;
using SkiaSharp;
using System.Runtime.InteropServices;

namespace SafeFutureInventorySystem.Controllers;

public class BarcodesController : Controller
{
    private string InventoryFile => Path.Combine(AppContext.BaseDirectory, "inventory.txt");
    private string BarcodesFile => Path.Combine(AppContext.BaseDirectory, "barcodes.txt");
    private readonly ILogger<BarcodesController> _logger;

    public BarcodesController(ILogger<BarcodesController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var records = LoadBarcodeRecords();
        return View(records);
    }

    // Show a page to generate/save barcode for an inventory item
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

        var value = $"INV-{item.Id:00000}";
        var format = "QR_CODE";
        var records = LoadBarcodeRecords();
        var nextId = (records.Any() ? records.Max(r => r.Id) : 0) + 1;

        var record = new BarcodeRecord
        {
            Id = nextId,
            InventoryItemId = item.Id,
            Value = value,
            Format = format,
            GeneratedAt = DateTime.UtcNow
        };

        records.Add(record);
        SaveBarcodeRecords(records);

        return RedirectToAction("Index");
    }

    // Return the PNG for a saved record
    public IActionResult ImageRecord(int id)
    {
        var records = LoadBarcodeRecords();
        var rec = records.FirstOrDefault(r => r.Id == id);
        if (rec == null) return NotFound();
        return GenerateImageResult(rec.Value);
    }

    // Return the PNG for a given inventory item value (preview)
    public IActionResult ImageForItem(int inventoryItemId)
    {
        var item = LoadInventory().FirstOrDefault(i => i.Id == inventoryItemId);
        if (item == null) return NotFound();
        var value = $"INV-{item.Id:00000}";
        return GenerateImageResult(value);
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
            if (!System.IO.File.Exists(BarcodesFile))
            {
                // create header
                System.IO.File.WriteAllText(BarcodesFile, "Id,InventoryItemId,Value,Format,GeneratedAt\n");
            }

            var lines = System.IO.File.ReadAllLines(BarcodesFile).Skip(1);
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
            _logger.LogError(ex, "Error reading barcodes file at {BarcodesFile}", BarcodesFile);
        }
        return records;
    }

    private void SaveBarcodeRecords(List<BarcodeRecord> records)
    {
        var lines = new List<string> { "Id,InventoryItemId,Value,Format,GeneratedAt" };
        lines.AddRange(records.Select(r => $"{r.Id},{r.InventoryItemId},{r.Value},{r.Format},{r.GeneratedAt:o}"));
        System.IO.File.WriteAllLines(BarcodesFile, lines);
    }
}
