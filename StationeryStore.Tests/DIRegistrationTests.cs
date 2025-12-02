using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StationeryStore.API.Extensions;
using StationeryStore.Application.Interfaces;
using FluentAssertions;
using Xunit;

namespace StationeryStore.Tests;

public class DIRegistrationTests
{
    [Fact]
    public void AddEgyptianServices_RegistersAllServices()
    {
        var services = new ServiceCollection();

        // Provide in-memory configuration for EgyptSettings
        var dict = new Dictionary<string, string>
        {
            ["EgyptSettings:DefaultVatRate"] = "14",
            ["EgyptSettings:DefaultCurrency"] = "EGP"
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(dict).Build();

        services.AddEgyptianServices(configuration);
        using var sp = services.BuildServiceProvider();

        sp.GetService<IEgyptianVatService>().Should().NotBeNull();
        sp.GetService<IEgyptianTaxNumberValidator>().Should().NotBeNull();
        sp.GetService<IEgyptianBusinessHours>().Should().NotBeNull();
        sp.GetService<IEtaEInvoiceService>().Should().NotBeNull();
        sp.GetService<ILocalizationService>().Should().NotBeNull();
        sp.GetService<ICurrencyFormatter>().Should().NotBeNull();
        sp.GetService<IAuditService>().Should().NotBeNull();
    }
}