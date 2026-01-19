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
        
        group.MapPut("/{id:guid}", UpdateGameAsync)
            .WithName("updateGame")
            .WithOpenApi()
            .RequireAuthorization("Admin");
        
        group.MapDelete("/{id:guid}",DeleteGameAsync)
            .WithName("deleteGame")
            .WithOpenApi()
            .RequireAuthorization("Admin");
    }

    private async Task<NoContent> DeleteGameAsync(Guid id, HttpContext context, IGameService service)
    {
        var deleteResult = await service.DeleteGameAsync(id);
        return TypedResults.NoContent();
    }

    private async Task<Ok<GameResponse>> UpdateGameAsync(Guid id, HttpContext context, UpdateGameRequest request, IGameService service)
    {
    
        var updatedGame = await service.UpdateGameAsync(id,request);
        return TypedResults.Ok(updatedGame);
    }

    private async Task<Results<Ok<GameResponse>,NotFound>> GetGameById(Guid gameId, IGameService service)
    {
        var game = await service.GetGameByIdAsync(gameId);
        if (game is null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(game);
    }

    private async Task<Ok<List<GameResponse>>> GetAllGames(IGameService service)
    {
        var games = await service.GetAllGamesAsync();
        return TypedResults.Ok(games.ToList());
    }

    private async Task<Ok<GameResponse>> CreateGameAsync(HttpContext context, CreateGameRequest request, IGameService service)
    {
        var newGame = await service.AddGameAsync(request);
        return TypedResults.Ok(newGame);
    }
}