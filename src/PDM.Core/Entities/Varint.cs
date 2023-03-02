namespace PDM.Entities;

internal sealed record Varint
{
    readonly byte[] _rawData;

    internal Varint(byte[] rawData)
    {
        _rawData = rawData;
    }

    internal Varint(UInt64 value)
    {
        var rawData = new List<byte>();
        var currentValue = value;

        while (currentValue > 0)
        {
            var currentByte = currentValue > 127 
                ? Convert.ToByte((currentValue & 0x7F) | 0x80) 
                : Convert.ToByte(currentValue & 0x7F);
            rawData.Add(currentByte);
            currentValue >>= 7;
        }

        _rawData = rawData.ToArray();
    }

    internal byte[] RawData => _rawData;
    internal int WireLength => _rawData.Length;
    internal byte[] DecodedData 
        => _rawData.Select(d => Convert.ToByte(d & 127)).ToArray();
    internal UInt64 Value => CalculateValue(this.DecodedData);

    internal static Varint Parse(byte[] message)
    {
        var length = 1;
        var currentByte = message[0];
        while (currentByte > 127)
        {
            currentByte = message[length];
            length++;
        }
        return new Varint(message[0..length]);
    }

    private static UInt64 CalculateValue(byte[] decodedBytes)
    {
        UInt64 total = 0;
        for (int i = decodedBytes.Length - 1; i >= 0; i--)
            total = (total << 7) | decodedBytes[i];
        return total;
    }
}

