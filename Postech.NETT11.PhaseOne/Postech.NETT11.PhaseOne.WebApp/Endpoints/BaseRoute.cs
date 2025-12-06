namespace Postech.NETT11.PhaseOne.WebApp.Endpoints;

public abstract class BaseRoute
{
    internal string Route = "/";

    public void AddRoute(WebApplication app)
    {
        var group = app.MapGroup(Route);
        AddRoute(group);
    }
    protected abstract void AddRoute(RouteGroupBuilder group);
}