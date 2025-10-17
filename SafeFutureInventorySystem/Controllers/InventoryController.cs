using Microsoft.AspNetCore.Mvc;
using SafeFutureInventorySystem.Models;

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

    // Show a page with the barcode for the given item id
    public IActionResult Barcode(int id)
    {
        var items = LoadItems();
        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return NotFound();
        }
        return View(item);
    }

    // Return the barcode image (PNG) for the given item id
    public IActionResult BarcodeImage(int id)
    {
        var items = LoadItems();
        var item = items.FirstOrDefault(i => i.Id == id);
        if (item == null)
        {
            return NotFound();
        }

        // Create a barcode (Code 128) using ZXing
        var barcodeValue = $"INV-{item.Id:00000}";

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

        var pixelData = writer.Write(barcodeValue);

        // Convert pixel data to PNG
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
                return File(ms.ToArray(), "image/png", $"barcode-{item.Id}.png");
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
}