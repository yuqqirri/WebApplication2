using WebApplication2.Domain.Models;
using WebApplication2.Domain.Models.DTO;

namespace WebApplication2.Domain.Interfaces
{
    public interface IAuthService
    {
        // Метод для регистрации. Возвращает объект пользователя или ошибку.
        Task<bool> RegisterAsync(RegisterRequest request);

        // Метод для входа. Возвращает готовый JWT-токен, если данные верны.
        Task<AuthResponse?> LoginAsync(LoginRequest request);
    }
}
