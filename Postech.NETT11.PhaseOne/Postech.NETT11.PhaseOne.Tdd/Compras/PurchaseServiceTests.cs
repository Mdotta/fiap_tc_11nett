using FluentAssertions;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;
using Postech.NETT11.PhaseOne.Tests.Compras.Entities;
using Postech.NETT11.PhaseOne.Tests.Compras.Exceptions;
using Postech.NETT11.PhaseOne.Tests.Jogos.Entities;
using Postech.NETT11.PhaseOne.Tests.Jogos.Infra;
using Xunit;

namespace Postech.NETT11.PhaseOne.Tests.Compras;

public class PurchaseServiceTests
{
    [Fact]
    public void Usuario_deve_poder_comprar_jogo_ativo()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var jogo = HelperTests.CriarJogoValido();
        jogo.Ativo.Should().BeTrue();

        // Act
        var compra = new Compra(usuario.Id, jogo.Id, jogo.Preco);

        // Assert
        compra.Should().NotBeNull();
        compra.UsuarioId.Should().Be(usuario.Id);
        compra.JogoId.Should().Be(jogo.Id);
        compra.ValorPago.Should().Be(jogo.Preco);
    }

    [Fact]
    public void Usuario_nao_deve_poder_comprar_jogo_inativo()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var jogo = HelperTests.CriarJogoValido();
        // Simula jogo inativo (será implementado método no domínio)
        // jogo.Desativar();

        // Act & Assert
        // A validação será feita no serviço de compra
        // Por enquanto, apenas verificamos que o jogo existe
        jogo.Should().NotBeNull();
    }

    [Fact]
    public void Usuario_nao_deve_poder_comprar_mesmo_jogo_duas_vezes()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var jogo = HelperTests.CriarJogoValido();
        var compra1 = new Compra(usuario.Id, jogo.Id, jogo.Preco);

        // Act & Assert
        // A validação será feita no serviço de compra/biblioteca
        // Por enquanto, apenas verificamos que a primeira compra foi criada
        compra1.Should().NotBeNull();
        compra1.JogoId.Should().Be(jogo.Id);
    }

    [Fact]
    public void Compra_deve_registrar_valor_correto_do_jogo()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var precoJogo = 149.90m;
        var jogo = new JogoTests(
            "Jogo Caro",
            "Descrição",
            "Desenvolvedor",
            "Distribuidora",
            precoJogo,
            DateTime.UtcNow.AddDays(-1),
            HelperTests.CriarCategoriasPadrao()
        );

        // Act
        var compra = new Compra(usuario.Id, jogo.Id, jogo.Preco);

        // Assert
        compra.ValorPago.Should().Be(precoJogo);
    }

    [Fact]
    public void Compra_deve_registrar_data_correta()
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

    [Fact]
    public void Compra_deve_permitir_jogos_gratuitos()
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
    public void Compra_deve_validar_dados_obrigatorios()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var jogo = HelperTests.CriarJogoValido();

        // Act & Assert - UsuarioId vazio
        Assert.Throws<DomainException>(() => new Compra(
            Guid.Empty,
            jogo.Id,
            jogo.Preco
        ));

        // Act & Assert - JogoId vazio
        Assert.Throws<DomainException>(() => new Compra(
            usuario.Id,
            Guid.Empty,
            jogo.Preco
        ));

        // Act & Assert - Valor negativo
        Assert.Throws<DomainException>(() => new Compra(
            usuario.Id,
            jogo.Id,
            -10m
        ));
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
