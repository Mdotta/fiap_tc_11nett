using Postech.NETT11.PhaseOne.Application.DTOs.Responses.Auth;

namespace Postech.NETT11.PhaseOne.Application.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> AuthenticateAsync(string username, string password, string? ipAddress = null);
}

