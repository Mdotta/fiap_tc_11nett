using Postech.NETT11.PhaseOne.Application.DTOs.Requests.User;
using Postech.NETT11.PhaseOne.Application.DTOs.Responses.User;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;

namespace Postech.NETT11.PhaseOne.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToResponse);
    }

    public async Task<UserResponse?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : MapToResponse(user);
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        //TODO: validar segurança da senha
        //TODO: allow anonymous create user
        ArgumentNullException.ThrowIfNull(request);
        
        var usernameInUse = await _userRepository.UsernameExistsAsync(request.Username);
        if (usernameInUse)
            throw new InvalidOperationException($"Username '{request.Username}' is already in use.");
        
        var user = new User
        {
            UserHandle = request.UserHandle,
            Username = request.Username,
            PasswordHash = _passwordHasher.HashPassword(request.Password)
        };

        var createdUser = await _userRepository.AddAsync(user);
        return MapToResponse(createdUser);
    }

    public async Task<UserResponse?> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return null;
        
        if (request.UserHandle != null)
            user.UserHandle = request.UserHandle;

        if (request.Username != null)
        {
            var usernameInUse = await _userRepository.UsernameExistsAsync(request.Username, id);
            if (usernameInUse)
                throw new InvalidOperationException($"Username '{request.Username}' is already in use.");
            
            user.Username = request.Username;
        }
        
        if (request.Password != null)
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
        
        if (request.Role.HasValue)
            user.Role = request.Role.Value;

        var updatedUser = await _userRepository.UpdateAsync(user);
        return MapToResponse(updatedUser);
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    private static UserResponse MapToResponse(User user)
    {
        //TODO: incorporar email com validação
        return new UserResponse(
            user.Id,
            user.UserHandle,
            user.Username,
            user.Role,
            user.CreatedAt
        );
    }
}
