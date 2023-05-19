using Microsoft.Extensions.Logging;
using PDM.Entities;

namespace PDM.Extensions;

internal static class ByteExtensions
{
    internal static async Task<byte[]> MapAsync(this byte[] sourceMessage, ILogger logger, Interfaces.IWireFormatParser parser, Interfaces.IProtobufWireFormatSerializer serializer, IEnumerable<Transformation> transformations)
    {
        var sourceFields = await parser
            .ParseAsync(sourceMessage)
            .ConfigureAwait(false);

        var targetFields = await transformations
            .AsTargetFieldsAsync(logger, sourceFields)
            .ConfigureAwait(false);

        return await serializer
            .ToByteArrayAsync(targetFields)
            .ConfigureAwait(false);
    }
}
