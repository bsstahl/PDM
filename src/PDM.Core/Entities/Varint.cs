namespace PDM.Entities;

public sealed record Varint
{
    internal byte[] RawData { get; }

    public int WireLength => this.RawData.Length;

    internal byte[] DecodedData
        => this.RawData
        .Select(d => Convert.ToByte(d & 127))
        .ToArray();

#pragma warning disable CS3003 // Type is not CLS-compliant
    public UInt64 Value => CalculateValue(this.DecodedData);
#pragma warning restore CS3003 // Type is not CLS-compliant


    internal Varint(byte[] rawData) 
        => this.RawData = rawData;

    internal Varint(long value)
        :this((ulong)value)
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
                    ? Convert.ToByte((currentValue & 0x7F) | 0x80)
                    : Convert.ToByte(currentValue & 0x7F);
                rawData.Add(currentByte);
                currentValue >>= 7;
            }

        this.RawData = rawData.ToArray();
    }

    public static Varint Parse(byte[] message)
    {
        if (message is null || message.Length < 1)
            throw new ArgumentNullException(nameof(message));

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

    //public override string ToString()
    //    => this.Value.ToString(CultureInfo.CurrentCulture);
}

