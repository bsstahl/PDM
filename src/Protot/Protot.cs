using PDM.Entities;
using Protot.Entities;
using Protot.Exceptions;
using Protot.Extensions;

namespace Protot;

public class Protot
{
    private string sourceProtoContent;
    private string targetProtoContent;
    readonly IEnumerable<Entities.ProtoTransformation> _transformations;

    public Protot(string source, string target, IEnumerable<Entities.ProtoTransformation>? transformations)
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
        this._transformations = transformations ?? Enumerable.Empty<ProtoTransformation>();
    }

    public async Task<IEnumerable<Transformation>> CompileAsync()
    {
        var sourceFileDescription = await new ProtoFileDescriptorParser(this.sourceProtoContent).ParseFileAsync();
        var targetFileDescription = await new ProtoFileDescriptorParser(this.targetProtoContent).ParseFileAsync();

        if (sourceFileDescription == null || targetFileDescription == null)
        {
            throw new PrototMapperException(
                $"Either {nameof(sourceFileDescription)} or {nameof(targetFileDescription)} is null");
        }

        return this._transformations.ToProtoMappingAsync(sourceFileDescription, targetFileDescription);
    }
}