using System.Text.Json;
using System.Text.Json.Serialization;

namespace PDM.Entities;

public class SourceMessageField
{
    public string Key { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Enums.WireType WireType { get; set; }

    public object? Value { get; set; }

    internal bool HasParent
        => this.Key.Contains('.', StringComparison.InvariantCulture);


    public SourceMessageField(string key, Enums.WireType wireType, object? value)
    {
        this.Key = key;
        this.WireType = wireType;
        this.Value = value;
    }

    public override string ToString()
        => JsonSerializer.Serialize(this);
}
