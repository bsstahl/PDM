namespace Protot.Entities;

internal sealed class ProtoFileDescriptor
{
   internal string Syntax { get; set; } = string.Empty;
   
   internal string Namespace { get; set; } = string.Empty;

   internal IList<ProtoEnum> Enums { get; set; } = Array.Empty<ProtoEnum>();

   internal IDictionary<string, ProtoMessage> Messages { get; set; } = null!;
}