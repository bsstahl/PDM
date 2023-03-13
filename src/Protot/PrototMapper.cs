using PDM.Builders;
using PDM.Entities;
using Protot.Builder;
using Protot.Entities;
using Protot.Exceptions;
using Protot.Extensions;

namespace Protot;

public class PrototMapper
{
    private string sourceProtoContent;
    private string targetProtoContent;
    readonly IEnumerable<Entities.TransformationInput> _transformations;

    public PrototMapper(string source, string target, IEnumerable<Entities.TransformationInput>? transformations)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentNullException($"{source} is empty");
        }

        if (string.IsNullOrWhiteSpace(target))
        {
            throw new ArgumentNullException($"{target} is empty");
        }

        if (transformations == null || !transformations.Any())
        {
            throw new ArgumentNullException($"{transformations} is null or empty");
        }
        this.sourceProtoContent = source;
        this.targetProtoContent = target;
        this._transformations = transformations ?? Enumerable.Empty<TransformationInput>();
    }

    public async Task<IEnumerable<Transformation>> CreateTransformationAsync()
    {
        var sourceFileDescription = await new ProtoFileDescriptorParser(this.sourceProtoContent).ParseFileAsync();
        var targetFileDescription = await new ProtoFileDescriptorParser(this.targetProtoContent).ParseFileAsync();

        if (sourceFileDescription == null || targetFileDescription == null)
        {
            throw new PrototMapperException(
                $"Either {nameof(sourceFileDescription)} or {nameof(targetFileDescription)} is null");
        }

        var transformationBuilder = new TransformationBuilder();
        foreach (var mapping in this._transformations)
        {
            transformationBuilder.RenameField(
                mapping.SourceField.TryGetFieldNumber(sourceFileDescription),
                mapping.TargetField.TryGetFieldNumber(targetFileDescription));
        }

        return transformationBuilder.Build();
    }
}