using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SafeFutureInventorySystem.Models;

namespace SafeFutureInventorySystem.Controllers
{
    [Authorize]  
    public class HomeController : Controller
    {

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
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
