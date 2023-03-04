using PDM.Constants;
using PDM.Entities;
using System.Globalization;
using System.Text;

namespace PDM.Extensions;

internal static class MappingExtensions
{
    private static readonly CultureInfo _formatProvider = CultureInfo.InvariantCulture;

    internal static void Include(this IList<Mapping> mappings, IEnumerable<MessageField> messageFields, IEnumerable<Transformation> transformations)
    {
        var includeTransforms = transformations
            .Where(t => t.IsReplaceField(TransformationSubtype.Include));

        if (!includeTransforms.Any())
        {
            // If there is no "replacefield.include" add all
            // existing fields by default
            mappings.IncludeFields(messageFields);
        }
        else
        {
            // If there is a replacefield.include, use only
            // the fields specified in the include list

            // TODO: Answer the Question: Is it ok to have multiple includes?
            foreach (var includeTransform in includeTransforms)
            {
                var includeFields = includeTransform.Value.Split(',');
                foreach (var includeField in includeFields)
                {
                    // TODO: Handle multiple instances of the same fieldNumber

                    // TODO: Log a warning if the field isn't found
                    var messageField = messageFields.FirstOrDefault(f => f.Key.ToString(_formatProvider) == includeField);
                    if (messageField is not null)
                    {
                        _ = mappings.IncludeField(messageField);
                    }
                }
            }
        }
    }

    internal static void IncludeFields(this IList<Mapping> mappings, IEnumerable<MessageField> messageFields)
    {
        foreach (var messageField in messageFields)
        {
            _ = mappings.IncludeField(messageField);
        }
    }

    internal static Mapping IncludeField(this IList<Mapping> mappings, MessageField messageField)
    {
        return mappings
            .IncludeField(messageField, messageField.Key.MapExpression());
    }

    internal static Mapping IncludeField(this IList<Mapping> mappings, MessageField messageField, string expression)
    {
        var fieldNumber = messageField.Key;
        var targetField = new MessageField(fieldNumber, messageField.WireType);
        var mapping = new Mapping(targetField, expression);
        mappings.Add(mapping);
        return mapping;
    }

    internal static void RemoveField(this List<Mapping> result, int key)
    {
        var item = result.SingleOrDefault(f => f.TargetField.Key == key);
        if (item is not null)
            _ = result.Remove(item);
    }

    internal static byte[] ToByteArray(this IEnumerable<MessageField> messageFields)
    {
        var result = new List<byte>();

        foreach (var messageField in messageFields)
        {
            if (messageField.IsValid)
            {
                var tag = new Tag(messageField.Key, messageField.WireType);

                switch (tag.WireType)
                {
                    case Enums.WireType.VarInt:
                        // messageField.IsValid so it has a Value
                        var varintValue = Convert.ToUInt64(messageField.Value, System.Globalization.CultureInfo.InvariantCulture);
                        var rawData = new Varint(varintValue).RawData;
                        if (rawData.Length > 0)
                        {
                            result.AddRange(tag.AsVarint().RawData);
                            result.AddRange(rawData);
                        }
                        break;
                    case Enums.WireType.I64:
                        // messageField.IsValid so it has a Value
                        if (typeof(byte[]).IsAssignableFrom(messageField.Value!.GetType()))
                        {
                            result.AddRange(tag.AsVarint().RawData);
                            result.AddRange((byte[])messageField.Value!);
                        }
                        break;
                    case Enums.WireType.Len:
                        // messageField.IsValid so it has a Value
                        var lenValue = (byte[])messageField.Value!;
                        if (lenValue.Length > 0)
                        {
                            var lenLength = new Varint(Convert.ToUInt64(lenValue.Length));
                            result.AddRange(tag.AsVarint().RawData);
                            result.AddRange(lenLength.RawData);
                            result.AddRange(lenValue);
                        }
                        break;
                    case Enums.WireType.SGroup:
                    case Enums.WireType.EGroup:
                        break;
                    case Enums.WireType.I32:
                        // messageField.IsValid so it has a Value
                        if (typeof(byte[]).IsAssignableFrom(messageField.Value!.GetType()))
                        {
                            result.AddRange(tag.AsVarint().RawData);
                            result.AddRange((byte[])messageField.Value!);
                        }
                        break;
                    default:
                        throw new InvalidOperationException("Unreachable code reached");
                }
            }
        }

        return result.ToArray();
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
