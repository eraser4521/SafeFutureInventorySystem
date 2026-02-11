using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SafeFutureInventorySystem.Data;
using SafeFutureInventorySystem.Helpers;
using SafeFutureInventorySystem.Models;

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
        var pathValue = QrCodeHelper.BuildInventoryDetailsPath(Url, item.Id);
        var absolute = QrCodeHelper.ResolveToAbsolute(_config, Request, pathValue);
        return GenerateImageResult(absolute);
    }

    private IActionResult GenerateImageResult(string value)
    {
        var pngBytes = QrCodeHelper.GeneratePng(value);
        return File(pngBytes, "image/png");
    }

    private List<InventoryItem> LoadInventory()
    {
        return _context.InventoryItems.ToList();
    }

}
