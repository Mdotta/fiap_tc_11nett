using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition.Enums;

namespace Postech.NETT11.PhaseOne.Application.DTOs.Responses.Game;

public sealed record GameResponse(
    Guid Id,
    string Title,
    string Description,
    string Developer,
    string Publisher,
    decimal? Price
);
