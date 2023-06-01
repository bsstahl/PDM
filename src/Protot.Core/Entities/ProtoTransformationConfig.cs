namespace Protot.Core.Entities;

public class ProtoTransformationConfig 
{
   public string SourceMessage { get; set; } = string.Empty;
   
   public string TargetMessage { get; set; } = string.Empty;
   public List<ProtoTransformation> Transformations { get; set; }
}