using Microsoft.AspNetCore.Mvc;
using SafeFutureInventorySystem.Models;

namespace SafeFutureInventorySystem.Controllers;

public class BarcodesController : Controller
{
    private string InventoryFile => Path.Combine(AppContext.BaseDirectory, "inventory.txt");
    private string BarcodesFile => Path.Combine(AppContext.BaseDirectory, "barcodes.txt");

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
        var format = "CODE_128";
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
            Format = ZXing.BarcodeFormat.CODE_128,
            Options = new ZXing.Common.EncodingOptions
            {
                Height = 80,
                Width = 300,
                Margin = 10
            }
        };

        var pixelData = writer.Write(value);

        using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
        {
            var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
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
        catch
        {
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
        catch
        {
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
