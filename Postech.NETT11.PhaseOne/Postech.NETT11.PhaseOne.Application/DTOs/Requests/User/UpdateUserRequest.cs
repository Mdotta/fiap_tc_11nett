using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;

namespace Postech.NETT11.PhaseOne.Application.DTOs.Requests.User;

public sealed record UpdateUserRequest(
    string? UserHandle,
    string? Username,
    string? Password,
    UserRole? Role
);
