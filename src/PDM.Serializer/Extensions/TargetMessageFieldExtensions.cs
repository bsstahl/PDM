using Microsoft.Extensions.Logging;
using PDM.Entities;
using PDM.Extensions;
using PDM.WireTypes;

namespace PDM.Serializer.Extensions;

internal static class TargetMessageFieldExtensions
{
    internal static Task<byte[]> ToByteArray(this IEnumerable<TargetMessageField> messageFields, ILogger logger)
    {
        logger.LogMethodEntry(nameof(TargetMessageFieldExtensions), nameof(ToByteArray));

        var result = new List<byte>();

        foreach (var messageField in messageFields)
        {
            if (messageField.IsValid)
            {
                var wireType = messageField.WireType;
                var tag = new Tag(messageField.Key.Last(), wireType);
                result.AddRange(tag.AsVarint().GetRawData());

                // messageField.IsValid so it has a Value
                var wireTypeValue = messageField.Value!.AsWiretypeValue(wireType);
                result.AddRange(wireTypeValue);
                logger.LogMessageFieldExported(messageField, wireTypeValue);
            }
            else
            {
                logger.LogInvalidTargetField(messageField);
            }
        }

        logger.LogMethodExit(nameof(TargetMessageFieldExtensions), nameof(ToByteArray));
        return Task.FromResult(result.ToArray());
    }

    internal static IEnumerable<byte> AsWiretypeValue(this object value, Enums.WireType wireType)
    {
        var result = new List<Byte>();

        switch (wireType)
        {
            case Enums.WireType.VarInt:
                byte[] rawData = (Varint.Create(value)).GetRawData();
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
                    // TODO: Handle Big Endian machines
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
                    result.AddRange(lenLength.GetRawData());
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
