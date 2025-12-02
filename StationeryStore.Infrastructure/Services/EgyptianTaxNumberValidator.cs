using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StationeryStore.Application.Interfaces;

namespace StationeryStore.Infrastructure.Services;

public class EgyptianTaxNumberValidator : IEgyptianTaxNumberValidator
{
    // Basic local validation: digits only and length 9 or 14 (common formats)
    private static readonly Regex _digitsOnly = new Regex(@"^\d+$");

    public bool IsValid(string taxNumber)
    {
        if (string.IsNullOrWhiteSpace(taxNumber)) return false;
        var clean = taxNumber.Trim();
        if (!_digitsOnly.IsMatch(clean)) return false;
        return clean.Length == 9 || clean.Length == 14;
    }

    public Task<bool> IsValidAsync(string taxNumber) => Task.FromResult(IsValid(taxNumber));
}