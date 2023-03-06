using System.Text.Json;
using System.Text.Json.Serialization;

namespace PDM.Entities;

public class TagLengthValue
{
    public int Key { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Enums.WireType WireType { get; set; }

    public int Length { get; set; }
    public string Value { get; set; } = string.Empty;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
