using WebApplication2.Models;

namespace WebApplication2.Interfaces
{
    public interface IAuthService
    {
        // Метод для регистрации. Возвращает объект пользователя или ошибку.
        Task<User?> RegisterAsync(string username, string password);

        // Метод для входа. Возвращает готовый JWT-токен, если данные верны.
        Task<string?> LoginAsync(string username, string password);
    }
}
