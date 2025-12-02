using StationeryStore.Domain.Common;

namespace StationeryStore.Domain.Entities;

public class Till : BaseEntity
{
    public string Number { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
}