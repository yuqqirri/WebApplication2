using WebApplication2.Domain.Models.DTO;

namespace WebApplication2.Domain.Interfaces;

public interface ICurrencyApiClient
{
    // Получение сырых данных из внешнего API (ЦБ РФ)
    Task<CbrCurrencyData?> GetRatesFromBankAsync(CancellationToken ct);
}