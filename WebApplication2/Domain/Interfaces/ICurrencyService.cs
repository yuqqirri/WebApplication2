using WebApplication2.Domain.Models;

namespace WebApplication2.Domain.Interfaces;

public interface ICurrencyService
{
    // Основная логика обновления (используется в Quartz)[cite: 1]
    Task UpdateCurrenciesAsync(CancellationToken ct);

    // Логика для Контроллеров (создание и поиск)[cite: 1]
    Task<Currency> CreateCurrencyAsync(Currency currency);
    Task<Currency?> GetCurrencyAsync(int id);
    Task<Rundown> CreateRundownAsync(Rundown rundown);
    Task<Rundown?> GetRundownAsync(int id);
}