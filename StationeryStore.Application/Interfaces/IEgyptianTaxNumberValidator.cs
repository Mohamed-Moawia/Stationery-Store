using System.Threading.Tasks;

namespace StationeryStore.Application.Interfaces;

public interface IEgyptianTaxNumberValidator
{
    bool IsValid(string taxNumber);
    Task<bool> IsValidAsync(string taxNumber);
}