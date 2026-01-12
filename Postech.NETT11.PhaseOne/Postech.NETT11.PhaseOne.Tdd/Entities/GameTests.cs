using Postech.NETT11.PhaseOne.Tests.Enuns;
using Postech.NETT11.PhaseOne.Tests.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Postech.NETT11.PhaseOne.Tests.Entities
{
    public class GameTests
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Developer { get; private set; }
        public string Publisher { get; private set; }
        public decimal Price { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime ReleaseDate { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public List<EnumCategories> Categories { get; set; }

        protected GameTests() { }

        public GameTests(
            string name,
            string description,
            string developer,
            string publisher,
            decimal price,
            DateTime releaseDate,
            List<EnumCategories> categories
        )
        {
            ValidateName(name);
            ValidateCompany(developer, "developer");
            ValidateCompany(publisher, "publisher");
            ValidatePrice(price);
            ValidateReleaseDate(releaseDate);

            Id = Guid.NewGuid();
            Name = name;
            Description = description ?? string.Empty;
            Developer = developer;
            Publisher = publisher;
            Price = price;
            ReleaseDate = releaseDate;

            IsActive = true;
            CreatedAt = DateTime.UtcNow;

            Categories = categories ?? new List<EnumCategories>();
            }

        public GameTests Deactivate()

        {
            IsActive = false;
            return this;
        }

        public GameTests UpdatePrice(decimal newPrice)
        {
            ValidatePrice(newPrice);
            Price = newPrice;
            return this;
        }

        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("O nome do jogo é obrigatório.");

            if (name.Length < 3)
                throw new DomainException("O nome do jogo deve conter no mínimo 3 caracteres.");
        }

        private void ValidateCompany(string value, string field)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException($"O {field} do jogo é obrigatório.");
        }

        private void ValidatePrice(decimal price)
        {
            if (price < 0)
                throw new DomainException("O preço do jogo não pode ser negativo.");
        }

        private void ValidateReleaseDate(DateTime releaseDate)
        {
            if (releaseDate > DateTime.UtcNow.Date)
                throw new DomainException("A data de lançamento não pode ser futura.");
        }
    }
}
