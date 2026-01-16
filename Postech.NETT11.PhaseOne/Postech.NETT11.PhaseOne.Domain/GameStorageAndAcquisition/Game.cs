using Postech.NETT11.PhaseOne.Domain.Common;
using Postech.NETT11.PhaseOne.Domain.Entities;
using Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition.Enums;

namespace Postech.NETT11.PhaseOne.Domain.GameStorageAndAcquisition;

public class Game : BaseEntity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public string Developer { get; private set; }
    public string Publisher { get; private set; }
    public decimal? Price { get; private set; }
    public GameStatus Status { get; private set; }

    private Game() { }

    public Game(
        string title,
        string description,
        string developer,
        string publisher)
    {
        ValidateAndSetTitle(title);
        ValidateAndSetDescription(description);
        ValidateAndSetDeveloper(developer);
        ValidateAndSetPublisher(publisher);
        
        Status = GameStatus.Active;
    }
    
    public void UpdateTitle(string title) => ValidateAndSetTitle(title);
    public void UpdateDescription(string description) => ValidateAndSetDescription(description);
    public void UpdateDeveloper(string developer) => ValidateAndSetDeveloper(developer);
    public void UpdatePublisher(string publisher) => ValidateAndSetPublisher(publisher);
    
    // Métodos para configurar propriedades opcionais
    public void SetPrice(decimal price)
    {
        if (price < 0)
            throw new DomainException("O preço não pode ser negativo.");
        Price = price;
    }

    public void SetStatus(GameStatus status)
    {
        Status = status;
    }

    // Validações
    private void ValidateAndSetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("O título do jogo não pode ser vazio.");
        if (title.Length > 200)
            throw new DomainException("O título não pode exceder 200 caracteres.");
        Title = title;
    }

    private void ValidateAndSetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("A descrição não pode ser vazia.");
        if (description.Length > 1000)
            throw new DomainException("A descrição não pode exceder 1000 caracteres.");
        Description = description;
    }

    private void ValidateAndSetDeveloper(string developer)
    {
        if (string.IsNullOrWhiteSpace(developer))
            throw new DomainException("O desenvolvedor não pode ser vazio.");
        if (developer.Length > 100)
            throw new DomainException("O desenvolvedor não pode exceder 100 caracteres.");
        Developer = developer;
    }

    private void ValidateAndSetPublisher(string publisher)
    {
        if (string.IsNullOrWhiteSpace(publisher))
            throw new DomainException("A editora não pode ser vazia.");
        if (publisher.Length > 100)
            throw new DomainException("A editora não pode exceder 100 caracteres.");
        Publisher = publisher;
    }

}