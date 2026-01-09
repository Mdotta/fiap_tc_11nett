using Postech.NETT11.PhaseOne.Tests.Compras.Exceptions;

namespace Postech.NETT11.PhaseOne.Tests.Compras.Entities;

public class Compra
{
    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public Guid JogoId { get; private set; }
    public decimal ValorPago { get; private set; }
    public DateTime DataCompra { get; private set; }

    protected Compra() { }

    public Compra(Guid usuarioId, Guid jogoId, decimal valorPago)
    {
        ValidarUsuarioId(usuarioId);
        ValidarJogoId(jogoId);
        ValidarValorPago(valorPago);

        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        JogoId = jogoId;
        ValorPago = valorPago;
        DataCompra = DateTime.UtcNow;
    }

    private void ValidarUsuarioId(Guid usuarioId)
    {
        if (usuarioId == Guid.Empty)
            throw new DomainException("O ID do usuário é obrigatório.");
    }

    private void ValidarJogoId(Guid jogoId)
    {
        if (jogoId == Guid.Empty)
            throw new DomainException("O ID do jogo é obrigatório.");
    }

    private void ValidarValorPago(decimal valorPago)
    {
        if (valorPago < 0)
            throw new DomainException("O valor pago não pode ser negativo.");
    }
}
