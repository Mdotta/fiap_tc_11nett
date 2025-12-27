using Microsoft.EntityFrameworkCore;
using Postech.NETT11.PhaseOne.Domain.Repositories;
using Postech.NETT11.PhaseOne.Infrastructure.Repository;
using Postech.NETT11.PhaseOne.WebApp.Endpoints;
using Postech.NETT11.PhaseOne.WebApp.Extensions;
using Postech.NETT11.PhaseOne.WebApp.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder
    .RegisterAuth()
    .RegisterOpenApi()
    .RegisterServices()
    .RegisterRepositories()
    .RegisterDbContext(builder.Configuration);

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

app.UseRoutes();

#endregion

app.UseHttpsRedirection();

app.Run();
