using PDM.Extensions;

namespace PDM;

public class ProtobufMapper
{
    readonly IEnumerable<Entities.Mapping> _targetMappings;

    public ProtobufMapper(IEnumerable<Entities.Mapping> targetMappings)
    {
        _targetMappings = targetMappings;
        // TODO: Validate mappings
    }

    public async Task<byte[]> MapAsync(byte[] sourceMessage)
    {
        return await sourceMessage
            .MapAsync(_targetMappings)
            .ConfigureAwait(false);
    }
}
