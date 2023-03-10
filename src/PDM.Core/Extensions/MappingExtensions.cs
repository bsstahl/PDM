using Microsoft.Extensions.Logging;
using System.Globalization;
using PDM.Constants;
using PDM.Entities;
using PDM.Enums;

namespace PDM.Extensions;

internal static class MappingExtensions
{
    internal static void Include(this IList<Mapping> mappings, IEnumerable<MessageField> messageFields, IEnumerable<Transformation> transformations)
    {
        if (!transformations.HasReplaceField(TransformationSubtype.Include))
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
            foreach (var includeTransform in transformations)
            {
                var includeFields = includeTransform.Value.Split(',');
                foreach (var includeField in includeFields)
                {
                    // TODO: Handle multiple instances of the same fieldNumber

                    // TODO: Log a warning if the field isn't found
                    var messageField = messageFields.FirstOrDefault(f => f.Key.ToString(CultureInfo.CurrentCulture) == includeField);
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
        var mappingExpression = new MappingExpression(ExpressionType.Linq, expression);
        return mappings.IncludeField(messageField, mappingExpression);
    }

    internal static Mapping IncludeField(this IList<Mapping> mappings, MessageField messageField, MappingExpression mappingExpression)
    {
        var mapping = new Mapping(messageField, mappingExpression);
        mappings.Add(mapping);
        return mapping;
    }

    internal static Mapping IncludeLiteral(this IList<Mapping> mappings, int key, WireType wireType, string expression)
    {
        var targetField = new MessageField(key, wireType);
        var mappingExpression = new MappingExpression(ExpressionType.Literal, expression);
        return mappings.IncludeField(targetField, mappingExpression);
    }

    internal static Mapping IncludeLiteral(this IList<Mapping> mappings, TagLengthValue tlv)
    {
        return mappings
            .IncludeLiteral(tlv.Key, tlv.WireType, tlv.Value);
    }

    internal static Mapping? RemoveField(this List<Mapping> result, ILogger logger, int key)
    {
        var item = result.SingleOrDefault(f => f.TargetField.Key == key);
        if (item is not null)
            _ = result.Remove(item);
        logger.LogMappingRemovalCompleted(key, item);
        return item;
    }

    internal static byte[] ToByteArray(this IEnumerable<MessageField> messageFields, ILogger logger)
    {
        logger.LogMethodEntry(nameof(MappingExtensions), nameof(ToByteArray));

        var result = new List<byte>();

        foreach (var messageField in messageFields)
        {
            if (messageField.IsValid)
            {
                var wireType = messageField.WireType;
                var tag = new Tag(messageField.Key, wireType);
                result.AddRange(tag.AsVarint().RawData);

                // messageField.IsValid so it has a Value
                var wireTypeValue = messageField.Value!.AsWiretypeValue(wireType);
                result.AddRange(wireTypeValue);
                logger.LogMessageFieldExported(messageField, wireTypeValue);
            }
            else
            {
                logger.LogInvalidMessageField(messageField);
            }
        }

        logger.LogMethodExit(nameof(MappingExtensions), nameof(ToByteArray));
        return result.ToArray();
    }

    internal static IEnumerable<byte> AsWiretypeValue(this object value, WireType wireType)
    {
        var result = new List<Byte>();

        switch (wireType)
        {
            case Enums.WireType.VarInt:
                byte[] rawData;
                if (long.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var longValue))
                    rawData = new Varint(longValue).RawData;
                else
                {
                    _ = ulong.TryParse(value.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var ulongValue);
                    rawData = new Varint(ulongValue).RawData;
                }

                if (rawData.Length > 0)
                {
                    result.AddRange(rawData);
                }
                break;
            case Enums.WireType.I64:
                if (typeof(byte[]).IsAssignableFrom(value!.GetType()))
                {
                    result.AddRange((byte[])value!);
                }
                else
                {
                    var bytes = Convert.FromHexString((string)value);
                    result.AddRange(bytes);
                }
                break;
            case Enums.WireType.Len:
                byte[] lenValue;
                if (typeof(string).IsAssignableFrom(value.GetType()))
                {
                    string stringValue = (string)value;
                    lenValue = stringValue.IsValidHexString()
                        ? Convert.FromHexString(stringValue)
                        : System.Text.Encoding.UTF8.GetBytes(stringValue);
                }
                else
                {
                    lenValue = (byte[])value!;
                }

                if (lenValue.Length > 0)
                {
                    var lenLength = new Varint(Convert.ToUInt64(lenValue.Length));
                    result.AddRange(lenLength.RawData);
                    result.AddRange(lenValue);
                }
                break;
            case Enums.WireType.SGroup:
            case Enums.WireType.EGroup:
                break;
            case Enums.WireType.I32:
                if (typeof(byte[]).IsAssignableFrom(value!.GetType()))
                {
                    result.AddRange((byte[])value!);
                }
                else
                {
                    result.AddRange(Convert.FromHexString((string)value));
                }
                break;
            default:
                throw new InvalidOperationException("Unreachable code reached");
        }

        return result;
    }
}
