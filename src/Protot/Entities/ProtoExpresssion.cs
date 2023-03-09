namespace Protot.Entities;

internal class ProtoExpresssion
{
    internal ProtoExpresssion(string? left, string? right)
    {
        this.Left = left ?? string.Empty;
        this.Right = right ?? string.Empty;
    }

    internal string Left { get; }
    internal string Right { get; }

    internal  ProtoExpresssion? IsValid()
    {
        if (string.IsNullOrWhiteSpace(this.Left) && string.IsNullOrWhiteSpace(this.Right))
        {
            return null;
        }
        
        return this;
    }
}