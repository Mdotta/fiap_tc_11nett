using Postech.NETT11.PhaseOne.Application.Services.Interfaces;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition;

namespace Postech.NETT11.PhaseOne.Application.Services;

public class GameService(IGameRepository repository):IGameService
{
    public Task<IEnumerable<Game>> GetAllGamesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Game> GetGameByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Game> AddGameAsync(Game game)
    {
        throw new NotImplementedException();
    }

    public Task<Game> UpdateGameAsync(Game game)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteGameAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}