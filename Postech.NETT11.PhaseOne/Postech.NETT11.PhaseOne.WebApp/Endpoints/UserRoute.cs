using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Postech.NETT11.PhaseOne.Application.DTOs.Requests.User;
using Postech.NETT11.PhaseOne.Application.DTOs.Responses.User;
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
            .RequireAuthorization("Admin");

        group.MapGet("/{id:guid}", GetUserById)
            .WithName("GetUserById")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .WithOpenApi()
            .AllowAnonymous();
        
        group.MapPut("/me", UpdateOwnProfile)
            .WithName("UpdateOwnProfile")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapPut("/me/password", ChangePassword)
            .WithName("ChangePassword")
            .WithOpenApi()
            .RequireAuthorization();
        
        group.MapPut("/{id:guid}", UpdateUser)
            .WithName("UpdateUser")
            .WithOpenApi()
            .RequireAuthorization("Admin");

        group.MapPut("/{id:guid}/reactivate", ReactivateUser)
            .WithName("ReactivateUser")
            .WithOpenApi()
            .RequireAuthorization("Admin");

        group.MapDelete("/{id:guid}", DeleteUser)
            .WithName("DeleteUser")
            .WithOpenApi()
            .RequireAuthorization("Admin");
    }

    private Guid? GetCurrentUserId(HttpContext context)
    {
        var userIdClaim = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
    }

    private bool IsAdmin(HttpContext context)
    {
        return context.User.IsInRole("Admin");
    }

    private async Task<Results<Ok<UserResponse>, UnauthorizedHttpResult, NotFound>> GetAuthenticatedUserData(
        HttpContext context,
        IUserService userService,
        ILogger<UserRoute> logger)
    {
        var userId = GetCurrentUserId(context);
        if (!userId.HasValue)
        {
            logger.LogWarning("Unauthorized request to GetCurrentUser");
            return TypedResults.Unauthorized();
        }

        logger.LogInformation("Getting data for user: {UserId}", userId.Value);
        var user = await userService.GetUserByIdAsync(userId.Value);
        
        if (user is null)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(user);
    }

    private async Task<IResult> GetAllUsers(
        HttpContext context,
        IUserService userService,
        ILogger<UserRoute> logger)
    {
        if (!IsAdmin(context))
            throw new UnauthorizedAccessException();
        
        logger.LogInformation("Getting all users");
        var users = await userService.GetAllUsersAsync();
        return TypedResults.Ok(users);
    }

    private async Task<Results<Ok<UserResponse>, NotFound, ForbidHttpResult>> GetUserById(
        Guid id,
        HttpContext context,
        IUserService userService,
        ILogger<UserRoute> logger)
    {
        var currentUserId = GetCurrentUserId(context);
        
        // Permitir acesso ao próprio perfil ou se for Admin
        if (currentUserId != id && !IsAdmin(context))
        {
            logger.LogWarning("Unauthorized access attempt to user {UserId} by user {CurrentUserId}", id, currentUserId);
            return TypedResults.Forbid();
        }

        logger.LogInformation("Getting user by id: {UserId}", id);
        var user = await userService.GetUserByIdAsync(id);
        
        if (user is null)
            return TypedResults.NotFound();
        
        return TypedResults.Ok(user);
    }

    private async Task<IResult> CreateUser(
        CreateUserRequest request,
        IUserService userService,
        ILogger<UserRoute> logger)
    {
        logger.LogInformation("Creating new user: {Username}", request.Username);
        var user = await userService.CreateUserAsync(request);
        return TypedResults.Created($"/user/{user.Id}", user);
    }

    private async Task<IResult> UpdateOwnProfile(
        HttpContext context,
        UpdateOwnProfileRequest request,
        IUserService userService,
        ILogger<UserRoute> logger)
    {
        var userId = GetCurrentUserId(context);
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        logger.LogInformation("Updating own profile for user: {UserId}", userId.Value);
        var user = await userService.UpdateOwnProfileAsync(userId.Value, request);
        return TypedResults.Ok(user);
    }

    private async Task<IResult> ChangePassword(
        HttpContext context,
        ChangePasswordRequest request,
        IUserService userService,
        ILogger<UserRoute> logger)
    {
        var userId = GetCurrentUserId(context);
        if (!userId.HasValue)
        {
            return TypedResults.Unauthorized();
        }

        logger.LogInformation("Changing password for user: {UserId}", userId.Value);
        await userService.ChangePasswordAsync(userId.Value, request);
        
        return TypedResults.Ok(new { message = "Password changed successfully." });
    }

    private async Task<IResult> UpdateUser(
        Guid id,
        HttpContext context,
        UpdateUserRequest request,
        IUserService userService,
        ILogger<UserRoute> logger)
    {
        if (!IsAdmin(context))
            throw new UnauthorizedAccessException();
        
        logger.LogInformation("Updating user: {UserId}", id);
        var user = await userService.UpdateUserAsync(id, request);
        return TypedResults.Ok(user);
    }

    private async Task<IResult> ReactivateUser(
        Guid id,
        HttpContext context,
        IUserService userService,
        ILogger<UserRoute> logger)
    {
        if (!IsAdmin(context))
            throw new UnauthorizedAccessException();
        
        logger.LogInformation("Reactivating user: {UserId}", id);
        var reactivated = await userService.ReactivateUserAsync(id);
        
        if (!reactivated)
            return TypedResults.NotFound();

        return TypedResults.Ok(new { message = "User has been activated." });
    }

    private async Task<IResult> DeleteUser(
        Guid id,
        HttpContext context,
        IUserService userService,
        ILogger<UserRoute> logger)
    {
        if (!IsAdmin(context))
            throw new UnauthorizedAccessException();
        
        var currentUserId = GetCurrentUserId(context);
        
        logger.LogInformation("Deleting user: {UserId} by user: {CurrentUserId}", id, currentUserId);
        await userService.DeleteUserAsync(id, currentUserId);
        return TypedResults.Ok(new { message = "User has been deactivated." });
    }
}