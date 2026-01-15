using Postech.NETT11.PhaseOne.Application.DTOs.Requests;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;

namespace Postech.NETT11.PhaseOne.WebApp.Endpoints;

public class AuthRoute:BaseRoute
{
    public AuthRoute()
    {
        Route = "/auth";
    }
    
    protected override void AddRoute(RouteGroupBuilder group)
    {
        group.MapPost("/login", Authenticate)
            .WithName("Authenticate")
            .WithOpenApi()
            .AllowAnonymous();
    }

    private async Task<IResult> Authenticate(AuthRequest request, IJwtService jwtService, IUserRepository userRepository, IPasswordHasher passwordHasher, ILogger<AuthRoute> logger)
    {
        //TODO: criar serviço de autenticação
        logger.LogInformation("Authenticating user: {Username}", request.Username);
        var user = await userRepository.GetByUsername(request.Username);
        if (user == null || !passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            logger.LogInformation("User {Username} not found or invalid password", request.Username);
            return TypedResults.Unauthorized();
        }
        
        var token = jwtService.GenerateToken(user.Id.ToString(), user.Role.ToString());

        return TypedResults.Ok(token);
    }
}