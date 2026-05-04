using Microsoft.EntityFrameworkCore;
using WebApplication2.Domain.Interfaces;
using WebApplication2.Domain.Models;
using WebApplication2.Infrastructure.Data;

namespace WebApplication2.Infrastructure.Repositories;

public class CurrencyRepository(ApplicationDbContext context) : ICurrencyRepository
{
    public async Task<List<string>> GetExistingCurrencyNamesAsync(CancellationToken ct) =>
        await context.Currencies.Select(c => c.Currency_name).ToListAsync(ct);

    public async Task AddSingleCurrencyAsync(Currency currency)
        => await context.Currencies.AddAsync(currency);

    public async Task<Currency?> GetCurrencyByIdAsync(int id)
        => await context.Currencies.FindAsync(id);

    public async Task AddRundownAsync(Rundown rundown)
        => await context.Rundowns.AddAsync(rundown);

    public async Task<Rundown?> GetRundownByIdAsync(int id)
        => await context.Rundowns.FindAsync(id);

    public async Task SaveChangesAsync()
        => await context.SaveChangesAsync();

    public async Task AddCurrenciesAsync(List<Currency> currencies, CancellationToken ct)
    {
        await context.Currencies.AddRangeAsync(currencies, ct);
        await context.SaveChangesAsync(ct);
    }
}