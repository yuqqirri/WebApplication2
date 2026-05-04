namespace WebApplication2.Domain.Models.DTO
{
    public record LoginRequest(string Username, string Password);
    public record RegisterRequest(string Username, string Password);
    public record AuthResponse(string Token, string Username);
}