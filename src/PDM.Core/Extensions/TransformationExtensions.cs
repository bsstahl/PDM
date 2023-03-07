using Microsoft.Extensions.Logging;
using PDM.Constants;
using PDM.Entities;
using PDM.Enums;
using System.Globalization;

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

    internal static IEnumerable<Entities.Mapping> AsMappings(this IEnumerable<Transformation> transformations, ILogger logger, IEnumerable<MessageField> messageFields)
    {
        logger.LogMethodEntry(nameof(TransformationExtensions), nameof(AsMappings));

        var mappings = new List<Entities.Mapping>();

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
                            foreach (var (sourceKey, targetKey) in fieldPairs)
                            {
                                mappings.RemoveField(targetKey);
                                var source = messageFields.SingleOrDefault(f => f.Key == sourceKey);
                                if (source is not null)
                                {
                                    var targetField = new MessageField(targetKey, source.WireType);
                                    var renamesMapping = mappings.IncludeField(targetField, sourceKey.MapExpression());
                                    logger.LogMappingBuilt(renamesMapping);
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

        logger.LogMethodExit(nameof(TransformationExtensions), nameof(AsMappings));
        return mappings;
    }

}

