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
            var pair = fieldPair.Split(':');
            if (pair.Length == 2)
            {
                if (int.TryParse(pair[0], NumberStyles.Integer, formatProvider, out var sourceKey)
                    && int.TryParse(pair[1], NumberStyles.Integer, formatProvider, out var targetKey))
                {
                    results.Add((sourceKey, targetKey));
                }
            }
        }

        return results;
    }
}
