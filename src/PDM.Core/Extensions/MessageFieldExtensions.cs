using PDM.Entities;

namespace PDM.Extensions;

public static class MessageFieldExtensions
{
    public static MessageField? Get(this IEnumerable<MessageField> messageFields, int[] keys)
    {
        return keys is null || !keys.Any()
            ? null
            : keys.Length switch
            {
                1 => messageFields.SingleOrDefault(f => f.Key == keys[0]),
                > 1 => throw new NotImplementedException(),
                _ => throw new InvalidOperationException("Unreachable code reached")
            };
    }
}
