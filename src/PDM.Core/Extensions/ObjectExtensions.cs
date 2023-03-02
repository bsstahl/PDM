namespace PDM.Extensions;

internal static class ObjectExtensions
{
    internal static bool IsByteArray(this object? value)
    {
        return value?.GetType().IsAssignableTo(typeof(byte[])) ?? true;
    }

    /// <summary>
    /// Returns the length of a byte array or 0 if the value is not an array
    /// </summary>
    internal static int GetByteArrayLength(this object? value)
    {
        var result = 0;
        if (value is not null && value.IsByteArray())
            result = ((byte[])value).Length;
        return result;
    }

}

