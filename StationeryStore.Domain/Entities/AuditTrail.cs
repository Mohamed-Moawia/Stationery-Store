using System;
using StationeryStore.Domain.Common;

namespace StationeryStore.Domain.Entities;

public class AuditTrail : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}