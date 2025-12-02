using StationeryStore.Domain.Common;

namespace StationeryStore.Domain.Entities;

public class Unit : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public decimal ConversionFactor { get; set; } = 1m;
    public string DisplayName { get; set; } = string.Empty;
}