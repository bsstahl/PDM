using PDM.Entities;

namespace PDM.Extensions;

public static class MessageFieldExtensions
{
    public static SourceMessageField? Get(this IEnumerable<SourceMessageField> messageFields, string key)
        => messageFields.SingleOrDefault(f => f.Key == key);
}
