using FluentAssertions;
using Postech.NETT11.PhaseOne.Application.Utils;
using Xunit;

namespace Postech.NETT11.PhaseOne.Tests.Services.Utils;

public class PasswordValidatorTests
{
    [Theory]
    [InlineData("Abc123!@", true, null)]
    [InlineData("MyP@ssw0rd", true, null)]
    [InlineData("C0mpl3x!Pass", true, null)]
    [InlineData("Test123!@#", true, null)]
    public void Validate_WithValidPassword_ShouldReturnTrue(string password, bool expectedIsValid, string? expectedError)
    {
        // Act
        var (isValid, errorMessage) = PasswordValidator.Validate(password);

        // Assert
        isValid.Should().Be(expectedIsValid);
        errorMessage.Should().Be(expectedError);
    }

    [Theory]
    [InlineData("", false, "Password cannot be empty.")]
    [InlineData("   ", false, "Password cannot be empty.")]
    public void Validate_WithEmptyPassword_ShouldReturnFalse(string password, bool expectedIsValid, string expectedError)
    {
        // Act
        var (isValid, errorMessage) = PasswordValidator.Validate(password);

        // Assert
        isValid.Should().Be(expectedIsValid);
        errorMessage.Should().Be(expectedError);
    }

    [Fact]
    public void Validate_WithNullPassword_ShouldReturnFalse()
    {
        // Arrange
        string? password = null;

        // Act
        var (isValid, errorMessage) = PasswordValidator.Validate(password!);

        // Assert
        isValid.Should().BeFalse();
        errorMessage.Should().Be("Password cannot be empty.");
    }

    [Fact]
    public void Validate_WithPasswordTooShort_ShouldReturnFalse()
    {
        // Arrange
        var password = "Abc1!";

        // Act
        var (isValid, errorMessage) = PasswordValidator.Validate(password);

        // Assert
        isValid.Should().BeFalse();
        errorMessage.Should().Be("Password must be at least 8 characters long.");
    }

    [Fact]
    public void Validate_WithoutLowercase_ShouldReturnFalse()
    {
        // Arrange
        var password = "ABCDEF123!";

        // Act
        var (isValid, errorMessage) = PasswordValidator.Validate(password);

        // Assert
        isValid.Should().BeFalse();
        errorMessage.Should().Be("Password must contain at least one lowercase letter.");
    }

    [Fact]
    public void Validate_WithoutUppercase_ShouldReturnFalse()
    {
        // Arrange
        var password = "abcdef123!";

        // Act
        var (isValid, errorMessage) = PasswordValidator.Validate(password);

        // Assert
        isValid.Should().BeFalse();
        errorMessage.Should().Be("Password must contain at least one uppercase letter.");
    }

    [Fact]
    public void Validate_WithoutNumber_ShouldReturnFalse()
    {
        // Arrange
        var password = "Abcdefgh!";

        // Act
        var (isValid, errorMessage) = PasswordValidator.Validate(password);

        // Assert
        isValid.Should().BeFalse();
        errorMessage.Should().Be("Password must contain at least one number.");
    }

    [Fact]
    public void Validate_WithoutSpecialCharacter_ShouldReturnFalse()
    {
        // Arrange
        var password = "Abcdef123";

        // Act
        var (isValid, errorMessage) = PasswordValidator.Validate(password);

        // Assert
        isValid.Should().BeFalse();
        errorMessage.Should().Be("Password must contain at least one special character.");
    }

    [Fact]
    public void ValidateAndThrow_WithValidPassword_ShouldNotThrow()
    {
        // Arrange
        var password = "MyP@ssw0rd";

        // Act
        Action act = () => PasswordValidator.ValidateAndThrow(password);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void ValidateAndThrow_WithInvalidPassword_ShouldThrowArgumentException()
    {
        // Arrange
        var password = "weak";

        // Act
        Action act = () => PasswordValidator.ValidateAndThrow(password);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Password must be at least 8 characters long.");
    }

    [Theory]
    [InlineData("Test!@#$%^&*()")]
    [InlineData("Test,.?\"':;{}")]
    [InlineData("Test|<>[]\\/_")]
    [InlineData("Test+=~`123")]
    public void Validate_WithVariousSpecialCharacters_ShouldReturnTrue(string basePassword)
    {
        // Arrange
        var password = "Abc123" + basePassword;

        // Act
        var (isValid, errorMessage) = PasswordValidator.Validate(password);

        // Assert
        isValid.Should().BeTrue();
        errorMessage.Should().BeNull();
    }
}

