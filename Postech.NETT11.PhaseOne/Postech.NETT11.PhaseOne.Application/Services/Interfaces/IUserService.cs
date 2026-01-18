using Postech.NETT11.PhaseOne.Application.DTOs.Requests.User;
using Postech.NETT11.PhaseOne.Application.DTOs.Responses.User;

namespace Postech.NETT11.PhaseOne.Application.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserResponse>> GetAllUsersAsync();
    Task<UserResponse?> GetUserByIdAsync(Guid id);
    Task<UserResponse> CreateUserAsync(CreateUserRequest request);
    Task<UserResponse> UpdateUserAsync(Guid id, UpdateUserRequest request);
    Task<UserResponse> UpdateOwnProfileAsync(Guid userId, UpdateOwnProfileRequest request);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    Task<bool> DeleteUserAsync(Guid id, Guid? currentUserId = null);
    Task<bool> ReactivateUserAsync(Guid id);
}
