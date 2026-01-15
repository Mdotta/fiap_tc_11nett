using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;

namespace Postech.NETT11.PhaseOne.Application.DTOs.Responses.Auth;

public sealed record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    string TokenType,
    AuthUserData User
);

public sealed record AuthUserData(
    Guid Id,
    string Username,
    UserRole Role
);

