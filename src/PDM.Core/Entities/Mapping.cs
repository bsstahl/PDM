namespace PDM.Entities;

internal class Mapping
{
    internal MessageField TargetField { get; set; }
    internal MappingExpression Expression { get; set; }

    internal Mapping(MessageField targetField, MappingExpression expression)
    {
        this.TargetField = targetField;
        this.Expression = expression;
    }

    internal Mapping(MessageField targetField, string linqExpression)
    {
        TargetField = targetField;
        Expression = new MappingExpression()
        {
            ExpressionType = Enums.ExpressionType.Linq,
            Value = linqExpression
        };
    }

}

