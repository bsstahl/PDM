using PDM.Entities;
using Protot.Core.Entities;
using Protot.Core.Exceptions;

namespace Protot.Core;

public class PrototSerializer
{  
    private ProtoFile protoFileInfo;
    private ProtoFileDescriptor? protoFileDescriptor;
    
    public PrototSerializer(ProtoFile protoFileInfo)
    {
        this.protoFileInfo = protoFileInfo;
    }
    
    public async Task<PrototSerializer> LoadFileDescriptor()
    {
        this.protoFileDescriptor =  await new ProtoFileDescriptorParser(this.protoFileInfo).ParseFileAsync();
        if (this.protoFileDescriptor == null)
        {
            throw new PrototMapperException(
                $"Either {nameof(protoFileDescriptor)} or {nameof(protoFileDescriptor)} is null");
        }

        return this;
    }

    public async Task Merge(IEnumerable<SourceMessageField> messageBytes)
    {
        
    }
}