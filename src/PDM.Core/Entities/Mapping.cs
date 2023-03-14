//using PDM.Extensions;
//using System.Text.Json;

//namespace PDM.Entities;

//internal sealed class Mapping
//{   
//    public TargetMessageField TargetField { get; set; }
//    public MappingExpression Expression { get; set; }

//    public string TargetKey => TargetField.Key.AsSourceKey();

//    internal Mapping(TargetMessageField targetField, MappingExpression expression)
//    {
//        this.TargetField = targetField;
//        this.Expression = expression;
//    }

//    public override string ToString() 
//        => JsonSerializer.Serialize(this);
//}

