using Microsoft.Extensions.Logging;
using PDM.Entities;
using System.Linq.Dynamic.Core;

namespace PDM.Extensions;

internal static class ByteExtensions
{
    internal static Task<IEnumerable<MessageField>> Parse(this byte[] message)
    {
        var result = new List<MessageField>();

        var i = 0;
        while (i < message.Length)
        {
            var vint = Varint.Parse(message[i..]);
            var tag = Tag.Parse(vint);
            i += vint.WireLength;

            switch (tag.WireType)
            {
                case Enums.WireType.VarInt:
                    var (varintField, wireLength) = message[i..].ParseVarint(tag);
                    i += wireLength;
                    result.Add(varintField);
                    break;
                case Enums.WireType.I64:
                    var i64Payload = message[i..(i + 8)];
                    i += 8;
                    result.Add(new MessageField(tag.FieldNumber, tag.WireType, i64Payload));
                    break;
                case Enums.WireType.Len:
                    var lenVarint = Varint.Parse(message[i..]);
                    var len = Convert.ToInt32(lenVarint.Value);
                    i += lenVarint.WireLength;
                    var lenPayload = message[i..(i + len)];
                    i += len;
                    result.Add(new MessageField(tag.FieldNumber, tag.WireType, lenPayload));
                    break;
                case Enums.WireType.SGroup:
                case Enums.WireType.EGroup:
                    break;
                case Enums.WireType.I32:
                    var i32Payload = message[i..(i + 4)];
                    i += 4;
                    result.Add(new MessageField(tag.FieldNumber, tag.WireType, i32Payload));
                    break;
                default:
                    throw new InvalidOperationException("Unreachable code reached");
            }
        }

        return Task.FromResult(result.AsEnumerable());
    }

    internal static (MessageField, int) ParseVarint(this byte[] remainingBytes, Tag tag)
    {
        var payload = Varint.Parse(remainingBytes);
        var messageField = new MessageField(tag.FieldNumber, tag.WireType, payload.Value);
        return (messageField, payload.WireLength);
    }

    internal async static Task<byte[]> MapAsync(this byte[] sourceMessage, ILogger logger, IEnumerable<Transformation> transformations)
    {
        if (sourceMessage is null)
        {
            string fieldName = nameof(sourceMessage);
#pragma warning disable CA1848 // TODO: Use the LoggerMessage delegates
            logger.LogError("{FieldName} is required to perform a mapping", fieldName);
#pragma warning restore CA1848 // Use the LoggerMessage delegates
            throw new ArgumentNullException(fieldName);
        }

        var sourceFields = await sourceMessage
            .Parse()
            .ConfigureAwait(false);

        var targetMappings = transformations.AsMappings(sourceFields);

        var source = sourceFields.AsQueryable();

        var targetFields = new List<MessageField>();
        foreach (var targetMapping in targetMappings)
        {
            dynamic targetValue = targetMapping.Expression.ExpressionType switch
            {
                Enums.ExpressionType.Linq => source
                        .Single(targetMapping.Expression.Value)
                        .Value,
                Enums.ExpressionType.Literal => targetMapping.Expression.Value,
                _ => throw new InvalidOperationException("Unreachable code reached")
            };

            if (targetValue is not null)
                targetFields.Add(new MessageField(targetMapping.TargetField.Key, targetMapping.TargetField.WireType, targetValue));
        }

        return targetFields.ToByteArray();
    }
}
