using PDM.Entities;

namespace PDM.Extensions;

internal static class MappingExtensions
{
    internal static void RemoveField(this List<Mapping> result, int key)
    {
        var item = result.SingleOrDefault(f => f.TargetField.Key == key);
        if (item is not null)
            result.Remove(item);
    }

}
