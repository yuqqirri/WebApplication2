using WebApplication2.Domain.Models.DTO;

namespace WebApplication2.Domain.Interfaces;

public interface ICurrencyApiClient
{
    Task<CbrCurrencyData?> GetRatesFromBankAsync(CancellationToken ct);
}

public interface ICurrencyService
{
    Task UpdateCurrenciesAsync(CancellationToken ct);
}