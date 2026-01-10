using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Contracts;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Services;

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

    IResult Authenticate(AuthRequest request, IJwtService jwtService, IUserRepository userRepository, ILogger<AuthRoute> logger)
    {
        logger.LogInformation("Authenticating user: {Username}", request.Username);
        var hashPass = request.Password;
        var user = userRepository.GetAll().FirstOrDefault(x=>x.Username == request.Username && x.PasswordHash == hashPass);

        if (user == null)
        {
            logger.LogInformation("User {Username} not found", request.Username);
            return TypedResults.Unauthorized();
        }
        
        var token = jwtService.GenerateToken(user.Id.ToString(), user.Role.ToString());

        return TypedResults.Ok(token);
    }
}