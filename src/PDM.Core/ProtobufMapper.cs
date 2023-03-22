using Microsoft.Extensions.Logging;
using PDM.Extensions;
using PDM.Interfaces;

namespace PDM;

public class ProtobufMapper
{
    readonly ILogger _logger;
    readonly IWireFormatParser _parser;
    readonly IProtobufWireFormatSerializer _serializer;
    readonly IEnumerable<Entities.Transformation> _transformations;

    public ProtobufMapper(ILogger<ProtobufMapper> logger, IWireFormatParser parser, IProtobufWireFormatSerializer serializer, IEnumerable<Entities.Transformation>? transformations)
    {
        _logger = logger ?? new DefaultLogger<ProtobufMapper>();
        _logger.LogMethodEntry(nameof(ProtobufMapper), "Ctor");

        if (logger is null)
            _logger.LogNoLoggerProvided();

        _parser = parser
            ?? throw new ArgumentNullException(nameof(parser));

        _serializer = serializer
            ?? throw new ArgumentNullException(nameof(serializer));

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
            .MapAsync(_logger, _parser, _serializer, _transformations)
            .ConfigureAwait(false);

        _logger.LogLargeData("Target Message", Convert.ToHexString(result));
        _logger.LogMethodExit(nameof(ProtobufMapper), nameof(MapAsync));

        return result;
    }
}
