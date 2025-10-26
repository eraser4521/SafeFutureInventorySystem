using Microsoft.AspNetCore.Mvc;
using SafeFutureInventorySystem.Models;
using SkiaSharp;
using System.Runtime.InteropServices;

namespace SafeFutureInventorySystem.Controllers;

public class InventoryController : Controller
{
    public IActionResult Index()
    {
        var items = new List<InventoryItem>();
        var filePath = Path.Combine(AppContext.BaseDirectory, "inventory.txt");

        try
        {
            
            var lines = System.IO.File.ReadAllLines(filePath).Skip(1);

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
        }
        catch (Exception ex)
        {
            
            ViewBag.Error = $"Error reading inventory file: {ex.Message}";
            return View(items);
        }

        
        var sortedItems = items.OrderBy(item => item.DateAdded).ToList();

        return View(sortedItems);
    }

    // Show a page with the QR code for the given item id
    public IActionResult QrCode(int id)
    {
        var items = LoadItems();
        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return NotFound();
        }
        // Render the QR code view
        return View("QrCode", item);
    }

    // Return the QR code image (PNG) for the given item id
    public IActionResult QrCodeImage(int id)
    {
        var items = LoadItems();
        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return NotFound();
        }

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

    private List<InventoryItem> LoadItems()
    {
        var items = new List<InventoryItem>();
        var filePath = Path.Combine(AppContext.BaseDirectory, "inventory.txt");

        try
        {
            var lines = System.IO.File.ReadAllLines(filePath).Skip(1);

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
        }
        catch
        {
            // ignore here; callers handle missing/empty lists
        }

        return items;
    }

    // Show details page for an item
    public IActionResult Details(int id)
    {
        var items = LoadItems();
        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null) return NotFound();
        return View(item);
    }

    // Edit page (GET)
    public IActionResult Edit(int id)
    {
        var items = LoadItems();
        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null) return NotFound();
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

        var items = LoadItems();
        var existing = items.FirstOrDefault(i => i.Id == model.Id);
        if (existing == null) return NotFound();

        // Update editable fields (allow name and quantity edits)
        existing.Name = model.Name;
        existing.Quantity = model.Quantity;

        SaveItems(items);

        return RedirectToAction("Details", new { id = model.Id });
    }

    private void SaveItems(List<InventoryItem> items)
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "inventory.txt");
        var lines = new List<string> { "Id,Name,Quantity,DateAdded" };
        lines.AddRange(items.Select(i => $"{i.Id},{EscapeCsv(i.Name)},{i.Quantity},{i.DateAdded:yyyy-MM-dd}"));
        System.IO.File.WriteAllLines(filePath, lines);
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