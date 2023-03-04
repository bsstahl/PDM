using PDM.Entities;
using System.Text;

namespace PDM.Extensions;

internal static class MappingExtensions
{
    internal static void RemoveField(this List<Mapping> result, int key)
    {
        var item = result.SingleOrDefault(f => f.TargetField.Key == key);
        if (item is not null)
            result.Remove(item);
    }

    internal static void InsertField(this List<Mapping> result, int key, string value)
    {
        MessageField targetField;

        if (int.TryParse(value, out var intValue))
        {
            var i32Payload = BitConverter.GetBytes(intValue);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(i32Payload);
            }
            targetField = new MessageField(key, Enums.WireType.I32, i32Payload);
        }
        else if (long.TryParse(value, out var longValue))
        {
            var i64Payload = BitConverter.GetBytes(longValue);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(i64Payload);
            }
            targetField = new MessageField(key, Enums.WireType.I64, i64Payload);
        }
        else
        {
            var lenPayload = Encoding.UTF8.GetBytes(value);
            targetField = new MessageField(key, Enums.WireType.Len, lenPayload);
        }

        result.Add(new Mapping(targetField, string.Empty));
    }
}
