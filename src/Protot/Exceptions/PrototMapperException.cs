namespace Protot.Exceptions;

public sealed class PrototMapperException : Exception
{
    public PrototMapperException()
    {
    }

    public PrototMapperException(string message)
        : base(message)
    {
    }

    public PrototMapperException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}