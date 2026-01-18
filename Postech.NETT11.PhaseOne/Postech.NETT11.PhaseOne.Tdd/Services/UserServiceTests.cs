using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Postech.NETT11.PhaseOne.Application.DTOs.Requests.User;
using Postech.NETT11.PhaseOne.Application.DTOs.Responses.User;
using Postech.NETT11.PhaseOne.Application.Services;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;
using Postech.NETT11.PhaseOne.Domain.Common;
using Xunit;

namespace Postech.NETT11.PhaseOne.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly Mock<ILogger<IUserService>> _mockLogger;
    private readonly IUserService _service;

    public UserServiceTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockLogger = new Mock<ILogger<IUserService>>();
        _service = new UserService(_mockRepo.Object, _mockPasswordHasher.Object, _mockLogger.Object);
    }

    private User GetValidUser()
    {
        return new User
        {
            UserHandle = "validhandle",
            Username = "validuser",
            Email = "valid@example.com",
            PasswordHash = "hashedpassword",
            Role = UserRole.Client,
            IsActive = true
        };
    }

    // Create Tests
    [Fact]
    public async Task CreateUser_WithValidData_ShouldReturnCreatedUser()
    {
        // Arrange
        var user = GetValidUser();
        var hashedPassword = "hashedpassword123";
        _mockPasswordHasher.Setup(p => p.HashPassword(It.IsAny<string>())).Returns(hashedPassword);
        _mockRepo.Setup(r => r.UserHandleExistsAsync(It.IsAny<string>(), It.IsAny<Guid?>())).ReturnsAsync(false);
        _mockRepo.Setup(r => r.UsernameExistsAsync(It.IsAny<string>(), It.IsAny<Guid?>())).ReturnsAsync(false);
        _mockRepo.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), It.IsAny<Guid?>())).ReturnsAsync(false);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) =>
        {
            u.PasswordHash = hashedPassword;
            return u;
        });

        var request = new CreateUserRequest(
            user.UserHandle,
            user.Username,
            "valid@example.com",
            "S@fePassword1!"
        );

        // Act
        var result = await _service.CreateUserAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.UserHandle.Should().Be(user.UserHandle);
        result.Username.Should().Be(user.Username);
        result.Email.Should().Be("valid@example.com");
        result.Role.Should().Be(user.Role);
        _mockPasswordHasher.Verify(p => p.HashPassword("S@fePassword1!"), Times.Once);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateUserAsync(null!));
    }
    
    [Fact]
    public async Task CreateUser_ShouldSetDefaultRoleToClient()
    {
        // Arrange
        var user = GetValidUser();
        var hashedPassword = "S@fePassword1!";
        _mockPasswordHasher.Setup(p => p.HashPassword(It.IsAny<string>())).Returns(hashedPassword);
        _mockRepo.Setup(r => r.UsernameExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), null)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) =>
        {
            u.PasswordHash = hashedPassword;
            return u;
        });

        var request = new CreateUserRequest(
            user.UserHandle,
            user.Username,
            "valid@example.com",
            "S@fePassword1!"
        );

        // Act
        var result = await _service.CreateUserAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Role.Should().Be(UserRole.Client);
        _mockRepo.Verify(r => r.AddAsync(It.Is<User>(u => u.Role == UserRole.Client)), Times.Once);
    }
    
    // Read Tests
    [Fact]
    public async Task GetUserById_WithExistingId_ShouldReturnUser()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;

        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _service.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.UserHandle.Should().Be(user.UserHandle);
        result.Username.Should().Be(user.Username);
        result.Role.Should().Be(user.Role);
        _mockRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserById_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetUserByIdAsync(userId);

        // Assert
        result.Should().BeNull();
        _mockRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnListOfUsers()
    {
        // Arrange
        var user1 = GetValidUser();
        var user2 = GetValidUser();
        user2.Username = "anotheruser";
        var users = new List<User> { user1, user2 };

        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = await _service.GetAllUsersAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllBeOfType<UserResponse>();
        _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    // Update Tests
    [Fact]
    public async Task UpdateUser_WithValidData_ShouldReturnUpdatedUser()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;

        var request = new UpdateUserRequest(
            "updatedhandle",
            "updateduser",
            "updated@example.com",
            null,
            UserRole.Admin
        );

        _mockRepo.Setup(r => r.GetByIdIncludingInactiveAsync(userId)).ReturnsAsync(user);
        _mockRepo.Setup(r => r.UserHandleExistsAsync(It.IsAny<string>(), userId)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.EmailExistsAsync(It.IsAny<string>(), userId)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        // Act
        var result = await _service.UpdateUserAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result!.UserHandle.Should().Be("updatedhandle");
        result.Username.Should().Be("updateduser");
        result.Role.Should().Be(UserRole.Admin);
        _mockRepo.Verify(r => r.GetByIdIncludingInactiveAsync(userId), Times.Once);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_WithPasswordUpdate_ShouldHashPassword()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;
        var newHashedPassword = "S@fePassword32!";

        var request = new UpdateUserRequest(
            null,
            null,
            null,
            "S@fePassword1!",
            null
        );

        _mockPasswordHasher.Setup(p => p.HashPassword("S@fePassword1!")).Returns(newHashedPassword);
        _mockRepo.Setup(r => r.GetByIdIncludingInactiveAsync(userId)).ReturnsAsync(user);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        // Act
        var result = await _service.UpdateUserAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        _mockPasswordHasher.Verify(p => p.HashPassword("S@fePassword1!"), Times.Once);
        _mockRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.PasswordHash == newHashedPassword)), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_WithPartialUpdate_ShouldOnlyUpdateProvidedFields()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;
        var originalUsername = user.Username;
        var originalRole = user.Role;

        var request = new UpdateUserRequest(
            "newhandle",
            null,
            null,
            null,
            null
        );

        _mockRepo.Setup(r => r.GetByIdIncludingInactiveAsync(userId)).ReturnsAsync(user);
        _mockRepo.Setup(r => r.UserHandleExistsAsync(It.IsAny<string>(), userId)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        // Act
        var result = await _service.UpdateUserAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result!.UserHandle.Should().Be("newhandle");
        result.Username.Should().Be(originalUsername);
        result.Role.Should().Be(originalRole);
    }

    [Fact]
    public async Task UpdateUser_WithNonExistingId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var request = new UpdateUserRequest("newhandle", null, null, null, null);

        _mockRepo.Setup(r => r.GetByIdIncludingInactiveAsync(userId)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateUserAsync(userId, request));
        _mockRepo.Verify(r => r.GetByIdIncludingInactiveAsync(userId), Times.Once);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateUser_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateUserAsync(userId, null!));
    }

    // Delete Tests
    [Fact]
    public async Task DeleteUser_WithExistingId_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;
        user.IsActive = true;
        user.Role = UserRole.Client;

        _mockRepo.Setup(r => r.GetByIdIncludingInactiveAsync(userId)).ReturnsAsync(user);
        _mockRepo.Setup(r => r.CountAdminsAsync()).ReturnsAsync(1);
        
        // Mock DeleteAsync to simulate logical delete (sets IsActive to false)
        _mockRepo.Setup(r => r.DeleteAsync(userId))
            .ReturnsAsync((Guid id) =>
            {
                if (user.Id == id && user.IsActive)
                {
                    user.IsActive = false;
                    return true;
                }
                return false;
            });

        // Act
        var result = await _service.DeleteUserAsync(userId);

        // Assert
        result.Should().BeTrue();
        user.IsActive.Should().BeFalse();
        _mockRepo.Verify(r => r.GetByIdIncludingInactiveAsync(userId), Times.Once);
        _mockRepo.Verify(r => r.DeleteAsync(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_WithNonExistingId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        _mockRepo.Setup(r => r.GetByIdIncludingInactiveAsync(userId)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.DeleteUserAsync(userId));
        _mockRepo.Verify(r => r.GetByIdIncludingInactiveAsync(userId), Times.Once);
        _mockRepo.Verify(r => r.DeleteAsync(userId), Times.Never);
    }

    [Fact]
    public async Task DeleteUser_WithAlreadyDeletedUser_ShouldThrowDomainException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;
        user.IsActive = false;

        _mockRepo.Setup(r => r.GetByIdIncludingInactiveAsync(userId)).ReturnsAsync(user);
        _mockRepo.Setup(r => r.DeleteAsync(userId))
            .ThrowsAsync(new DomainException("User is already deleted"));

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _service.DeleteUserAsync(userId));
        _mockRepo.Verify(r => r.GetByIdIncludingInactiveAsync(userId), Times.Once);
        _mockRepo.Verify(r => r.DeleteAsync(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_WithSelfDeletion_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.DeleteUserAsync(userId, userId));
        _mockRepo.Verify(r => r.GetByIdIncludingInactiveAsync(It.IsAny<Guid>()), Times.Never);
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUser_WithLastAdmin_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;
        user.Role = UserRole.Admin;

        _mockRepo.Setup(r => r.GetByIdIncludingInactiveAsync(userId)).ReturnsAsync(user);
        _mockRepo.Setup(r => r.CountAdminsAsync()).ReturnsAsync(1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.DeleteUserAsync(userId));
        _mockRepo.Verify(r => r.GetByIdIncludingInactiveAsync(userId), Times.Once);
        _mockRepo.Verify(r => r.CountAdminsAsync(), Times.Once);
        _mockRepo.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteUser_WithAdminAndOtherAdminsExist_ShouldDeleteSuccessfully()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;
        user.Role = UserRole.Admin;

        _mockRepo.Setup(r => r.GetByIdIncludingInactiveAsync(userId)).ReturnsAsync(user);
        _mockRepo.Setup(r => r.CountAdminsAsync()).ReturnsAsync(2);
        _mockRepo.Setup(r => r.DeleteAsync(userId)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteUserAsync(userId);

        // Assert
        result.Should().BeTrue();
        _mockRepo.Verify(r => r.GetByIdIncludingInactiveAsync(userId), Times.Once);
        _mockRepo.Verify(r => r.CountAdminsAsync(), Times.Once);
        _mockRepo.Verify(r => r.DeleteAsync(userId), Times.Once);
    }

    // UpdateOwnProfile Tests
    [Fact]
    public async Task UpdateOwnProfile_WithValidData_ShouldReturnUpdatedUser()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;

        var request = new UpdateOwnProfileRequest(
            "newhandle",
            "newusername",
            "newemail@example.com"
        );

        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockRepo.Setup(r => r.UserHandleExistsAsync("newhandle", userId)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.UsernameExistsAsync("newusername", userId)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.EmailExistsAsync("newemail@example.com", userId)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        // Act
        var result = await _service.UpdateOwnProfileAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.UserHandle.Should().Be("newhandle");
        result.Username.Should().Be("newusername");
        result.Email.Should().Be("newemail@example.com");
        _mockRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateOwnProfile_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _service.UpdateOwnProfileAsync(userId, null!));
    }

    [Fact]
    public async Task UpdateOwnProfile_WithNonExistingId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var request = new UpdateOwnProfileRequest("newhandle", null, null);

        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.UpdateOwnProfileAsync(userId, request));
        _mockRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateOwnProfile_WithEmptyUserHandle_ShouldThrowArgumentException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;

        var request = new UpdateOwnProfileRequest("   ", null, null);

        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.UpdateOwnProfileAsync(userId, request));
        _mockRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateOwnProfile_WithSameUserHandle_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;
        user.UserHandle = "existinghandle";

        var request = new UpdateOwnProfileRequest("existinghandle", null, null);

        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.UpdateOwnProfileAsync(userId, request));
        exception.Message.Should().Contain("already set to");
        _mockRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateOwnProfile_WithDuplicateUserHandle_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;

        var request = new UpdateOwnProfileRequest("duplicatehandle", null, null);

        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockRepo.Setup(r => r.UserHandleExistsAsync("duplicatehandle", userId)).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.UpdateOwnProfileAsync(userId, request));
        exception.Message.Should().Contain("already in use");
        _mockRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task UpdateOwnProfile_WithPartialUpdate_ShouldOnlyUpdateProvidedFields()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;
        var originalEmail = user.Email;

        var request = new UpdateOwnProfileRequest("newhandle", null, null);

        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockRepo.Setup(r => r.UserHandleExistsAsync("newhandle", userId)).ReturnsAsync(false);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        // Act
        var result = await _service.UpdateOwnProfileAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result.UserHandle.Should().Be("newhandle");
        result.Email.Should().Be(originalEmail);
        _mockRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.UserHandle == "newhandle" && u.Email == originalEmail)), Times.Once);
    }

    // ChangePassword Tests
    [Fact]
    public async Task ChangePassword_WithValidData_ShouldChangePassword()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;
        user.PasswordHash = "oldhashedpassword";

        var request = new ChangePasswordRequest(
            "OldPassword123!",
            "NewPassword123!"
        );

        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockPasswordHasher.Setup(p => p.VerifyPassword("OldPassword123!", "oldhashedpassword")).Returns(true);
        _mockPasswordHasher.Setup(p => p.VerifyPassword("NewPassword123!", "oldhashedpassword")).Returns(false);
        _mockPasswordHasher.Setup(p => p.HashPassword("NewPassword123!")).Returns("newhashedpassword");
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        // Act
        await _service.ChangePasswordAsync(userId, request);

        // Assert
        _mockPasswordHasher.Verify(p => p.VerifyPassword("OldPassword123!", "oldhashedpassword"), Times.Once);
        _mockPasswordHasher.Verify(p => p.VerifyPassword("NewPassword123!", "oldhashedpassword"), Times.Once);
        _mockPasswordHasher.Verify(p => p.HashPassword("NewPassword123!"), Times.Once);
        _mockRepo.Verify(r => r.UpdateAsync(It.Is<User>(u => u.PasswordHash == "newhashedpassword")), Times.Once);
    }

    [Fact]
    public async Task ChangePassword_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => 
            _service.ChangePasswordAsync(userId, null!));
    }

    [Fact]
    public async Task ChangePassword_WithNonExistingId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var request = new ChangePasswordRequest("OldPassword123!", "NewPassword123!");

        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.ChangePasswordAsync(userId, request));
        _mockRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ChangePassword_WithIncorrectCurrentPassword_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;
        user.PasswordHash = "oldhashedpassword";

        var request = new ChangePasswordRequest(
            "WrongPassword123!",
            "NewPassword123!"
        );

        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockPasswordHasher.Setup(p => p.VerifyPassword("WrongPassword123!", "oldhashedpassword")).Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.ChangePasswordAsync(userId, request));
        exception.Message.Should().Contain("Current password is incorrect");
        _mockPasswordHasher.Verify(p => p.VerifyPassword("WrongPassword123!", "oldhashedpassword"), Times.Once);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task ChangePassword_WithSameNewPassword_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;
        user.PasswordHash = "oldhashedpassword";

        var request = new ChangePasswordRequest(
            "OldPassword123!",
            "OldPassword123!"
        );

        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockPasswordHasher.Setup(p => p.VerifyPassword("OldPassword123!", "oldhashedpassword")).Returns(true);
        _mockPasswordHasher.Setup(p => p.VerifyPassword("OldPassword123!", "oldhashedpassword")).Returns(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
            _service.ChangePasswordAsync(userId, request));
        exception.Message.Should().Contain("New password must be different");
        _mockPasswordHasher.Verify(p => p.VerifyPassword("OldPassword123!", "oldhashedpassword"), Times.Exactly(2));
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    // ReactivateUser Tests
    [Fact]
    public async Task ReactivateUser_WithExistingInactiveUser_ShouldReturnTrue()
    {
        // Arrange
        var userId = Guid.CreateVersion7();

        _mockRepo.Setup(r => r.ReactivateUserAsync(userId)).ReturnsAsync(true);

        // Act
        var result = await _service.ReactivateUserAsync(userId);

        // Assert
        result.Should().BeTrue();
        _mockRepo.Verify(r => r.ReactivateUserAsync(userId), Times.Once);
    }

    [Fact]
    public async Task ReactivateUser_WithNonExistingUser_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.CreateVersion7();

        _mockRepo.Setup(r => r.ReactivateUserAsync(userId)).ReturnsAsync(false);

        // Act
        var result = await _service.ReactivateUserAsync(userId);

        // Assert
        result.Should().BeFalse();
        _mockRepo.Verify(r => r.ReactivateUserAsync(userId), Times.Once);
    }
}
