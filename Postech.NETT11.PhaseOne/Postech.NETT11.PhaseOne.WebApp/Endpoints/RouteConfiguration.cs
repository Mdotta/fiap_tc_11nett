using System.Reflection;

namespace Postech.NETT11.PhaseOne.WebApp.Endpoints;

public static class RouteConfiguration
{
    public static void UseRoutes(this WebApplication app)
    {
        Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(BaseRoute)) && !t.IsAbstract)
            .Select(Activator.CreateInstance)
            .Cast<BaseRoute>()
            .ToList()
            .ForEach(endpoint => endpoint.AddRoute(app));
            
        // new AuthEndpoint().AddRoute(app);
    }
}