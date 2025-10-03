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
}