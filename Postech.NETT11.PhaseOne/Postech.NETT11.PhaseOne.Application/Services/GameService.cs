using Microsoft.Extensions.Logging;
using Postech.NETT11.PhaseOne.Application.DTOs.Requests.Game;
using Postech.NETT11.PhaseOne.Application.DTOs.Responses.Game;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition.Enums;

namespace Postech.NETT11.PhaseOne.Application.Services;

public class GameService(IGameRepository repository,ILogger<IGameService> logger):IGameService
{
    public async Task<IEnumerable<GameResponse>> GetAllGamesAsync()
    {
        var games = await repository.GetAllAsync();
        return games.Select(MapToResponse);
    }

    public async Task<GameResponse?> GetGameByIdAsync(Guid id)
    {
        var game = await repository.GetByIdAsync(id);
        if (game is null)
        {
            logger.LogWarning("Game with ID {GameId} not found.", id);
            return null;
        }
        return MapToResponse(game);
    }

    public async Task<GameResponse> AddGameAsync(CreateGameRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var game = new Game(request.Name, request.Description, request.Developer, request.Publisher);
        game.SetPrice(request.Price ?? 0);
        
        var addedGame = await repository.AddAsync(game);
    
        logger.LogDebug("Added new game with ID {GameId}.", addedGame.Id);
    
        return MapToResponse(addedGame);
    }

    public async Task<GameResponse> UpdateGameAsync(Guid id,UpdateGameRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        var gameToUpdate = await repository.GetByIdAsync(id);
        
        if (gameToUpdate is null)
        {
            logger.LogWarning("Game with ID {GameId} not found for update.", id);
            throw new KeyNotFoundException($"Game with ID {id} not found.");
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
        
        var updatedGame = await repository.UpdateAsync(gameToUpdate);
        
        logger.LogDebug("Updated game with ID {GameId}.", id);
        
        return MapToResponse(updatedGame);
    }

    public async Task<bool> DeleteGameAsync(Guid id)
    {
        var deleted = await repository.DeleteAsync(id);
        
        return deleted;
    }

    private static GameResponse MapToResponse(Game game)
    {
        return new GameResponse(
            game.Id,
            game.Title,
            game.Description,
            game.Developer,
            game.Publisher,
            game.Price
        );
    }
}