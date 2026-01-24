using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SafeFutureInventorySystem.Data;
using SafeFutureInventorySystem.Models;
using SkiaSharp;
using System.Runtime.InteropServices;

namespace SafeFutureInventorySystem.Controllers;

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
        var pathValue = Url.Action("Details", "Inventory", new { id = item.Id }) ?? $"/Inventory/Details/{item.Id}";
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
        return _context.InventoryItems.ToList();
    }

    // Resolve a path into an absolute URL for QR rendering
    private string ResolveToAbsolute(string path)
    {
        if (string.IsNullOrEmpty(path)) return path ?? string.Empty;

        // If it's already an absolute URL, return as-is
        if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return path;
        }

        // path is a relative path (e.g. /Inventory/Details/12)
        var configuredBase = _config?["QrCodeBaseUrl"];
        if (!string.IsNullOrEmpty(configuredBase))
        {
            // ensure no double slashes
            return configuredBase.TrimEnd('/') + (path.StartsWith("/") ? path : "/" + path);
        }

        // Fallback: build from current request
        var scheme = Request?.Scheme ?? "http";
        var host = Request?.Host.Value ?? "localhost";
        return scheme + "://" + host + (path.StartsWith("/") ? path : "/" + path);
    }
}
