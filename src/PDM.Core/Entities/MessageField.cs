using System.Text.Json;
using System.Text.Json.Serialization;

namespace PDM.Entities;

public class MessageField
{
    public int Key { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Enums.WireType WireType { get; set; }
    
    public object? Value { get; set; }


    public bool IsValid => (this.Key > 0)
        && (this.Value is not null);

    public MessageField(int key, Enums.WireType wireType)
        : this(key, wireType, null)
    { }

    public MessageField(int key, Enums.WireType wireType, object? value)
    {
        this.Key = key;
        this.WireType = wireType;
        this.Value = value;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
