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

        if (logger is null)
            _logger.NoLoggerProvided();

        _transformations = transformations ?? Array.Empty<Entities.Transformation>();
        // TODO: Validate any transformations

        var transformationJson = System.Text.Json.JsonSerializer.Serialize(_transformations);
        _logger.LogLargeData("Transformations", transformationJson);

        _logger.LogMethodExit(nameof(ProtobufMapper), "Ctor");
    }

    public async Task<byte[]> MapAsync(byte[] sourceMessage)
    {
        _logger.LogMethodEntry(nameof(ProtobufMapper), nameof(MapAsync));

        if (sourceMessage is null)
        {
            string fieldName = nameof(sourceMessage);
            _logger.LogRequiredFieldMissing(fieldName);
            throw new ArgumentNullException(fieldName);
        }

        _logger.LogLargeData("Source Message", Convert.ToHexString(sourceMessage));

        var result = await sourceMessage
            .MapAsync(_logger, _transformations)
            .ConfigureAwait(false);

        _logger.LogLargeData("Target Message", Convert.ToHexString(result));
        _logger.LogMethodExit(nameof(ProtobufMapper), nameof(MapAsync));

        return result;
    }
}
