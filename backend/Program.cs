using backend.auth;
using Microsoft.AspNetCore.Authentication;

namespace backend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("Basic", options => { });
        builder.Services.AddAuthorization();
        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapGet("/login", () => { }).RequireAuthorization();

        app.Run();
    }
}
