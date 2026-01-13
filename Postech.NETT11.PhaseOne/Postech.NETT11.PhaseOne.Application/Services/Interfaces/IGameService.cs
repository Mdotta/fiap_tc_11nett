using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition;

namespace Postech.NETT11.PhaseOne.Application.Services.Interfaces;

public interface IGameService
{
    Task<IEnumerable<Game>> GetAllGamesAsync();
    
    Task<Game> GetGameByIdAsync(Guid id);
    
    Task<Game> AddGameAsync(Game game);
    Task<Game> UpdateGameAsync(Game game);
    Task<bool> DeleteGameAsync(Guid id);
}