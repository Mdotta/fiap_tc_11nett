using Postech.NETT11.PhaseOne.Application.DTOs.Requests.Game;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition;

namespace Postech.NETT11.PhaseOne.Application.Services.Interfaces;

public interface IGameService
{
    Task<IEnumerable<Game>> GetAllGamesAsync();
    
    Task<Game?> GetGameByIdAsync(Guid id);
    
    Task<Game> AddGameAsync(CreateGameRequest game);
    Task<Game> UpdateGameAsync(UpdateGameRequest game);
    Task<bool> DeleteGameAsync(Guid id);
}