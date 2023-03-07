using System.Text.Json;
using System.Text.Json.Serialization;

namespace PDM.Entities;

internal sealed class MappingExpression
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Enums.ExpressionType ExpressionType { get; set; }

    public string Value { get; set; } = string.Empty;

    public MappingExpression()
    { }

    public MappingExpression(Enums.ExpressionType expressionType, string value)
    {
        this.ExpressionType = expressionType;
        this.Value = value;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

