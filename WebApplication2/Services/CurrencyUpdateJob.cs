using Quartz;
using WebApplication2.Data;
using WebApplication2.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace WebApplication2.Services; // Убрали лишние скобки { } для всего файла

[DisallowConcurrentExecution]
// Вот так выглядит основной конструктор. Мы объявляем зависимости прямо возле имени класса!
public class CurrencyUpdateJob(
    ApplicationDbContext dbContext,
    IHttpClientFactory httpClientFactory,
    ILogger<CurrencyUpdateJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("Запуск Quartz Job обновления валют: {Time}", DateTime.Now);

        try
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetStringAsync("https://www.cbr-xml-daily.ru/daily_json.js", context.CancellationToken);
            var data = JsonConvert.DeserializeObject<CurrencyData>(response);

            if (data != null)
            {
                await ProcessCurrenciesAsync(data, context.CancellationToken);
                await ProcessRundownsAsync(data, context.CancellationToken);

                logger.LogInformation("Данные успешно обновлены через Quartz.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка в работе Quartz Job");
        }
    }

    private async Task ProcessCurrenciesAsync(CurrencyData currencyData, CancellationToken ct)
    {
        var existingNames = await dbContext.Currencies
            .Select(c => c.Currency_name)
            .ToListAsync(ct);

        var newCurrencies = currencyData.Valute.Keys
            .Where(name => !existingNames.Contains(name))
            .Select(name => new Currency { Currency_name = name })
            .ToList();

        if (newCurrencies.Any())
        {
            await dbContext.Currencies.AddRangeAsync(newCurrencies, ct);
            await dbContext.SaveChangesAsync(ct);
        }
    }

    private async Task ProcessRundownsAsync(CurrencyData currencyData, CancellationToken ct)
    {
        var currencies = await dbContext.Currencies
            .Where(c => currencyData.Valute.Keys.Contains(c.Currency_name))
            .ToDictionaryAsync(c => c.Currency_name, ct);

        var rundowns = currencyData.Valute
            .Where(kvp => currencies.ContainsKey(kvp.Key))
            .Select(kvp =>
            {
                var info = kvp.Value;
                return new Rundown
                {
                    Rundown_date = currencyData.Date,
                    Rundown_value = info.Value / info.Nominal,
                    Currency_id = currencies[kvp.Key].Currency_id
                };
            })
            .ToList();

        await dbContext.Rundowns.AddRangeAsync(rundowns, ct);
        await dbContext.SaveChangesAsync(ct);
    }
}

// А ВОТ И НАШИ ПОТЕРЯННЫЕ КЛАССЫ!
public class CurrencyData
{
    public DateTime Date { get; set; }
    public Dictionary<string, CurrencyInfo> Valute { get; set; } = new();
}

public class CurrencyInfo
{
    public decimal Value { get; set; }
    public int Nominal { get; set; }
}