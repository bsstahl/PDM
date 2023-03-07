using PDM.Entities;
using PDM.Extensions;
using System.Globalization;
using System.Runtime.InteropServices;

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
            .Select(x => x.Value)
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

    internal static ulong ParseVarint(this string value)
    {
        ulong g;
        if (bool.TryParse(value, out var boolValue))
            g = Convert.ToUInt64(boolValue);
        else if (ulong.TryParse(value, out var ulongValue))
            g = ulongValue;
        else if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
            g = BitConverter.ToUInt64(BitConverter.GetBytes(longValue));
        //else if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var f))
        //    g = f;
        else
            throw new NotImplementedException();

        //byte[] b;
        //if (g is byte)
        //    b = new byte[] { g };
        //else
        //    b = BitConverter.IsLittleEndian
        //        ? BitConverter.GetBytes(g)
        //        : BitConverter.GetBytes(g).Reverse();

        //return b;

        return g;
    }

    internal static byte[] ParseI32(this string value)
    {
        dynamic g;
        if (uint.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var x))
            g = x;
        else if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var y))
            g = y;
        else if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var f))
            g = f;
        else
            throw new NotImplementedException();

        byte[] b = BitConverter.IsLittleEndian
            ? BitConverter.GetBytes(g)
            : BitConverter.GetBytes(g).Reverse();

        return b;
    }

}
