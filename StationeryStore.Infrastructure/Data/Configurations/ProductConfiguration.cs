// StationeryStore.Infrastructure/Data/Configurations/ProductConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StationeryStore.Domain.Entities;

namespace StationeryStore.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products", "catalog");

        builder.HasKey(p => p.Id);

        // Egyptian SKU must be unique per branch
        builder.HasIndex(p => new { p.BranchId, p.SKU })
               .IsUnique()
               .HasDatabaseName("IX_Products_BranchId_SKU");

        // Egyptian barcode must be unique globally
        // Use provider-neutral SQL fragment for partial index
        builder.HasIndex(p => p.EgyptianBarcode)
               .IsUnique()
               .HasFilter("EgyptianBarcode IS NOT NULL");

        // Arabic/English name constraints
        builder.Property(p => p.NameAr)
               .IsRequired()
               .HasMaxLength(200);
        // .UseCollation("ar_EG.utf8"); // use provider-specific collation in DbContext if necessary

        builder.Property(p => p.NameEn)
               .IsRequired()
               .HasMaxLength(200);

        // Pricing with Egyptian precision
        builder.Property(p => p.CostPrice)
               .HasPrecision(18, 3);

        builder.Property(p => p.SalePrice)
               .HasPrecision(18, 3);

        // ETA tax mapping fields
        builder.Property(p => p.EtaItemCode)
               .HasMaxLength(50);

        builder.Property(p => p.EtaUnitCode)
               .HasMaxLength(10);

        // Relationships
        builder.HasOne(p => p.Branch)
               .WithMany(b => b.Products)
               .HasForeignKey(p => p.BranchId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Unit)
               .WithMany()
               .HasForeignKey(p => p.UnitId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.VatRate)
               .WithMany()
               .HasForeignKey(p => p.VatRateId)
               .OnDelete(DeleteBehavior.Restrict);

        // Soft delete query filter
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}