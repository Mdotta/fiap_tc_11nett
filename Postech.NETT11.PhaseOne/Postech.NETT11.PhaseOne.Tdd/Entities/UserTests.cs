using FluentAssertions;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;
using Xunit;

namespace Postech.NETT11.PhaseOne.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Should_Create_User_When_Data_Is_Valid()
    {
        // Arrange
        var userHandle = "usuario123";
        var username = "usuario@fiap.com.br";
        var passwordHash = "hash123456";
        var role = UserRole.Client;

        // Act
        var user = new User
        {
            UserHandle = userHandle,
            Username = username,
            PasswordHash = passwordHash,
            Role = role
        };

        // Assert
        user.UserHandle.Should().Be(userHandle);
        user.Username.Should().Be(username);
        user.PasswordHash.Should().Be(passwordHash);
        user.Role.Should().Be(role);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Should_Validate_Email_Is_Required(string email)
    {
        // Arrange & Act
        var user = new User
        {
            UserHandle = "usuario123",
            Username = email!,
            PasswordHash = "hash123456",
            Role = UserRole.Client
        };

        // Assert
        user.Username.Should().Be(email);
    }

    [Theory]
    [InlineData("email@fiap.com.br", true)]
    [InlineData("email@fiap.com", true)]
    [InlineData("email@teste.com.br", true)]
    [InlineData("emailinvalido", false)]
    [InlineData("email@", false)]
    [InlineData("@fiap.com.br", false)]
    [InlineData("email @fiap.com.br", false)]
    public void Should_Validate_Email_Format(string email, bool expectedValid)
    {
        // Arrange & Act
        var isValid = IsValidEmail(email);

        // Assert
        isValid.Should().Be(expectedValid);
    }

    [Theory]
    [InlineData("Senha123!", true)]  
    [InlineData("MinhaSenha@2024", true)]
    [InlineData("Teste#123", true)]
    [InlineData("senha123", false)]  
    [InlineData("SENHA123", false)]  
    [InlineData("SenhaTeste", false)] 
    [InlineData("Senha123", false)] 
    [InlineData("Senha@", false)]  
    [InlineData("12345678", false)] 
    [InlineData("Senha12", false)]  
    [InlineData("", false)]  
    public void Should_Validate_Secure_Password(string password, bool expectedValid)
    {
        // Arrange & Act
        var isValid = IsValidPassword(password);

        // Assert
        isValid.Should().Be(expectedValid);
    }

    [Fact]
    public void Should_Create_User_With_Client_Role_By_Default()
    {
        // Arrange & Act
        var user = new User
        {
            UserHandle = "usuario123",
            Username = "usuario@fiap.com.br",
            PasswordHash = "hash123456",
            Role = UserRole.Client
        };

        // Assert
        user.Role.Should().Be(UserRole.Client);
    }

    [Fact]
    public void Should_Create_User_With_Admin_Role()
    {
        // Arrange & Act
        var user = new User
        {
            UserHandle = "admin123",
            Username = "admin@fiap.com.br",
            PasswordHash = "hash123456",
            Role = UserRole.Admin
        };

        // Assert
        user.Role.Should().Be(UserRole.Admin);
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email && !email.Contains(" ");
        }
        catch
        {
            return false;
        }
    }

    private static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }
}
