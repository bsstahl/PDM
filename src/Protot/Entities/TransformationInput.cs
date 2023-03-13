using System.Text.Json;
using System.Text.Json.Serialization;
using PDM.Enums;

namespace Protot.Entities;

public class TransformationInput
{
    [Newtonsoft.Json.JsonConverter(typeof(JsonStringEnumConverter))]
    public TransformationType TransformationType { get; set; }

    public TransformationField SourceField { get; set; } 
    
    public TransformationField TargetField { get; set; } 
    
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}