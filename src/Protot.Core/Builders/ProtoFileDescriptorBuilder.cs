using Google.Protobuf.Reflection;
using Protot.Core.Entities;
using Protot.Core.Extensions;

namespace Protot.Core.Builders;

internal class ProtoFileDescriptorBuilder
{
    private readonly ProtoFileDescriptor _protoFileDescriptor = new();

    internal ProtoFileDescriptor Build() => _protoFileDescriptor;

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

        this._protoFileDescriptor.Message = protoMessage;
        return this._protoFileDescriptor;
    }
    internal ProtoFileDescriptor AddReferenceMessage(ProtoMessage? protoMessage)
    {
        if (protoMessage is null)
        {
            return this._protoFileDescriptor;
        }
        this._protoFileDescriptor.ReferenceMessages ??= new Dictionary<string, ProtoMessage>();
        this._protoFileDescriptor.ReferenceMessages.Add(protoMessage.Name, protoMessage);
        return this._protoFileDescriptor;
    }
}