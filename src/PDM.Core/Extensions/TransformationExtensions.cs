using PDM.Entities;
using System.Globalization;

namespace PDM.Extensions;

internal static class TransformationExtensions
{
    private static readonly CultureInfo _formatProvider = CultureInfo.InvariantCulture;

    internal static IEnumerable<Entities.Mapping> AsMappings(this IEnumerable<Transformation>? transformations, IEnumerable<MessageField>? messageFields)
    {
        var mappings = new List<Entities.Mapping>();

        foreach (var messageField in messageFields ?? Array.Empty<MessageField>())
        {
            var fieldNumber = messageField.Key;
            var targetField = new MessageField(fieldNumber, messageField.WireType);
            mappings.Add(new Mapping(targetField, fieldNumber.MapExpression()));
        }

        foreach (var transform in transformations ?? Array.Empty<Transformation>())
        {
            switch (transform.TransformationType)
            {
                case Enums.TransformationType.ReplaceField:
                    switch (transform.SubType.ToLower(_formatProvider))
                    {
                        case "blacklist":
                            var key = Convert.ToInt32(transform.Value, _formatProvider);
                            mappings.RemoveField(key);
                            break;
                        case "renames":
                            var fieldPairs = transform.Value.ParseFieldPairs(_formatProvider);
                            foreach (var (sourceKey, targetKey) in fieldPairs)
                            {
                                mappings.RemoveField(targetKey);
                                var source = messageFields?.SingleOrDefault(f => f.Key == sourceKey);
                                if (source is not null)
                                {
                                    var targetField = new MessageField(targetKey, source.WireType);
                                    mappings.Add(new Mapping(targetField, sourceKey.MapExpression()));
                                }
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unreachable code reached");
            }
        }

        return mappings;
    }

}

