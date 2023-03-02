using PDM.Entities;
using PDM.Enums;

namespace PDM.Builders;

public class MappingBuilder
{
    private readonly List<Mapping> _mappings = new();

    public IEnumerable<Mapping> Build() => _mappings;

    public MappingBuilder AddUnmodifiedSourceField(int targetFieldNumber, WireType targetWireType, int sourceFieldNumber)
    {
        var targetField = new MessageField(targetFieldNumber, targetWireType);
        string expression = $"s => (s.Key == {sourceFieldNumber})";
        _mappings.Add(new Mapping(targetField, expression));
        return this;
    }
}

