using System;

namespace StationeryStore.Application.Interfaces;

public interface IEgyptianVatService
{
    decimal CalculateVat(decimal netAmount);
    decimal CalculateGross(decimal netAmount);
    decimal ExtractNet(decimal grossAmount);
}