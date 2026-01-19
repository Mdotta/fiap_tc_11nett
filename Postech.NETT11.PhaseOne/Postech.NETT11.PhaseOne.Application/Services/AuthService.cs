using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Postech.NETT11.PhaseOne.Application.DTOs.Responses.Auth;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;
using Postech.NETT11.PhaseOne.Domain.AccessAndAuthorization;

namespace Postech.NETT11.PhaseOne.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResponse?> AuthenticateAsync(string username, string password, string? ipAddress = null)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning(
                    "Authentication failed: Invalid credentials format. IpAddress: {IpAddress}",
                    ipAddress ?? "Unknown");
                return null;
            }

            _logger.LogInformation(
                "Authentication attempt for user: {Username}, IpAddress: {IpAddress}",
                username,
                ipAddress ?? "Unknown");

            var user = await _userRepository.GetByUsernameIncludingInactiveAsync(username);
            
            if (user == null)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Authentication failed: User not found. Username: {Username}, IpAddress: {IpAddress}, Duration: {Duration}ms",
                    username,
                    ipAddress ?? "Unknown",
                    stopwatch.ElapsedMilliseconds);
                return null;
            }

            if (!user.IsActive)
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Authentication failed: User account is inactive. UserId: {UserId}, Username: {Username}, IpAddress: {IpAddress}, Duration: {Duration}ms",
                    user.Id,
                    username,
                    ipAddress ?? "Unknown",
                    stopwatch.ElapsedMilliseconds);
                throw new UnauthorizedAccessException("User is not active. Contact admin.");
            }

            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            {
                stopwatch.Stop();
                _logger.LogWarning(
                    "Authentication failed: Invalid password. UserId: {UserId}, Username: {Username}, IpAddress: {IpAddress}, Duration: {Duration}ms",
                    user.Id,
                    username,
                    ipAddress ?? "Unknown",
                    stopwatch.ElapsedMilliseconds);
                return null;
            }

            var tokenData = _jwtService.GenerateToken(user.Id.ToString(), user.Role.ToString());

            stopwatch.Stop();
            
            _logger.LogInformation(
                "Authentication successful. UserId: {UserId}, Username: {Username}, Role: {Role}, IpAddress: {IpAddress}, Duration: {Duration}ms, TokenExpiresAt: {TokenExpiresAt}",
                user.Id,
                username,
                user.Role,
                ipAddress ?? "Unknown",
                stopwatch.ElapsedMilliseconds,
                tokenData.ExpiresAt);

            return new AuthResponse(
                tokenData.Token,
                tokenData.ExpiresAt,
                "Bearer",
                new AuthUserData(user.Id, user.Username, user.Role)
            );
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex,
                "Authentication error occurred. Username: {Username}, IpAddress: {IpAddress}, Duration: {Duration}ms",
                username,
                ipAddress ?? "Unknown",
                stopwatch.ElapsedMilliseconds);
            throw;
        }
    }
}

