using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Postech.NETT11.PhaseOne.Application.DTOs.Requests.User;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;

namespace Postech.NETT11.PhaseOne.WebApp.Endpoints;

public class UserRoute : BaseRoute
{
    public UserRoute()
    {
        Route = "/user";
    }

    protected override void AddRoute(RouteGroupBuilder group)
    {
        group.MapGet("/me", GetAuthenticatedUserData)
            .WithName("GetCurrentUser")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/", GetAllUsers)
            .WithName("GetAllUsers")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/{id:guid}", GetUserById)
            .WithName("GetUserById")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .WithOpenApi()
            .RequireAuthorization();
        
        //TODO: Criar update route para o proprio usuario alterar seus dados (excluindo role)
        
        group.MapPut("/{id:guid}", UpdateUser)
            .WithName("UpdateUser")
            .WithOpenApi()
            .RequireAuthorization("Admin");

        group.MapDelete("/{id:guid}", DeleteUser)
            .WithName("DeleteUser")
            .WithOpenApi()
            .RequireAuthorization("Admin");
    }

    private async Task<IResult> GetAuthenticatedUserData(
        HttpContext context,
        IUserService userService,
        ILogger<UserRoute> logger)
    {
        var userId = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            logger.LogInformation("User {Username} not found", context.User.Identity?.Name);
            return TypedResults.Unauthorized();
        }

        logger.LogInformation("Getting data for user: {UserId}", userId.Value);
        var user = await userService.GetUserByIdAsync(Guid.Parse(userId.Value));
        
        if (user == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(user);
    }

    private async Task<IResult> GetAllUsers(
        IUserService userService,
        ILogger<UserRoute> logger)
    {
        logger.LogInformation("Getting all users");
        var users = await userService.GetAllUsersAsync();
        return TypedResults.Ok(users);
    }

    private async Task<IResult> GetUserById(
        Guid id,
        IUserService userService,
        ILogger<UserRoute> logger)
    {
        logger.LogInformation("Getting user by id: {UserId}", id);
        var user = await userService.GetUserByIdAsync(id);
        
        if (user == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(user);
    }

    private async Task<IResult> CreateUser(
        CreateUserRequest request,
        IUserService userService,
        ILogger<UserRoute> logger)
    {
        logger.LogInformation("Creating new user: {Username}", request.Username);
        
        try
        {
            var user = await userService.CreateUserAsync(request);
            return TypedResults.Created($"/user/{user.Id}", user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating user: {Username}", request.Username);
            return TypedResults.BadRequest("Failed to create user");
        }
    }

    private async Task<IResult> UpdateUser(
        Guid id,
        UpdateUserRequest request,
        IUserService userService,
        ILogger<UserRoute> logger)
    {
        logger.LogInformation("Updating user: {UserId}", id);
        
        var user = await userService.UpdateUserAsync(id, request);
        
        if (user == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(user);
    }

    private async Task<IResult> DeleteUser(
        Guid id,
        IUserService userService,
        ILogger<UserRoute> logger)
    {
        logger.LogInformation("Deleting user: {UserId}", id);
        
        var deleted = await userService.DeleteUserAsync(id);
        
        if (!deleted)
            return TypedResults.NotFound();

        return TypedResults.NoContent();
    }
}