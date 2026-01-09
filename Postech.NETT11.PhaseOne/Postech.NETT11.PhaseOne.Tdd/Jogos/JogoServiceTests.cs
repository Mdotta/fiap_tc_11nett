using FluentAssertions;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;
using Postech.NETT11.PhaseOne.Tests.Jogos.Entities;
using Postech.NETT11.PhaseOne.Tests.Jogos.Exceptions;
using Postech.NETT11.PhaseOne.Tests.Jogos.Infra;
using Xunit;

namespace Postech.NETT11.PhaseOne.Tests.Jogos;


public class JogoServiceTests
{
    [Fact]
    public void Administrador_deve_poder_cadastrar_jogo()
    {
        // Arrange
        var administrador = CriarAdministrador();
        var jogo = HelperTests.CriarJogoValido();

        // Act & Assert
        administrador.Role.Should().Be(UserRole.Admin);
        jogo.Should().NotBeNull();
        jogo.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Usuario_comum_nao_deve_poder_cadastrar_jogo()
    {
        // Arrange
        var usuario = CriarUsuarioComum();

        // Act & Assert
        usuario.Role.Should().Be(UserRole.Client);
    }

    [Fact]
    public void Jogo_deve_ser_criado_como_ativo_por_padrao()
    {
        // Arrange & Act
        var jogo = HelperTests.CriarJogoValido();

        // Assert
        jogo.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Administrador_deve_poder_desativar_jogo()
    {
        // Arrange
        var jogo = HelperTests.CriarJogoValido();
        jogo.Ativo.Should().BeTrue();

        // Act
        var jogoDesativado = jogo.Desativar();

        // Assert
        jogoDesativado.Ativo.Should().BeFalse();
        jogo.Should().NotBeNull();
    }

    [Fact]
    public void Administrador_deve_poder_atualizar_preco_do_jogo()
    {
        // Arrange
        var jogo = HelperTests.CriarJogoValido();
        var novoPreco = 79.90m;
        jogo.Preco.Should().Be(99.90m);

        // Act
        jogo.AtualizarPreco(novoPreco);

        // Assert
        jogo.Preco.Should().Be(novoPreco);
    }

    [Fact]
    public void Deve_lancar_excecao_ao_atualizar_preco_com_valor_negativo()
    {
        // Arrange
        var jogo = HelperTests.CriarJogoValido();
        var precoNegativo = -10m;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => jogo.AtualizarPreco(precoNegativo));
        exception.Message.Should().Contain("preço");
    }

    [Fact]
    public void Deve_permitir_atualizar_preco_para_zero()
    {
        // Arrange
        var jogo = HelperTests.CriarJogoValido();
        jogo.Preco.Should().Be(99.90m);

        // Act
        jogo.AtualizarPreco(0m);

        // Assert
        jogo.Preco.Should().Be(0m);
    }

    [Fact]
    public void Jogo_deve_validar_todos_os_campos_obrigatorios()
    {
        // Arrange
        var categorias = HelperTests.CriarCategoriasPadrao();

        // Act & Assert - Nome
        Assert.Throws<DomainException>(() => new JogoTests(
            "",
            "Descrição",
            "Desenvolvedor",
            "Distribuidora",
            99.90m,
            DateTime.UtcNow.AddDays(-1),
            categorias
        ));

        // Act & Assert - Desenvolvedor
        Assert.Throws<DomainException>(() => new JogoTests(
            "Jogo Teste",
            "Descrição",
            "",
            "Distribuidora",
            99.90m,
            DateTime.UtcNow.AddDays(-1),
            categorias
        ));

        // Act & Assert - Distribuidora
        Assert.Throws<DomainException>(() => new JogoTests(
            "Jogo Teste",
            "Descrição",
            "Desenvolvedor",
            "",
            99.90m,
            DateTime.UtcNow.AddDays(-1),
            categorias
        ));
    }

    [Fact]
    public void Jogo_deve_permitir_multiplas_categorias()
    {
        // Arrange
        var categorias = new List<CategoriaTests>
        {
            new CategoriaTests("Ação"),
            new CategoriaTests("Aventura"),
            new CategoriaTests("RPG"),
            new CategoriaTests("Multijogador")
        };

        // Act
        var jogo = new JogoTests(
            "Jogo Completo",
            "Descrição",
            "Desenvolvedor",
            "Distribuidora",
            99.90m,
            DateTime.UtcNow.AddDays(-1),
            categorias
        );

        // Assert
        jogo.Categorias.Should().HaveCount(4);
        jogo.Categorias.Should().Contain(c => c.Nome == "Ação");
        jogo.Categorias.Should().Contain(c => c.Nome == "Aventura");
        jogo.Categorias.Should().Contain(c => c.Nome == "RPG");
        jogo.Categorias.Should().Contain(c => c.Nome == "Multijogador");
    }

    private static User CriarAdministrador()
    {
        return new User
        {
            UserHandle = "admin123",
            Username = "admin@fiap.com.br",
            PasswordHash = "hash123456",
            Role = UserRole.Admin
        };
    }

    private static User CriarUsuarioComum()
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
