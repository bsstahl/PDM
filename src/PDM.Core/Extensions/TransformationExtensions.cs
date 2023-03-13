using Microsoft.Extensions.Logging;
using PDM.Entities;
using PDM.Enums;
using System.Text.Json;

namespace PDM.Extensions;

internal static class TransformationExtensions
{
    internal static bool HasReplaceFieldTransform(this IEnumerable<Transformation> transformations, string subType)
        => transformations.Any(t => t.IsReplaceFieldTransform(subType));

    internal static bool IsReplaceFieldTransform(this Transformation t, string subType)
    {
        var rf = t.TransformationType == TransformationType.ReplaceField;
        var st = t.SubType == subType;
        return rf && st;
    }

    /// <summary>
    ///  Take the transformation list, combine it with the source
    ///  fields that were parsed from the input message, and return
    ///  a list of fields for the output message
    /// </summary>
    internal static Task<IEnumerable<TargetMessageField>> AsTargetFieldsAsync(this IEnumerable<Transformation> transformations, ILogger logger, IEnumerable<SourceMessageField> messageFields)
    {
        logger.LogMethodEntry(nameof(TransformationExtensions), nameof(AsTargetFieldsAsync));

        var results = new List<TargetMessageField>();

        results.Include(logger, messageFields, transformations);
        results.ModifyTargetFields(logger, messageFields, transformations);

        logger.LogLargeData("Target Message Fields", JsonSerializer.Serialize(results));
        logger.LogMethodExit(nameof(TransformationExtensions), nameof(AsTargetFieldsAsync));
        return Task.FromResult(results.AsEnumerable());
    }
}
