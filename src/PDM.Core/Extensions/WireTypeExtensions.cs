namespace PDM.Extensions;

internal static class WireTypeExtensions
{
    /// <summary>
    /// Returns false if the value cannot possibly be of 
    /// the specified wire type, otherwise returns true
    /// </summary>
    internal static bool IsValid(this Enums.WireType wireType, object? value)
    {
        var result = true;

        if (value is not null)
            switch (wireType)
            {
                case Enums.WireType.VarInt:
                    result = value.GetType().IsValueType;
                    break;
                case Enums.WireType.I64:
                    result = value.GetByteArrayLength() == 8;
                    break;
                case Enums.WireType.Len:
                    result = typeof(byte[]).IsAssignableFrom(value.GetType());
                    break;
                case Enums.WireType.SGroup:
                    result = true;
                    break;
                case Enums.WireType.EGroup:
                    result = true;
                    break;
                case Enums.WireType.I32:
                    result = value.GetByteArrayLength() == 4;
                    break;
                default:
                    throw new InvalidOperationException("Unreachable code reached");
            }

        return result;
    }
}
