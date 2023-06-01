using PDM.Entities;
using Protot.Core.Entities;
using Protot.Core.Exceptions;
using Protot.Core.Extensions;

namespace Protot.Core;

public class PrototMapper
{
    private ProtoFile source;
    private ProtoFile target;
    readonly ProtoTransformationConfig transformationConfig;

    public PrototMapper(ProtoFile source, ProtoFile target,ProtoTransformationConfig? transformationConfig)
    {
        if (transformationConfig == null || !transformationConfig.Transformations.Any())
        {
            throw new ArgumentNullException($"{transformationConfig} is null or empty");
        }
        if (string.IsNullOrWhiteSpace(transformationConfig.SourceMessage))
        {
            throw new ArgumentNullException($"{transformationConfig.SourceMessage} is null or empty");
        }
        if (string.IsNullOrWhiteSpace(transformationConfig.TargetMessage))
        {
            throw new ArgumentNullException($"{transformationConfig.TargetMessage} is null or empty");
        }
        this.source = source;
        this.target = target;
        this.transformationConfig = transformationConfig;
    }

    public async Task<IEnumerable<Transformation>> CompileAsync()
    {
        var sourceFileDescription = await new ProtoFileDescriptorParser(this.source, this.transformationConfig.SourceMessage).ParseFileAsync();
        var targetFileDescription = await new ProtoFileDescriptorParser(this.target, this.transformationConfig.TargetMessage).ParseFileAsync();

        if (sourceFileDescription == null || targetFileDescription == null)
        {
            throw new PrototMapperException(
                $"Either {nameof(sourceFileDescription)} or {nameof(targetFileDescription)} is null");
        }

        return this.transformationConfig.ToProtoMappingAsync(sourceFileDescription, targetFileDescription);
    }
}