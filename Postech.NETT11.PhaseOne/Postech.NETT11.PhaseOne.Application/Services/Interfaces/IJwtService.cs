namespace Postech.NETT11.PhaseOne.Application.Services.Interfaces;

public interface IJwtService
{
    public string GenerateToken(string userId, string userName);
}