using FluentAssertions;
using Moq;
using Postech.NETT11.PhaseOne.Application.DTOs.Requests.User;
using Postech.NETT11.PhaseOne.Application.DTOs.Responses.User;
using Postech.NETT11.PhaseOne.Application.Services;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;
using Xunit;

namespace Postech.NETT11.PhaseOne.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly Mock<IPasswordHasher> _mockPasswordHasher;
    private readonly IUserService _service;

    public UserServiceTests()
    {
        _mockRepo = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _service = new UserService(_mockRepo.Object, _mockPasswordHasher.Object);
    }

    private User GetValidUser()
    {
        return new User
        {
            UserHandle = "validhandle",
            Username = "validuser",
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
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) =>
        {
            u.PasswordHash = hashedPassword;
            return u;
        });

        var request = new CreateUserRequest(
            user.UserHandle,
            user.Username,
            "plainpassword",
            user.Role
        );

        // Act
        var result = await _service.CreateUserAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.UserHandle.Should().Be(user.UserHandle);
        result.Username.Should().Be(user.Username);
        result.Role.Should().Be(user.Role);
        _mockPasswordHasher.Verify(p => p.HashPassword("plainpassword"), Times.Once);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_WithNullRequest_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.CreateUserAsync(null!));
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
            null,
            UserRole.Admin
        );

        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        // Act
        var result = await _service.UpdateUserAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        result!.UserHandle.Should().Be("updatedhandle");
        result.Username.Should().Be("updateduser");
        result.Role.Should().Be(UserRole.Admin);
        _mockRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUser_WithPasswordUpdate_ShouldHashPassword()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;
        var newHashedPassword = "newhashedpassword123";

        var request = new UpdateUserRequest(
            null,
            null,
            "newpassword",
            null
        );

        _mockPasswordHasher.Setup(p => p.HashPassword("newpassword")).Returns(newHashedPassword);
        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);

        // Act
        var result = await _service.UpdateUserAsync(userId, request);

        // Assert
        result.Should().NotBeNull();
        _mockPasswordHasher.Verify(p => p.HashPassword("newpassword"), Times.Once);
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
            null
        );

        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);
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
    public async Task UpdateUser_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var request = new UpdateUserRequest("newhandle", null, null, null);

        _mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await _service.UpdateUserAsync(userId, request);

        // Assert
        result.Should().BeNull();
        _mockRepo.Verify(r => r.GetByIdAsync(userId), Times.Once);
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
        _mockRepo.Verify(r => r.DeleteAsync(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_WithNonExistingId_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        _mockRepo.Setup(r => r.DeleteAsync(userId)).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteUserAsync(userId);

        // Assert
        result.Should().BeFalse();
        _mockRepo.Verify(r => r.DeleteAsync(userId), Times.Once);
    }

    [Fact]
    public async Task DeleteUser_WithAlreadyDeletedUser_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var user = GetValidUser();
        user.Id = userId;
        user.IsActive = false;

        _mockRepo.Setup(r => r.DeleteAsync(userId))
            .ThrowsAsync(new Exception("User is already deleted"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _service.DeleteUserAsync(userId));
        _mockRepo.Verify(r => r.DeleteAsync(userId), Times.Once);
    }
}
