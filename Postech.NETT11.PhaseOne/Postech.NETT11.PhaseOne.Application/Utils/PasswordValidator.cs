using System.Text.RegularExpressions;

namespace Postech.NETT11.PhaseOne.Application.Utils;

public static class PasswordValidator
{
    private const int MinimumLength = 8;
    
    public static (bool IsValid, string? ErrorMessage) Validate(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return (false, "Password cannot be empty.");
        }

        if (password.Length < MinimumLength)
        {
            return (false, $"Password must be at least {MinimumLength} characters long.");
        }

        if (!Regex.IsMatch(password, @"[a-z]"))
        {
            return (false, "Password must contain at least one lowercase letter.");
        }

        if (!Regex.IsMatch(password, @"[A-Z]"))
        {
            return (false, "Password must contain at least one uppercase letter.");
        }

        if (!Regex.IsMatch(password, @"\d"))
        {
            return (false, "Password must contain at least one number.");
        }

        if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""':;{}|<>[\]\\/_+=\-~`]"))
        {
            return (false, "Password must contain at least one special character.");
        }

        return (true, null);
    }

    public static void ValidateAndThrow(string password)
    {
        var (isValid, errorMessage) = Validate(password);
        if (!isValid)
        {
            throw new ArgumentException(errorMessage);
        }
    }
}

