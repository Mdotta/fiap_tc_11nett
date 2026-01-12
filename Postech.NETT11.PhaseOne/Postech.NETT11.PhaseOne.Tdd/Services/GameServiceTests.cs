using FluentAssertions;
using Moq;
using Postech.NETT11.PhaseOne.Application.Services;
using Postech.NETT11.PhaseOne.Application.Services.Interfaces;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition;
using Xunit;

namespace Postech.NETT11.PhaseOne.Tests.Services;


public class GameServiceTests
{
    protected readonly Mock<IGameRepository> _mockRepo;
    protected readonly IGameService _service;
    
    public GameServiceTests()
    {
        //TODO: Implementar Mock do Repositório
        _mockRepo = new Mock<IGameRepository>();
        _service = new GameService();
    }
    
    [Fact]
    public void Admin_Should_Be_Able_To_Register_Game()
    {
       
    }

    [Fact]
    public void Regular_User_Should_Not_Be_Able_To_Register_Game()
    {
        
    }

    [Fact]
    public void Game_Should_Be_Created_As_Active_By_Default()
    {
        
    }

    [Fact]
    public void Admin_Should_Be_Able_To_Deactivate_Game()
    {
        
    }

    [Fact]
    public void Admin_Should_Be_Able_To_Update_Game_Price()
    {
       
    }

    [Fact]
    public void Should_Throw_Exception_When_Updating_Price_With_Negative_Value()
    {
      
    }

    [Fact]
    public void Should_Allow_Updating_Price_To_Zero()
    {
        
    }

    [Fact]
    public void Game_Should_Validate_All_Required_Fields()
    {
        
    }

    [Fact]
    public void Game_Should_Allow_Multiple_Categories()
    {
        
    }

    
}
