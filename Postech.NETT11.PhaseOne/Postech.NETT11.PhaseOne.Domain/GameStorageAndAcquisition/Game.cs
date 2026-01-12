using Postech.NETT11.PhaseOne.Domain.Entities;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition.Enums;

namespace Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition;

public class Game:BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Developer { get; private set; }
    public string Publisher { get; private set; }
    public decimal? Price { get; private set; }
    public GameStatus IsActive { get; private set; }
    public DateTime? ReleaseDate { get; private set; }
    public List<GameCategory>? Categories { get; set; }

    public Game(string name, string description, string developer, string publisher, decimal price, GameStatus status, DateTime releaseDate)
    {
        SetName(name);
        SetDeveloper(developer);
        SetPublisher(publisher);
        SetPrice(price);
        SetReleaseDate(releaseDate);
        SetStatus(status);
    }
    
    //Todo: Mudar forma de validação para não usar exceptions
    private void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Name cannot be null or empty.");
        }
        Name = name;
    }
    
    private void SetDeveloper(string developer)
    {
        if (string.IsNullOrWhiteSpace(developer))
        {
            throw new ArgumentException("Developer cannot be null or empty.");
        }
        Developer = developer;
    }

    private void SetPublisher(string publisher)
    {
        if (string.IsNullOrWhiteSpace(publisher))
        {
            throw new ArgumentException("Publisher cannot be null or empty.");
        }
        Publisher = publisher;
    }
    
    private void SetPrice(decimal price)
    {
        if (price < 0)
        {
            throw new ArgumentException("Price cannot be negative.");
        }
        Price = price;
    }
    
    private void SetReleaseDate(DateTime releaseDate)
    {
        if (releaseDate > DateTime.UtcNow)
        {
            throw new ArgumentException("Release date cannot be in the future.");
        }
        ReleaseDate = releaseDate;
    }
    
    private void SetStatus(GameStatus status)
    {
        IsActive = status;
    }
    
    public void AddCategory(GameCategory category)
    {
        if (Categories == null)
        {
            Categories = new List<GameCategory>();
        }
        Categories.Add(category);
    }
    
    public void AddCategories(List<GameCategory> categories)
    {
        if (Categories == null)
        {
            Categories = new List<GameCategory>();
        }
        
        Categories = categories.Union(Categories).ToList();
    }
}