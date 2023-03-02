namespace PDM.Entities;

public class Mapping
{
    public MessageField TargetField { get; set; }
    public string Expression { get; set; } = string.Empty;

    public Mapping()
        : this(new MessageField(), string.Empty)
    { }

    public Mapping(MessageField targetField, string expression)
    {
        this.TargetField = targetField;
        this.Expression = expression;
    }
}

