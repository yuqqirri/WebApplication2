using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using WebApplication2.Repositories;

namespace WebApplication2.Services
{
    public class CurrencyRateService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<CurrencyRateService> _logger;
        private readonly ApplicationDbContext dbContext;
        private readonly UserRepository _userRepository;

        public CurrencyRateService(
            IServiceScopeFactory scopeFactory,
            IHttpClientFactory httpClientFactory,
            ILogger<CurrencyRateService> logger,
            ApplicationDbContext context,
            UserRepository userRepository)
        {
            _scopeFactory = scopeFactory;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            dbContext = context;
            _userRepository = userRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var users = await _userRepository.GetAllUsersAsync();

            _logger.LogInformation("Сервис обновления курсов валют запущен");

            while (!stoppingToken.IsCancellationRequested)
            {
                await FetchCurrencyRatesAsync(stoppingToken);

                // Обновляем каждый 1 час
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }

        private async Task FetchCurrencyRatesAsync(CancellationToken ct)
        {
            var client = _httpClientFactory.CreateClient();
            const string url = "https://www.cbr-xml-daily.ru/daily_json.js";

            try
            {
                var response = await client.GetStringAsync(url, ct);
                var currencyData = JsonConvert.DeserializeObject<CurrencyData>(response);

                await ProcessCurrenciesAsync(currencyData.Valute, ct);
                await ProcessRundownsAsync(currencyData, ct);

                // Русское сообщение с временем из API
                _logger.LogInformation("Сводка обновлена (актуальность данных: {DataTime})",
                    currencyData.Date.ToString("dd.MM.yyyy HH:mm:ss"));
            }
            catch (TaskCanceledException) when (ct.IsCancellationRequested)
            {
                _logger.LogInformation("Запрошена остановка сервиса");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении курсов валют");
            }
        }

        private async Task ProcessCurrenciesAsync(
            Dictionary<string, CurrencyInfo> valute,
            CancellationToken ct)
        {
            var existingCurrencies = await dbContext.Currencies
                .Where(c => valute.Keys.Contains(c.Currency_name))
                .ToDictionaryAsync(c => c.Currency_name, ct);

            var newCurrencies = valute.Keys
                .Where(code => !existingCurrencies.ContainsKey(code))
                .Select(code => new Currency { Currency_name = code })
                .ToList();

            if (newCurrencies.Any())
            {
                await dbContext.Currencies.AddRangeAsync(newCurrencies, ct);
                await dbContext.SaveChangesAsync(ct);
            }
        }

        private async Task ProcessRundownsAsync(
            CurrencyData currencyData,
            CancellationToken ct)
        {
            var currencies = await dbContext.Currencies
                .Where(c => currencyData.Valute.Keys.Contains(c.Currency_name))
                .ToDictionaryAsync(c => c.Currency_name, ct);

            var rundowns = currencyData.Valute
                .Where(kvp => currencies.ContainsKey(kvp.Key))
                .Select(kvp =>
                {
                    var currencyInfo = kvp.Value;
                    return new Rundown
                    {
                        Rundown_date = currencyData.Date.ToUniversalTime(),
                        Rundown_value = currencyInfo.Value / currencyInfo.Nominal,
                        Currency_id = currencies[kvp.Key].Currency_id
                    };
                })
                .ToList();

            await dbContext.Rundowns.AddRangeAsync(rundowns, ct);
            await dbContext.SaveChangesAsync(ct);
        }
    }

    public class CurrencyData
    {
        public DateTime Date { get; set; }
        public Dictionary<string, CurrencyInfo> Valute { get; set; }
    }

    public class CurrencyInfo
    {
        public decimal Value { get; set; }
        public int Nominal { get; set; }
    }
}