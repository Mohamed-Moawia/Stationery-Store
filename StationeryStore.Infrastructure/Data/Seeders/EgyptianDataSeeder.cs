// StationeryStore.Infrastructure/Data/Seeders/EgyptianDataSeeder.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;    // added for AnyAsync/FirstAsync/etc.
using StationeryStore.Domain.Entities;
using StationeryStore.Application.Models;

namespace StationeryStore.Infrastructure.Data.Seeders;

public class EgyptianDataSeeder
{
    public static async Task SeedAsync(StoreDbContext context)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // Seed only if no data exists
            if (!await context.Branches.AnyAsync())
            {
                await SeedBranches(context);
            }

            if (!await context.Categories.AnyAsync())
            {
                await SeedCategories(context);
            }

            if (!await context.Products.AnyAsync())
            {
                await SeedProducts(context);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static async Task SeedBranches(StoreDbContext context)
    {
        var branches = new[]
        {
            new Branch
            {
                Id = Guid.NewGuid(),
                NameAr = "فرع المعادي",
                NameEn = "Maadi Branch",
                AddressAr = "شارع 9، المعادي، القاهرة",
                AddressEn = "Street 9, Maadi, Cairo",
                TaxRegistrationNumber = "310-123-456",
                TaxActivityCode = "47891",
                GovernorateAr = "القاهرة",
                GovernorateEn = "Cairo",
                CityAr = "المعادي",
                CityEn = "Maadi",
                Phone = "+201001234567",
                IsActive = true,
                IsHeadquarters = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "SYSTEM"
            },
            new Branch
            {
                Id = Guid.NewGuid(),
                NameAr = "فرع مدينة نصر",
                NameEn = "Nasr City Branch",
                AddressAr = "ميدان هليوبوليس، مدينة نصر",
                AddressEn = "Heliopolis Square, Nasr City",
                TaxRegistrationNumber = "311-456-789",
                TaxActivityCode = "47891",
                GovernorateAr = "القاهرة",
                GovernorateEn = "Cairo",
                CityAr = "مدينة نصر",
                CityEn = "Nasr City",
                Phone = "+201002345678",
                IsActive = true,
                IsHeadquarters = false,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "SYSTEM"
            }
        };

        await context.Branches.AddRangeAsync(branches);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCategories(StoreDbContext context)
    {
        var categories = new[]
        {
            new Category
            {
                Id = Guid.NewGuid(),
                NameAr = "أقلام حبر",
                NameEn = "Pens",
                DescriptionAr = "أقلام حبر جاف وحبر سائل",
                DescriptionEn = "Ballpoint and ink pens",
                ParentCategoryId = null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "SYSTEM"
            },
            new Category
            {
                Id = Guid.NewGuid(),
                NameAr = "دفاتر",
                NameEn = "Notebooks",
                DescriptionAr = "دفاتر مدرسية ومذكرات",
                DescriptionEn = "School notebooks and diaries",
                ParentCategoryId = null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "SYSTEM"
            },
            new Category
            {
                Id = Guid.NewGuid(),
                NameAr = "أوراق طباعة",
                NameEn = "Printing Paper",
                DescriptionAr = "أوراق A4 وأوراق طباعة",
                DescriptionEn = "A4 and printing papers",
                ParentCategoryId = null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "SYSTEM"
            }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();
    }

    private static async Task SeedProducts(StoreDbContext context)
    {
        var branch = await context.Branches.FirstAsync();
        var category = await context.Categories.FirstAsync(c => c.NameEn == "Pens");
        var unit = await context.Units.FirstAsync(u => u.Code == "PCS");
        var vatRate = await context.VatRates.FirstAsync(v => v.Rate == 14.00m);

        var products = new[]
        {
            new Product
            {
                Id = Guid.NewGuid(),
                SKU = "PEN-BIC-BLUE",
                EgyptianBarcode = "6221234567890",
                NameAr = "قلم حبر بيك أزرق",
                NameEn = "BIC Ballpoint Pen Blue",
                DescriptionAr = "قلم حبر جاف من ماركة بيك، لون أزرق",
                DescriptionEn = "BIC ballpoint pen, blue color",
                CostPrice = 2.500m,
                SalePrice = 5.000m,
                VatRateId = vatRate.Id,
                EtaItemCode = "960810",
                EtaUnitCode = "PCE",
                MinimumStockLevel = 100,
                MaximumStockLevel = 1000,
                ReorderPoint = 200,
                CategoryId = category.Id,
                UnitId = unit.Id,
                BranchId = branch.Id,
                IsActive = true,
                RequiresTaxInvoice = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "SYSTEM"
            },
            new Product
            {
                Id = Guid.NewGuid(),
                SKU = "NOTE-A4-80G",
                EgyptianBarcode = "6229876543210",
                NameAr = "دفتر A4 80 جرام",
                NameEn = "A4 Notebook 80gsm",
                DescriptionAr = "دفتر A4 بجودة 80 جرام، 100 ورقة",
                DescriptionEn = "A4 notebook 80gsm quality, 100 sheets",
                CostPrice = 15.000m,
                SalePrice = 30.000m,
                VatRateId = vatRate.Id,
                EtaItemCode = "482010",
                EtaUnitCode = "PCE",
                MinimumStockLevel = 50,
                MaximumStockLevel = 500,
                ReorderPoint = 100,
                CategoryId = (await context.Categories.FirstAsync(c => c.NameEn == "Notebooks")).Id,
                UnitId = unit.Id,
                BranchId = branch.Id,
                IsActive = true,
                RequiresTaxInvoice = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "SYSTEM"
            }
        };

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}