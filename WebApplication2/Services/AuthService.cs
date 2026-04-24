using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication2.Models;
using WebApplication2.Models.Response;
using WebApplication2.Repositories;

namespace WebApplication2.Services;

public class AuthService(UserRepository userRepository, IConfiguration _configuration)
{
    public async Task<LoginResponse> LoginAsync(LoginModel model)
    {
        var user = await userRepository.GetUser(model);

        if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
            return new LoginResponse();

        var token = GenerateJwtToken(user);

        var loginResponse = new LoginResponse
        {
            Token = token
        };

        return loginResponse;
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        var credentials = new SigningCredentials(
            securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private bool VerifyPassword(string password, string storedHash)
    {
        string hashedInput = HashPassword(password);
        return hashedInput == storedHash;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
