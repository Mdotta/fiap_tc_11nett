using Postech.NETT11.PhaseOne.WebApp.Models;
using Postech.NETT11.PhaseOne.WebApp.Services.Auth;

namespace Postech.NETT11.PhaseOne.WebApp.Endpoints;

public static class AuthEndpoint
{
    public static void RegisterAuthEndpoints(this WebApplication app)
    {
        var auth = app.MapGroup("/auth");

        auth.MapPost("/auth", Authenticate)
            .WithName("Authenticate")
            .WithOpenApi()
            .AllowAnonymous();
        
        auth.MapGet("/me", () => TypedResults.Ok("Get user data"))
            .WithName("Me")
            .WithOpenApi()
            .RequireAuthorization();
    }

    static IResult Authenticate(AuthRequest request, IJwtService jwtService)
    {
        if (request.Username == "admin" && request.Password == "password")
        {
            var token = jwtService.GenerateToken(request.Username, "Admin");

            return TypedResults.Ok(token);
        }

        return TypedResults.Unauthorized();
    }
}