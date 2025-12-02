using System;
using System.Threading.Tasks;
using StationeryStore.Application.Interfaces;

namespace StationeryStore.Infrastructure.Services;

public class EtaEInvoiceSandboxService : IEtaEInvoiceService
{
    public Task<string> SubmitInvoiceAsync(string invoicePayload)
    {
        // Return a deterministic id for simplicity
        return Task.FromResult($"SAMPLE-{Guid.NewGuid():N}");
    }

    public Task<string> GetInvoiceStatusAsync(string invoiceId)
    {
        // Return a simple status
        return Task.FromResult("Accepted");
    }
}