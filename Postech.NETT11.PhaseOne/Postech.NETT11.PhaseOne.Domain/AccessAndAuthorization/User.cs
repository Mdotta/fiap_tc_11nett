using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;
using Postech.NETT11.PhaseOne.Domain.Entities;

namespace Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;

public class User:BaseEntity
{
    public required string UserHandle { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; } = UserRole.Client;
    public bool IsActive { get; set; } = true;
}