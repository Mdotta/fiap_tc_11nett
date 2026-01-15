using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;

namespace Postech.NETT11.PhaseOne.Application.DTOs.Requests.User;

public sealed record CreateUserRequest(
    string UserHandle,
    string Username,
    string Password
);
