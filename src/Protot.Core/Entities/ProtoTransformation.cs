using System.Text.Json;
using System.Text.Json.Serialization;
using PDM.Enums;
using TransformationSubtype = Protot.Core.Enums.TransformationSubtype;

namespace Protot.Core.Entities;

public class ProtoTransformation
{
    [Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
    public TransformationType TransformationType { get; set; }
    
    public TransformationSubtype SubType { get; set; }
    public string Value { get; set; } = string.Empty;
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}