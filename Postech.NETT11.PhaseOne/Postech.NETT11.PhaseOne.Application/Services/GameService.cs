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

        var game = new Game(request.Name, request.Description, request.Developer, request.Publisher);
        game.SetPrice(request.Price ?? 0);
        game.SetReleaseDate(request.ReleaseDate ?? null);
        game.AddCategories(request.Categories);
        
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
        
        if (gameToUpdate is null)
        {
            logger.LogWarning("Game with ID {GameId} not found for update.", request.Id);
            throw new KeyNotFoundException($"Game with ID {request.Id} not found.");
        }
        
        if (request.Name is not null)
            gameToUpdate.UpdateTitle(request.Name);
        if (request.Description is not null)
            gameToUpdate.UpdateDescription(request.Description);
        if (request.Developer is not null)
            gameToUpdate.UpdateDeveloper(request.Developer);
        if (request.Publisher is not null)
            gameToUpdate.UpdatePublisher(request.Publisher);
        if (request.Price.HasValue)
            gameToUpdate.SetPrice(request.Price.Value);
        if (request.ReleaseDate.HasValue)
            gameToUpdate.SetReleaseDate(request.ReleaseDate.Value);
        if (request.Categories is not null)
            gameToUpdate.AddCategories(request.Categories);
        
        var updatedGame = await repository.UpdateAsync(gameToUpdate);
        
        logger.LogDebug("Updated game with ID {GameId}.", request.Id);
        
        return updatedGame;
    }

    public async Task<bool> DeleteGameAsync(Guid id)
    {
        var deleted = await repository.DeleteAsync(id);

        return deleted;
    }
}