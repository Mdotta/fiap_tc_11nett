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

    [Fact]
    public void Validate_WithDisallowedDomain_ReturnsFalse()
    {
        var (isValid, error) = EmailValidator.Validate("user@example.io"); // .io is not allowed by the regex

        isValid.Should().BeFalse();
        error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ValidateAndThrow_WithInvalidEmail_ThrowsArgumentException()
    {
        Action act = () => EmailValidator.ValidateAndThrow("bad@domain.io");

        act.Should().Throw<ArgumentException>().WithMessage("Email format is invalid or domain is not allowed.");
    }
}
