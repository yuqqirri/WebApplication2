using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication2.Data;
using Microsoft.Extensions.Configuration;
using WebApplication2.Models;
using WebApplication2.Services;
using Microsoft.AspNetCore.Identity.Data;
using WebApplication2.Models.Response;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly AuthService _authService;

    public AuthController(ApplicationDbContext context, IConfiguration configuration, AuthService authService)
    {
        _context = context;
        _configuration = configuration;
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginModel model)
    {
        var token = await _authService.LoginAsync(model);

        if (token.Token.IsNullOrEmpty())
        {
            return Unauthorized("Неверный логин или пароль");
        }

        return Ok(token);
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterModel model)
    {
        if (_context.Users.Any(u => u.Username == model.Username))
            return BadRequest("Пользователь с таким именем уже существует");

        var user = new User
        {
            Username = model.Username,
            PasswordHash = HashPassword(model.Password)
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        // Генерация токена после успешной регистрации
        var token = GenerateJwtToken(user);
        return Ok(new
        {
            Token = token,
            Message = "Пользователь успешно зарегистрирован"
        });
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

public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class RegisterModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}