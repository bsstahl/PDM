using PDM.Constants;
using PDM.Entities;
using PDM.Enums;
using System.Globalization;

namespace PDM.Extensions;

internal static class TransformationExtensions
{
    private static readonly CultureInfo _formatProvider = CultureInfo.InvariantCulture;

    internal static bool IsReplaceField(this Transformation t, string subType)
    {
        var rf = t.TransformationType == TransformationType.ReplaceField;
        var st = t.SubType == subType;
        return rf && st;
    }

    internal static IEnumerable<Entities.Mapping> AsMappings(this IEnumerable<Transformation> transformations, IEnumerable<MessageField> messageFields)
    {
        var mappings = new List<Entities.Mapping>();

        mappings.Include(messageFields, transformations);

        foreach (var transform in transformations)
        {
            var transformTypeName = Enums.TransformationType.ReplaceField.ToString().ToLower(_formatProvider);
            var subTypeName = transform.SubType.ToLower(_formatProvider);
            switch (transform.TransformationType)
            {
                case Enums.TransformationType.InsertField:
                    switch (subTypeName)
                    {
                        case TransformationSubtype.Static:
                            var tlv = transform.Value.ParseTLV(_formatProvider);
                            mappings.RemoveField(tlv.Key);
                            _ = mappings.IncludeLiteral(tlv);
                            break;
                        default:
                            throw new NotImplementedException($"Handler for \"transforms.{transformTypeName}.{subTypeName}\" not yet implemented.");
                    }
                    break;
                case Enums.TransformationType.ReplaceField:
                    switch (subTypeName)
                    {
                        case TransformationSubtype.Blacklist:
                            var key = Convert.ToInt32(transform.Value, _formatProvider);
                            mappings.RemoveField(key);
                            break;
                        case TransformationSubtype.Renames:
                            var fieldPairs = transform.Value.ParseFieldPairs(_formatProvider);
                            foreach (var (sourceKey, targetKey) in fieldPairs)
                            {
                                mappings.RemoveField(targetKey);
                                var source = messageFields.SingleOrDefault(f => f.Key == sourceKey);
                                if (source is not null)
                                {
                                    var targetField = new MessageField(targetKey, source.WireType);
                                    _ = mappings.IncludeField(targetField, sourceKey.MapExpression());
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

        return mappings;
    }

}

