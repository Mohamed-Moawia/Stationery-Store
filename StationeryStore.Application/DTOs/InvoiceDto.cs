namespace StationeryStore.Application.DTOs;

public class InvoiceDto
{
    public string Id { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public string Status { get; set; } = "Pending";
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}