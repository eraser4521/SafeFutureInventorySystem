namespace SafeFutureInventorySystem.Models;

public class InventoryItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public DateTime DateAdded { get; set; }
}