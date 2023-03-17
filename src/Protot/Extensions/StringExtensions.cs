namespace Protot.Extensions;

internal static class StringExtensions
{
    internal static bool StartArgsWith(this string value, string startsWith)
    {
        return value.StartsWith(startsWith, StringComparison.InvariantCultureIgnoreCase);
    }
}