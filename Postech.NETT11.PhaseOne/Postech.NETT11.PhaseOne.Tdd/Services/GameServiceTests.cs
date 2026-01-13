using FluentAssertions;
using Moq;
using Postech.NETT11.PhaseOne.Application.Services;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition.Enums;
using Xunit;

namespace Postech.NETT11.PhaseOne.Tests.Services;

public class GameServiceTests
{
    private readonly Mock<IGameRepository> _mockRepo;
    private readonly IGameService _service;

    public GameServiceTests()
    {
        _mockRepo = new Mock<IGameRepository>();
        _service = new GameService(_mockRepo.Object);
    }

    private Game GetValidGame()
        {
            return new Game(
                "Valid name",
                "Valid Description",
                "Valid Developer",
                "Valid Publisher",
                10,
                GameStatus.Released,
                DateTime.UtcNow.AddHours(-1)
                );
        }
    
    
    // Create Tests
    [Fact]
    public async Task CreateGame_WithValidData_ShouldReturnCreatedGame()
    {
        // Arrange
        var game = GetValidGame();
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Game>())).ReturnsAsync(game);

        // Act
        var result = await _service.AddGameAsync(game);

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
        var game = GetValidGame();
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Game>())).ReturnsAsync(game);

        // Act
        var result = await _service.UpdateGameAsync(game);

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
