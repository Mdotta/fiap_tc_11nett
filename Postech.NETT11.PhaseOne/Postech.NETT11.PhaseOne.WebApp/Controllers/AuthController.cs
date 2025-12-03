using Microsoft.AspNetCore.Mvc;
using Postech.NETT11.PhaseOne.Domain.Contracts.Auth;
using Postech.NETT11.PhaseOne.Domain.Repositories;
using Postech.NETT11.PhaseOne.WebApp.Services.Auth;

namespace Postech.NETT11.PhaseOne.WebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController:ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    
    public AuthController(IJwtService jwtService, IUserRepository userRepository)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
    }

    [HttpPost("login")]
    public IActionResult Authenticate([FromBody] AuthRequest request)
    {
        var hashPass = request.Password;
        var user = _userRepository.GetAll().FirstOrDefault(x=>x.Username == request.Username && x.PasswordHash == hashPass);

        if (user == null)
            return Unauthorized();
        
        var token = _jwtService.GenerateToken(user.Id.ToString(), user.Role.ToString());

        return Ok(token);
    }
}