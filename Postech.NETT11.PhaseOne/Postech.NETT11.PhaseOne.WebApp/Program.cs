using Postech.NETT11.PhaseOne.WebApp.Endpoints;
using Postech.NETT11.PhaseOne.WebApp.Extensions;
using Postech.NETT11.PhaseOne.WebApp.Middlewares;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Postech.NETT11.PhaseOne application");

    var builder = WebApplication.CreateBuilder(args);
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Log.Information("Connection string: {ConnectionString}", connectionString);

    builder.Host.UseSerilog((context, services, options) =>
    {
        options
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext();
    });
    
    builder
        .RegisterAuth()
        .RegisterOpenApi()
        .RegisterServices()
        .RegisterRepositories()
        .RegisterDbContext(builder.Configuration);

    
    // Build the app
    var app = builder.Build();

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
        
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
            diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
            
            if (httpContext.Items.TryGetValue("CorrelationId", out var correlationId))
            {
                diagnosticContext.Set("CorrelationId", correlationId);
            }
        };
    });

    app.UseOpenApi();
    
    //apply migrations
    await app.MigrateDatabaseAsync();

    #region Auth
    app.UseAuthentication();
    app.UseAuthorization();
    #endregion

    #region Middlewares
    app
        .UseCorrelationId()
        .UseRequestLogging()
        .UseGlobalExceptionHandling();
    #endregion

    #region Endpoints
    app.UseRoutes();
    #endregion

    app.UseHttpsRedirection();

    Log.Information("Application started successfully");
    Log.Information("Kibana: {KibanaUrl}", builder.Configuration["Kibana:Url"] ?? "http://localhost:5601");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.Information("Application shutting down");
    Log.CloseAndFlush();
}