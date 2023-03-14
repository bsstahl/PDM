namespace PDM.Parser.Extensions;

internal static class StringExtensions
{
    internal static string GetFullyQualifiedKey(this string value, string prefix)
    {
        return string.IsNullOrWhiteSpace(prefix) 
            ? value.ToString() 
            : $"{prefix}.{value}";
    }
}
