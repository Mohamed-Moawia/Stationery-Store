using System.Globalization;

namespace StationeryStore.Application.Interfaces;

public interface ICurrencyFormatter
{
    string Format(decimal amount, CultureInfo? culture = null);
    string FormatWithCurrency(decimal amount, CultureInfo? culture = null);
}