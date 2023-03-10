using PDM.Entities;
using System.Globalization;

namespace PDM.Extensions;

internal static class StringExtensions
{
    internal static string ToLiteralExpression(this string value, Enums.WireType wireType)
        => wireType switch
        {
            Enums.WireType.VarInt => value.ParseVarint().ToString(CultureInfo.InvariantCulture),
            Enums.WireType.I64 => Convert.ToHexString(value.ParseI64()),
            Enums.WireType.Len => value,
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
    
    internal static IEnumerable<(int[], int)> ParseFieldPairs(this string value, CultureInfo formatProvider)
    {
        var results = new List<(int[], int)>();
        var fieldPairs = value.Split(',');
        foreach (var fieldPair in fieldPairs)
        {
            results.Add(fieldPair
                .ParseFieldPair(formatProvider));
        }

        return results;
    }

    internal static (int[], int) ParseFieldPair(this string fieldPair, CultureInfo formatProvider)
    {
        var (item1, item2) = fieldPair.ParsePair();

        var sourceKeys = item1
            .Split('.', StringSplitOptions.RemoveEmptyEntries)
            .Select(item => int.TryParse(item, NumberStyles.Integer, formatProvider, out var result)
                ? result
                : (int?)null)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .ToArray();

        _ = int.TryParse(item2, NumberStyles.Integer, formatProvider, out var targetKey);

        return (sourceKeys, targetKey);
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

    internal static dynamic ParseVarint(this string value)
    {
        dynamic g;
        if (bool.TryParse(value, out var boolValue))
            g = Convert.ToUInt64(boolValue);
        else if (byte.TryParse(value, out var byteValue))
            g = byteValue;
        else if (uint.TryParse(value, out var uintValue))
            g = uintValue;
        else if (int.TryParse(value, out var intValue))
            g = intValue;
        else if (ulong.TryParse(value, out var ulongValue))
            g = ulongValue;
        else
        {
            _ = long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue);
            g = longValue;
        }

        return g;
    }

    internal static byte[] ParseI32(this string value)
    {
        object g;
        if (uint.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var x))
            g = x;
        else if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var y))
            g = y;
        else
        {
            _ = float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var f);
            g = f;
        }

        return g.ToLittleEndianBytes();
    }

    internal static byte[] ParseI64(this string value)
    {
        object g;
        if (ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var x))
            g = x;
        else if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var y))
            g = y;
        else
        {
            _ = double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var f);
            g = f;
        }

        return g.ToLittleEndianBytes();
    }

    internal static bool IsValidHexString(this string value)
        => value.All(h => Uri.IsHexDigit(h));
}
