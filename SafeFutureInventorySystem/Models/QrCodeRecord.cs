using System.ComponentModel.DataAnnotations.Schema;

namespace SafeFutureInventorySystem.Models;

public class QrCode
{
    public int Id { get; set; }
    public int InventoryItemId { get; set; }

    [ForeignKey("InventoryItemId")]
    public virtual InventoryItem InventoryItem { get; set; }
    public string Value { get; set; }
    public string Format { get; set; }
    public DateTime GeneratedAt { get; set; }
}
