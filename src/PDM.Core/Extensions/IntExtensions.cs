namespace PDM.Extensions;

internal static class IntExtensions
{
    public static string AsSourceKey(this IEnumerable<int> key)
        => string.Join(',', key);

    internal static ulong ZZEncode(this long value)
        => (ulong)(value << 1) ^ (ulong)(value >> 63);

    internal static uint ZZEncode(this int value)
        => (uint)(value << 1) ^ (uint)(value >> 31);

}
