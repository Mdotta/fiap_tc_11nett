using Postech.NETT11.PhaseOne.Domain.Contracts.Auth;
using Postech.NETT11.PhaseOne.Domain.Repositories;
using Postech.NETT11.PhaseOne.WebApp.Services.Auth;

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

    IResult Authenticate(AuthRequest request, IJwtService jwtService, IUserRepository userRepository)
    {
        var hashPass = request.Password;
        var user = userRepository.GetAll().FirstOrDefault(x=>x.Username == request.Username && x.PasswordHash == hashPass);

        if (user == null)
            return TypedResults.Unauthorized();
        
        var token = jwtService.GenerateToken(user.Id.ToString(), user.Role.ToString());

        return TypedResults.Ok(token);
    }
}