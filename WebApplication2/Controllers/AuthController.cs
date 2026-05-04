using Microsoft.AspNetCore.Mvc;
using WebApplication2.Domain.Models.DTO;
using WebApplication2.Domain.Interfaces;

namespace WebApplication2.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel request)
    {
        var response = await authService.LoginAsync(request);
        if (response == null) return Unauthorized(new { Message = "Неверный логин или пароль" });
        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel request)
    {
        var success = await authService.RegisterAsync(request);
        if (!success) return BadRequest(new { Message = "Пользователь уже существует" });
        return Ok(new { Message = "Успешная регистрация" });
    }
}