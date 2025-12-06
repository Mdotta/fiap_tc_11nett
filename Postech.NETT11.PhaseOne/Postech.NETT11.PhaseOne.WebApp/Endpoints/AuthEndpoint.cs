using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Contracts;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Services;
using Postech.NETT11.PhaseOne.Domain.Repositories;
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
            .RequireAuthorization("Admin");
    }

    static IResult Authenticate(AuthRequest request, IJwtService jwtService, IUserRepository repository)
    {
        //TODO: Validate user credentials from database
        var hashPass = request.Password;
        var user = repository.GetAll().FirstOrDefault(x=>x.Username == request.Username && x.PasswordHash == hashPass);
        
        if (user == null)
            return TypedResults.Unauthorized();
        
        var token = jwtService.GenerateToken(user.Id.ToString(), user.Role.ToString());

        return TypedResults.Ok(token);
    }
}