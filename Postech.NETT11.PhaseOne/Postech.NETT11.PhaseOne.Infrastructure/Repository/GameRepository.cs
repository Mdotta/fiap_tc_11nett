using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition;

namespace Postech.NETT11.PhaseOne.Infrastructure.Repository;

public class GameRepository:EFRepository<Game>, IGameRepository
{
    public GameRepository(AppDbContext context) : base(context)
    {
    }
}