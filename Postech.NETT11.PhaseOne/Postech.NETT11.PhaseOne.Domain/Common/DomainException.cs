namespace Postech.NETT11.PhaseOne.Domain.Common;

public class DomainException:Exception
{
    public DomainException(string message) : base(message)
    {
    }
}