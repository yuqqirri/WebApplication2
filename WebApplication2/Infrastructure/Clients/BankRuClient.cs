using System.Text.Json;
using WebApplication2.Domain.Interfaces;
using WebApplication2.Domain.Models.DTO;

namespace WebApplication2.Infrastructure.Clients;

public class BankRuClient(HttpClient httpClient) : ICurrencyApiClient
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<CbrCurrencyData?> GetRatesFromBankAsync(CancellationToken ct)
    {
        var response = await httpClient.GetStreamAsync("https://www.cbr-xml-daily.ru/daily_json.js", ct);
        return await JsonSerializer.DeserializeAsync<CbrCurrencyData>(response, _jsonOptions, ct);
    }
}