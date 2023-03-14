namespace PDM.TestUtils.Extensions;

[ExcludeFromCodeCoverage]
internal static class ByteExtensions
{
    public static byte[] GetRandom(this byte[] _)
    {
        var len = 100.GetRandom(8);

        var result = new List<byte>();
        for (var i = 0; i < len; i++)
        {
            result.Add(Convert.ToByte(255.GetRandom(0)));
        }

        return result.ToArray();
    }
}
