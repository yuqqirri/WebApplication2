using WebApplication2.Domain.Models;

namespace WebApplication2.Domain.Interfaces;

public interface ICurrencyRepository
{
    // Методы для Quartz-задачи
    Task<List<string>> GetExistingCurrencyNamesAsync(CancellationToken ct);
    Task AddCurrenciesAsync(List<Currency> currencies, CancellationToken ct);

    // Методы для Контроллеров[cite: 1]
    Task AddSingleCurrencyAsync(Currency currency);
    Task<Currency?> GetCurrencyByIdAsync(int id);
    Task AddRundownAsync(Rundown rundown);
    Task<Rundown?> GetRundownByIdAsync(int id);
    Task SaveChangesAsync();
}