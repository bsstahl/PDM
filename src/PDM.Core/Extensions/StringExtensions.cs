using System.Globalization;

namespace PDM.Extensions;

internal static class StringExtensions
{
    internal static IEnumerable<(int, int)> ParseFieldPairs(this string value, CultureInfo formatProvider)
    {
        var results = new List<(int, int)>();
        var fieldPairs = value.Split(',');
        foreach (var fieldPair in fieldPairs)
        {
            results.Add(fieldPair
                .ParseFieldPair(formatProvider));
        }

        return results;
    }

    private static (int, int) ParseFieldPair(this string fieldPair, CultureInfo formatProvider)
    {
        (int, int) results = (0, 0);

        var pair = fieldPair.Split(':');
        if (pair.Length == 2)
        {
            _ = int.TryParse(pair[0], NumberStyles.Integer, formatProvider, out var sourceKey);
            _ = int.TryParse(pair[1], NumberStyles.Integer, formatProvider, out var targetKey);
            results = (sourceKey, targetKey);
        }

        return results;
    }
}
