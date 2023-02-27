using Google.Protobuf.Reflection;
using PDM.Extensions;

namespace PDM;

public class ProtobufMapper
{
    private readonly MessageDescriptor _sourceDescriptor;
    private readonly MessageDescriptor _targetDescriptor;

    public ProtobufMapper(MessageDescriptor sourceDescriptor, MessageDescriptor targetDescriptor)
    {
        _sourceDescriptor = sourceDescriptor;
        _targetDescriptor = targetDescriptor;
    }

    public async Task<byte[]> MapAsync(byte[] sourceMessage, string transformation)
    {
        return await sourceMessage
            .MapAsync(_sourceDescriptor, _targetDescriptor, transformation)
            .ConfigureAwait(false);
    }
}
