namespace Protot.Entities;

internal sealed class ProtoMessage
{
    internal string Name { get; set; } = string.Empty;
    
    internal IDictionary<string, ProtoMessageField> Fields { get; set; } 
}