using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Postech.NETT11.PhaseOne.Application.DTOs.Requests.Game;
using Postech.NETT11.PhaseOne.Application.DTOs.Responses.Game;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition;

namespace Postech.NETT11.PhaseOne.WebApp.Endpoints;

public class GameRoute:BaseRoute
{
    public GameRoute()
    {
        Route = "/game";
    }
    protected override void AddRoute(RouteGroupBuilder group)
    {
        group.MapGet("/", GetAllGames)
            .WithName("getGames")
            .WithOpenApi()
            .RequireAuthorization();
        
        group.MapGet("/{gameId:guid}", GetGameById)
            .WithName("getGameById")
            .WithOpenApi()
            .RequireAuthorization();
        
        group.MapPost("/", CreateGameAsync)
            .WithName("createGame")
            .WithOpenApi()
            .RequireAuthorization("Admin");
        
        group.MapPut("/", UpdateGameAsync)
            .WithName("updateGame")
            .WithOpenApi()
            .RequireAuthorization("Admin");
        
        group.MapDelete("/{gameId:guid}",DeleteGameAsync)
            .WithName("deleteGame")
            .WithOpenApi()
            .RequireAuthorization("Admin");
    }

    private async Task<NoContent> DeleteGameAsync([FromHeader]Guid gameId, IGameService service)
    {
        var deleteResult = await service.DeleteGameAsync(gameId);
        return TypedResults.NoContent();
    }

    private async Task<Ok<Game>> UpdateGameAsync(UpdateGameRequest request, IGameService service)
    {
        var updatedGame = await service.UpdateGameAsync(request);
        return TypedResults.Ok(updatedGame);
    }

    private async Task<Results<Ok<Game>,NotFound>> GetGameById([FromHeader]Guid gameId, IGameService service)
    {
        var game = await service.GetGameByIdAsync(gameId);
        if (game is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(game);
    }

    private async Task<Ok<List<Game>>> GetAllGames(IGameService service)
    {
        var games = await service.GetAllGamesAsync();
        return TypedResults.Ok(games.ToList());
    }

    private async Task<Ok<Game>> CreateGameAsync(CreateGameRequest request, IGameService service)
    {
        var newGame = await service.AddGameAsync(request);
        return TypedResults.Ok(newGame);
    }
}