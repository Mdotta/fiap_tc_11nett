using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;

namespace Postech.NETT11.PhaseOne.Application.DTOs.Responses.User;

public sealed record UserResponse(
    Guid Id,
    string UserHandle,
    string Username,
    string Email,
    UserRole Role,
    DateTime CreatedAt
);
