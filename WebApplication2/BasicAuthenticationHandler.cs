using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using WebApplication2.Data;
using System.Security.Cryptography;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ApplicationDbContext _context;

    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        ApplicationDbContext context) : base(options, logger, encoder, clock)
    {
        _context = context;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string authHeader = Request.Headers["Authorization"];
        if (string.IsNullOrEmpty(authHeader))
            return AuthenticateResult.Fail("Authorization header missing");

        if (!authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.Fail("Invalid authentication scheme");

        try
        {
            string encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
            byte[] decodedBytes = Convert.FromBase64String(encodedCredentials);
            string decodedCredentials = Encoding.UTF8.GetString(decodedBytes);

            string[] credentials = decodedCredentials.Split(':', 2);
            if (credentials.Length != 2)
                return AuthenticateResult.Fail("Invalid credentials format");

            string username = credentials[0];
            string password = credentials[1];

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user == null)
                return AuthenticateResult.Fail("User not found");

            if (!VerifyPassword(password, user.PasswordHash))
                return AuthenticateResult.Fail("Invalid password");

            var claims = new[] { new Claim(ClaimTypes.Name, user.Username) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        catch
        {
            return AuthenticateResult.Fail("Error processing authentication");
        }
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