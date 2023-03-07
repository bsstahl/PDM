using PDM.Entities;
using PDM.Extensions;
using System.Globalization;

namespace PDM.Extensions;

internal static class StringExtensions
{
    internal static string ToLiteralExpression(this string value, Enums.WireType wireType)
        => wireType switch
        {
            Enums.WireType.VarInt => value,
            Enums.WireType.I64 => throw new NotImplementedException(),
            Enums.WireType.Len => $"s => new object {{ Value=\"{value}\" }}",
            Enums.WireType.SGroup | Enums.WireType.EGroup => value,
            Enums.WireType.I32 => Convert.ToHexString(value.ParseI32()),
            _ => throw new InvalidOperationException("Unreachable code reached")
        };

    internal static TagLengthValue ParseTLV(this string value, CultureInfo formatProvider)
    {
        var results = new TagLengthValue();
        var fields = value.Split(':');
        if (fields.Length != 3)
            throw new ArgumentException("Invalid value for TLV");

        _ = int.TryParse(fields[0], NumberStyles.Number, formatProvider, out var key);
        var wireType = Enum.Parse<Enums.WireType>(fields[1]);
        var dataField = fields[2];

        results.Key = key;
        results.WireType = wireType;
        results.Value = dataField.ToLiteralExpression(wireType);

        return results;
    }

    internal static IEnumerable<(int, int)> ParseFieldPairs(this string value, CultureInfo formatProvider)
    {
        var results = new List<(int, int)>();
        var fieldPairs = value.Split(',');
        foreach (var fieldPair in fieldPairs)
        {
            results.Add(fieldPair
                .ParseFieldPair(formatProvider));
        }

        return results;
    }

    internal static (int, int) ParseFieldPair(this string fieldPair, CultureInfo formatProvider)
    {
        var pair = fieldPair.ParsePair();
        _ = int.TryParse(pair.Item1, NumberStyles.Integer, formatProvider, out var sourceKey);
        _ = int.TryParse(pair.Item2, NumberStyles.Integer, formatProvider, out var targetKey);
        return (sourceKey, targetKey);
    }

    internal static (string, string) ParsePair(this string pair)
    {
        (string, string) results = (String.Empty, String.Empty);

        var itemPair = pair.Split(':');
        if (itemPair.Length == 2)
        {
            results = (itemPair[0], itemPair[1]);
        }

        return results;
    }

    internal static byte[] ParseI32(this string value)
    {
        var num = float.Parse(value, CultureInfo.InvariantCulture);
        var b = BitConverter.IsLittleEndian
            ? BitConverter.GetBytes(num)
            : BitConverter.GetBytes(num).Reverse();
        return b.ToArray();
    }
}
