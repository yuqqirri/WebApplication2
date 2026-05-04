using Quartz;
using WebApplication2.Domain.Interfaces;

namespace WebApplication2.Infrastructure.Jobs;

[DisallowConcurrentExecution]
public class CurrencyUpdateJob(ICurrencyService currencyService, ILogger<CurrencyUpdateJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            logger.LogInformation("Quartz: Начало обновления валют...");
            await currencyService.UpdateCurrenciesAsync(context.CancellationToken);
            logger.LogInformation("Quartz: Обновление завершено.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка в работе Quartz Job");
        }
    }
}