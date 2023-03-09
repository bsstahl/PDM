using Protot.Entities;

namespace Protot.Extensions;

internal static class ParseExtensions
{
    internal static ProtoExpresssion? ToProtoExpression(this string line)
    {
        var parts = line.SplitFrom('=');
        if (parts.Length != 2)
        {
            throw new Exception($"{line} is Proto Expression");
        }

        return new ProtoExpresssion(parts.FirstOrDefault(), parts.LastOrDefault()).IsValid();
    }
}