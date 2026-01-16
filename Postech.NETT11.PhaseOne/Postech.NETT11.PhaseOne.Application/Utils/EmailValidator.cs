using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Postech.NETT11.PhaseOne.Application.Utils;

public static class EmailValidator
{
    public static (bool IsValid, string? ErrorMessage) Validate(string email)
    {
        string regex = @"^[^@\s]+@[^@\s]+\.(com|net|org|gov)$";
        var isValid = Regex.IsMatch(email, regex, RegexOptions.IgnoreCase);
        if (isValid)
        {
            return (true, null);
        }
        return (false, "Email format is invalid or domain is not allowed.");
    }

    public static void ValidateAndThrow(string email)
    {
        var (isValid, errorMessage) = Validate(email);
        if (!isValid)
        {
            throw new ArgumentException(errorMessage);
        }
    }
}

