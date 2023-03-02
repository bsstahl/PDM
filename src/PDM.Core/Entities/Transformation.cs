namespace PDM.Entities;

public class Transformation
{
    public Enums.TransformationType TransformationType { get; set; }
    
    public string SubType { get; set; } = String.Empty;

    public string Value { get; set; } = string.Empty;
}

