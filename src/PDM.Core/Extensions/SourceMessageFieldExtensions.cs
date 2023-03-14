using PDM.Entities;

namespace PDM.Extensions;

internal static class SourceMessageFieldExtensions
{
    internal static IEnumerable<SourceMessageField> RootFields(this IEnumerable<SourceMessageField> sourceFields) 
        => sourceFields.Where(f => !f.HasParent);

}
