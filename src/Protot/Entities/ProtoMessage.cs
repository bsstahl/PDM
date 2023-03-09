namespace Protot.Entities;

internal class ProtoMessage
{
    public string MessageType { get; set; } = string.Empty;
    public IList<Field> Fields { get; set; } 
}