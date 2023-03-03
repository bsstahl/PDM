using PDM.Extensions;

namespace PDM;

public class ProtobufMapper
{
    readonly IEnumerable<Entities.Transformation> _transformations;

    public ProtobufMapper(IEnumerable<Entities.Transformation>? transformations)
    {
        _transformations = transformations ?? Array.Empty<Entities.Transformation>();
        // TODO: Validate any transformations
    }

    public async Task<byte[]> MapAsync(byte[] sourceMessage)
    {
        return await sourceMessage
            .MapAsync(_transformations)
            .ConfigureAwait(false);
    }
}
