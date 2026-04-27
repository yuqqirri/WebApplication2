using Microsoft.AspNetCore.Mvc;
using WebApplication2.Domain.Interfaces;
using WebApplication2.Domain.Models.DTO;
using WebApplication2.Services;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var response = await _authService.LoginAsync(model);
        if (response == null) return Unauthorized("Неверный логин или пароль");
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest model)
    {
        var result = await _authService.RegisterAsync(model);
        if (result is false) return BadRequest("Пользователь уже существует");
        return Ok("Регистрация успешна");
    }
}