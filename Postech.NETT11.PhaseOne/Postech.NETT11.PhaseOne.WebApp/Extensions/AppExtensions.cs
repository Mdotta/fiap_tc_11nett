using Microsoft.EntityFrameworkCore;
using Postech.NETT11.PhaseOne.Infrastructure;
using Scalar.AspNetCore;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

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
                    PreferredSecurityScheme = "Bearer",
                    ApiKey = new ApiKeyOptions
                    {
                        Token = ""
                    }
                };
            });
        }
        
        return app;
    }
    
    public static WebApplication MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var db = services.GetRequiredService<AppDbContext>();
            Log.Information("Applying database migrations (if any)...");
            db.Database.Migrate();
            Log.Information("Database migrations applied.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while migrating or initializing the database.");
            throw;
        }

        return app;
    }

}