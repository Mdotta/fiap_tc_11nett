using FluentAssertions;
using Postech.NETT11.PhaseOne.Tests.Jogos.Entities;
using Postech.NETT11.PhaseOne.Tests.Jogos.Exceptions;
using Xunit;

namespace Postech.NETT11.PhaseOne.Tests.Jogos.Infra
{
    public class HelperTests
    {
        public static List<CategoriaTests> CriarCategoriasPadrao()
        {
            return new List<CategoriaTests>
            {
                new CategoriaTests("Ação"),
                new CategoriaTests("Aventura"),
                new CategoriaTests("Multijogador")
            };
        }

        public static JogoTests CriarJogoValido()
        {
            return new JogoTests(
                "Jogo Teste",
                "Descrição do jogo",
                "Desenvolvedor Teste",
                "Distribuidora Teste",
                99.90m,
                DateTime.UtcNow.AddDays(-1),
                CriarCategoriasPadrao()
            );
        }
    }
}
