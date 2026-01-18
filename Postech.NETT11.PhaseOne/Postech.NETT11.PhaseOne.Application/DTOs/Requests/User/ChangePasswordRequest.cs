namespace Postech.NETT11.PhaseOne.Application.DTOs.Requests.User;

public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);
