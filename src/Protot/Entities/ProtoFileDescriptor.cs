namespace Protot.Entities;

internal sealed class ProtoFileDescriptor
{
   internal string Syntax { get; set; }
   
   internal string Namespace { get; set; }
   
   internal IList<ProtoEnum> Enums { get; set; }
   
   internal IDictionary<string, ProtoMessage> Messages { get; set; }
}