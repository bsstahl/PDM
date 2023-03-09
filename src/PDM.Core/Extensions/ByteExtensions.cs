using Microsoft.Extensions.Logging;
using PDM.Entities;
using System.Linq.Dynamic.Core;

namespace PDM.Extensions;

internal static class ByteExtensions
{
    internal static Task<IEnumerable<MessageField>> Parse(this byte[] message, ILogger logger)
    {
        logger.LogMethodEntry(nameof(ByteExtensions), nameof(Parse));

        var result = new List<MessageField>();

        var i = 0;
        while (i < message.Length)
        {
            var vint = Varint.Parse(message[i..]);
            var tag = Tag.Parse(vint);
            i += vint.WireLength;

            logger.LogParsingField(tag);

            MessageField currentField = null;
            switch (tag.WireType)
            {
                case Enums.WireType.VarInt:
                    var (varintField, wireLength) = message[i..].ParseVarint(tag);
                    i += wireLength;
                    currentField = varintField;
                    break;
                case Enums.WireType.I64:
                    var i64Payload = message[i..(i + 8)];
                    i += 8;
                    currentField = new MessageField(tag.FieldNumber, tag.WireType, i64Payload);
                    break;
                case Enums.WireType.Len:
                    var lenVarint = Varint.Parse(message[i..]);
                    var len = Convert.ToInt32(lenVarint.Value);
                    i += lenVarint.WireLength;
                    var lenPayload = message[i..(i + len)];
                    i += len;
                    currentField = new MessageField(tag.FieldNumber, tag.WireType, lenPayload);
                    break;
                case Enums.WireType.SGroup:
                case Enums.WireType.EGroup:
                    break;
                case Enums.WireType.I32:
                    var i32Payload = message[i..(i + 4)];
                    i += 4;
                    currentField = new MessageField(tag.FieldNumber, tag.WireType, i32Payload);
                    break;
                default:
                    throw new InvalidOperationException("Unreachable code reached");
            }

            if (currentField is not null)
            {
                result.Add(currentField);
                logger.LogParseFieldResult(tag, currentField.Value ?? "null");
            }

        }

        logger.LogParseMessageResult(result);

        logger.LogMethodExit(nameof(ByteExtensions), nameof(Parse));
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
        var sourceFields = await sourceMessage
            .Parse(logger)
            .ConfigureAwait(false);

        var targetMappings = transformations
            .AsMappings(logger, sourceFields);

        var source = sourceFields.AsQueryable();

        var targetFields = new List<MessageField>();
        foreach (var targetMapping in targetMappings)
        {
            dynamic targetValue = targetMapping.Expression.ExpressionType switch
            {
                Enums.ExpressionType.Linq => source
                        .Single(targetMapping.Expression.Value)
                        .Value,

                Enums.ExpressionType.Literal => targetMapping
                    .Expression
                    .Value,

                _ => throw new InvalidOperationException("Unreachable code reached")
            };

            if (targetValue is not null)
            {
                var targetField = new MessageField(targetMapping.TargetField.Key, targetMapping.TargetField.WireType, targetValue);
                targetFields.Add(targetField);
                logger.LogFieldMappingProcessed(targetField);
            }
        }

        return targetFields.ToByteArray(logger);
    }
}
