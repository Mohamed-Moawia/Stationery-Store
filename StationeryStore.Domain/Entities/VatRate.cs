using StationeryStore.Domain.Common;

namespace StationeryStore.Domain.Entities;

public class VatRate : BaseEntity
{
    public decimal Rate { get; set; }
    public string? Description { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? EtaTaxTypeCode { get; set; }
}