using System.Text.Json.Serialization;

namespace PDM.Entities;

public class TagLengthValue
{
    public string Key { get; set; } = string.Empty;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Enums.WireType WireType { get; set; }

    public string Value { get; set; } = string.Empty;

}
