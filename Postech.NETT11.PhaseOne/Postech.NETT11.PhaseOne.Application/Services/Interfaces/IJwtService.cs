using Postech.NETT11.PhaseOne.Application.DTOs.Responses.Auth;

namespace Postech.NETT11.PhaseOne.Application.Services.Interfaces;

public interface IJwtService
{
    TokenData GenerateToken(string userId, string role);
}