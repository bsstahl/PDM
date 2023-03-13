using System.Text.Json.Serialization;

namespace PDM.Entities;

public class Transformation
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Enums.TransformationType TransformationType { get; set; }
    
    public string SubType { get; set; } = String.Empty;

    public string Value { get; set; } = string.Empty;

}

