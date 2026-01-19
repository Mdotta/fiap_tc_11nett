namespace Postech.NETT11.PhaseOne.Application.DTOs.Requests.User;

public sealed record UpdateOwnProfileRequest(
    string? UserHandle,
    string? Username,
    string? Email
);
