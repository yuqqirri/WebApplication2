using WebApplication2.Domain.Models;
using WebApplication2.Domain.Models.DTO;

namespace WebApplication2.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserAsync(LoginModel request);
    Task AddUserAsync(User user);
    Task SaveChangesAsync();
}

public interface ICurrencyRepository
{
    Task AddCurrenciesAsync(List<Currency> currencies, CancellationToken ct);
    Task<List<string>> GetExistingCurrencyNamesAsync(CancellationToken ct);
}