namespace Postech.NETT11.PhaseOne.Tests.Usuarios.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message)
        : base(message)
    {
    }
}
