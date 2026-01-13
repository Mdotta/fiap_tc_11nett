using System.Security.Claims;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;

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

    private IResult GetAuthenticatedUserData(HttpContext context, IUserRepository userRepository, ILogger<UserRoute> logger)
    {
        var userId = context.User.Claims.FirstOrDefault(x=>x.Type == ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            logger.LogInformation("User {Username} not found", context.User.Identity.Name);
            return TypedResults.Unauthorized();
        }
        
        logger.LogInformation("Getting data for user: {UserId}", userId.Value);
        var user = userRepository.GetByIdAsync(Guid.Parse(userId.Value));
        return TypedResults.Ok(user);
    }
}