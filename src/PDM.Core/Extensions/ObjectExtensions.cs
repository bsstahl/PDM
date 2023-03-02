namespace PDM.Extensions;

public static class ObjectExtensions
{
    /// <summary>
    /// Returns True if the supplied value is assignable to a byte array
    /// otherwise returns False. Null values return true.
    /// </summary>
    public static bool IsByteArray(this object? value)
        => value?.GetType().IsAssignableTo(typeof(byte[])) ?? true;

    /// <summary>
    /// Returns the length of a byte array or 0 if the value is not an array
    /// </summary>
    public static int GetByteArrayLength(this object? value)
    {
        var result = 0;
        if (value is not null && value.IsByteArray())
        {
            result = ((byte[])value).Length;
        }

        return result;
    }

}

