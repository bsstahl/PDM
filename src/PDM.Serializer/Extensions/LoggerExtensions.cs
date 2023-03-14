using Microsoft.Extensions.Logging;
using PDM.Constants;
using PDM.Entities;

namespace PDM.Serializer.Extensions;

internal static class LoggerExtensions
{
    static readonly Action<ILogger, TargetMessageField, IEnumerable<byte>, Exception?> _messageFieldExported
    = LoggerMessage.Define<TargetMessageField, IEnumerable<byte>>(LogLevel.Information,
        LogEventId.MessageFieldExported,
        "Message field {Field} exported with value {Value}");

    static readonly Action<ILogger, TargetMessageField, Exception?> _invalidTargetMessageField
        = LoggerMessage.Define<TargetMessageField>(LogLevel.Warning,
            LogEventId.InvalidMessageField,
            "Invalid target message field {Field}");


    internal static void LogMessageFieldExported(this ILogger logger, TargetMessageField messageField, IEnumerable<byte> wireTypeValue)
    => _messageFieldExported.Invoke(logger, messageField, wireTypeValue, null);

    internal static void LogInvalidTargetField(this ILogger logger, TargetMessageField messageField)
        => _invalidTargetMessageField.Invoke(logger, messageField, null);

}
