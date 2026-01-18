using Postech.NETT11.PhaseOne.Domain.Common;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
        
        _logger.LogError(exception,
            "Unhandled exception occurred. CorrelationId: {CorrelationId}, Path: {Path}, Method: {Method}, User: {User}",
            correlationId,
            context.Request.Path,
            context.Request.Method,
            context.User?.Identity?.Name ?? "Anonymous");

        var (statusCode, title, detail) = MapExceptionToResponse(exception);

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var problemDetails = new
        {
            type = $"https://httpstatuses.com/{statusCode}",
            title,
            status = statusCode,
            detail = _environment.IsDevelopment() ? detail : null,
            correlationId,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private (int StatusCode, string Title, string? Detail) MapExceptionToResponse(Exception exception)
    {
        return exception switch
        {
            ArgumentNullException or ArgumentException => 
                (400, "Bad Request", exception.Message),
            
            InvalidOperationException => 
                (400, "Bad Request", exception.Message),
            
            KeyNotFoundException => 
                (404, "Not Found", exception.Message),
            
            UnauthorizedAccessException => 
                (403, "Forbidden", string.IsNullOrWhiteSpace(exception.Message) 
                    ? "You do not have permission to access this resource." 
                    : exception.Message),
            
            DomainException => 
                (400, "Bad Request", exception.Message),
            
            _ => (500, "Internal Server Error", 
                  _environment.IsDevelopment() 
                      ? exception.ToString() 
                      : "An unexpected error occurred. Please contact support with the correlation ID.")
        };
    }
}
