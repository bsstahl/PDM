using PDM.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Protot.Core.Entities;

public class ProtoTransformation
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TransformationType TransformationType { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Protot.Core.Enums.TransformationSubtype SubType { get; set; }
    public string Value { get; set; } = string.Empty;

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}