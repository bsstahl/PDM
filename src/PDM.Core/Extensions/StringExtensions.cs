using PDM.Entities;
using System.Globalization;

namespace PDM.Extensions;

public static class StringExtensions
{
    public static bool IsValidHexString(this string value)
        => value.All(h => Uri.IsHexDigit(h));

    internal static string MapExpression(this string fieldNumber)
        => $"s => (s.Key == \"{fieldNumber}\")";

    internal static IEnumerable<int> AsTargetKey(this string key)
        => key.Split('.')
        .Select(k => int.Parse(k, CultureInfo.InvariantCulture));

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

    internal static TagLengthValue ParseTLV(this string value)
    {
        var results = new TagLengthValue();
        var fields = value.Split(':');
        if (fields.Length != 3)
            throw new ArgumentException("Invalid value for TLV");

        var key = fields[0];
        var wireType = Enum.Parse<Enums.WireType>(fields[1]);
        var dataField = fields[2];

        results.Key = key;
        results.WireType = wireType;
        results.Value = dataField.ToLiteralExpression(wireType);

        return results;
    }

    internal static IEnumerable<(string, IEnumerable<int>)> ParseFieldPairs(this string value, CultureInfo formatProvider)
    {
        var results = new List<(string, IEnumerable<int>)>();
        var fieldPairs = value.Split(',');
        foreach (var fieldPair in fieldPairs)
        {
            results.Add(fieldPair.ParseFieldPair(formatProvider));
        }

        return results;
    }

    internal static (string, IEnumerable<int>) ParseFieldPair(this string fieldPair, CultureInfo formatProvider)
    {
        var (item1, item2) = fieldPair.ParsePair();

        var targetKeys = item2
            .Split('.', StringSplitOptions.RemoveEmptyEntries)
            .Select(item => int.TryParse(item, NumberStyles.Integer, formatProvider, out var result)
                ? result
                : (int?)null)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .ToArray();

        return (item1, targetKeys);
    }

    internal static (string, string) ParsePair(this string pair)
    {
        (string, string) results = (String.Empty, String.Empty);

        var itemPair = pair.Split(':');
        if (itemPair.Length == 1)
            results = (itemPair[0], string.Empty);
        else if (itemPair.Length == 2)
            results = (itemPair[0], itemPair[1]);
        else
            throw new InvalidOperationException();

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
        else if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
            g = longValue;
        else if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var floatValue))
            g = floatValue;
        else if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue))
            g = doubleValue;
        else
            throw new NotImplementedException($"{value} cannot be parsed as a numeric");

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

}
