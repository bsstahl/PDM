using Microsoft.Extensions.Logging;
using PDM.Constants;
using PDM.Entities;
using PDM.Enums;

namespace PDM.Parser.Extensions;

internal static class LoggerExtensions
{
    static readonly Action<ILogger, int, Enums.WireType, Exception?> _parsingFieldMessage
    = LoggerMessage.Define<int, Enums.WireType>(LogLevel.Debug,
        LogEventId.ParsingField,
        "Parsing field {FieldNumber} with wiretype {WireType}");

    static readonly Action<ILogger, string, Enums.WireType, object, Exception?> _parseFieldResultMessage
        = LoggerMessage.Define<string, Enums.WireType, object>(LogLevel.Information,
            LogEventId.ParseFieldResult,
            "Field {FieldNumber} with wiretype {WireType} parsed as {Value}");

    static readonly Action<ILogger, int, Exception?> _unableToParseFieldMessage
        = LoggerMessage.Define<int>(LogLevel.Warning,
            LogEventId.UnableToParseField,
            "Unable to parse field from byte position {Position}. This often results when we attempt to parse a LEN type field as an embedded message. In this circumstance, this message is expected and normal.");

    static readonly Action<ILogger, IEnumerable<SourceMessageField>, Exception?> _parseMessageResultMessage
        = LoggerMessage.Define<IEnumerable<SourceMessageField>>(LogLevel.Debug,
            LogEventId.ParseMessageResult,
            "Parse message result: {Result}");

    static readonly Action<ILogger, string, Exception?> _unableToParseMessageMessage
        = LoggerMessage.Define<string>(LogLevel.Warning,
            LogEventId.UnableToParseMessage,
            "Unable to parse message {Message}. This often results when we attempt to parse a LEN type field as an embedded message. In this circumstance, this message is expected and normal.");


    internal static void LogParsingField(this ILogger logger, Entities.Tag tag)
    => _parsingFieldMessage.Invoke(logger, tag.FieldNumber, tag.WireType, null);

    internal static void LogParseFieldResult(this ILogger logger, string fieldNumber, WireType wireType, object value)
        => _parseFieldResultMessage.Invoke(logger, fieldNumber, wireType, value, null);

    internal static void LogUnableToParseField(this ILogger logger, int bytePosition)
        => _unableToParseFieldMessage.Invoke(logger, bytePosition, null);

    internal static void LogParseMessageResult(this ILogger logger, IEnumerable<SourceMessageField> results)
        => _parseMessageResultMessage.Invoke(logger, results, null);

    internal static void LogUnableToParseMessage(this ILogger logger, byte[] messageBytes)
        => _unableToParseMessageMessage.Invoke(logger, Convert.ToHexString(messageBytes), null);

}
