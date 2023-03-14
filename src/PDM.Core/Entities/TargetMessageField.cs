using PDM.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PDM.Entities;

public class TargetMessageField
{
    public IEnumerable<int> Key { get; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Enums.WireType WireType { get; set; }

    public object? Value { get; set; }

    [JsonIgnore]
    public bool IsValid => this.Key.Any() 
        && this.Key.All(k => k > 0)
        && (this.Value is not null)
        && (!string.IsNullOrEmpty(this.Value.ToString()));

    internal TargetMessageField(SourceMessageField sourceField)
        : this(sourceField.Key.AsTargetKey(), sourceField.WireType, sourceField.Value)
    { }

    public TargetMessageField(IEnumerable<int> key, Enums.WireType wireType)
        : this(key, wireType, null)
    { }

    public TargetMessageField(IEnumerable<int> key, Enums.WireType wireType, object? value)
        : this(key, wireType, value, Array.Empty<SourceMessageField>())
    { }

    public TargetMessageField(IEnumerable<int> key, Enums.WireType wireType, object? value, IEnumerable<SourceMessageField> childFields)
    {
        this.Key = key;
        this.WireType = wireType;
        this.Value = value;
    }

    public override string ToString()
        => JsonSerializer.Serialize(this);
}
