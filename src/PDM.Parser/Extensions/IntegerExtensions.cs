using System.Globalization;

namespace PDM.Parser.Extensions;

internal static class IntegerExtensions
{
    internal static string GetFullyQualifiedKey(this int value, string prefix)
        => value.ToString(CultureInfo.InvariantCulture).GetFullyQualifiedKey(prefix);
}
