using PDM.Extensions;
using System.Globalization;

namespace PDM.WireTypes;

public sealed record I64
{
    private readonly byte[] _rawData = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };

    public byte[] GetRawData()
        => _rawData;

    public int WireLength
        => _rawData.Length;

    internal ulong Value
        => CalculateValue(_rawData);


    internal I64(byte[] rawData)
        => _rawData = rawData;

    public I64(long value)
        : this((ulong)value)
    { }

    public I64(double value)
        => _rawData = value.ToLittleEndianBytes();

    internal I64(ulong value)
        => _rawData = value.ToLittleEndianBytes();

    /// <summary>
    /// Parses the first 8 bytes out of the message
    /// to create an I64 value
    public static I64 Parse(byte[] message)
    {
        return message is null || message.Length < 8
            ? I64.Empty
            : new I64(message[0..8]);
    }

    public static I64 Create(object value)
    {
        return value switch
        {
            long => new I64((long)value),
            ulong => new I64((ulong)value),
            double => new I64((double)value),
            string => CreateFromString((string)value ?? string.Empty),
            _ => throw new NotImplementedException($"{value} ({value.GetType().Name}) cannot be parsed as a numeric")
        };
    }

    private static I64 CreateFromString(string value)
    {
        I64 result;
        if (long.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
            result = new I64(longValue);
        else if (ulong.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var ulongValue))
            result = new I64(ulongValue);
        else if (double.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue))
            result = new I64(doubleValue);
        else
            throw new NotImplementedException("{value} cannot be parsed as a numeric");
        return result;
    }

    private static ulong CalculateValue(byte[] value)
        => BitConverter.ToUInt64(value);

    public static I64 Empty
        => new I64(0);
}

