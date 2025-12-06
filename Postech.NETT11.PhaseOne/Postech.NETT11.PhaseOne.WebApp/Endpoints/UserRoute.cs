using System.Security.Claims;
using Postech.NETT11.PhaseOne.Domain.Repositories;

namespace Postech.NETT11.PhaseOne.WebApp.Endpoints;

public class UserRoute:BaseRoute
{
    public UserRoute()
    {
        Route = "/user";
    }
    protected override void AddRoute(RouteGroupBuilder group)
    {
        group.MapGet("/me", GetAuthenticatedUserData)
            .WithName("Me")
            .WithOpenApi()
            .RequireAuthorization();
    }

    private IResult GetAuthenticatedUserData(HttpContext context, IUserRepository userRepository)
    {
        var userId = context.User.Claims.FirstOrDefault(x=>x.Type == ClaimTypes.NameIdentifier);
        if (userId == null)
            return TypedResults.Unauthorized();
        var user = userRepository.GetById(Guid.Parse(userId.Value));
        return TypedResults.Ok(user);
    }
}