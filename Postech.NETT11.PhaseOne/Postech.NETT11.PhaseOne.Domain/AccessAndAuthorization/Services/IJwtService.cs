namespace Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Services;

public interface IJwtService
{
    public string GenerateToken(string userId, string userName);
}