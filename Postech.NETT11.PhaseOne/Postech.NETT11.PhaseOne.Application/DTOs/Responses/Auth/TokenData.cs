namespace Postech.NETT11.PhaseOne.Application.DTOs.Responses.Auth;

public sealed record TokenData(
    string Token,
    DateTime ExpiresAt
);

