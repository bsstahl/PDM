namespace Protot.Extensions;

internal static class IntExtensions
{
    internal static int TypeIntValue(this string value)
    {
        if (!int.TryParse(value, out int returnVal))
        {
            throw new InvalidCastException($"{value} is Invalid to cast to Int");
        }

        return returnVal;
    }
}