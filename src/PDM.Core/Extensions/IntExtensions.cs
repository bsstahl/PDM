namespace PDM.Extensions;

internal static class IntExtensions
{
    internal static string MapExpression(this int fieldNumber)
        => $"s => (s.Key == {fieldNumber})";
}
