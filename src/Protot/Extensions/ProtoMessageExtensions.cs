using Protot.Builder;
using Protot.Constants;
using Protot.Entities;
using Protot.Enum;

namespace Protot.Extensions;

internal static class ProtoMessageExtensions
{
    internal static ProtoMessage? ToProtoMessage(this IList<string>? lines)
    {
        if (lines == null || !lines.Any())
        {
            return null;
        }

        var builder = new ProtoMessageBuilder();
        foreach (var line in lines)
        {
            if (line.ContainsValue(ProtoIdentifiers.Message))
            {
                var lineParts = line.SplitFrom(' ');
                builder.CreateMessage(lineParts.Last());
                continue;
            }
            var fieldStatement = line.ToProtoExpression();
            if (fieldStatement == null)
            {
                throw new FileLoadException($"{line} is Invalid to Get Field Information");
            }
           
            var fileNameAndType = fieldStatement?.Left.SplitFrom(' ');
            if (fileNameAndType == null || fileNameAndType.Length > 3)
            {
                throw new FileLoadException($"{line} is Invalid to Get Field Information");
            }

            var fieldName = fileNameAndType.LastOrDefault().ValidateString();
            var type = fieldStatement?.Left.ReplaceWith(fieldName, string.Empty);
            builder.AddField(new Field()
            {
                TypeAsString = type?.Trim(),
                Name = fieldName.Trim(),
                Index = fieldStatement?.Right.TypeIntValue()
            });
        }
        return builder.Build();
    }

    internal static ProtoEnum? ToProtoEnum(this IList<string>? lines)
    {
        if (lines == null || !lines.Any())
        {
            return null;
        }

        var builder = new ProtoEnumBuilder();
        foreach (var line in lines)
        {
            if (line.ContainsValue(ProtoIdentifiers.Enum))
            {
                var lineParts = line.SplitFrom(' ');
                builder.CreateEnum(lineParts.Last());
                continue;
            }
            var value = line.ContainsValue("=")? line.ToProtoExpression()?.Left: line;
            if (value == null)
            {
                throw new FileLoadException($"{line} is Invalid to Get Field Information");
            }
            
            builder.AddValue(value);
        }
        return builder.Build();
    }
    
    private static string ValidateString(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidDataException($"{value} is Invalid Data");
        }

        return value;
    }
}