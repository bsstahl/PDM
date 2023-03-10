using Microsoft.Extensions.Logging;
using PDM.Entities;
using System.Linq.Dynamic.Core;

namespace PDM.Extensions;

internal static class ByteExtensions
{
    internal async static Task<byte[]> MapAsync(this byte[] sourceMessage, ILogger logger, Interfaces.IWireFormatParser parser, IEnumerable<Transformation> transformations)
    {
        var sourceFields = await parser
            .ParseAsync(sourceMessage)
            .ConfigureAwait(false);

        var targetMappings = await transformations
            .AsMappingsAsync(logger, sourceFields)
            .ConfigureAwait(false);

        var source = sourceFields.AsQueryable();

        var targetFields = new List<MessageField>();
        foreach (var targetMapping in targetMappings)
        {
            if (targetMapping.TargetField is null)
                throw new InvalidDataException(nameof(targetMapping.TargetField));

            dynamic? targetValue = targetMapping.Expression.ExpressionType switch
            {
                Enums.ExpressionType.Linq => source
                        .Single(targetMapping.Expression.Value)
                        .Value,

                Enums.ExpressionType.Literal => !string.IsNullOrWhiteSpace(targetMapping.Expression.Value)
                    ? targetMapping.Expression.Value
                    : targetMapping.TargetField.Value,

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
