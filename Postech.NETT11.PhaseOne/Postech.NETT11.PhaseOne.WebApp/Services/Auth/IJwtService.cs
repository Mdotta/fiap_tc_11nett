namespace Postech.NETT11.PhaseOne.WebApp.Services.Auth;

public interface IJwtService
{
    public string GenerateToken(string userId, string userName);
}