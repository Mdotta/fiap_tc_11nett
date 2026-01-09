namespace Postech.NETT11.PhaseOne.Tests.Biblioteca.Entities;

public class BibliotecaUsuario
{
    public Guid Id { get; private set; }
    public Guid UsuarioId { get; private set; }
    public List<JogoBiblioteca> Jogos { get; private set; }

    protected BibliotecaUsuario() 
    {
        Jogos = new List<JogoBiblioteca>();
    }

    public BibliotecaUsuario(Guid usuarioId)
    {
        if (usuarioId == Guid.Empty)
            throw new ArgumentException("O ID do usuário é obrigatório.", nameof(usuarioId));

        Id = Guid.NewGuid();
        UsuarioId = usuarioId;
        Jogos = new List<JogoBiblioteca>();
    }

    public void AdicionarJogo(Guid jogoId, Guid compraId)
    {
        if (jogoId == Guid.Empty)
            throw new ArgumentException("O ID do jogo é obrigatório.", nameof(jogoId));

        if (compraId == Guid.Empty)
            throw new ArgumentException("O ID da compra é obrigatório.", nameof(compraId));

        if (PossuiJogo(jogoId))
            throw new InvalidOperationException("O jogo já está na biblioteca do usuário.");

        Jogos.Add(new JogoBiblioteca(jogoId, compraId));
    }

    public bool PossuiJogo(Guid jogoId)
    {
        return Jogos.Any(j => j.JogoId == jogoId);
    }

    public JogoBiblioteca? ObterJogo(Guid jogoId)
    {
        return Jogos.FirstOrDefault(j => j.JogoId == jogoId);
    }

    public int TotalJogos => Jogos.Count;
}

public class JogoBiblioteca
{
    public Guid JogoId { get; private set; }
    public Guid CompraId { get; private set; }
    public DateTime DataAdicao { get; private set; }

    protected JogoBiblioteca() { }

    public JogoBiblioteca(Guid jogoId, Guid compraId)
    {
        JogoId = jogoId;
        CompraId = compraId;
        DataAdicao = DateTime.UtcNow;
    }
}
