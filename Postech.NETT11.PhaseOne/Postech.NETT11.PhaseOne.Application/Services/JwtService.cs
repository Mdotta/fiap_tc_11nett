using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Postech.NETT11.PhaseOne.Application.DTOs.Responses.Auth;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;

namespace Postech.NETT11.PhaseOne.Application.Services;

public class JwtService(IConfiguration configuration):IJwtService
{
    public TokenData GenerateToken(string userId, string role)
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
        
        var expiresAt = DateTime.UtcNow.AddMinutes(int.Parse(expiresInMinutes));
        
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);
        
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        
        return new TokenData(tokenString, expiresAt);
    }
}