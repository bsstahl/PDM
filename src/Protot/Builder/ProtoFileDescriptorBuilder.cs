using Protot.Entities;

namespace Protot.Builder;

internal class ProtoFileDescriptorBuilder
{
    private readonly ProtoFileDescriptor _protoFileDescriptor = new();

    public ProtoFileDescriptor Build() => _protoFileDescriptor;

    public ProtoFileDescriptor AddSyntax(string syntax)
    {
        this._protoFileDescriptor.Syntax = syntax;
        return this._protoFileDescriptor;
    }
    
    public ProtoFileDescriptor AddNameSpace(string namespaceName)
    {
        this._protoFileDescriptor.Namespace = namespaceName;
        return this._protoFileDescriptor;
    }

    public ProtoFileDescriptor AddEnum(ProtoEnum? protoEnum)
    {
        if (protoEnum is null)
        {
            return this._protoFileDescriptor;
        }
        this._protoFileDescriptor.Enums ??= new List<ProtoEnum>();
        this._protoFileDescriptor.Enums.Add(protoEnum);
        return this._protoFileDescriptor;
    }
    
    public ProtoFileDescriptor AddMessage(ProtoMessage? protoMessage)
    {
        if (protoMessage is null)
        {
            return this._protoFileDescriptor;
        }
        this._protoFileDescriptor.Messages ??= new Dictionary<string, ProtoMessage>();
        this._protoFileDescriptor.Messages.Add(protoMessage.Name, protoMessage);
        return this._protoFileDescriptor;
    }
}