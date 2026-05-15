using WebApplication2.Domain.Models;

namespace WebApplication2.Domain.Interfaces;

public interface ICurrencyRepository
{
    Task<List<string>> GetExistingCurrencyNamesAsync(CancellationToken ct);
    Task<List<Currency>> GetAllCurrenciesAsync(CancellationToken ct);
    Task AddCurrenciesAsync(List<Currency> currencies, CancellationToken ct);
    Task AddSingleCurrencyAsync(Currency currency);
    Task<Currency?> GetCurrencyByIdAsync(int id);
    Task AddRundownAsync(Rundown rundown);
    Task<Rundown?> GetRundownByIdAsync(int id);
    Task<Rundown?> GetLatestRundownAsync(string currencyName);
    Task<List<Rundown>> GetRundownHistoryAsync(string currencyName, DateTime from, DateTime to);
    Task<Rundown?> GetRundownByDateAsync(string currencyName, DateTime date);
    Task SaveChangesAsync();


}