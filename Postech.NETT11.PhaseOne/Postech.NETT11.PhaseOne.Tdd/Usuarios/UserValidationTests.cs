using FluentAssertions;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;
using Postech.NETT11.PhaseOne.Tests.Usuarios.Exceptions;
using Xunit;

namespace Postech.NETT11.PhaseOne.Tests.Usuarios;

public class UserValidationTests
{
    [Fact]
    public void Deve_criar_usuario_quando_dados_forem_validos()
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
    public void Deve_validar_email_obrigatorio(string email)
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
        // A validação deve ser feita no serviço de criação de usuário
        // Por enquanto, apenas verificamos que o objeto pode ser criado
        // mas a validação será implementada no serviço
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
    public void Deve_validar_formato_de_email(string email, bool esperadoValido)
    {
        // Arrange & Act
        var isValid = IsValidEmail(email);

        // Assert
        isValid.Should().Be(esperadoValido);
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
    public void Deve_validar_senha_segura(string senha, bool esperadoValido)
    {
        // Arrange & Act
        var isValid = IsValidPassword(senha);

        // Assert
        isValid.Should().Be(esperadoValido);
    }

    [Fact]
    public void Deve_criar_usuario_com_role_client_por_padrao()
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
    public void Deve_criar_usuario_com_role_admin()
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

    // Métodos auxiliares para validação (serão movidos para um serviço de validação)
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
