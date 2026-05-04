using Microsoft.EntityFrameworkCore;
using WebApplication2.Domain.Interfaces;
using WebApplication2.Domain.Models;
using WebApplication2.Infrastructure.Data;

namespace WebApplication2.Infrastructure.Repositories;

public class CurrencyRepository(ApplicationDbContext context) : ICurrencyRepository
{
    public async Task<List<string>> GetExistingCurrencyNamesAsync(CancellationToken ct) =>
        await context.Currencies.Select(c => c.Currency_name).ToListAsync(ct);

    public async Task AddCurrenciesAsync(List<Currency> currencies, CancellationToken ct)
    {
        await context.Currencies.AddRangeAsync(currencies, ct);
        await context.SaveChangesAsync(ct);
    }
}