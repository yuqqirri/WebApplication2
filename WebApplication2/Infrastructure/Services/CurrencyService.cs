using WebApplication2.Domain.Interfaces;
using WebApplication2.Domain.Models;
using WebApplication2.Domain.Models.DTO;

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

    public async Task<Currency> CreateCurrencyAsync(Currency currency)
    {
        await repository.AddSingleCurrencyAsync(currency);
        await repository.SaveChangesAsync();
        return currency;
    }

    public async Task<Currency?> GetCurrencyAsync(int id) => await repository.GetCurrencyByIdAsync(id);

    public async Task<Rundown> CreateRundownAsync(Rundown rundown)
    {
        await repository.AddRundownAsync(rundown);
        await repository.SaveChangesAsync();
        return rundown;
    }

    public async Task<Rundown?> GetRundownAsync(int id) => await repository.GetRundownByIdAsync(id);


    public async Task<RateResponse?> GetLatestRateAsync(LatestRateRequest request)
    {
        var rundown = await repository.GetLatestRundownAsync(request.Target);
        if (rundown == null) return null;

        return new RateResponse(request.Base, request.Target, rundown.Rundown_value, rundown.Rundown_date);
    }

    public async Task<List<RateResponse>> GetHistoryAsync(HistoryRequest request)
    {
        var history = await repository.GetRundownHistoryAsync(request.Target, request.From, request.To);
        return history.Select(r => new RateResponse(request.Base, request.Target, r.Rundown_value, r.Rundown_date)).ToList();
    }

    public async Task<decimal?> GetPriceChangeAsync(ChangeRequest request)
    {
        var past = await repository.GetRundownByDateAsync(request.Target, request.Past);
        var current = await repository.GetRundownByDateAsync(request.Target, request.Current);

        if (past == null || current == null || past.Rundown_value == 0)
        {
            logger.LogWarning("Недостаточно данных для расчета курса {Target}", request.Target);
            return null;
        }


        return ((current.Rundown_value - past.Rundown_value) / past.Rundown_value) * 100;
    }
}