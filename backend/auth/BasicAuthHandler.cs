using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using backend.db;
using common.Models;

namespace backend.auth;

public class BasicAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly SqliteDb _db;
    public BasicAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        SqliteDb db)
        : base(options, logger, encoder)
    {
        _db = db;
    }
        
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out Microsoft.Extensions.Primitives.StringValues value))
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));

        var authHeader = AuthenticationHeaderValue.Parse(value!);

        try
        {
            if (authHeader.Scheme != "Basic")
                return Task.FromResult(AuthenticateResult.Fail("Invalid scheme"));

            var credentialsBytes = Convert.FromBase64String(authHeader.Parameter!);
            var credentials = Encoding.UTF8.GetString(credentialsBytes).Split(':', 2);

            var username = credentials[0];
            var passwd = credentials[1];

            var login = _db.QueryFirst<LoginModel>("SELECT Username, Password FROM users WHERE Username=@username", new { username = username });
            if (login.Password != passwd)
            {
                return Task.FromResult(AuthenticateResult.Fail($"Invalid credentials"));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            throw;
        }
    }
}