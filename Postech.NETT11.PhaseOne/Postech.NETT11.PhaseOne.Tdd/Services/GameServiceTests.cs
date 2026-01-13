using Castle.Core.Logging;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Postech.NETT11.PhaseOne.Application.DTOs.Requests.Game;
using Postech.NETT11.PhaseOne.Application.Services;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition.Enums;
using Xunit;

namespace Postech.NETT11.PhaseOne.Tests.Services;

public class GameServiceTests
{
    private readonly Mock<IGameRepository> _mockRepo;
    private readonly Mock<ILogger<IGameService>> _mockLogger = new Mock<ILogger<IGameService>>();
    private readonly IGameService _service;

    public GameServiceTests()
    {
        _mockRepo = new Mock<IGameRepository>();
        _service = new GameService(_mockRepo.Object,_mockLogger.Object);
    }

    private Game GetValidGame()
    {
        return new GameBuilder()
            .WithName("Valid Game")
            .WithDescription("Valid game description")
            .WithDeveloper("Valid Developer")
            .WithPublisher("Valid Publisher")
            .WithPrice(10m)
            .WithReleaseDate(DateTime.UtcNow.AddDays(-1))
            .Build();
    }
    
    
    // Create Tests
    [Fact]
    public async Task CreateGame_WithValidData_ShouldReturnCreatedGame()
    {
        // Arrange
        var game = GetValidGame();
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Game>())).ReturnsAsync(game);
        var request = new CreateGameRequest()
        {
            Name = game.Name,
            Description = game.Description,
            Developer = game.Developer,
            Publisher = game.Publisher,
            Price = game.Price,
            ReleaseDate = game.ReleaseDate
        };
        // Act
        var result = await _service.AddGameAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(game.Id);
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Game>()), Times.Once);
    }


    [Fact]
    public async Task CreateGame_WithNullGame_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddGameAsync(null));
    }

    // Read Tests
    [Fact]
    public async Task GetGameById_WithExistingId_ShouldReturnGame()
    {
        // Arrange
        var gameId = Guid.CreateVersion7();
        var game = GetValidGame();
        game.Id = gameId;
        
        _mockRepo.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync(game);

        // Act
        var result = await _service.GetGameByIdAsync(gameId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(gameId);
        _mockRepo.Verify(r => r.GetByIdAsync(gameId), Times.Once);
    }

    [Fact]
    public async Task GetGameById_WithNonExistingId_ShouldReturnNull()
    {
        // Arrange
        var gameId = Guid.CreateVersion7();
        _mockRepo.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync((Game)null);

        // Act
        var result = await _service.GetGameByIdAsync(gameId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllGames_ShouldReturnListOfGames()
    {
        // Arrange
        var game = GetValidGame();
        var games = new List<Game>
        {
            game,
            game
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(games);

        // Act
        var result = await _service.GetAllGamesAsync();

        // Assert
        result.Should().HaveCount(2);
        _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
    }

    // Update Tests
    [Fact]
    public async Task UpdateGame_WithValidData_ShouldReturnUpdatedGame()
    {
        // Arrange
        var gameId = Guid.CreateVersion7();
        var game = GetValidGame();
        var request = new UpdateGameRequest()
        {
            Id = gameId,
            Name = "Updated Game",
            Description = game.Description,
            Developer = game.Developer,
            Publisher = game.Publisher,
            Price = game.Price,
            ReleaseDate = game.ReleaseDate
        };
        _mockRepo.Setup(r=>r.GetByIdAsync(gameId)).ReturnsAsync(game);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Game>())).ReturnsAsync(game);

        // Act
        var result = await _service.UpdateGameAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Game");
        _mockRepo.Verify(r => r.UpdateAsync(It.IsAny<Game>()), Times.Once);
    }

    [Fact]
    public async Task UpdateGame_WithNullGame_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateGameAsync(null));
    }

    // Delete Tests
    [Fact]
    public async Task DeleteGame_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        var gameId = Guid.CreateVersion7();
        _mockRepo.Setup(r => r.DeleteAsync(gameId)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteGameAsync(gameId);

        // Assert
        result.Should().BeTrue();
        _mockRepo.Verify(r => r.DeleteAsync(gameId), Times.Once);
    }

    [Fact]
    public async Task DeleteGame_WithNonExistingId_ShouldReturnFalse()
    {
        // Arrange
        var gameId = Guid.CreateVersion7();
        _mockRepo.Setup(r => r.DeleteAsync(gameId)).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteGameAsync(gameId);

        // Assert
        result.Should().BeFalse();
    }
}
