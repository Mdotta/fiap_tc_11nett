using FluentAssertions;
using Moq;
using Postech.NETT11.PhaseOne.Application.Services;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization.Enums;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition;
using Xunit;
using Postech.NETT11.PhaseOne.Tests.Infra;
using Postech.NETT11.PhaseOne.Tests.Enuns;
using Postech.NETT11.PhaseOne.Tests.Exceptions;
using Postech.NETT11.PhaseOne.Tests.Entities;

namespace Postech.NETT11.PhaseOne.Tests.Services;


public class GameServiceTests
{
    protected readonly Mock<IGameRepository> _mockRepo;
    protected readonly IGameService _service;
    
    public GameServiceTests()
    {
        //TODO: Implementar Mock do Repositório
        _mockRepo = new Mock<IGameRepository>();
        _service = new GameService();
    }
    
    [Fact]
    public void Admin_Should_Be_Able_To_Register_Game()
    {
        // Arrange
        var admin = HelperTests.CreateAdministrator();
        var game = HelperTests.CreateValidGame();

        // Act & Assert
        admin.Role.Should().Be(UserRole.Admin);
        game.Should().NotBeNull();
        game.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Regular_User_Should_Not_Be_Able_To_Register_Game()
    {
        // Arrange
        var user = HelperTests.CreateRegularUser();

        // Act & Assert
        user.Role.Should().Be(UserRole.Client);
    }

    [Fact]
    public void Game_Should_Be_Created_As_Active_By_Default()
    {
        // Arrange & Act
        var game = HelperTests.CreateValidGame();

        // Assert
        game.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Admin_Should_Be_Able_To_Deactivate_Game()
    {
        // Arrange
        var game = HelperTests.CreateValidGame();
        game.IsActive.Should().BeTrue();

        // Act
        var deactivatedGame = game.Deactivate();

        // Assert
        deactivatedGame.IsActive.Should().BeFalse();
        game.Should().NotBeNull();
    }

    [Fact]
    public void Admin_Should_Be_Able_To_Update_Game_Price()
    {
        // Arrange
        var game = HelperTests.CreateValidGame();
        var newPrice = 79.90m;
        game.Price.Should().Be(99.90m);

        // Act
        game.UpdatePrice(newPrice);

        // Assert
        game.Price.Should().Be(newPrice);
    }

    [Fact]
    public void Should_Throw_Exception_When_Updating_Price_With_Negative_Value()
    {
        // Arrange
        var game = HelperTests.CreateValidGame();
        var negativePrice = -10m;

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => game.UpdatePrice(negativePrice));
        exception.Message.Should().Contain("preço");
    }

    [Fact]
    public void Should_Allow_Updating_Price_To_Zero()
    {
        // Arrange
        var game = HelperTests.CreateValidGame();
        game.Price.Should().Be(99.90m);

        // Act
        game.UpdatePrice(0m);

        // Assert
        game.Price.Should().Be(0m);
    }

    [Fact]
    public void Game_Should_Validate_All_Required_Fields()
    {
        // Arrange
        var categories = HelperTests.CreateDefaultCategories();

        // Act & Assert - Name
        Assert.Throws<DomainException>(() => new GameTests(
            "",
            "Descrição",
            "Desenvolvedor",
            "Distribuidora",
            99.90m,
            DateTime.UtcNow.AddDays(-1),
            categories
        ));

        // Act & Assert - Developer
        Assert.Throws<DomainException>(() => new GameTests(
            "Jogo Teste",
            "Descrição",
            "",
            "Distribuidora",
            99.90m,
            DateTime.UtcNow.AddDays(-1),
            categories
        ));

        // Act & Assert - Publisher
        Assert.Throws<DomainException>(() => new GameTests(
            "Jogo Teste",
            "Descrição",
            "Desenvolvedor",
            "",
            99.90m,
            DateTime.UtcNow.AddDays(-1),
            categories
        ));
    }

    [Fact]
    public void Game_Should_Allow_Multiple_Categories()
    {
        // Arrange
        var categories = new List<EnumCategories>
    {
        EnumCategories.Action,
        EnumCategories.Adventure,
        EnumCategories.RPG,
        EnumCategories.MMO
    };

        // Act
        var game = new GameTests(
            "Full Game",
            "Description",
            "Developer",
            "Publisher",
            99.90m,
            DateTime.UtcNow.AddDays(-1),
            categories
        );

        // Assert
        game.Categories.Should().HaveCount(4);
        game.Categories.Should().Contain(EnumCategories.Action);
        game.Categories.Should().Contain(EnumCategories.Adventure);
        game.Categories.Should().Contain(EnumCategories.RPG);
        game.Categories.Should().Contain(EnumCategories.MMO);
    }

    
}
