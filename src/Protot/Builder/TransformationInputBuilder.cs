using PDM.Enums;
using Protot.Entities;

namespace Protot.Builder;

public class TransformationInputBuilder
{
    private readonly List<TransformationInput> _transformations = new();

    public IEnumerable<TransformationInput> Build() => _transformations;

    public TransformationInputBuilder AddTransformation(
        TransformationType transformationType,
        TransformationField source,
        TransformationField target)
    {
        _transformations.Add(new TransformationInput()
        {
            TransformationType = transformationType,
            SourceField = source,
            TargetField = target
        });
        return this;
    }
}