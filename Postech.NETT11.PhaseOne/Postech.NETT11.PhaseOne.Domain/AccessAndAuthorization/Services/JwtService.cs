using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Services;

namespace Postech.NETT11.PhaseOne.WebApp.Services.Auth;

public class JwtService(IConfiguration configuration):IJwtService
{
    public string GenerateToken(string userId, string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,userId),
            new Claim(ClaimTypes.Role,role),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        };
        
        var jwtKey = configuration["Jwt:Key"];
        ArgumentException.ThrowIfNullOrEmpty(jwtKey);
        var expiresInMinutes = configuration["Jwt:ExpiresInMinutes"];
        ArgumentException.ThrowIfNullOrEmpty(expiresInMinutes);
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(int.Parse(expiresInMinutes)),
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}