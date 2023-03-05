using Microsoft.Extensions.Logging;
using PDM.Extensions;

namespace PDM;

public class ProtobufMapper
{
    readonly ILogger _logger;
    readonly IEnumerable<Entities.Transformation> _transformations;

    public ProtobufMapper(ILogger logger, IEnumerable<Entities.Transformation>? transformations)
    {
        _logger = logger ?? new DefaultLogger();
        _logger.LogMethodEntry(nameof(ProtobufMapper), "Ctor");

        _transformations = transformations ?? Array.Empty<Entities.Transformation>();
        // TODO: Validate any transformations

        _logger.LogMethodExit(nameof(ProtobufMapper), "Ctor");
    }

    public async Task<byte[]> MapAsync(byte[] sourceMessage)
    {
        _logger.LogMethodEntry(nameof(ProtobufMapper), nameof(MapAsync));

#pragma warning disable CA1848 // TODO: Use the LoggerMessage delegates
        _logger.LogDebug("Source Message Received: {SourceMessage}", Convert.ToBase64String(sourceMessage));
#pragma warning restore CA1848 // Use the LoggerMessage delegates

        var result = await sourceMessage
            .MapAsync(_logger, _transformations)
            .ConfigureAwait(false);

#pragma warning disable CA1848 // TODO: Use the LoggerMessage delegates
        _logger.LogDebug("Target Message Produced: {TargetMessage}", Convert.ToBase64String(result));
#pragma warning restore CA1848 // Use the LoggerMessage delegates

        _logger.LogMethodExit(nameof(ProtobufMapper), nameof(MapAsync));
        return result;
    }
}
