using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using PDM.Constants;
using PDM.Enums;
using TransformationSubtype = Protot.Enums.TransformationSubtype;

namespace Protot.Entities;

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