using PDM.Entities;

namespace PDM.Extensions;

internal static class MessageFieldExtensions
{
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
                        var varintValue = Convert.ToUInt64(messageField.Value);
                        var rawData = new Varint(varintValue).RawData;
                        if (rawData.Length > 0)
                        {
                            result.AddRange((tag.AsVarint()).RawData);
                            result.AddRange(rawData);
                        }
                        break;
                    case Enums.WireType.I64:
                        // messageField.IsValid so it has a Value
                        if (typeof(byte[]).IsAssignableFrom(messageField.Value!.GetType()))
                        {
                            result.AddRange((tag.AsVarint()).RawData);
                            result.AddRange((byte[])messageField.Value!);
                        }
                        break;
                    case Enums.WireType.Len:
                        // messageField.IsValid so it has a Value
                        var lenValue = (byte[])messageField.Value!;
                        if (lenValue.Length > 0)
                        {
                            var lenLength = new Varint(Convert.ToUInt64(lenValue.Length));
                            result.AddRange((tag.AsVarint()).RawData);
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
                            result.AddRange((tag.AsVarint()).RawData);
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
}

