using Postech.NETT11.PhaseOne.WebApp.Endpoints;
using Postech.NETT11.PhaseOne.WebApp.Extensions;
using Postech.NETT11.PhaseOne.WebApp.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder
    .RegisterAuth()
    .RegisterOpenApi()
    .RegisterServices()
    .RegisterDependencyInjection();

//App
var app = builder.Build();

app.UseOpenApi();

#region Auth

app.UseAuthentication();
app.UseAuthorization();

#endregion

#region Middlewares

app.UseRequestLogging();
app.UseGlobalExceptionHandling();

#endregion

#region Endpoints

//Test endpoint
app.MapGet("/ping", () => TypedResults.Ok("pong"))
    .WithName("Ping")
    .WithOpenApi()
    .AllowAnonymous();

app.UseEndpoints();

#endregion

app.UseHttpsRedirection();

app.Run();
