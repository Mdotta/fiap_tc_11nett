using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition.Enums;

namespace Postech.NETT11.PhaseOne.Application.DTOs.Requests.Game;

public sealed record UpdateGameRequest
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Developer { get; set; }
    public string? Publisher { get; set; }
    public decimal? Price { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public List<GameCategory>? Categories { get; set; }
}