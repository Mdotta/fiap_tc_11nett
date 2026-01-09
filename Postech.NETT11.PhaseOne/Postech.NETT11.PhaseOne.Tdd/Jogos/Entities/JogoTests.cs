using Postech.NETT11.PhaseOne.Tests.Jogos.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postech.NETT11.PhaseOne.Tests.Jogos.Entities
{
    public class JogoTests
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Descricao { get; private set; }
        public string Desenvolvedor { get; private set; }
        public string Distribuidora { get; private set; }
        public decimal Preco { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime DataLancamento { get; private set; }
        public DateTime DataCriacao { get; private set; }

        public List<CategoriaTests> Categorias { get; set; }

        protected JogoTests() { }

        public JogoTests(
            string nome,
            string descricao,
            string desenvolvedor,
            string distribuidora,
            decimal preco,
            DateTime dataLancamento,
            List<CategoriaTests> categorias
        )
        {
            ValidarNome(nome);
            ValidarEmpresa(desenvolvedor, "desenvolvedor");
            ValidarEmpresa(distribuidora, "distribuidora");
            ValidarPreco(preco);
            ValidarDataLancamento(dataLancamento);

            Id = Guid.NewGuid();
            Nome = nome;
            Descricao = descricao ?? string.Empty;
            Desenvolvedor = desenvolvedor;
            Distribuidora = distribuidora;
            Preco = preco;
            DataLancamento = dataLancamento;

            Ativo = true;
            DataCriacao = DateTime.UtcNow;

            Categorias = categorias ?? new List<CategoriaTests>();
        }

        public JogoTests Desativar()
        {
            Ativo = false;
            return this;
        }

        public JogoTests AtualizarPreco(decimal novoPreco)
        {
            ValidarPreco(novoPreco);
            Preco = novoPreco;
            return this;
        }

        private void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("O nome do jogo é obrigatório.");

            if (nome.Length < 3)
                throw new DomainException("O nome do jogo deve conter no mínimo 3 caracteres.");
        }

        private void ValidarEmpresa(string valor, string campo)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new DomainException($"O {campo} do jogo é obrigatório.");
        }

        private void ValidarPreco(decimal preco)
        {
            if (preco < 0)
                throw new DomainException("O preço do jogo não pode ser negativo.");
        }

        private void ValidarDataLancamento(DateTime dataLancamento)
        {
            if (dataLancamento > DateTime.UtcNow.Date)
                throw new DomainException("A data de lançamento não pode ser futura.");
        }
    }
}
