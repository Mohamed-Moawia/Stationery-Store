using System.Globalization;
using Microsoft.Extensions.Options;
using StationeryStore.Application.Interfaces;
using StationeryStore.Application.Models;

namespace StationeryStore.Infrastructure.Services;

public class EgyptianCurrencyFormatter : ICurrencyFormatter
{
    private readonly EgyptSettings _settings;

    public EgyptianCurrencyFormatter(IOptions<EgyptSettings> options)
    {
        _settings = options.Value;
    }

    public string Format(decimal amount, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.GetCultureInfo("ar-EG");
        return Math.Round(amount, _settings.DecimalPlaces).ToString("N", culture);
    }

    public string FormatWithCurrency(decimal amount, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.GetCultureInfo("ar-EG");
        // Use currency symbol if available for the culture
        return string.Format(culture, "{0:C" + _settings.DecimalPlaces + "}", amount);
    }
}