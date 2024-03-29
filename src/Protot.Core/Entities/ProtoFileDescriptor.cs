namespace Protot.Core.Entities;

internal sealed class ProtoFileDescriptor
{
   internal string Syntax { get; set; } = string.Empty;
   
   internal string Namespace { get; set; } = string.Empty;

   internal IList<ProtoEnum> Enums { get; set; } = null!;

   internal IDictionary<string, ProtoMessage> Messages { get; set; } = null!;
}