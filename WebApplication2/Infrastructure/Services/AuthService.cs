using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using WebApplication2.Domain.Models;
using WebApplication2.Domain.Models.DTO;
using WebApplication2.Domain.Models.Response;
using WebApplication2.Domain.Interfaces;

namespace WebApplication2.Infrastructure.Services;

public class AuthService(IUserRepository userRepository, IConfiguration configuration) : IAuthService
{
    public async Task<AuthResponse?> LoginAsync(LoginModel request)
    {
        var user = await userRepository.GetUserAsync(request);
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            return null;

        var token = GenerateJwtToken(user);
        return new AuthResponse(token, user.Username);
    }

    public async Task<bool> RegisterAsync(RegisterModel request)
    {

        var checkRequest = new LoginModel(request.Username, string.Empty);
        if (await userRepository.GetUserAsync(checkRequest) != null)
            return false;

        var user = new User
        {
            Username = request.Username,
            PasswordHash = HashPassword(request.Password)
        };

        await userRepository.AddUserAsync(user);
        await userRepository.SaveChangesAsync();
        return true;
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[] {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            configuration["Jwt:Issuer"], configuration["Jwt:Audience"],
            claims, expires: DateTime.Now.AddHours(1), signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string HashPassword(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string storedHash) => HashPassword(password) == storedHash;
}