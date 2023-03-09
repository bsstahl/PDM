using Protot.Entities;

namespace Protot.Builder;

internal class ProtoMessageBuilder
{
    private readonly ProtoMessage _protoMessage = new();

    public ProtoMessage Build() => _protoMessage;

    public ProtoMessageBuilder CreateMessage(string messageType)
    {
        _protoMessage.MessageType = messageType;
        return this;
    }
    
    
    public ProtoMessageBuilder AddField(Field field)
    {
        _protoMessage.Fields ??= new List<Field>();
        _protoMessage.Fields.Add(field);
        return this;
    }

}