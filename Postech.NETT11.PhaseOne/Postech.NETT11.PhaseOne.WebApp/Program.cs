using Microsoft.EntityFrameworkCore;
using Postech.NETT11.PhaseOne.Domain.Repositories;
using Postech.NETT11.PhaseOne.Infrastructure.Repository;
using Postech.NETT11.PhaseOne.WebApp.Endpoints;
using Postech.NETT11.PhaseOne.WebApp.Extensions;
using Postech.NETT11.PhaseOne.WebApp.Middlewares;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;


// BOOTSTRAP LOGGER \\

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

// BOOTSTRAP LOGGER \\    

try
{
    Log.Information("Starting Postech.NETT11.PhaseOne application");

    var builder = WebApplication.CreateBuilder(args);

// CONFIGURE SERILOG \\
    builder.Host.UseSerilog((context, services, configuration) =>
    {
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithThreadId()
            .Enrich.WithProcessId()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}"
            )
            .WriteTo.File(
                path: "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}"
            )
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://192.168.224.2:9200"))
            {
                IndexFormat = "logs-{0:yyyy.MM.dd}",
                AutoRegisterTemplate = true,
                NumberOfShards = 1,
                NumberOfReplicas = 0,
                EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog,
                BatchPostingLimit = 50,
                Period = TimeSpan.FromSeconds(2)
            });
    });
// CONFIGURE SERILOG \\

    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

    builder
        .RegisterAuth()
        .RegisterOpenApi()
        .RegisterServices()
        .RegisterRepositories()
        .RegisterDbContext(configuration);

// App \\
    var app = builder.Build();
// App \\

// SERILOG REQUEST LOGGING \\
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
// SERILOG REQUEST LOGGING \\

    app.UseOpenApi();

    #region Auth

    app.UseAuthentication();
    app.UseAuthorization();

    #endregion

    #region Middlewares

    app.UseCorrelationId();           // Add BEFORE the other middlewares \\
    app.UseGlobalExceptionHandling();

    #endregion

    #region Endpoints

    app.UseRoutes();

    #endregion

    app.UseHttpsRedirection();

    Log.Information("Application started successfully");
    Log.Information("Kibana: http://192.168.244.2:5601");

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