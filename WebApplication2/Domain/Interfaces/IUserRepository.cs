using WebApplication2.Domain.Models;
using WebApplication2.Domain.Models.DTO;

namespace WebApplication2.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserAsync(LoginModel request);
    Task AddUserAsync(User user);
    Task SaveChangesAsync();
}