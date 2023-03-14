﻿using Microsoft.Extensions.Logging;
using System.Globalization;
using PDM.Constants;
using PDM.Entities;
using PDM.Enums;

namespace PDM.Extensions;

internal static class TargetMessageFieldExtensions
{
    internal static void ModifyTargetFields(this IList<TargetMessageField> targetFields, ILogger logger, IEnumerable<SourceMessageField> sourceFields, IEnumerable<Transformation> transformations)
    {
        foreach (var transformation in transformations)
            targetFields.ModifyTargetField(logger, sourceFields, transformation);
    }

    internal static void ModifyTargetField(this IList<TargetMessageField> targetFields, ILogger logger, IEnumerable<SourceMessageField> sourceFields, Transformation transformation)
    {
        var transformTypeName = transformation
            .TransformationType
            .ToString()
            .ToLower(CultureInfo.CurrentCulture);

        var subTypeName = transformation.SubType.ToLower(CultureInfo.CurrentCulture);
        var activityName = $"{transformation.TransformationType}.{subTypeName}";

        logger.LogBuildingMapping(transformation);

        switch (transformation.TransformationType)
        {
            case Enums.TransformationType.InsertField:
                switch (subTypeName)
                {
                    case TransformationSubtype.Static:
                        var tlv = transformation.Value.ParseTLV();
                        targetFields.RemoveField(logger, tlv.Key, activityName);
                        var addedField = targetFields.IncludeLiteral(tlv);
                        logger.LogMappingBuilt(addedField, activityName);
                        break;
                    default:
                        throw new NotImplementedException($"Handler for \"transforms.{transformTypeName}.{subTypeName}\" not yet implemented.");
                }
                break;
            case Enums.TransformationType.ReplaceField:
                switch (subTypeName)
                {
                    case TransformationSubtype.Blacklist:
                        targetFields.RemoveField(logger, transformation.Value, activityName);
                        break;
                    case TransformationSubtype.Renames:
                        var fieldPairs = transformation.Value.ParseFieldPairs(CultureInfo.InvariantCulture);
                        foreach (var (sourceKeys, targetKey) in fieldPairs)
                        {
                            if (!targetKey.Any())
                                logger.LogInvalidTransformation(transformation, activityName);
                            else
                            {
                                targetFields.RemoveField(logger, targetKey, activityName);
                                var source = sourceFields.Get(sourceKeys);
                                if (source is not null)
                                {
                                    var targetField = new TargetMessageField(targetKey, source!.WireType, source!.Value);
                                    targetFields.IncludeField(targetField);
                                    logger.LogMappingBuilt(targetField, activityName);
                                }
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

    internal static void Include(this IList<TargetMessageField> mappings, ILogger logger, IEnumerable<SourceMessageField> sourceFields, IEnumerable<Transformation> transformations)
    {
        if (!transformations.HasReplaceFieldTransform(TransformationSubtype.Include))
        {
            // If there is no "replacefield.include" add all
            // existing, root-level fields by default
            mappings.IncludeFields(sourceFields.RootFields());
        }
        else
        {
            // If there is a replacefield.include, use only
            // the fields specified in the include list

            // TODO: Answer the Question: Is it ok to have multiple includes?
            foreach (var includeTransform in transformations)
            {
                var sourceKeys = includeTransform.Value.Split(',');
                foreach (var sourceKey in sourceKeys)
                {
                    var activityName = $"{includeTransform.TransformationType}.{includeTransform.SubType}";
                    var (sourceFieldKey, targetFieldKey) = sourceKey.ParsePair();
                    var sourceField = sourceFields
                            .FirstOrDefault(f => f.Key == sourceFieldKey);

                    if (sourceField is null)
                        logger.LogSourceFieldNotFound(includeTransform, activityName);
                    else
                    {
                        // Since this is a straight-up include, we use the
                        // Source Key as the Target Key
                        var targetField = new TargetMessageField(sourceField);
                        _ = mappings.IncludeField(targetField);
                    }
                }
            }
        }
    }

    internal static void IncludeFields(this IList<TargetMessageField> mappings, IEnumerable<SourceMessageField> messageFields)
    {
        foreach (var messageField in messageFields)
            _ = mappings.IncludeField(new TargetMessageField(messageField));
    }

    internal static TargetMessageField IncludeField(this IList<TargetMessageField> mappings, TargetMessageField targetMessageField)
    {
        mappings.Add(targetMessageField);
        return targetMessageField;
    }

    internal static TargetMessageField IncludeLiteral(this IList<TargetMessageField> mappings, int[] targetKey, WireType wireType, string expression)
    {
        var targetField = new TargetMessageField(targetKey, wireType, expression);
        return mappings.IncludeField(targetField);
    }

    internal static TargetMessageField IncludeLiteral(this IList<TargetMessageField> mappings, TagLengthValue tlv)
    {
        return mappings
            .IncludeLiteral(tlv.Key.AsTargetKey().ToArray(), tlv.WireType, tlv.Value);
    }

    /// <summary>
    /// Removes the field matching the target key and
    /// everything below it in the hierarchy
    /// </summary>
    internal static void RemoveField(this IList<TargetMessageField> result, ILogger logger, IEnumerable<int> targetKey, string activityName) 
        => result.RemoveField(logger, targetKey.AsSourceKey(), activityName);

    internal static void RemoveField(this IList<TargetMessageField> result, ILogger logger, string targetKeyValue, string activityName)
    {
        var items = result
            .Where(f => f.Key.AsSourceKey().StartsWith(targetKeyValue, false, CultureInfo.InvariantCulture))
            .ToArray();

        foreach (var item in items)
        {
            _ = result.Remove(item);
            logger.LogMappingRemovalCompleted(targetKeyValue, item, activityName);
        }
    }

    internal static byte[] ToByteArray(this IEnumerable<TargetMessageField> messageFields, ILogger logger)
    {
        logger.LogMethodEntry(nameof(TargetMessageFieldExtensions), nameof(ToByteArray));

        var result = new List<byte>();

        foreach (var messageField in messageFields)
        {
            if (messageField.IsValid)
            {
                var wireType = messageField.WireType;
                var tag = new Tag(messageField.Key.Last(), wireType);
                result.AddRange(tag.AsVarint().RawData);

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
        return result.ToArray();
    }

    internal static IEnumerable<byte> AsWiretypeValue(this object value, WireType wireType)
    {
        var result = new List<Byte>();

        switch (wireType)
        {
            case Enums.WireType.VarInt:
                byte[] rawData = (Varint.Create(value)).RawData;
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
