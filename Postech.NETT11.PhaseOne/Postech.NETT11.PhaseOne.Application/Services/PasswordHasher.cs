using BCrypt.Net;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;

namespace Postech.NETT11.PhaseOne.Application.Services;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
