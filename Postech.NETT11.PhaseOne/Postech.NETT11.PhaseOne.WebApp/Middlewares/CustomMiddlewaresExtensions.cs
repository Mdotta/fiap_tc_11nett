using Postech.NETT11.PhaseOne.WebApp.Middlewares;

public static class CustomMiddlewaresExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }

    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            Console.WriteLine($"Incoming request: {context.Request.Method} {context.Request.Path}");
            await next.Invoke();
            Console.WriteLine($"Outgoing response: {context.Response.StatusCode}");
        });
    }

    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}