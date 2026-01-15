using Postech.NETT11.PhaseOne.Application.DTOs.Requests;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;

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

    private async Task<IResult> Authenticate(
        HttpContext httpContext,
        AuthRequest request, 
        IAuthService authService)
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        
        var authResponse = await authService.AuthenticateAsync(
            request.Username, 
            request.Password, 
            ipAddress);

        if (authResponse == null)
        {
            return TypedResults.Unauthorized();
        }

        return TypedResults.Ok(authResponse);
    }
}