using PDM.Entities;

namespace PDM.Extensions;

internal static class SourceMessageFieldExtensions
{
    internal static IEnumerable<SourceMessageField> RootFields(this IEnumerable<SourceMessageField> sourceFields)
        => sourceFields.Where(f => !f.HasParent);

    public static SourceMessageField? Get(this IEnumerable<SourceMessageField> messageFields, string key)
        => messageFields.SingleOrDefault(f => f.Key == key);
}
