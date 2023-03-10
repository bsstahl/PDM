using Microsoft.Extensions.Logging;
using PDM.Constants;
using PDM.Entities;
using PDM.Enums;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace PDM.Extensions;

internal static class TransformationExtensions
{
    internal static bool HasReplaceField(this IEnumerable<Transformation> transformations, string subType)
        => transformations.Any(t => t.IsReplaceField(subType));

    internal static bool IsReplaceField(this Transformation t, string subType)
    {
        var rf = t.TransformationType == TransformationType.ReplaceField;
        var st = t.SubType == subType;
        return rf && st;
    }

    internal static async Task<IEnumerable<Entities.Mapping>> AsMappingsAsync(this IEnumerable<Transformation> transformations, ILogger logger, IEnumerable<MessageField> messageFields)
    {
        logger.LogMethodEntry(nameof(TransformationExtensions), nameof(AsMappingsAsync));

        var mappings = new List<Entities.Mapping>();
        var parsedEmbeddedMessages = new Dictionary<int, IEnumerable<MessageField>>();

        mappings.Include(logger, messageFields, transformations);

        foreach (var transform in transformations)
        {
            var transformTypeName = transform
                .TransformationType
                .ToString()
                .ToLower(CultureInfo.CurrentCulture);

            var subTypeName = transform.SubType.ToLower(CultureInfo.CurrentCulture);

            logger.LogBuildingMapping(transform);

            switch (transform.TransformationType)
            {
                case Enums.TransformationType.InsertField:
                    switch (subTypeName)
                    {
                        case TransformationSubtype.Static:
                            var tlv = transform.Value.ParseTLV(CultureInfo.InvariantCulture);
                            var replacedField = mappings.RemoveField(tlv.Key);
                            var addedField = mappings.IncludeLiteral(tlv);
                            logger.LogMappingRemovalCompleted(tlv.Key, replacedField);
                            logger.LogMappingBuilt(addedField);
                            break;
                        default:
                            throw new NotImplementedException($"Handler for \"transforms.{transformTypeName}.{subTypeName}\" not yet implemented.");
                    }
                    break;
                case Enums.TransformationType.ReplaceField:
                    switch (subTypeName)
                    {
                        case TransformationSubtype.Blacklist:
                            var key = Convert.ToInt32(transform.Value, CultureInfo.InvariantCulture);
                            var removedMapping = mappings.RemoveField(key);
                            logger.LogMappingRemovalCompleted(key, removedMapping);
                            break;
                        case TransformationSubtype.Renames:
                            var fieldPairs = transform.Value.ParseFieldPairs(CultureInfo.InvariantCulture);
                            foreach (var (sourceKeys, targetKey) in fieldPairs)
                            {
                                mappings.RemoveField(targetKey);

                                switch (sourceKeys.Length)
                                {
                                    case 1:
                                        var sourceKey = sourceKeys[0];
                                        var source = messageFields.SingleOrDefault(f => f.Key == sourceKey);
                                        var targetField = new MessageField(targetKey, source.WireType);
                                        var renamesMapping = mappings.IncludeField(targetField, sourceKey.MapExpression());
                                        logger.LogMappingBuilt(renamesMapping);
                                        break;

                                    case > 1:
                                        await mappings.MapEmbeddedRenameAsync(
                                            parsedEmbeddedMessages,
                                            sourceKeys,
                                            messageFields,
                                            targetKey,
                                            logger).ConfigureAwait(false);
                                        break;
                                }
                            }
                            break;
                        case TransformationSubtype.Include:
                            // Ignore here -- handled above
                            break;
                        default:
                            throw new NotImplementedException($"Handler for \"transforms.{transformTypeName}.{subTypeName}\" not yet implemented.");
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unreachable code reached");
            }
        }

        logger.LogMethodExit(nameof(TransformationExtensions), nameof(AsMappingsAsync));
        return mappings;
    }

    internal static async Task MapEmbeddedRenameAsync(
        this IList<Mapping> mappings,
        IDictionary<int, IEnumerable<MessageField>> parsedEmbeddedMessages,
        int[] sourceKeys,
        IEnumerable<MessageField>? sourceFields,
        int targetKey,
        ILogger logger)
    {
        for (var i = 0; i < sourceKeys.Length - 1; i++)
        {
            var sourceField = sourceFields?.FirstOrDefault(x => x.Key == sourceKeys[i]);
            var sourceFieldBytes = sourceField is not null && sourceField.Value.IsByteArray()
                ? sourceField.Value as byte[]
                : null;

            if (!parsedEmbeddedMessages.ContainsKey(sourceKeys[i]))
            {
                var parseEmbeddedMessageTask = sourceFieldBytes?.ParseAsync(logger);
                if (parseEmbeddedMessageTask is not null
                    && await parseEmbeddedMessageTask.ConfigureAwait(false)
                    is IEnumerable<MessageField> parsedEmbeddedMessage)
                {
                    parsedEmbeddedMessages[sourceKeys[i]] = parsedEmbeddedMessage;
                }
            }

            parsedEmbeddedMessages.TryGetValue(sourceKeys[i], out sourceFields);
        }

        var sourceKey = sourceKeys?.LastOrDefault();

        if (sourceKey.HasValue
            && sourceFields?.FirstOrDefault(x => x.Key == sourceKey) is MessageField source)
        {
            var targetField = new MessageField(targetKey, source.WireType)
            {
                Value = source.Value
            };
            var targetExpression = new MappingExpression(ExpressionType.Literal, string.Empty);
            var mapping = new Mapping(targetField, targetExpression);
            mappings.Add(mapping);
            logger.LogMappingBuilt(mapping);
        }
    }

}

