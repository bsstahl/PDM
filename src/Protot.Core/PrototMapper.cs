using PDM.Entities;
using Protot.Core.Entities;
using Protot.Core.Exceptions;
using Protot.Core.Extensions;

namespace Protot.Core;

public class PrototMapper
{
    private ProtoFile source;
    private ProtoFile target;
    readonly IEnumerable<Entities.ProtoTransformation> _transformations;

    public PrototMapper(ProtoFile source, ProtoFile target, IEnumerable<Entities.ProtoTransformation>? transformations)
    {
        if (transformations == null || !transformations.Any())
        {
            throw new ArgumentNullException($"{transformations} is null or empty");
        }
        this.source = source;
        this.target = target;
        this._transformations = transformations ?? Enumerable.Empty<ProtoTransformation>();
    }

    public async Task<IEnumerable<Transformation>> CompileAsync()
    {
        var sourceFileDescription = await new ProtoFileDescriptorParser(this.source).ParseFileAsync();
        var targetFileDescription = await new ProtoFileDescriptorParser(this.target).ParseFileAsync();

        if (sourceFileDescription == null || targetFileDescription == null)
        {
            throw new PrototMapperException(
                $"Either {nameof(sourceFileDescription)} or {nameof(targetFileDescription)} is null");
        }

        return this._transformations.ToProtoMappingAsync(sourceFileDescription, targetFileDescription);
    }
}