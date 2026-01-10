using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Postech.NETT11.PhaseOne.Infrastructure;
using Postech.NETT11.PhaseOne.WebApp.Middlewares;
using Postech.NETT11.PhaseOne.WebApp.Services.Auth;
using Scalar.AspNetCore;

namespace Postech.NETT11.PhaseOne.WebApp.Extensions;

public static class AppExtensions
{
    
    public static WebApplication UseOpenApi(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.Authentication = new ScalarAuthenticationOptions()
                {
                    
                };
            });
        }
        
        return app;
    }
    
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        const int maxRetries = 10;
        var delay = TimeSpan.FromSeconds(3);

        for (int i = 1; i <= maxRetries; i++)
        {
            try
            {
                logger.LogInformation("Applying database migrations (attempt {Attempt}/{MaxRetries})...", i, maxRetries);
                await db.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully!");
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Migration failed on attempt {Attempt}/{MaxRetries}", i, maxRetries);
                
                if (i == maxRetries)
                {
                    logger.LogCritical("Failed to apply migrations after {MaxRetries} attempts. Exiting.", maxRetries);
                    throw;
                }
                
                await Task.Delay(delay);
            }
        }
    }

}