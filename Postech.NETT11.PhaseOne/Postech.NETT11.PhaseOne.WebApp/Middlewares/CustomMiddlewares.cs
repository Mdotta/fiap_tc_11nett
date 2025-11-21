namespace Postech.NETT11.PhaseOne.WebApp.Middlewares;

public static class CustomMiddlewares
{
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
        return app.Use(async (context, next) =>
        {
            try
            {
                await next.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("An unexpected error occurred, please try again later.");
            }
        });
    }
}