using FluentAssertions;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;
using Postech.NETT11.PhaseOne.Tests.Compras.Entities;
using Postech.NETT11.PhaseOne.Tests.Compras.Exceptions;
using Postech.NETT11.PhaseOne.Tests.Jogos.Entities;
using Postech.NETT11.PhaseOne.Tests.Jogos.Infra;
using Xunit;

namespace Postech.NETT11.PhaseOne.Tests.Compras;

public class PurchaseTests
{
    [Fact]
    public void Deve_criar_compra_quando_dados_forem_validos()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var jogo = HelperTests.CriarJogoValido();

        // Act
        var compra = new Compra(usuario.Id, jogo.Id, jogo.Preco);

        // Assert
        compra.UsuarioId.Should().Be(usuario.Id);
        compra.JogoId.Should().Be(jogo.Id);
        compra.ValorPago.Should().Be(jogo.Preco);
        compra.DataCompra.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Deve_lancar_excecao_quando_usuario_id_for_vazio()
    {
        // Arrange
        var jogo = HelperTests.CriarJogoValido();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Compra(
            Guid.Empty,
            jogo.Id,
            jogo.Preco
        ));

        exception.Message.Should().Contain("usuário");
    }

    [Fact]
    public void Deve_lancar_excecao_quando_jogo_id_for_vazio()
    {
        // Arrange
        var usuario = CriarUsuarioValido();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Compra(
            usuario.Id,
            Guid.Empty,
            99.90m
        ));

        exception.Message.Should().Contain("jogo");
    }

    [Fact]
    public void Deve_lancar_excecao_quando_valor_pago_for_negativo()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var jogo = HelperTests.CriarJogoValido();

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => new Compra(
            usuario.Id,
            jogo.Id,
            -10m
        ));

        exception.Message.Should().Contain("valor");
    }

    [Fact]
    public void Deve_aceitar_valor_zero_para_jogos_gratuitos()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var jogo = new JogoTests(
            "Jogo Grátis",
            "Descrição",
            "Desenvolvedor",
            "Distribuidora",
            0m,
            DateTime.UtcNow.AddDays(-1),
            HelperTests.CriarCategoriasPadrao()
        );

        // Act
        var compra = new Compra(usuario.Id, jogo.Id, 0m);

        // Assert
        compra.ValorPago.Should().Be(0m);
    }

    [Fact]
    public void Deve_criar_compra_com_data_atual()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var jogo = HelperTests.CriarJogoValido();
        var antes = DateTime.UtcNow;

        // Act
        var compra = new Compra(usuario.Id, jogo.Id, jogo.Preco);
        var depois = DateTime.UtcNow;

        // Assert
        compra.DataCompra.Should().BeAfter(antes.AddSeconds(-1));
        compra.DataCompra.Should().BeBefore(depois.AddSeconds(1));
    }

    private static User CriarUsuarioValido()
    {
        return new User
        {
            UserHandle = "usuario123",
            Username = "usuario@fiap.com.br",
            PasswordHash = "hash123456",
            Role = UserRole.Client
        };
    }
}
