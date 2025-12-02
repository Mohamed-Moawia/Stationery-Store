// StationeryStore.Domain/Entities/Product.cs
using System;
using System.Collections.Generic;
using StationeryStore.Domain.Common;
using StationeryStore.Domain.Enums;

namespace StationeryStore.Domain.Entities;

public class Product : BaseEntity
{
    public Guid BranchId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UnitId { get; set; }
    public Guid VatRateId { get; set; }

    // Egyptian SKU/Barcode standards
    public string SKU { get; set; } = string.Empty;
    public string? EgyptianBarcode { get; set; } // GS1 Egypt format

    // Bilingual names (required by Egyptian law for receipts)
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }

    // Pricing (Egyptian VAT considerations)
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? WholesalePrice { get; set; }

    // Egyptian VAT (14% standard)
    public virtual VatRate VatRate { get; set; } = null!;

    // ETA Tax Mapping (Egyptian specific)
    public string? EtaItemCode { get; set; } // ETA product classification
    public string? EtaUnitCode { get; set; } // ETA unit code

    // Inventory
    public decimal MinimumStockLevel { get; set; }
    public decimal MaximumStockLevel { get; set; }
    public decimal ReorderPoint { get; set; }

    // Egyptian categorization
    public virtual Category Category { get; set; } = null!;

    public virtual Unit Unit { get; set; } = null!;

    // Branch-specific pricing (Egypt has regional price variations)
    public virtual Branch Branch { get; set; } = null!;

    public bool IsActive { get; set; } = true;
    public DateTime? LastRestocked { get; set; }

    // Egyptian compliance flags
    public bool RequiresTaxInvoice { get; set; } = true;
    public bool IsExemptFromVat { get; set; } = false;

    // Image storage (common in Egyptian stores)
    public string? ImageUrl { get; set; }
    public string? ThumbnailUrl { get; set; }

    // Navigation
    public virtual ICollection<InventoryMovement> Movements { get; set; } = new List<InventoryMovement>();
}