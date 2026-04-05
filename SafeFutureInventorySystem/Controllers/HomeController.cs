using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SafeFutureInventorySystem.Data;
using SafeFutureInventorySystem.Models;
namespace SafeFutureInventorySystem.Controllers
{
    [Authorize]  
    public class HomeController : Controller
    {
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }


        public IActionResult Index()
        {
            var today = DateTime.Now.Date;
            var weekFromNow = today.AddDays(7);

            var items = _context.InventoryItems
                .Include(i => i.DonationLogs)
                .Include(i => i.AdjustmentLogs)
                .AsSplitQuery()
                .ToList();

            var recentActivity = items
                .SelectMany(item =>
                {
                    var adjustmentEvents = item.AdjustmentLogs.Select(log => new HomeDashboardActivityItem
                    {
                        InventoryItemId = item.Id,
                        ItemName = item.Name,
                        ActivityDate = log.AdjustmentDate,
                        ActivityType = log.OldQuantity == log.NewQuantity ? "Metadata Update" : "Quantity Update",
                        Summary = BuildAdjustmentSummary(log),
                        PerformedBy = string.IsNullOrWhiteSpace(log.AdjustedBy) ? "System" : log.AdjustedBy,
                        ActivityIcon = log.OldQuantity == log.NewQuantity ? "fa-pen" : "fa-exchange-alt",
                        ActivityBadgeClass = log.OldQuantity == log.NewQuantity ? "badge-info text-white" : "badge-primary",
                        ActivityMeta = BuildAdjustmentMeta(log)
                    });

                    var donationEvents = item.DonationLogs.Select(log => new HomeDashboardActivityItem
                    {
                        InventoryItemId = item.Id,
                        ItemName = item.Name,
                        ActivityDate = log.DonationDate,
                        ActivityType = "Donation",
                        Summary = BuildDonationSummary(log),
                        PerformedBy = string.IsNullOrWhiteSpace(log.DonorName) ? "Unknown donor" : log.DonorName,
                        ActivityIcon = "fa-hand-holding-heart",
                        ActivityBadgeClass = "badge-success",
                        ActivityMeta = $"+{log.QuantityDonated} unit{(log.QuantityDonated == 1 ? "" : "s")}"
                    });

                    return adjustmentEvents.Concat(donationEvents);
                })
                .OrderByDescending(activity => activity.ActivityDate)
                .Take(6)
                .ToList();

            var viewModel = new HomeDashboardViewModel
            {
                TotalItems = items.Count,
                LowStockCount = items.Count(i => i.Quantity > 0 && i.IsLowStock),
                NoStockCount = items.Count(i => i.Quantity <= 0),
                ExpiredCount = items.Count(i => i.ExpirationDate.HasValue &&
                    i.ExpirationDate.Value.Date < today),
                ExpiringSoonCount = items.Count(i => i.ExpirationDate.HasValue &&
                    i.ExpirationDate.Value.Date >= today &&
                    i.ExpirationDate.Value.Date <= weekFromNow),
                AttentionItems = items
                    .Where(i => i.Quantity <= 0 || i.IsLowStock || (i.ExpirationDate.HasValue &&
                        i.ExpirationDate.Value.Date >= today &&
                        i.ExpirationDate.Value.Date <= weekFromNow))
                    .OrderBy(i => i.Quantity <= 0 ? 0 : i.IsLowStock ? 1 : 2)
                    .ThenBy(i => i.ExpirationDate ?? DateTime.MaxValue)
                    .ThenBy(i => i.Name)
                    .Take(6)
                    .ToList(),
                RecentActivity = recentActivity
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        private static string BuildDonationSummary(DonationLog log)
        {
            var summary = $"Donation received: {log.QuantityDonated} unit{(log.QuantityDonated == 1 ? "" : "s")}";
            if (!string.IsNullOrWhiteSpace(log.Notes))
            {
                summary += $": {log.Notes}";
            }

            return summary;
        }

        private static string BuildAdjustmentSummary(InventoryAdjustmentLog log)
        {
            if (log.OldQuantity != log.NewQuantity)
            {
                return $"Quantity adjusted from {log.OldQuantity} to {log.NewQuantity}";
            }

            if (string.IsNullOrWhiteSpace(log.Reason))
            {
                return "Item details updated";
            }

            var reason = log.Reason.Replace("Item metadata updated:", "").Trim();
            return string.IsNullOrWhiteSpace(reason) ? "Item details updated" : reason;
        }

        private static string BuildAdjustmentMeta(InventoryAdjustmentLog log)
        {
            return log.OldQuantity != log.NewQuantity
                ? $"{log.OldQuantity} -> {log.NewQuantity}"
                : "Fields updated";
        }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? code = null)
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        var statusFeature = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

        var path = exceptionFeature?.Path ?? statusFeature?.OriginalPath ?? HttpContext.Request?.Path.Value;
        var statusCode = code ?? HttpContext.Response?.StatusCode;

        if (exceptionFeature?.Error != null)
        {
            _logger.LogError(exceptionFeature.Error, "Unhandled exception at {Path}", path);
        }
        else if (statusCode.HasValue && statusCode.Value >= 400)
        {
            _logger.LogWarning("Request to {Path} returned status code {StatusCode}", path, statusCode);
        }

        var model = new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            UserMessage = statusCode == 404
                ? "We couldn't find what you were looking for."
                : "We couldn't process your request right now.",
            Detail = exceptionFeature?.Error?.ToString(),
            Path = path,
            StatusCode = statusCode
        };

        return View(model);
    }
    }
}
