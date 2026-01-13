using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition.Enums;

namespace Postech.NETT11.PhaseOne.Application.DTOs.Responses.Game;

public class GameResponse
{
    
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Developer { get; private set; }
    public string Publisher { get; private set; }
    public decimal? Price { get; private set; }
    public DateTime? ReleaseDate { get; private set; }
    public List<GameCategory>? Categories { get; set; }
}