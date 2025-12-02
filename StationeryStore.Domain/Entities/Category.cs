using System.Collections.Generic;
using StationeryStore.Domain.Common;

namespace StationeryStore.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}