using Postech.NETT11.PhaseOne.Domain.Entities;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition.Enums;

namespace Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition;

public class GameBuilder
{
    private Game _game;

    public GameBuilder()
    {
        _game = new Game();
    }

    public GameBuilder(Game game)
    {
        _game = game;
    }
    
    public Game Build()
    {
        //TODO: Validar se game pode ser construido
        return _game;
    }
    
    public GameBuilder WithTitle(string title)
    {
        _game.Title = title;
        return this;
    }

    public GameBuilder WithDescription(string description)
    {
        _game.Description = description;
        return this;
    }
    
    public GameBuilder WithDeveloper(string developer)
    {
        _game.Developer = developer;
        return this;
    }
    
    public GameBuilder WithPublisher(string publisher)
    {
        _game.Publisher = publisher;
        return this;
    }
    
    public GameBuilder WithPrice(decimal price)
    {
        _game.Price = price;
        return this;
    }
    
    public GameBuilder WithReleaseDate(DateTime releaseDate)
    {
        _game.ReleaseDate = releaseDate;
        return this;
    }
    
    public GameBuilder WithStatus(GameStatus status)
    {
        _game.Status = status;
        return this;
    }
    
    public GameBuilder WithCategories(List<GameCategory> categories)
    {
        _game.Categories = categories;
        return this;
    }

    public GameBuilder AddCategory(GameCategory category)
    {
        if (_game.Categories == null)
            _game.Categories = new List<GameCategory>();
        _game.Categories.Add(category);
        return this;
    }
}

public class Game:BaseEntity
{
    public string Title { get; internal set; }
    public string Description { get; internal set; }
    public string Developer { get; internal set; }
    public string Publisher { get; internal set; }
    public decimal? Price { get; internal set; }
    public GameStatus Status { get; internal set; }
    public DateTime? ReleaseDate { get; internal set; }
    public List<GameCategory>? Categories { get; set; }

    public Game()
    {
        
    }
}