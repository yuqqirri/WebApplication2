using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SecureController : ControllerBase
{
    [HttpGet("test")]
    public IActionResult GetSecretData()
    {
        return Ok("Авторизация произведена успешна!");
    }
}