using Microsoft.Extensions.Logging;
using Postech.NETT11.PhaseOne.Application.DTOs.Requests.User;
using Postech.NETT11.PhaseOne.Application.DTOs.Responses.User;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;
using Postech.NETT11.PhaseOne.Application.Utils;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;

namespace Postech.NETT11.PhaseOne.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<IUserService> _logger;

    public UserService(
        IUserRepository userRepository, 
        IPasswordHasher passwordHasher,
        ILogger<IUserService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToResponse);
    }

    public async Task<UserResponse?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found.", id);
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }
        
        return MapToResponse(user);
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        if (string.IsNullOrWhiteSpace(request.UserHandle))
            throw new ArgumentException("UserHandle cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(request.Username))
            throw new ArgumentException("Username cannot be empty.");
        
        if (string.IsNullOrWhiteSpace(request.Email))
            throw new ArgumentException("Email cannot be empty.");
        
        PasswordValidator.ValidateAndThrow(request.Password);
        
        EmailValidator.ValidateAndThrow(request.Email);
        
        var userHandleInUse = await _userRepository.UserHandleExistsAsync(request.UserHandle);
        if (userHandleInUse)
            throw new InvalidOperationException($"UserHandle '{request.UserHandle}' is already in use.");
        
        var usernameInUse = await _userRepository.UsernameExistsAsync(request.Username);
        if (usernameInUse)
            throw new InvalidOperationException($"Username '{request.Username}' is already in use.");
        
        var emailInUse = await _userRepository.EmailExistsAsync(request.Email);
        if (emailInUse)
            throw new InvalidOperationException($"Email '{request.Email}' is already in use.");
        
        var user = new User
        {
            UserHandle = request.UserHandle,
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password)
        };

        var createdUser = await _userRepository.AddAsync(user);
        
        _logger.LogDebug("Created new user with ID {UserId}.", createdUser.Id);
        
        return MapToResponse(createdUser);
    }

    public async Task<UserResponse> UpdateUserAsync(Guid id, UpdateUserRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var user = await _userRepository.GetByIdIncludingInactiveAsync(id);
        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found for update.", id);
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }
        
        if (request.UserHandle != null)
        {
            if (string.IsNullOrWhiteSpace(request.UserHandle))
                throw new ArgumentException("UserHandle cannot be empty.");
            
            if (user.UserHandle == request.UserHandle)
                throw new InvalidOperationException($"UserHandle is already set to '{request.UserHandle}'. No changes needed.");
            
            var userHandleInUse = await _userRepository.UserHandleExistsAsync(request.UserHandle, id);
            if (userHandleInUse)
                throw new InvalidOperationException($"UserHandle '{request.UserHandle}' is already in use.");
            
            user.UserHandle = request.UserHandle;
        }

        if (request.Username != null)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                throw new ArgumentException("Username cannot be empty.");
            
            if (user.Username == request.Username)
                throw new InvalidOperationException($"Username is already set to '{request.Username}'. No changes needed.");
            
            var usernameInUse = await _userRepository.UsernameExistsAsync(request.Username, id);
            if (usernameInUse)
                throw new InvalidOperationException($"Username '{request.Username}' is already in use.");
            
            user.Username = request.Username;
        }
        
        if (request.Email != null)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email cannot be empty.");
            
            EmailValidator.ValidateAndThrow(request.Email);
            
            if (user.Email == request.Email)
                throw new InvalidOperationException($"Email is already set to '{request.Email}'. No changes needed.");
            
            var emailInUse = await _userRepository.EmailExistsAsync(request.Email, id);
            if (emailInUse)
                throw new InvalidOperationException($"Email '{request.Email}' is already in use.");
            
            user.Email = request.Email;
        }
        
        if (request.Password != null)
        {
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Password cannot be empty.");
            
            PasswordValidator.ValidateAndThrow(request.Password);
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
        }
        
        if (request.Role.HasValue)
            user.Role = request.Role.Value;

        var updatedUser = await _userRepository.UpdateAsync(user);
        
        _logger.LogDebug("Updated user with ID {UserId}.", id);
        
        return MapToResponse(updatedUser);
    }

    public async Task<UserResponse> UpdateOwnProfileAsync(Guid userId, UpdateOwnProfileRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found for profile update.", userId);
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }
        
        if (request.UserHandle != null)
        {
            if (string.IsNullOrWhiteSpace(request.UserHandle))
                throw new ArgumentException("UserHandle cannot be empty.");
            
            if (user.UserHandle == request.UserHandle)
                throw new InvalidOperationException($"UserHandle is already set to '{request.UserHandle}'. No changes needed.");
            
            var userHandleInUse = await _userRepository.UserHandleExistsAsync(request.UserHandle, userId);
            if (userHandleInUse)
                throw new InvalidOperationException($"UserHandle '{request.UserHandle}' is already in use.");
            
            user.UserHandle = request.UserHandle;
        }

        if (request.Username != null)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                throw new ArgumentException("Username cannot be empty.");
            
            if (user.Username == request.Username)
                throw new InvalidOperationException($"Username is already set to '{request.Username}'. No changes needed.");
            
            var usernameInUse = await _userRepository.UsernameExistsAsync(request.Username, userId);
            if (usernameInUse)
                throw new InvalidOperationException($"Username '{request.Username}' is already in use.");
            
            user.Username = request.Username;
        }
        
        if (request.Email != null)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email cannot be empty.");
            
            EmailValidator.ValidateAndThrow(request.Email);
            
            if (user.Email == request.Email)
                throw new InvalidOperationException($"Email is already set to '{request.Email}'. No changes needed.");
            
            var emailInUse = await _userRepository.EmailExistsAsync(request.Email, userId);
            if (emailInUse)
                throw new InvalidOperationException($"Email '{request.Email}' is already in use.");
            
            user.Email = request.Email;
        }

        var updatedUser = await _userRepository.UpdateAsync(user);
        
        _logger.LogDebug("Updated own profile for user with ID {UserId}.", userId);
        
        return MapToResponse(updatedUser);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found for password change.", userId);
            throw new KeyNotFoundException($"User with ID {userId} not found.");
        }
        
        if (!_passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            throw new InvalidOperationException("Current password is incorrect.");
        
        PasswordValidator.ValidateAndThrow(request.NewPassword);
        
        if (_passwordHasher.VerifyPassword(request.NewPassword, user.PasswordHash))
            throw new InvalidOperationException("New password must be different from the current password.");
        
        user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
        await _userRepository.UpdateAsync(user);
        
        _logger.LogDebug("Changed password for user with ID {UserId}.", userId);
    }

    public async Task<bool> DeleteUserAsync(Guid id, Guid? currentUserId = null)
    {
        if (currentUserId.HasValue && id == currentUserId.Value)
            throw new InvalidOperationException("Users cannot delete their own account.");
        
        var user = await _userRepository.GetByIdIncludingInactiveAsync(id);
        if (user is null)
        {
            _logger.LogWarning("User with ID {UserId} not found for deletion.", id);
            throw new KeyNotFoundException($"User with ID {id} not found.");
        }
        
        if (user.Role == UserRole.Admin)
        {
            var adminCount = await _userRepository.CountAdminsAsync();
            if (adminCount <= 1)
                throw new InvalidOperationException("Cannot delete the last administrator. At least one admin must remain in the system.");
        }

        var deleted = await _userRepository.DeleteAsync(id);
        
        _logger.LogDebug("Deleted user with ID {UserId}.", id);
        
        return deleted;
    }

    public async Task<bool> ReactivateUserAsync(Guid id)
    {
        var reactivated = await _userRepository.ReactivateUserAsync(id);
        
        if (!reactivated)
        {
            _logger.LogWarning("User with ID {UserId} not found for reactivation.", id);
        }
        else
        {
            _logger.LogDebug("Reactivated user with ID {UserId}.", id);
        }
        
        return reactivated;
    }

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse(
            user.Id,
            user.UserHandle,
            user.Username,
            user.Email,
            user.Role,
            user.CreatedAt
        );
    }
}
