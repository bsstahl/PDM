using PDM.Enums;
using Protot.Core.Entities;
using Protot.Core.Enums;

namespace Protot.Core.Builders;

public class ProtoTransformationBuilder
{
    private readonly List<ProtoTransformation> _transformations = new();

    public IEnumerable<ProtoTransformation> Build() => _transformations;

    public ProtoTransformationBuilder AddTransformation(
        TransformationType transformationType,
        TransformationSubtype subType,
        string value)
    {
        _transformations.Add(new ProtoTransformation()
        {
            TransformationType = transformationType,
            Value = value,
            SubType = subType
        });
        return this;
    }
    
}