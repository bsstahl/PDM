using System.Globalization;

namespace PDM.WireTypes;

public sealed record Varint
{
    private readonly byte[] _rawData = Array.Empty<byte>();

    public byte[] GetRawData()
        => _rawData;

    public int WireLength => _rawData.Length;

#pragma warning disable CS3003 // Type is not CLS-compliant
    public ulong Value => CalculateValue(DecodedData);
#pragma warning restore CS3003 // Type is not CLS-compliant


    internal Varint(byte[] rawData)
        => _rawData = rawData;

    public Varint(long value)
        : this((ulong)value)
    { }

    public Varint(double value)
        : this((ulong)value)
    { }

    internal Varint(ulong value)
    {
        var rawData = new List<byte>();
        var currentValue = value;

        if (currentValue == 0)
            rawData.Add(0);
        else
            while (currentValue > 0)
            {
                var currentByte = currentValue > 127
                    ? Convert.ToByte(currentValue & 0x7F | 0x80)
                    : Convert.ToByte(currentValue & 0x7F);
                rawData.Add(currentByte);
                currentValue >>= 7;
            }

        _rawData = rawData.ToArray();
    }

    public static Varint Parse(byte[] message)
    {
        var result = Varint.Empty;

        if (message is not null && message.Length > 0)
        {
            bool ok = true;
            var length = 1;
            var currentByte = message[0];
            while (ok && currentByte > 127)
            {
                if (length >= message.Length)
                    ok = false;
                else
                {
                    currentByte = message[length];
                    length++;
                }
            }

            if (ok)
                result = new Varint(message[0..length]);
        }

        return result;
    }

    public static Varint Create(object value)
    {
        return value switch
        {
            long => new Varint((long)value),
            ulong => new Varint((ulong)value),
            double => new Varint((double)value),
            string => CreateFromString((string)value ?? string.Empty),
            _ => throw new NotImplementedException($"{value} ({value.GetType().Name}) cannot be parsed as a numeric")
        };
    }

    private static Varint CreateFromString(string value)
    {
        Varint result;
        if (long.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
            result = new Varint(longValue);
        else if (ulong.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var ulongValue))
            result = new Varint(ulongValue);
        else if (double.TryParse(value.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue))
            result = new Varint(doubleValue);
        else
            throw new NotImplementedException("{value} cannot be parsed as a numeric");
        return result;
    }

    private byte[] DecodedData
        => GetRawData()
        .Select(d => Convert.ToByte(d & 127))
        .ToArray();

    private static ulong CalculateValue(byte[] decodedBytes)
    {
        ulong total = 0;
        for (int i = decodedBytes.Length - 1; i >= 0; i--)
            total = total << 7 | decodedBytes[i];
        return total;
    }

    public static Varint Empty
        => new Varint(Array.Empty<byte>());

}

