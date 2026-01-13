using Microsoft.Extensions.Logging;
using Postech.NETT11.PhaseOne.Application.DTOs.Requests.Game;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition.Enums;

namespace Postech.NETT11.PhaseOne.Application.Services;

public class GameService(IGameRepository repository,ILogger<IGameService> logger):IGameService
{
    public async Task<IEnumerable<Game>> GetAllGamesAsync()
    {
        var games = await repository.GetAllAsync();
        return games;
    }

    public async Task<Game?> GetGameByIdAsync(Guid id)
    {
        var game = await repository.GetByIdAsync(id);
        if (game is null)
        {
            logger.LogWarning("Game with ID {GameId} not found.", id);
        }
        return game;
    }

    public async Task<Game> AddGameAsync(CreateGameRequest request)
    {
        if (request is null)
        {
            logger.LogError("CreateGameRequest is null.");
            throw new ArgumentNullException(nameof(request), "CreateGameRequest cannot be null.");
        }
        
        var game = new GameBuilder()
            .WithTitle(request.Name)
            .WithDescription(request.Description)
            .WithDeveloper(request.Developer)
            .WithPublisher(request.Publisher)
            .WithPrice(request.Price ?? 0)
            .WithReleaseDate(request.ReleaseDate ?? DateTime.UtcNow)
            .WithCategories(request.Categories ?? new List<GameCategory>())
            .Build();
        
        var addedGame = await repository.AddAsync(game);
        
        logger.LogDebug("Added new game with ID {GameId}.", addedGame.Id);
        
        return addedGame;
    }

    public async Task<Game> UpdateGameAsync(UpdateGameRequest request)
    {
        if (request is null)
        {
            logger.LogError("UpdateGameRequest is null.");
            throw new ArgumentNullException(nameof(request), "UpdateGameRequest cannot be null.");
        }
        
        var gameToUpdate = await repository.GetByIdAsync(request.Id);
        
        //TODO: Ajustar para não usar exception
        if (gameToUpdate is null)
        {
            logger.LogWarning("Game with ID {GameId} not found for update.", request.Id);
            throw new KeyNotFoundException($"Game with ID {request.Id} not found.");
        }
        
        var gameBuilder = new GameBuilder(gameToUpdate);
        
        if (request.Name is not null)
            gameBuilder.WithTitle(request.Name);
        if (request.Description is not null)
            gameBuilder.WithDescription(request.Description);
        if (request.Developer is not null)
            gameBuilder.WithDeveloper(request.Developer);
        if (request.Publisher is not null)
            gameBuilder.WithPublisher(request.Publisher);
        if (request.Price.HasValue)
            gameBuilder.WithPrice(request.Price.Value);
        if (request.ReleaseDate.HasValue)
            gameBuilder.WithReleaseDate(request.ReleaseDate.Value);
        if (request.Categories is not null)
            gameBuilder.WithCategories(request.Categories);
        
        var game = gameBuilder.Build();
        
        var updatedGame = await repository.UpdateAsync(game);
        
        logger.LogDebug("Updated game with ID {GameId}.", request.Id);
        
        return updatedGame;
    }

    public async Task<bool> DeleteGameAsync(Guid id)
    {
        var deleted = await repository.DeleteAsync(id);

        return deleted;
    }
}