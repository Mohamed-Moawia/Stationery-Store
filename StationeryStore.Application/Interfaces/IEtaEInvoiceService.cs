using System.Threading.Tasks;

namespace StationeryStore.Application.Interfaces;

public interface IEtaEInvoiceService
{
    Task<string> SubmitInvoiceAsync(string invoicePayload);
    Task<string> GetInvoiceStatusAsync(string invoiceId);
}