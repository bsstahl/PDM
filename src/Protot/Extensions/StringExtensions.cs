using Protot.Entities;

namespace Protot.Extensions;

internal static class StringExtensions
{
    internal static string ReplaceWith(this string line, string value, string replaceWIth)
    {
        return line.Replace(value, string.Empty, StringComparison.CurrentCultureIgnoreCase);
    }
    
    internal static bool EqualValue(this string line, string value)
    {
        return line.Equals(value, StringComparison.InvariantCulture);
        
    }
    internal static bool ContainsValue(this string line, string value)
    {
        return line.Contains(value, StringComparison.InvariantCulture);
    }
    
    internal static string[] SplitFrom(this string line, char splitFrom)
    {
        return line.Split(splitFrom, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
    }
}