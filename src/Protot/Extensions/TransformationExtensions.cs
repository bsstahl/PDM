using PDM.Builders;
using PDM.Entities;
using PDM.Enums;
using Protot.Entities;

namespace Protot.Extensions;

internal static class TransformationExtensions
{
    internal static IEnumerable<Transformation> ToProtoMappingAsync(
        this IEnumerable<ProtoTransformation> transformationInputs,
        ProtoFileDescriptor source,
        ProtoFileDescriptor target)
    {
        var transformationBuilder = new TransformationBuilder();
        foreach (var transformation in transformationInputs)
        {
            switch (transformation.TransformationType)
            {
                case TransformationType.ReplaceField:
                    var pairs = transformation.Value.ParsePairs();
                    IEnumerable<string> protoTags = pairs.Select(pair => $"{pair.sourceField.TryGetFieldNumber(source)}:{pair.tragetField.TryGetFieldNumber(target)}");
                    transformationBuilder.AddTransformation(
                        TransformationType.ReplaceField,
                        transformation.SubType.ToLowerEnum(),
                        protoTags.JoinPairs());
                    break;
            }
        }

        return transformationBuilder.Build();
    }
}