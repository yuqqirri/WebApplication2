using WebApplication2.Domain.Models.DTO;
using WebApplication2.Domain.Models.Response;

namespace WebApplication2.Domain.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginModel request);
    Task<bool> RegisterAsync(RegisterModel request);
}