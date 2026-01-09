using Postech.NETT11.PhaseOne.Tests.Jogos.Exceptions;

namespace Postech.NETT11.PhaseOne.Tests.Jogos.Entities
{
    public class CategoriaTests
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }

        protected CategoriaTests() { }

        public CategoriaTests(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome da categoria é obrigatório.");

            Id = Guid.NewGuid();
            Nome = nome;
        }
    }
}