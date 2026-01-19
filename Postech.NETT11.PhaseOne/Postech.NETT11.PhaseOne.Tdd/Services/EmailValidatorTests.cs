using FluentAssertions;
using Postech.NETT11.PhaseOne.Application.Services;
using Postech.NETT11.PhaseOne.Application.Utils;
using Xunit;

namespace Postech.NETT11.PhaseOne.Tests.Services;

public class EmailValidatorTests
{
    [Fact]
    public void Validate_WithValidEmail_ReturnsTrue()
    {
        var (isValid, error) = EmailValidator.Validate("user@example.com");

        isValid.Should().BeTrue();
        error.Should().BeNull();
    }

    [Fact]
    public void Validate_WithInvalidFormat_ReturnsFalse()
    {
        var (isValid, error) = EmailValidator.Validate("userexample.com"); // missing @

        isValid.Should().BeFalse();
        error.Should().NotBeNullOrEmpty();
    }

    [Theory]
    [InlineData("user@.com")] 
    [InlineData("user@com")]  
    [InlineData("user@domain.c")] 
    public void ValidateAndThrow_WithInvalidEmail_ThrowsArgumentException(string email)
    {
        Action act = () => EmailValidator.ValidateAndThrow(email);

        act.Should().Throw<ArgumentException>().WithMessage("Email format is invalid or domain is not allowed.");
    }
}
