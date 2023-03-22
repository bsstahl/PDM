using System.Text;

namespace PDM.WireTypes;

public sealed class Len
{
    private readonly byte[] _rawData = Array.Empty<byte>();

    public byte[] GetRawData()
        => _rawData;

    public int WireLength 
        => _rawData.Length;

    public string Value 
        => CalculateValue(_rawData);


    public Len(IEnumerable<byte> rawData)
        => _rawData = rawData.ToArray();

    public Len(string value)
        => _rawData = Encoding.UTF8.GetBytes(value);

    public static Len Parse(byte[] message) 
        => throw new NotImplementedException();

    public static Len Create(object value)
    {
        return value switch
        {
            IEnumerable<byte> => new Len((IEnumerable<byte>)value),
            string => CreateFromString((string)value ?? string.Empty),
            _ => throw new NotImplementedException($"{value} ({value.GetType().Name}) cannot be parsed as a numeric")
        };
    }

    private static Len CreateFromString(string value) 
        => throw new NotImplementedException();

    private static string CalculateValue(byte[] bytes) 
        => Encoding.UTF8.GetString(bytes);

    public static Len Empty
        => new Len(Array.Empty<byte>());

}
