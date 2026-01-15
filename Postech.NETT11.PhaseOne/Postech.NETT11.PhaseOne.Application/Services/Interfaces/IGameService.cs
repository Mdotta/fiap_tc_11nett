using Postech.NETT11.PhaseOne.Application.DTOs.Requests.Game;
using Postech.NETT11.PhaseOne.Application.DTOs.Responses.Game;

namespace Postech.NETT11.PhaseOne.Application.Services.Interfaces;

public interface IGameService
{
    Task<IEnumerable<GameResponse>> GetAllGamesAsync();
    
    Task<GameResponse?> GetGameByIdAsync(Guid id);
    
    Task<GameResponse> AddGameAsync(CreateGameRequest game);
    Task<GameResponse> UpdateGameAsync(Guid id,UpdateGameRequest game);
    Task<bool> DeleteGameAsync(Guid id);
}