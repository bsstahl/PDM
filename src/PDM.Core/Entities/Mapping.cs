using System.Text.Json;

namespace PDM.Entities;

internal sealed class Mapping
{   
    public MessageField TargetField { get; set; }
    public MappingExpression Expression { get; set; }

    internal Mapping(MessageField targetField, MappingExpression expression)
    {
        this.TargetField = targetField;
        this.Expression = expression;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

