using WebApplication2.Domain.Models;
using WebApplication2.Domain.Models.DTO;

namespace WebApplication2.Domain.Interfaces;

public interface ICurrencyService
{

    Task UpdateCurrenciesAsync(CancellationToken ct);


    Task<Currency> CreateCurrencyAsync(Currency currency);
    Task<Currency?> GetCurrencyAsync(int id);
    Task<Rundown> CreateRundownAsync(Rundown rundown);
    Task<Rundown?> GetRundownAsync(int id);

    Task<RateResponse?> GetLatestRateAsync(LatestRateRequest request);
    Task<List<RateResponse>> GetHistoryAsync(HistoryRequest request);
    Task<decimal?> GetPriceChangeAsync(ChangeRequest request);
}