using Microsoft.Extensions.Logging;
using PDM.Entities;
using System.Linq.Dynamic.Core;

namespace PDM.Extensions;

internal static class ByteExtensions
{
    internal async static Task<byte[]> MapAsync(this byte[] sourceMessage, ILogger logger, Interfaces.IWireFormatParser parser, IEnumerable<Transformation> transformations)
    {
        var sourceFields = await parser
            .ParseAsync(sourceMessage)
            .ConfigureAwait(false);

        var targetFields = await transformations
            .AsTargetFieldsAsync(logger, sourceFields)
            .ConfigureAwait(false);

        return targetFields.ToByteArray(logger);
    }
}
