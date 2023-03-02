namespace PDM.Extensions;

internal static class WireTypeExtensions
{
    /// <summary>
    /// Returns false if the value cannot possibly be of 
    /// the specified wire type, otherwise returns true
    /// </summary>
    internal static bool IsValid(this Enums.WireType wireType, object? value)
    {
        return (value is null) || wireType switch
            {
                Enums.WireType.VarInt => value.GetType().IsValueType,
                Enums.WireType.I64 => value.GetByteArrayLength() == 8,
                Enums.WireType.Len => typeof(byte[]).IsAssignableFrom(value.GetType()),
                Enums.WireType.SGroup or Enums.WireType.EGroup => true,
                Enums.WireType.I32 => value.GetByteArrayLength() == 4,
                _ => throw new InvalidOperationException("Unreachable code reached"),
            };
    }
}
