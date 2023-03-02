using PDM.Extensions;

namespace PDM;

public class ProtobufMapper
{
#pragma warning disable CA1822 // Mark members as static
    public async Task<byte[]> MapAsync(byte[] sourceMessage, IEnumerable<Entities.Mapping> targetMappings)
    {
        return await sourceMessage
            .MapAsync(targetMappings)
            .ConfigureAwait(false);
    }
#pragma warning restore CA1822 // Mark members as static
}
