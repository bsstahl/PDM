namespace Protot.Exceptions;

internal sealed class ProtoFileParserException : Exception
{
    public ProtoFileParserException()
    {
    }

    public ProtoFileParserException(string message)
        : base(message)
    {
    }

    public ProtoFileParserException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}