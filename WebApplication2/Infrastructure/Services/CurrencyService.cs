using WebApplication2.Domain.Interfaces;
using WebApplication2.Domain.Models;

namespace WebApplication2.Infrastructure.Services;

public class CurrencyService(
    ICurrencyRepository repository,
    ICurrencyApiClient apiClient,
    ILogger<CurrencyService> logger) : ICurrencyService
{
    public async Task UpdateCurrenciesAsync(CancellationToken ct)
    {
        var data = await apiClient.GetRatesFromBankAsync(ct);
        if (data == null) return;

        var existingNames = await repository.GetExistingCurrencyNamesAsync(ct);

        var newCurrencies = data.Valute.Keys
            .Where(name => !existingNames.Contains(name))
            .Select(name => new Currency { Currency_name = name })
            .ToList();

        if (newCurrencies.Count > 0)
        {
            await repository.AddCurrenciesAsync(newCurrencies, ct);
            logger.LogInformation("Добавлено {Count} новых валют", newCurrencies.Count);
        }
    }
}