using System;
using Microsoft.Extensions.Options;
using StationeryStore.Application.Interfaces;
using StationeryStore.Application.Models;

namespace StationeryStore.Infrastructure.Services;

public class EgyptianVatService : IEgyptianVatService
{
    private readonly decimal _vatRate;

    public EgyptianVatService(IOptions<EgyptSettings> options)
    {
        _vatRate = options?.Value?.DefaultVatRate ?? 14m;
    }

    public decimal CalculateVat(decimal netAmount) => Math.Round(netAmount * (_vatRate / 100m), 3);

    public decimal CalculateGross(decimal netAmount) => Math.Round(netAmount + CalculateVat(netAmount), 3);

    public decimal ExtractNet(decimal grossAmount)
    {
        var factor = 1 + (_vatRate / 100m);
        return Math.Round(grossAmount / factor, 3);
    }
}