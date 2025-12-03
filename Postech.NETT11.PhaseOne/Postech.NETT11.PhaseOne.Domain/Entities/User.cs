using Postech.NETT11.PhaseOne.Domain.Enums;

namespace Postech.NETT11.PhaseOne.Domain.Entities;

public class User:BaseEntity
{
    public required string UserHandle { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; }
}