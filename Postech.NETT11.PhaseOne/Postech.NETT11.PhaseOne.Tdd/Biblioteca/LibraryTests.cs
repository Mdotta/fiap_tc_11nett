using FluentAssertions;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;
using Postech.NETT11.PhaseOne.Tests.Biblioteca.Entities;
using Postech.NETT11.PhaseOne.Tests.Compras.Entities;
using Postech.NETT11.PhaseOne.Tests.Jogos.Entities;
using Postech.NETT11.PhaseOne.Tests.Jogos.Infra;
using Xunit;

namespace Postech.NETT11.PhaseOne.Tests.Biblioteca;

public class LibraryTests
{
    [Fact]
    public void Deve_criar_biblioteca_para_usuario()
    {
        // Arrange
        var usuario = CriarUsuarioValido();

        // Act
        var biblioteca = new BibliotecaUsuario(usuario.Id);

        // Assert
        biblioteca.UsuarioId.Should().Be(usuario.Id);
        biblioteca.Jogos.Should().BeEmpty();
    }

    [Fact]
    public void Deve_adicionar_jogo_na_biblioteca()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var biblioteca = new BibliotecaUsuario(usuario.Id);
        var jogo = HelperTests.CriarJogoValido();
        var compra = new Compra(usuario.Id, jogo.Id, jogo.Preco);

        // Act
        biblioteca.AdicionarJogo(jogo.Id, compra.Id);

        // Assert
        biblioteca.Jogos.Should().HaveCount(1);
        biblioteca.Jogos.First().JogoId.Should().Be(jogo.Id);
        biblioteca.Jogos.First().CompraId.Should().Be(compra.Id);
    }

    [Fact]
    public void Deve_adicionar_multiplos_jogos_na_biblioteca()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var biblioteca = new BibliotecaUsuario(usuario.Id);
        var jogo1 = HelperTests.CriarJogoValido();
        var jogo2 = new JogoTests(
            "Jogo 2",
            "Descrição 2",
            "Desenvolvedor 2",
            "Distribuidora 2",
            49.90m,
            DateTime.UtcNow.AddDays(-1),
            HelperTests.CriarCategoriasPadrao()
        );
        var compra1 = new Compra(usuario.Id, jogo1.Id, jogo1.Preco);
        var compra2 = new Compra(usuario.Id, jogo2.Id, jogo2.Preco);

        // Act
        biblioteca.AdicionarJogo(jogo1.Id, compra1.Id);
        biblioteca.AdicionarJogo(jogo2.Id, compra2.Id);

        // Assert
        biblioteca.Jogos.Should().HaveCount(2);
    }

    [Fact]
    public void Deve_verificar_se_jogo_esta_na_biblioteca()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var biblioteca = new BibliotecaUsuario(usuario.Id);
        var jogo = HelperTests.CriarJogoValido();
        var compra = new Compra(usuario.Id, jogo.Id, jogo.Preco);
        biblioteca.AdicionarJogo(jogo.Id, compra.Id);

        // Act
        var possuiJogo = biblioteca.PossuiJogo(jogo.Id);

        // Assert
        possuiJogo.Should().BeTrue();
    }

    [Fact]
    public void Deve_retornar_false_quando_jogo_nao_esta_na_biblioteca()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var biblioteca = new BibliotecaUsuario(usuario.Id);
        var jogo = HelperTests.CriarJogoValido();

        // Act
        var possuiJogo = biblioteca.PossuiJogo(jogo.Id);

        // Assert
        possuiJogo.Should().BeFalse();
    }

    [Fact]
    public void Deve_obter_jogo_da_biblioteca_por_id()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var biblioteca = new BibliotecaUsuario(usuario.Id);
        var jogo = HelperTests.CriarJogoValido();
        var compra = new Compra(usuario.Id, jogo.Id, jogo.Preco);
        biblioteca.AdicionarJogo(jogo.Id, compra.Id);

        // Act
        var jogoBiblioteca = biblioteca.ObterJogo(jogo.Id);

        // Assert
        jogoBiblioteca.Should().NotBeNull();
        jogoBiblioteca!.JogoId.Should().Be(jogo.Id);
        jogoBiblioteca.CompraId.Should().Be(compra.Id);
    }

    [Fact]
    public void Deve_retornar_null_quando_jogo_nao_existe_na_biblioteca()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var biblioteca = new BibliotecaUsuario(usuario.Id);
        var jogo = HelperTests.CriarJogoValido();

        // Act
        var jogoBiblioteca = biblioteca.ObterJogo(jogo.Id);

        // Assert
        jogoBiblioteca.Should().BeNull();
    }

    [Fact]
    public void Deve_contar_total_de_jogos_na_biblioteca()
    {
        // Arrange
        var usuario = CriarUsuarioValido();
        var biblioteca = new BibliotecaUsuario(usuario.Id);
        var jogo1 = HelperTests.CriarJogoValido();
        var jogo2 = new JogoTests(
            "Jogo 2",
            "Descrição 2",
            "Desenvolvedor 2",
            "Distribuidora 2",
            49.90m,
            DateTime.UtcNow.AddDays(-1),
            HelperTests.CriarCategoriasPadrao()
        );
        var compra1 = new Compra(usuario.Id, jogo1.Id, jogo1.Preco);
        var compra2 = new Compra(usuario.Id, jogo2.Id, jogo2.Preco);
        biblioteca.AdicionarJogo(jogo1.Id, compra1.Id);
        biblioteca.AdicionarJogo(jogo2.Id, compra2.Id);

        // Act
        var total = biblioteca.TotalJogos;

        // Assert
        total.Should().Be(2);
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
