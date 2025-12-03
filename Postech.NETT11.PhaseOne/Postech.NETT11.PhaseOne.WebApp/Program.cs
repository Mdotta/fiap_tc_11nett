using Microsoft.EntityFrameworkCore;
using Postech.NETT11.PhaseOne.Domain.Repositories;
using Postech.NETT11.PhaseOne.Infrastructure.Repository;
using Postech.NETT11.PhaseOne.WebApp.Endpoints;
using Postech.NETT11.PhaseOne.WebApp.Extensions;
using Postech.NETT11.PhaseOne.WebApp.Middlewares;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

builder
    .RegisterAuth()
    .RegisterOpenApi()
    .RegisterServices()
    .RegisterRepositories()
    .RegisterDbContext(configuration);

builder.Services.AddControllers();

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

app.MapControllers();

// //Test endpoint
// app.MapGet("/ping", () => TypedResults.Ok("pong"))
//     .WithName("Ping")
//     .WithOpenApi()
//     .AllowAnonymous();
//
// app.UseEndpoints();

#endregion

app.UseHttpsRedirection();

app.Run();
