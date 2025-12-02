// StationeryStore.API/Extensions/ServiceCollectionExtensions.cs
using System;                                       // added
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StationeryStore.Application.Interfaces;
using StationeryStore.Infrastructure.Services;
using StationeryStore.Application.Models;  // <-- now references shared settings

namespace StationeryStore.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEgyptianServices(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        services.AddOptions();
        services.Configure<EgyptSettings>(configuration.GetSection("EgyptSettings"));

        // Egyptian VAT Service
        services.AddScoped<IEgyptianVatService, EgyptianVatService>();

        // Egyptian Tax Number Validator
        services.AddSingleton<IEgyptianTaxNumberValidator, EgyptianTaxNumberValidator>();

        // Egyptian Business Hours Service
        services.AddSingleton<IEgyptianBusinessHours, EgyptianBusinessHours>();

        // ETA E-Invoicing Service (Sandbox for development)
        services.AddScoped<IEtaEInvoiceService, EtaEInvoiceSandboxService>();

        // Arabic/English Localization Service
        services.AddScoped<ILocalizationService, ArabicEnglishLocalizationService>();

        // Egyptian Currency Formatter
        services.AddSingleton<ICurrencyFormatter, EgyptianCurrencyFormatter>();

        // Audit Service for Egyptian compliance
        services.AddScoped<IAuditService, AuditService>();

        // Memory Cache with Egyptian time-based expiration
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 1024; // size units; set larger as needed
            options.CompactionPercentage = 0.25;
        });

        return services;
    }
}

// Egyptian settings model
public class EgyptSettings
{
    public string CountryCode { get; set; } = "EG";
    public string DefaultLanguage { get; set; } = "ar";
    public string DefaultCurrency { get; set; } = "EGP";
    public decimal DefaultVatRate { get; set; } = 14.0m;
    public string TimeZone { get; set; } = "Egypt Standard Time";
    public int DecimalPlaces { get; set; } = 3;
    public string[] SupportedLanguages { get; set; } = { "ar", "en" };
}