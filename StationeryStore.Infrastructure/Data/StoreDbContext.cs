// StationeryStore.Infrastructure/Data/StoreDbContext.cs
using System;
using System.Collections.Generic;
using System.Linq;                 // added for LINQ
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DCommon = StationeryStore.Domain.Common;   // alias to avoid conflict
using DEnt = StationeryStore.Domain.Entities;    // alias for entities

namespace StationeryStore.Infrastructure.Data;

public class StoreDbContext : DbContext
{
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
    {
    }

    public DbSet<DEnt.Branch> Branches { get; set; }
    public DbSet<DEnt.Product> Products { get; set; }
    public DbSet<DEnt.Category> Categories { get; set; }
    public DbSet<DEnt.Unit> Units { get; set; }
    public DbSet<DEnt.VatRate> VatRates { get; set; }
    public DbSet<DEnt.User> Users { get; set; }
    public DbSet<DEnt.Role> Roles { get; set; }
    public DbSet<DEnt.AuditTrail> AuditTrails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply Egyptian-specific configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreDbContext).Assembly);

        // Set decimal precision for Egyptian currency (3 decimals for milliemes)
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                {
                    property.SetPrecision(18);
                    property.SetScale(3); // Egyptian piasters (قرش)
                }

                // Arabic collation for string columns
                if (property.ClrType == typeof(string) &&
                    (property.Name.Contains("Ar", StringComparison.OrdinalIgnoreCase) ||
                     property.Name.Contains("Name", StringComparison.OrdinalIgnoreCase)))
                {
                    property.SetCollation("arabic_ci");
                }
            }
        }

        // Query filters for soft delete
        modelBuilder.Entity<DEnt.Branch>().HasQueryFilter(b => !b.IsDeleted);
        modelBuilder.Entity<DEnt.Product>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<DEnt.User>().HasQueryFilter(u => !u.IsDeleted);

        // Seed Egyptian data
        SeedEgyptianData(modelBuilder);
    }

    private void SeedEgyptianData(ModelBuilder modelBuilder)
    {
        // Seed VAT rates for Egypt
        var standardVat = new DEnt.VatRate
        {
            Id = Guid.NewGuid(),
            NameAr = "ضريبة القيمة المضافة القياسية",
            NameEn = "Standard Value Added Tax",
            Rate = 14.00m,
            EtaTaxTypeCode = "T1",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "SYSTEM"
        };

        var exemptVat = new DEnt.VatRate
        {
            Id = Guid.NewGuid(),
            NameAr = "معفى",
            NameEn = "Exempt",
            Rate = 0.00m,
            EtaTaxTypeCode = "T2",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "SYSTEM"
        };

        modelBuilder.Entity<DEnt.VatRate>().HasData(standardVat, exemptVat);

        // Seed Egyptian units
        var units = new[]
        {
            new DEnt.Unit { Id = Guid.NewGuid(), Code = "PCS", NameAr = "قطعة", NameEn = "Piece",
                      ConversionFactor = 1.000000m, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" },
            new DEnt.Unit { Id = Guid.NewGuid(), Code = "BOX", NameAr = "علبة", NameEn = "Box",
                      ConversionFactor = 12.000000m, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" },
            new DEnt.Unit { Id = Guid.NewGuid(), Code = "REAM", NameAr = "رزمة", NameEn = "Ream",
                      ConversionFactor = 500.000000m, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" },
            new DEnt.Unit { Id = Guid.NewGuid(), Code = "PKT", NameAr = "حزمة", NameEn = "Packet",
                      ConversionFactor = 10.000000m, CreatedAt = DateTime.UtcNow, CreatedBy = "SYSTEM" }
        };

        modelBuilder.Entity<DEnt.Unit>().HasData(units);

        // Seed default Egyptian branch (Cairo)
        var cairoBranch = new DEnt.Branch
        {
            Id = Guid.NewGuid(),
            NameAr = "المقر الرئيسي - القاهرة",
            NameEn = "Headquarters - Cairo",
            AddressAr = "شارع التحرير، وسط البلد",
            AddressEn = "Tahrir Street, Downtown",
            TaxRegistrationNumber = "123-456-789",
            TaxActivityCode = "47891",
            GovernorateAr = "القاهرة",
            GovernorateEn = "Cairo",
            CityAr = "القاهرة",
            CityEn = "Cairo",
            Phone = "+201234567890",
            IsActive = true,
            IsHeadquarters = true,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "SYSTEM"
        };

        modelBuilder.Entity<DEnt.Branch>().HasData(cairoBranch);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Egyptian requirement: Audit all changes
        UpdateAuditableEntities();
        var result = await base.SaveChangesAsync(cancellationToken);
        await DispatchDomainEvents();
        return result;
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries<DCommon.BaseEntity>();
        var now = DateTime.UtcNow;
        var user = "SYSTEM"; // TODO: Get from current user

        foreach (var entry in entries)
        {
            var entity = entry.Entity;

            switch (entry.State)
            {
                case EntityState.Added:
                    entity.CreatedAt = now;
                    entity.CreatedBy = user;
                    entity.UpdatedAt = now;
                    entity.UpdatedBy = user;
                    break;

                case EntityState.Modified:
                    entity.UpdatedAt = now;
                    entity.UpdatedBy = user;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entity.IsDeleted = true;
                    entity.DeletedAt = now;
                    entity.DeletedBy = user;
                    break;
            }
        }
    }

    private Task DispatchDomainEvents()
    {
        var domainEntities = ChangeTracker.Entries<DCommon.BaseEntity>()
            .Select(x => x.Entity)
            .Where(x => x.DomainEvents.Any())
            .ToList();

        foreach (var entity in domainEntities)
        {
            var events = entity.DomainEvents.ToList();
            // Use BaseEntity helper to clear domain events (IReadOnlyCollection has no Clear())
            entity.ClearDomainEvents();

            foreach (var domainEvent in events)
            {
                // TODO: Implement domain event handling
                // await _domainEventService.Publish(domainEvent);
            }
        }

        // No async work yet — return completed task to keep the api async-friendly
        return Task.CompletedTask;
    }
}