using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
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
    
}