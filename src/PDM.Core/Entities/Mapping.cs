namespace PDM.Entities;

internal class Mapping
{
    internal MessageField TargetField { get; set; }
    internal string Expression { get; set; } = string.Empty;

    internal Mapping(MessageField targetField, string expression)
    {
        this.TargetField = targetField;
        this.Expression = expression;
    }
}

