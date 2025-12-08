namespace SafeFutureInventorySystem.Models;

public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

    public string? UserMessage { get; set; }

    public string? Detail { get; set; }

    public string? Path { get; set; }

    public int? StatusCode { get; set; }
}
