namespace Postech.NETT11.PhaseOne.WebApp.Endpoints;

public static class EndpointsConfiguration
{
    public static void UseEndpoints(this WebApplication app)
    {
        app.RegisterAuthEndpoints();
    }
}