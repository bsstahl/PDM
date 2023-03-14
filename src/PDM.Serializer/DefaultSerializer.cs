using Microsoft.Extensions.Logging;
using PDM.Entities;
using PDM.Extensions;
using PDM.Serializer.Extensions;

namespace PDM.Serializer;

public class DefaultSerializer: Interfaces.IProtobufWireFormatSerializer
{
    private readonly ILogger _logger;

    public DefaultSerializer(ILogger<DefaultSerializer> logger)
    {
        _logger = logger ?? new DefaultLogger<DefaultSerializer>();
        if (logger is null)
            _logger.LogNoLoggerProvided();
    }

    public async Task<byte[]> ToByteArrayAsync(IEnumerable<TargetMessageField> messageFields)
    {
        return messageFields is null 
            ? throw new ArgumentNullException(nameof(messageFields)) 
            : await messageFields.ToByteArray(_logger).ConfigureAwait(false);
    }
}
