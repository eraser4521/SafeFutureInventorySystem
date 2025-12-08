namespace SafeFutureInventorySystem.Models;

public class BarcodeRecord
{
    public int Id { get; set; }
    public int InventoryItemId { get; set; }
    public string Value { get; set; }
    public string Format { get; set; }
    public DateTime GeneratedAt { get; set; }
}
