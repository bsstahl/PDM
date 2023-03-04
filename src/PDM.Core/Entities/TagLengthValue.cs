namespace PDM.Entities;

public class TagLengthValue
{
    public int Key { get; set; }
    public Enums.WireType WireType { get; set; }
    public int Length { get; set; }
    public string Value { get; set; } = string.Empty;
}
