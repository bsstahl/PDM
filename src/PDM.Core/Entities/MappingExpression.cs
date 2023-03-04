namespace PDM.Entities;

internal class MappingExpression
{
    public Enums.ExpressionType ExpressionType { get; set; }
    public string Value { get; set; } = string.Empty;

    public MappingExpression()
    { }

    public MappingExpression(Enums.ExpressionType expressionType, string value)
    {
        this.ExpressionType = expressionType;
        this.Value = value;
    }
}

