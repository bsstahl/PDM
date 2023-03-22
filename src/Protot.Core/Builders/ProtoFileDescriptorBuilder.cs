using Protot.Core.Entities;

namespace Protot.Core.Builders;

internal class ProtoFileDescriptorBuilder
{
    private readonly ProtoFileDescriptor _protoFileDescriptor = new();

    internal ProtoFileDescriptor Build() => _protoFileDescriptor;

    internal ProtoFileDescriptor AddSyntax(string syntax)
    {
        this._protoFileDescriptor.Syntax = syntax;
        return this._protoFileDescriptor;
    }

    internal ProtoFileDescriptor AddNameSpace(string namespaceName)
    {
        this._protoFileDescriptor.Namespace = namespaceName;
        return this._protoFileDescriptor;
    }

    internal ProtoFileDescriptor AddEnum(ProtoEnum? protoEnum)
    {
        if (protoEnum is null)
        {
            return this._protoFileDescriptor;
        }
        this._protoFileDescriptor.Enums ??= new List<ProtoEnum>();
        this._protoFileDescriptor.Enums.Add(protoEnum);
        return this._protoFileDescriptor;
    }

    internal ProtoFileDescriptor AddMessage(ProtoMessage? protoMessage)
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