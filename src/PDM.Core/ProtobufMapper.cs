using PDM.Extensions;

namespace PDM;

public class ProtobufMapper
{
    public async Task<byte[]> MapAsync(byte[] sourceMessage, IEnumerable<Entities.Mapping> targetMappings)
    {
        return await sourceMessage
            .MapAsync(targetMappings)
            .ConfigureAwait(false);
    }
}
